#!/usr/bin/env python3
"""Forces every third-party plugin's own compileSdk to at least 36.

Three earlier attempts at this fix (overriding :app's own build.gradle,
overriding android/local.properties, then patching each plugin's own
build.gradle but only for one specific syntax pattern) all failed to
change anything for :file_picker's "checkReleaseAarMetadata" failure. A
diagnostic step that printed the real, generated files revealed why: it's
not a matter of *which* file to edit (this script was already looking at
the right one) but of matching the *actual syntax*. Real examples pulled
straight from a CI run's pub cache:

    file_picker/android/build.gradle:              compileSdk 34
    connectivity_plus/android/build.gradle:         compileSdk 34
    flutter_secure_storage/android/build.gradle:    compileSdk 34
    geolocator_android/android/build.gradle:        compileSdk flutter.compileSdkVersion
    firebase_messaging/android/build.gradle:        compileSdkVersion project.ext.compileSdk
    path_provider_android/.../build.gradle.kts:     compileSdk = flutter.compileSdkVersion

So plugin authors set this line every which way: with or without the
"Version" suffix, with or without "=", and the right-hand side is
sometimes a hardcoded literal (not tied to Flutter at all - several
plugins just wrote "34" directly), sometimes `flutter.compileSdkVersion`,
sometimes their own custom ext property. The previous version of this
script only matched one specific "compileSdkVersion flutter.compileSdkVersion"
shape and silently skipped every other one - which is nearly all of them.

Rather than pattern-match specific right-hand sides, this now matches the
line generically - "compileSdk" or "compileSdkVersion", optionally
"= ", then whatever expression follows - and wraps THAT expression in
max(expression, 36), for both Groovy (Math.max) and Kotlin DSL (maxOf).
That works uniformly whether the expression is a literal or a property
reference, and is idempotent (a line already wrapped in Math.max(/maxOf( is
left alone, so re-running this script is harmless).

Runs after `flutter pub get` (once the pub cache is actually populated),
patching only each plugin's own top-level android/build.gradle(.kts) - not
its example/ subfolder, which isn't part of this project's build at all.
"""
import os
import pathlib
import re

MIN_COMPILE_SDK = 36

pub_cache = pathlib.Path(os.environ.get("PUB_CACHE", str(pathlib.Path.home() / ".pub-cache")))
hosted_dirs = list(pub_cache.glob("hosted/*/*"))  # hosted/pub.dev/<package>-<version>/

# Matches a real "compileSdk 34" / "compileSdk flutter.compileSdkVersion" /
# "compileSdkVersion project.ext.compileSdk" line (Groovy: no "=", whitespace
# separates the keyword from whatever expression follows) - anchored to the
# start of the line (after leading whitespace) so it can't match the keyword
# appearing inside a comment or string elsewhere on a line.
GROOVY_LINE = re.compile(r"^(?P<indent>[ \t]*)compileSdk(?P<suffix>Version)?(?P<sep>[ \t]+)(?P<rhs>\S.*?)[ \t]*$", re.MULTILINE)
# Matches Kotlin DSL's "compileSdk = <expression>" (always "=", never the
# "Version" suffix).
KOTLIN_LINE = re.compile(r"^(?P<indent>[ \t]*)compileSdk(?P<sep>[ \t]*=[ \t]*)(?P<rhs>\S.*?)[ \t]*$", re.MULTILINE)


def patch_groovy(text: str) -> str:
    def repl(m: re.Match) -> str:
        rhs = m.group("rhs")
        if "Math.max(" in rhs:
            return m.group(0)  # already patched - leave alone (idempotent)
        suffix = m.group("suffix") or ""
        return f"{m.group('indent')}compileSdk{suffix}{m.group('sep')}Math.max({rhs}, {MIN_COMPILE_SDK})"

    return GROOVY_LINE.sub(repl, text)


def patch_kotlin(text: str) -> str:
    def repl(m: re.Match) -> str:
        rhs = m.group("rhs")
        if "maxOf(" in rhs:
            return m.group(0)  # already patched - leave alone (idempotent)
        return f"{m.group('indent')}compileSdk{m.group('sep')}maxOf({rhs}, {MIN_COMPILE_SDK})"

    return KOTLIN_LINE.sub(repl, text)


patched = []
for pkg_dir in hosted_dirs:
    for name, patch_fn in (
        ("build.gradle", patch_groovy),
        ("build.gradle.kts", patch_kotlin),
    ):
        build_file = pkg_dir / "android" / name
        if not build_file.exists():
            continue
        original = build_file.read_text()
        new_text = patch_fn(original)
        if new_text != original:
            build_file.write_text(new_text)
            patched.append(str(build_file))

print(f"Patched {len(patched)} plugin build.gradle file(s) to compileSdk >= {MIN_COMPILE_SDK}:")
for p in patched:
    print(f"  - {p}")
