param(
    [Parameter(Mandatory = $true)]
    [string]$PublishDir,

    [Parameter(Mandatory = $false)]
    [string]$OutFile = "$(Split-Path -Parent $PSCommandPath)\PublishedFiles.wxs"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -Path $PublishDir)) {
    throw "PublishDir does not exist: $PublishDir"
}

Write-Host "Harvesting published files -> $OutFile" -ForegroundColor Cyan

# heat.exe options:
# -cg PublishedComponents: ComponentGroup id expected by Product.wxs
# -dr INSTALLDIR: Install into the INSTALLDIR directory defined in Product.wxs
# -gg: generate GUIDs for components
# -scom / -sreg / -sfrag: reduce clutter in output
#
# Note: Requires WiX Toolset to be installed (heat.exe on PATH).
heat dir "$PublishDir" `
    -cg PublishedComponents `
    -dr INSTALLDIR `
    -gg `
    -scom -sreg -sfrag `
    -out "$OutFile"

