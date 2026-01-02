#!/usr/bin/env bash
set -euo pipefail

# Cake bootstrapper (dotnet tool)

dotnet tool restore

dotnet cake build.cake "$@"
