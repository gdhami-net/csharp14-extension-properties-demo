#!/usr/bin/env bash
# Asserts every claim the blog post makes about C# 14 extension properties.
# DOTNET can point at a .NET 10 SDK that is not on PATH; it defaults to `dotnet`.
set -u
cd "$(dirname "$0")"
DOTNET="${DOTNET:-dotnet}"
fails=0

pass() { echo "PASS  $1"; [ -n "${2:-}" ] && echo "      $2"; return 0; }
fail() { echo "FAIL  $1"; [ -n "${2:-}" ] && echo "      $2"; fails=$((fails + 1)); }

# Build output, one line per diagnostic, with the absolute paths cut off.
diagnostics() {
  "$DOTNET" build "$1" --no-incremental -nologo "${@:2}" 2>&1 \
    | grep -E ": (warning|error) [A-Z]+[0-9]+" \
    | sed -E 's/ \[[^]]*\]$//; s#^.*[\\/](probes|src)[\\/]#\1/#; s#\\#/#g' \
    | sort -u
}

# Exit code of a plain build, so "zero warnings" can't pass on a build that broke.
builds() { "$DOTNET" build "$1" -nologo >/dev/null 2>&1; }

sdk=$("$DOTNET" --version)
case "$sdk" in
  10.*) pass "SDK is .NET 10" "$sdk" ;;
  *) fail "SDK is .NET 10" "got $sdk"; echo "STOPPED"; exit 1 ;;
esac

if grep -rq --include="*.csproj" --include="*.props" "<LangVersion" src probes tests Directory.Build.props; then
  fail "no project sets LangVersion"
else
  pass "no project sets LangVersion: extension blocks compile with the net10.0 default (C# 14)"
fi

out=$(diagnostics src/Orders)
if builds src/Orders && [ -z "$out" ]; then pass "src/Orders builds with zero warnings, the classic IsOverdue() query included"; else fail "src/Orders builds with zero warnings" "$out"; fi

out=$(diagnostics probes/ExpressionTree)
if builds probes/ExpressionTree; then fail "probes/ExpressionTree must not build"; fi
expected="probes/ExpressionTree/OverdueQuery.cs(9,30): error CS9296: An expression tree may not contain an extension property access"
if [ "$out" = "$expected" ]; then pass "db.Orders.Where(o => o.IsOverdue) does not compile: exactly one CS9296" "$out"; else fail "db.Orders.Where(o => o.IsOverdue) does not compile: exactly one CS9296" "$out"; fi

out=$(diagnostics probes/Shadowing)
if builds probes/Shadowing && [ -z "$out" ]; then pass "real members next to extensions with the same names: zero warnings"; else fail "real members next to extensions with the same names: zero warnings" "$out"; fi

out=$("$DOTNET" run --project probes/Shadowing --no-build 2>&1 | tr -d '\r')
expected=$'invoice.IsOverdue = False\ninvoice.IsDue() = False'
if [ "$out" = "$expected" ]; then pass "and the real members win: neither extension's rule runs (property and classic method alike)" "$out"; else fail "and the real members win" "$out"; fi

echo "----- dotnet run --project src/Orders"
"$DOTNET" run --project src/Orders --no-build
echo "-----"

if "$DOTNET" test tests/Orders.Tests -nologo 2>&1 | tail -n 3 | grep -q "Passed!"; then
  pass "dotnet test: plain code, JSON, reflection, the classic method's run-time exception, the expression query"
else
  fail "dotnet test" "run '$DOTNET test tests/Orders.Tests' to see which one"
fi

if [ "$fails" -eq 0 ]; then echo "ALL CHECKS PASSED"; else echo "$fails CHECK(S) FAILED"; exit 1; fi
