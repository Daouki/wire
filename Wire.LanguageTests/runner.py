#!/usr/bin/env python3

import os
import subprocess
import sys

from importlib import reload
from pathlib import Path

this_dir = os.path.dirname(os.path.realpath(__file__))
root_dir = Path.resolve(Path(this_dir).parent)
wirec_dir = os.path.join(root_dir, "WireC.Driver")

wirec_exe = os.path.join(wirec_dir, "exe", "wirec",)


subprocess.run(["dotnet", "publish", wirec_dir, "/p:PublishProfile=PortableExe"],
               stdout=subprocess.DEVNULL)


verbose = False
only_failed = False
for arg in sys.argv:
    if arg == "-v":
        verbose = True
    if arg == "-f":
        only_failed = True


def compile_file(file, should_succeed=True):
    partial_file_path = get_partial_test_file_path(file, this_dir)

    compiler_output = [0, ""]
    try:
        compiler_output = subprocess.getstatusoutput([wirec_exe, file])
        if (compiler_output[0] == 0) == should_succeed:
            print(f"[ \u001b[32mOK\u001b[0m ] [COMPTIME] {partial_file_path}")
            return True
        else:
            fail_compile_test(partial_file_path, compiler_output[1])
    except UnicodeDecodeError:
        msg = "Internal runner error: Python cannot into unicode..."
        fail_compile_test(partial_file_path, msg)


def fail_compile_test(test_file, compiler_output):
    if verbose:
        print("")
    print(f"[\u001b[31mFAIL\u001b[0m] [COMPTIME] {test_file}")
    if verbose:
        print(compiler_output.rstrip())
    return False


def get_partial_test_file_path(abs_file_path, tests_path):
    return abs_file_path[len(tests_path) + 1:]


tests_ran = 0
tests_failed = 0
previous_test_failed = False


comptime_successes_path = os.path.join(this_dir, "comptime", "successes")
for test_file in [str(path) for path in Path(comptime_successes_path).rglob("*.wire")]:
    tests_ran += 1
    if previous_test_failed and verbose:
        print()

    if compile_file(test_file):
        previous_test_failed = False
    else:
        tests_failed += 1
        previous_test_failed = True


comptime_failures_path = os.path.join(this_dir, "comptime", "failures")
for test_file in [str(path) for path in Path(comptime_failures_path).rglob("*.wire")]:
    tests_ran += 1
    if previous_test_failed and verbose:
        print()

    if compile_file(test_file, False):
        previous_test_failed = False
    else:
        tests_failed += 1
        previous_test_failed = True


runtime_path = os.path.join(this_dir, "runtime")
for test_file in [str(path) for path in Path(runtime_path).rglob("*.wire")]:
    tests_ran += 1
    if previous_test_failed and verbose:
        print()

    if not compile_file(test_file):
        tests_failed += 1
        previous_test_failed = True
        continue

    partial_file_path = get_partial_test_file_path(test_file, this_dir)
    program_output = subprocess.getstatusoutput(["out"])
    if program_output[0] == 0:
        print(f"[ \u001b[32mOK\u001b[0m ] [RUNTIME ] {partial_file_path}")
        previous_test_failed = False
    else:
        if verbose:
            print()
        print(f"[\u001b[31mFAIL\u001b[0m] [RUNTIME ] {partial_file_path}")
        if verbose:
            print(program_output[1])
        tests_failed += 1
        previous_test_failed = True


tests_passed = tests_ran - tests_failed
print(f"\nRan {tests_ran} tests, {tests_passed} passed, {tests_failed} failed")
sys.exit(0 if tests_failed == 0 else 1)
