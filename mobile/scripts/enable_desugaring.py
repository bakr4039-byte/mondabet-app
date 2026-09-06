#!/usr/bin/env python3
"""Enables Android core library desugaring in the freshly-scaffolded
android/app/build.gradle(.kts).

flutter_local_notifications (used for local notification scheduling) needs
this enabled - see https://developer.android.com/studio/write/java8-support.html -
but `flutter create` does not turn it on by default. Since mobile/android/
isn't committed to the repo (it's generated fresh on every CI run - see
.github/workflows/build-apk.yml), this script runs right after the
`flutter create --platforms=android .` scaffolding step to patch the
generated file in place. It's idempotent (safe to run more than once) and
handles both of Flutter's current templates: Groovy (build.gradle) and
Kotlin DSL (build.gradle.kts).

Two separate things have to happen, in two different Gradle blocks:
  1. `compileOptions { coreLibraryDesugaringEnabled true }` - turns the
     feature on. Flutter's template always has a `compileOptions {}` block
     (it sets source/targetCompatibility), so this is a safe in-place edit.
  2. A dependency on the desugaring runtime library, in the
     `coreLibraryDesugaring` configuration. Flutter's newer templates don't
     always have an existing top-level `dependencies {}` block to inject
     into (unlike compileOptions) - trying to find-and-replace one silently
     does nothing when it's absent. Gradle allows more than one
     `dependencies {}` block per file (they're merged), so instead of
     searching for one, this just appends a brand new block at the end of
     the file. That works whether or not the template already had one.
"""
import pathlib
import sys

DESUGAR_JDK_LIBS_VERSION = "2.1.4"

groovy_path = pathlib.Path("android/app/build.gradle")
kotlin_path = pathlib.Path("android/app/build.gradle.kts")

if groovy_path.exists():
    path = groovy_path
    text = path.read_text()

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

    path.write_text(text)
    assert "coreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring " in text
    print(f"Patched {path} for core library desugaring")

elif kotlin_path.exists():
    path = kotlin_path
    text = path.read_text()

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

    path.write_text(text)
    assert "isCoreLibraryDesugaringEnabled" in text and "coreLibraryDesugaring(" in text
    print(f"Patched {path} for core library desugaring")

else:
    sys.exit(
        "Neither android/app/build.gradle nor build.gradle.kts found - "
        "was the 'Scaffold Android platform' step run first?"
    )
