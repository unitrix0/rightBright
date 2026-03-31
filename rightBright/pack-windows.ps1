<#
.SYNOPSIS
    Builds and packages rightBright for Windows using WiX.
.DESCRIPTION
    Publishes the app as a self-contained win-x64 binary, then uses WiX to
    produce an MSI + a Burn bundle bootstrapper (rightBrightSetup.exe).
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

Write-Host "`n--- WiX harvest published files ---" -ForegroundColor Yellow
if (-not (Test-Path $releasesDir)) {
    New-Item -ItemType Directory -Path $releasesDir | Out-Null
}

if (-not (Test-Path $wixDir)) {
    Write-Error "WiX directory does not exist: $wixDir"
    exit 1
}

$publishedWxsPath = Join-Path $wixDir "PublishedFiles.wxs"
& "$wixDir\\HarvestPublishedFiles.ps1" -PublishDir $publishDir -OutFile $publishedWxsPath
if ($LASTEXITCODE -ne 0) { Write-Error "heat harvest failed"; exit 1 }

Write-Host "`n--- Detect rightBright.exe File Id (for CustomAction FileRef) ---" -ForegroundColor Yellow
$publishedWxsContent = Get-Content $publishedWxsPath -Raw
$fileIdMatch = [regex]::Match($publishedWxsContent, '<File\s+Id="([^"]+)"[^>]*rightBright\.exe', 'IgnoreCase')
if (-not $fileIdMatch.Success) {
    Write-Error "Failed to detect File Id for rightBright.exe in $publishedWxsPath"
    exit 1
}
$rightBrightExeFileId = $fileIdMatch.Groups[1].Value
Write-Host "Detected FileRef id: $rightBrightExeFileId" -ForegroundColor Green

$objDir = Join-Path $wixDir "obj"
if (-not (Test-Path $objDir)) {
    New-Item -ItemType Directory -Path $objDir | Out-Null
}

$productWxsPath = Join-Path $wixDir "Product.wxs"
$bundleWxsPath = Join-Path $wixDir "Bundle.wxs"

Write-Host "`n--- candle (MSI) ---" -ForegroundColor Yellow
& candle -nologo `
    -dProductVersion=$Version `
    -dRightBrightExeFileId=$rightBrightExeFileId `
    -out $objDir `
    $productWxsPath `
    $publishedWxsPath

if ($LASTEXITCODE -ne 0) { Write-Error "candle (MSI) failed"; exit 1 }

Write-Host "`n--- light (MSI) ---" -ForegroundColor Yellow
$productObj = Join-Path $objDir "Product.wixobj"
$publishedObj = Join-Path $objDir "PublishedFiles.wixobj"
& light -nologo `
    -out $msiPath `
    $productObj `
    $publishedObj

if ($LASTEXITCODE -ne 0) { Write-Error "light (MSI) failed"; exit 1 }

Write-Host "`n--- candle (Bundle) ---" -ForegroundColor Yellow
$bundleObjDir = Join-Path $wixDir "bundle_obj"
if (-not (Test-Path $bundleObjDir)) {
    New-Item -ItemType Directory -Path $bundleObjDir | Out-Null
}

& candle -nologo `
    -dProductVersion=$Version `
    -dMsiPath=$msiPath `
    -out $bundleObjDir `
    $bundleWxsPath

if ($LASTEXITCODE -ne 0) { Write-Error "candle (Bundle) failed"; exit 1 }

Write-Host "`n--- light (Bundle) ---" -ForegroundColor Yellow
$bundleObj = Join-Path $bundleObjDir "Bundle.wixobj"
& light -nologo `
    -out $setupExePath `
    $bundleObj

if ($LASTEXITCODE -ne 0) { Write-Error "light (Bundle) failed"; exit 1 }

if (-not (Test-Path $setupExePath)) { Write-Error "Build did not produce $setupExePath"; exit 1 }

Write-Host "`n=== Done! ===" -ForegroundColor Green
Write-Host "Installer outputs:" -ForegroundColor Green
Write-Host "  MSI : $msiPath"
Write-Host "  Setup.exe : $setupExePath"
