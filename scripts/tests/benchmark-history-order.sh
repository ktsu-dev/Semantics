#!/usr/bin/env bash
# Checks that `benchmark-history.cs ingest` keeps the history in SemVer 2.0 precedence order,
# whatever order the versions arrive in (#515). A prerelease sorts before its own release, and
# prerelease identifiers compare numerically when numeric and ordinally otherwise.
set -euo pipefail

script="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/benchmark-history.cs"
work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

mkdir -p "$work/results"
cat > "$work/results/Fake-report-full.json" <<'JSON'
{
  "Benchmarks": [
    {
      "FullName": "Fake.Benchmarks.Work",
      "Parameters": "",
      "Statistics": { "Mean": 1.0 },
      "Memory": { "BytesAllocatedPerOperation": 0 }
    }
  ]
}
JSON

check() {
  local expected=$1
  shift
  rm -f "$work/history.json"
  for version in "$@"; do
    dotnet run "$script" -- ingest --history "$work/history.json" --results "$work/results" \
      --version "$version" --baseline-ns 1 >/dev/null
  done
  local actual
  actual=$(python3 -c 'import json,sys; print(" ".join(e["version"] for e in json.load(open(sys.argv[1]))["entries"]))' "$work/history.json")
  if [[ "$actual" != "$expected" ]]; then
    echo "FAIL: ingesting '$*' gave '$actual', expected '$expected'" >&2
    return 1
  fi
  echo "ok: $*  ->  $actual"
}

failed=0
check "5.11.0 5.11.1-pre.1 5.11.1" 5.11.1 5.11.1-pre.1 5.11.0 || failed=1
check "5.11.0 5.11.1-pre.1 5.11.1" 5.11.1-pre.1 5.11.0 5.11.1 || failed=1
check "1.0.0-pre.2 1.0.0-pre.10 1.0.0" 1.0.0 1.0.0-pre.10 1.0.0-pre.2 || failed=1
check "1.0.0-alpha 1.0.0-alpha.1 1.0.0-beta 1.0.0-rc.1 1.0.0" 1.0.0-rc.1 1.0.0 1.0.0-beta 1.0.0-alpha.1 1.0.0-alpha || failed=1
check "1.0.0-1 1.0.0-alpha 1.0.0" 1.0.0-alpha 1.0.0 1.0.0-1 || failed=1
check "nightly 3.3.1 4.0.0 5.3.10" 5.3.10 4.0.0 nightly 3.3.1 || failed=1
exit "$failed"
