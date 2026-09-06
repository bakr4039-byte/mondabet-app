#!/usr/bin/env python3
"""Forces every third-party plugin's own compileSdk to at least 36.

Two earlier attempts at this fix (overriding :app's own build.gradle, then
overriding android/local.properties) both failed to change anything for
:file_picker's "checkReleaseAarMetadata" failure, because that check is
against :file_picker's OWN compileSdk - and :file_picker's build.gradle
isn't part of this repo at all. It's a third-party plugin, downloaded by
`flutter pub get` into the pub cache (not committed, just like
mobile/android/ itself), and its own build.gradle sets its compileSdk from
the same shared `flutter.compileSdkVersion` extension property that
android/app/build.gradle and android/local.properties were edited to
override - except neither edit actually changes what that property
resolves to inside a plugin's own build.gradle (its exact resolution
mechanism turned out not to be documented/stable enough to rely on).

Rather than keep guessing at which override point actually reaches plugin
subprojects, this sidesteps the question entirely: it runs after
`flutter pub get` (so the pub cache is populated) and directly rewrites
every downloaded plugin's own android/build.gradle(.kts) wherever it
references `flutter.compileSdkVersion`, replacing it with
max(flutter.compileSdkVersion, 36). This fixes the current :file_picker
failure and any other plugin that hits the same AAR-metadata check later,
without depending on Flutter's internal property-resolution behavior at
all - it edits each plugin's own file directly, the same way the app's own
android/app/build.gradle was already being edited.
"""
import os
import pathlib
import re

MIN_COMPILE_SDK = 36

pub_cache = pathlib.Path(os.environ.get("PUB_CACHE", str(pathlib.Path.home() / ".pub-cache")))
hosted_dirs = list(pub_cache.glob("hosted/*/*"))  # hosted/pub.dev/<package>-<version>/

GROOVY_PATTERN = re.compile(r"compileSdkVersion\s+flutter\.compileSdkVersion")
KOTLIN_PATTERN = re.compile(r"compileSdk\s*=\s*flutter\.compileSdkVersion")

patched = []
for pkg_dir in hosted_dirs:
    for name, pattern, replacement in (
        (
            "build.gradle",
            GROOVY_PATTERN,
            f"compileSdkVersion Math.max(flutter.compileSdkVersion, {MIN_COMPILE_SDK})",
        ),
        (
            "build.gradle.kts",
            KOTLIN_PATTERN,
            f"compileSdk = maxOf(flutter.compileSdkVersion, {MIN_COMPILE_SDK})",
        ),
    ):
        build_file = pkg_dir / "android" / name
        if not build_file.exists():
            continue
        text = build_file.read_text()
        new_text, count = pattern.subn(replacement, text)
        if count:
            build_file.write_text(new_text)
            patched.append(str(build_file))

print(f"Patched {len(patched)} plugin build.gradle file(s) to compileSdk >= {MIN_COMPILE_SDK}:")
for p in patched:
    print(f"  - {p}")
