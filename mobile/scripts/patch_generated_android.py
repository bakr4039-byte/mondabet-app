#!/usr/bin/env python3
"""Patches the freshly-scaffolded mobile/android/ project.

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
  2. flutter.compileSdkVersion raised to 36 in android/local.properties.
     flutter_plugin_android_lifecycle (a transitive dependency of several
     plugins - file_picker, geolocator_android, etc.) requires compiling
     against API 36+, but Flutter's own bundled default hasn't caught up
     to that yet. `flutter.compileSdkVersion` is a single project-wide
     value read from local.properties by the Flutter Gradle plugin and
     shared by every subproject - including third-party plugins pulled
     from the pub cache, which we can't edit directly since they're not
     part of this repo. Overriding just android/app/build.gradle's own
     compileSdk (an earlier version of this fix) only changed the app
     module's value, not this shared one, so plugin subprojects like
     :file_picker kept compiling against the old default regardless -
     local.properties is the one override point that reaches all of them.
"""
import pathlib
import re
import sys

DESUGAR_JDK_LIBS_VERSION = "2.1.4"
MIN_COMPILE_SDK = 36

groovy_path = pathlib.Path("android/app/build.gradle")
kotlin_path = pathlib.Path("android/app/build.gradle.kts")
local_properties_path = pathlib.Path("android/local.properties")


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
    assert "coreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring " in text
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
    assert "isCoreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring(" in text
    return text


def patch_local_properties(text: str) -> str:
    line = f"flutter.compileSdkVersion={MIN_COMPILE_SDK}"
    if re.search(r"^flutter\.compileSdkVersion=", text, flags=re.MULTILINE):
        text = re.sub(r"^flutter\.compileSdkVersion=.*$", line, text, flags=re.MULTILINE)
    else:
        if text and not text.endswith("\n"):
            text += "\n"
        text += line + "\n"
    assert line in text
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

if not local_properties_path.exists():
    sys.exit(
        "android/local.properties not found - was the 'Scaffold Android "
        "platform' step run first?"
    )
local_properties_path.write_text(patch_local_properties(local_properties_path.read_text()))
print(f"Patched {local_properties_path} (flutter.compileSdkVersion={MIN_COMPILE_SDK})")
