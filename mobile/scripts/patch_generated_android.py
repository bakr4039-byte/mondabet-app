#!/usr/bin/env python3
"""Patches the freshly-scaffolded android/app/build.gradle(.kts).

mobile/android/ isn't committed to the repo - it's generated fresh on every
CI run via `flutter create --platforms=android .` (see
.github/workflows/build-apk.yml), since the sandbox this project is
normally developed in has no internet/Android SDK access to test it
against. That means issues in the generated Gradle config only surface the
first time a real build actually runs on GitHub Actions, one at a time.
This script collects the fixes discovered that way in one place instead of
scattering ad-hoc patch steps across the workflow, and each fix here is
idempotent (safe to run more than once) and self-checking (asserts the
patch actually took effect, instead of silently no-op'ing if the generated
template ever changes shape again).

Fixes applied:
  1. Core library desugaring, required by flutter_local_notifications
     (https://developer.android.com/studio/write/java8-support.html).
     `compileOptions { coreLibraryDesugaringEnabled true }` turns it on -
     Flutter's template always has a `compileOptions {}` block already, so
     this is an in-place edit. The actual desugaring runtime dependency is
     appended as a brand new `dependencies {}` block instead of injected
     into an existing one, since Flutter's newer templates don't reliably
     have one to inject into, and Gradle merges multiple such blocks fine.
  2. compileSdk bumped to at least 36: flutter_plugin_android_lifecycle (a
     transitive dependency of several plugins, e.g. file_picker,
     geolocator_android) requires compiling against API 36+, but Flutter's
     own bundled `flutter.compileSdkVersion` default hasn't caught up to
     that yet. Uses max(flutter's default, 36) rather than hardcoding 36
     outright, so this stops being needed on its own once Flutter's
     default catches up.
"""
import pathlib
import sys

DESUGAR_JDK_LIBS_VERSION = "2.1.4"
MIN_COMPILE_SDK = 36

groovy_path = pathlib.Path("android/app/build.gradle")
kotlin_path = pathlib.Path("android/app/build.gradle.kts")


def patch_groovy(text: str) -> str:
    if "coreLibraryDesugaringEnabled" not in text:
        text = text.replace(
            "compileOptions {",
            "compileOptions {\n        coreLibraryDesugaringEnabled true",
            1,
        )
    if "coreLibraryDesugaring " not in text:
        text += (
            "\ndependencies {\n"
            f"    coreLibraryDesugaring 'com.android.tools:desugar_jdk_libs:{DESUGAR_JDK_LIBS_VERSION}'\n"
            "}\n"
        )
    if "compileSdkVersion flutter.compileSdkVersion" in text:
        text = text.replace(
            "compileSdkVersion flutter.compileSdkVersion",
            f"compileSdkVersion Math.max(flutter.compileSdkVersion, {MIN_COMPILE_SDK})",
            1,
        )
    assert "coreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring " in text
    assert "flutter.compileSdkVersion" not in text or f"Math.max(flutter.compileSdkVersion, {MIN_COMPILE_SDK})" in text
    return text


def patch_kotlin(text: str) -> str:
    if "isCoreLibraryDesugaringEnabled" not in text:
        text = text.replace(
            "compileOptions {",
            "compileOptions {\n        isCoreLibraryDesugaringEnabled = true",
            1,
        )
    if "coreLibraryDesugaring(" not in text:
        text += (
            "\ndependencies {\n"
            f'    coreLibraryDesugaring("com.android.tools:desugar_jdk_libs:{DESUGAR_JDK_LIBS_VERSION}")\n'
            "}\n"
        )
    if "compileSdk = flutter.compileSdkVersion" in text:
        text = text.replace(
            "compileSdk = flutter.compileSdkVersion",
            f"compileSdk = maxOf(flutter.compileSdkVersion, {MIN_COMPILE_SDK})",
            1,
        )
    assert "isCoreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring(" in text
    assert "flutter.compileSdkVersion" not in text or f"maxOf(flutter.compileSdkVersion, {MIN_COMPILE_SDK})" in text
    return text


if groovy_path.exists():
    path = groovy_path
    path.write_text(patch_groovy(path.read_text()))
    print(f"Patched {path}")
elif kotlin_path.exists():
    path = kotlin_path
    path.write_text(patch_kotlin(path.read_text()))
    print(f"Patched {path}")
else:
    sys.exit(
        "Neither android/app/build.gradle nor build.gradle.kts found - "
        "was the 'Scaffold Android platform' step run first?"
    )
