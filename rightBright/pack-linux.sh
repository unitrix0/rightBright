#!/usr/bin/env bash
#
# Builds and packages rightBright as a Flatpak for Linux.
#
# Usage:
#   ./pack-linux.sh [--version 0.9.0]
#
# Prerequisites:
#   - .NET SDK 9 (dotnet)
#   - flatpak & flatpak-builder

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR/rightBright"
CSPROJ="$PROJECT_DIR/rightBright.csproj"
PUBLISH_DIR="$SCRIPT_DIR/publish/linux-x64"
RELEASES_DIR="$SCRIPT_DIR/releases"
FLATPAK_DIR="$SCRIPT_DIR/flatpak"
MANIFEST="$FLATPAK_DIR/io.github.unitrix0.rightBright.yml"
APP_ID="io.github.unitrix0.rightBright"
BUILD_DIR="$SCRIPT_DIR/.flatpak-build"
REPO_DIR="$SCRIPT_DIR/.flatpak-repo"

RUNTIME="org.freedesktop.Platform//24.08"
SDK="org.freedesktop.Sdk//24.08"

# ---------------------------------------------------------------------------
# Parse arguments
# ---------------------------------------------------------------------------
VERSION=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --version) VERSION="$2"; shift 2 ;;
        *) echo "Unknown option: $1"; exit 1 ;;
    esac
done

if [[ -z "$VERSION" ]]; then
    VERSION=$(grep -oPm1 '(?<=<Version>)[^<]+' "$CSPROJ" || true)
    if [[ -z "$VERSION" ]]; then
        echo "Error: No version specified and none found in .csproj. Use --version 1.0.0"
        exit 1
    fi
fi

echo "=== Building rightBright v$VERSION for Linux (linux-x64) ==="

# ---------------------------------------------------------------------------
# 1. Stamp version into metainfo.xml
# ---------------------------------------------------------------------------
METAINFO="$FLATPAK_DIR/io.github.unitrix0.rightBright.metainfo.xml"
TODAY=$(date +%Y-%m-%d)
echo ""
echo "--- Updating metainfo.xml (version=$VERSION, date=$TODAY) ---"
sed -i "s/<release version=\"[^\"]*\" date=\"[^\"]*\"/<release version=\"$VERSION\" date=\"$TODAY\"/" "$METAINFO"

# ---------------------------------------------------------------------------
# 2. dotnet publish
# ---------------------------------------------------------------------------
echo ""
echo "--- dotnet publish ---"
dotnet publish "$CSPROJ" \
    -c Release \
    -r linux-x64 \
    --self-contained \
    -p:Version="$VERSION" \
    -o "$PUBLISH_DIR"

# ---------------------------------------------------------------------------
# 3. Ensure Flatpak runtime & SDK are installed
# ---------------------------------------------------------------------------
echo ""
echo "--- Checking Flatpak runtime & SDK ---"
if ! flatpak info "$RUNTIME" &>/dev/null; then
    echo "Installing $RUNTIME ..."
    flatpak install -y --noninteractive flathub "$RUNTIME"
fi
if ! flatpak info "$SDK" &>/dev/null; then
    echo "Installing $SDK ..."
    flatpak install -y --noninteractive flathub "$SDK"
fi

# ---------------------------------------------------------------------------
# 4. Build the Flatpak
# ---------------------------------------------------------------------------
echo ""
echo "--- flatpak-builder ---"
flatpak-builder --force-clean --disable-rofiles-fuse "$BUILD_DIR" "$MANIFEST"

# ---------------------------------------------------------------------------
# 5. Export to local repo & create bundle
# ---------------------------------------------------------------------------
echo ""
echo "--- Exporting to local repo ---"
flatpak-builder --repo="$REPO_DIR" --force-clean --disable-rofiles-fuse "$BUILD_DIR" "$MANIFEST"

mkdir -p "$RELEASES_DIR"
BUNDLE_PATH="$RELEASES_DIR/rightBright-${VERSION}-linux-x64.flatpak"

echo ""
echo "--- Creating bundle ---"
flatpak build-bundle "$REPO_DIR" "$BUNDLE_PATH" "$APP_ID"

echo ""
echo "=== Done! ==="
echo "Flatpak bundle: $BUNDLE_PATH"
