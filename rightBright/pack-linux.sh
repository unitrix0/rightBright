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
# 1. dotnet publish
# ---------------------------------------------------------------------------
echo ""
echo "--- dotnet publish ---"
dotnet publish "$CSPROJ" \
    -c Release \
    -r linux-x64 \
    --self-contained \
    -o "$PUBLISH_DIR"

# ---------------------------------------------------------------------------
# 2. Ensure Flatpak runtime & SDK are installed
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
# 3. Build the Flatpak
# ---------------------------------------------------------------------------
echo ""
echo "--- flatpak-builder ---"
flatpak-builder --force-clean "$BUILD_DIR" "$MANIFEST"

# ---------------------------------------------------------------------------
# 4. Export to local repo & create bundle
# ---------------------------------------------------------------------------
echo ""
echo "--- Exporting to local repo ---"
flatpak-builder --repo="$REPO_DIR" --force-clean "$BUILD_DIR" "$MANIFEST"

mkdir -p "$RELEASES_DIR"
BUNDLE_PATH="$RELEASES_DIR/rightBright-${VERSION}-linux-x64.flatpak"

echo ""
echo "--- Creating bundle ---"
flatpak build-bundle "$REPO_DIR" "$BUNDLE_PATH" "$APP_ID"

# ---------------------------------------------------------------------------
# 5. Install udev rule for Yoctopuce USB sensors
# ---------------------------------------------------------------------------
UDEV_RULE="$PROJECT_DIR/Assets/udev_rule/99-yoctopuce.rules"
echo ""
echo "--- Installing udev rule (requires sudo) ---"
sudo install -Dm644 "$UDEV_RULE" /etc/udev/rules.d/99-yoctopuce.rules
sudo udevadm control --reload-rules
sudo udevadm trigger

# ---------------------------------------------------------------------------
# 6. Install the Flatpak bundle & launch the app
# ---------------------------------------------------------------------------
echo ""
echo "--- Installing Flatpak bundle ---"
flatpak install --user -y "$BUNDLE_PATH"

echo ""
echo "=== Done! ==="
echo "Flatpak bundle: $BUNDLE_PATH"

echo ""
echo "--- Launching rightBright ---"
flatpak run "$APP_ID" &
