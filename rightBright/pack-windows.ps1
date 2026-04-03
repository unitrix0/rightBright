<#
.SYNOPSIS
    Builds and packages rightBright for Windows using WiX v4.
.DESCRIPTION
    Publishes the app as a self-contained win-x64 binary, then uses the WiX v4
    CLI (wix build) to produce a Burn bundle bootstrapper (rightBrightSetup.exe).
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
$wixDir = "$PSScriptRoot/wix_installer"
$setupExeName = "rightBrightSetup.exe"
$msiName = "rightBright.msi"
$setupExePath = Join-Path $releasesDir $setupExeName
$msiPath = Join-Path $releasesDir $msiName

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

Write-Host "`n--- WiX harvest published files ---" -ForegroundColor Yellow
if (-not (Test-Path $releasesDir)) {
    New-Item -ItemType Directory -Path $releasesDir | Out-Null
}

if (-not (Test-Path $wixDir)) {
    Write-Error "WiX directory does not exist: $wixDir"
    exit 1
}

$publishedWxsPath = Join-Path $wixDir "PublishedFiles.wxs"
& "$wixDir/HarvestPublishedFiles.ps1" -PublishDir $publishDir -OutFile $publishedWxsPath
if ($LASTEXITCODE -ne 0) { Write-Error "Harvest failed"; exit 1 }

$productWxsPath = Join-Path $wixDir "Product.wxs"
$bundleWxsPath = Join-Path $wixDir "Bundle.wxs"

Write-Host "`n--- wix build (MSI) ---" -ForegroundColor Yellow
wix build `
    -d ProductVersion=$Version `
    -b $publishDir `
    -o $msiPath `
    $productWxsPath `
    $publishedWxsPath

if ($LASTEXITCODE -ne 0) { Write-Error "wix build (MSI) failed"; exit 1 }

Write-Host "`n--- wix build (Bundle) ---" -ForegroundColor Yellow
wix build `
    -d ProductVersion=$Version `
    -d MsiPath=$msiPath `
    -ext WixToolset.BootstrapperApplications.wixext `
    -o $setupExePath `
    $bundleWxsPath

if ($LASTEXITCODE -ne 0) { Write-Error "wix build (Bundle) failed"; exit 1 }

if (-not (Test-Path $setupExePath)) { Write-Error "Build did not produce $setupExePath"; exit 1 }

Remove-Item $msiPath -Force

Write-Host "`n=== Done! ===" -ForegroundColor Green
Write-Host "Installer: $setupExePath"
