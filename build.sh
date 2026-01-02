#!/usr/bin/env bash
set -euo pipefail

# Cake bootstrapper (dotnet tool)

DOTNET_PATH="$(command -v dotnet || true)"
if [[ -z "${DOTNET_PATH}" ]]; then
  echo "dotnet not found on PATH" >&2
  exit 1
fi

# Run all dotnet invocations via the resolved dotnet path so we don't rely on /usr/local/bin/dotnet.
"${DOTNET_PATH}" tool restore

"${DOTNET_PATH}" cake build.cake "$@"
