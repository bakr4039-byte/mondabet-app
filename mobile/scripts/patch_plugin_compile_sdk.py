#!/usr/bin/env python3
"""Forces every third-party plugin's own compileSdk to at least 36.

Real examples pulled straight from a CI run's pub cache show plugin
authors set this line every which way:

    file_picker/android/build.gradle:              compileSdk 34
    connectivity_plus/android/build.gradle:         compileSdk 34
    geolocator_android/android/build.gradle:        compileSdk flutter.compileSdkVersion
    firebase_messaging/android/build.gradle:        compileSdkVersion project.ext.compileSdk
    local_auth_android/android/build.gradle:        compileSdk = flutter.compileSdkVersion   (!)
    path_provider_android/.../build.gradle.kts:     compileSdk = flutter.compileSdkVersion

The local_auth_android line above is the trap: it's a plain Groovy
build.gradle (not .kts), but still uses "=" - so "the separator tells you
which DSL you're in" is false. An earlier version of this script assumed
Groovy files never use "=" and Kotlin files always do, so its Groovy regex
only accepted whitespace as the separator; against local_auth_android's
"compileSdk = flutter.compileSdkVersion" it matched "=" as the start of
the right-hand side and produced "compileSdk Math.max(=
flutter.compileSdkVersion, 36)" - which crashed Gradle's Groovy parser
outright ("Unexpected input: '{'" a few lines later, since the malformed
expression threw off parsing for the rest of the file).

Fixed by not trying to reconstruct the separator at all: the regex now
accepts either whitespace-only or "="-flanked-by-optional-whitespace as
the separator, captures it verbatim, and copies it back unchanged in the
replacement - only the right-hand side expression gets touched, wrapped
in max(expression, 36) (Math.max for Groovy .gradle files, maxOf for
Kotlin .kts files, chosen by file extension, never by what the separator
looked like). This is idempotent (a line already wrapped in Math.max(/
maxOf( is left alone) and works for both a hardcoded literal and any kind
of property reference, since it never has to recognize the expression
itself - it just wraps whatever is there.

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

# Matches "compileSdk 34", "compileSdkVersion flutter.compileSdkVersion",
# "compileSdk = flutter.compileSdkVersion", etc. - the separator (group
# "sep") is captured verbatim and never reconstructed, since Groovy files
# can use either a bare space or "=" and Kotlin files always use "=";
# guessing the separator from the file type is what broke local_auth_android.
# Anchored to the start of the line (after leading whitespace) so it can't
# match "compileSdk" appearing inside a comment elsewhere on the line.
LINE = re.compile(
    r"^(?P<indent>[ \t]*)compileSdk(?P<suffix>Version)?"
    r"(?P<sep>[ \t]*=[ \t]*|[ \t]+)(?P<rhs>\S.*?)[ \t]*$",
    re.MULTILINE,
)


def _patch(text: str, wrap_prefix: str) -> str:
    marker = f"{wrap_prefix}("

    def repl(m: re.Match) -> str:
        rhs = m.group("rhs")
        if marker in rhs:
            return m.group(0)  # already patched - leave alone (idempotent)
        suffix = m.group("suffix") or ""
        return (
            f"{m.group('indent')}compileSdk{suffix}{m.group('sep')}"
            f"{wrap_prefix}({rhs}, {MIN_COMPILE_SDK})"
        )

    return LINE.sub(repl, text)


def patch_groovy(text: str) -> str:
    return _patch(text, "Math.max")


def patch_kotlin(text: str) -> str:
    return _patch(text, "maxOf")


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
