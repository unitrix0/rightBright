<#
.SYNOPSIS
    Builds and packages rightBright for Windows using Inno Setup.
.DESCRIPTION
    Publishes the app as a self-contained win-x64 binary, then uses the Inno
    Setup compiler (iscc) to produce the installer (rightBrightSetup.exe).
.PARAMETER Version
    Semantic version for this release (default: reads from .csproj Version property).
#>
param(
    [string]$Version
)

# Self-elevate if not running as Administrator (skipped in CI environments)
if (-not $env:CI -and
    -not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()
        ).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Start-Process powershell.exe `
        -Verb RunAs `
        -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" $(if ($Version) { "-Version $Version" })"
    exit
}

$ErrorActionPreference = "Stop"

$projectDir = "$PSScriptRoot/rightBright"
$publishDir = "$PSScriptRoot/publish/win-x64"
$releasesDir = "$PSScriptRoot/releases"
$innoDir = "$PSScriptRoot/inno_installer"
$setupExeName = "rightBrightSetup.exe"
$setupExePath = Join-Path $releasesDir $setupExeName

if (-not $Version) {
    [xml]$csproj = Get-Content "$projectDir/rightBright.csproj"
    $Version = $csproj.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
    if (-not $Version) {
        Write-Error "No version specified and none found in .csproj. Use -Version 1.0.0"
        exit 1
    }
}

Write-Host "=== Building rightBright v$Version for Windows (win-x64) ===" -ForegroundColor Cyan

Write-Host "`n--- dotnet publish ---" -ForegroundColor Yellow
dotnet publish "$projectDir/rightBright.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:Version=$Version `
    -o $publishDir

if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish failed"; exit 1 }

$amd64Yapi = Join-Path $publishDir "amd64\yapi.dll"
if (Test-Path $amd64Yapi) {
    Write-Host "Replacing 32-bit yapi.dll with 64-bit variant" -ForegroundColor Yellow
    Copy-Item $amd64Yapi -Destination (Join-Path $publishDir "yapi.dll") -Force
    Remove-Item (Join-Path $publishDir "amd64") -Recurse -Force
}

if (-not (Test-Path $releasesDir)) {
    New-Item -ItemType Directory -Path $releasesDir | Out-Null
}

$issPath = Join-Path $innoDir "rightBright.iss"
if (-not (Test-Path $issPath)) {
    Write-Error "Inno Setup script not found: $issPath"
    exit 1
}

Write-Host "`n--- Inno Setup (iscc) ---" -ForegroundColor Yellow
iscc /DAppVersion=$Version $issPath

if ($LASTEXITCODE -ne 0) { Write-Error "Inno Setup build failed"; exit 1 }

if (-not (Test-Path $setupExePath)) { Write-Error "Build did not produce $setupExePath"; exit 1 }

Write-Host "`n=== Done! ===" -ForegroundColor Green
Write-Host "Installer: $setupExePath"
