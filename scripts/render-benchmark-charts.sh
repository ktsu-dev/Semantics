#!/usr/bin/env bash
# Redraws every benchmark chart from its committed history.
#
# The one render invocation both workflows run: benchmark-history.yml after it records a point,
# and verify-charts.yml to check that the committed charts are what the committed histories draw.
# Sharing it is what keeps the guard and the recorder from drifting apart (#303).
#
# Reads the subject table from $SUBJECTS, one `name|history|chart|filter` line per subject, as
# benchmark-history.yml declares it. The filter is not used here.
set -euo pipefail

: "${SUBJECTS:?SUBJECTS must hold the subject table from benchmark-history.yml}"

while IFS='|' read -r subject history chart _filter; do
  [[ -n "$subject" ]] || continue
  if [[ ! -f "$history" ]]; then
    echo "No history for $subject yet; nothing to draw."
    continue
  fi
  # stdin from /dev/null: a process that drained the here-string would end the loop early and
  # still exit zero, silently skipping the remaining subjects.
  dotnet run scripts/benchmark-history.cs -- render \
    --subject "$subject" --history "$history" --out "$chart" </dev/null
done <<< "$SUBJECTS"
