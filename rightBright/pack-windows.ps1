<#
.SYNOPSIS
    Builds and packages rightBright for Windows using Velopack.
.DESCRIPTION
    Publishes the app as a self-contained win-x64 binary, then uses vpk to
    produce a Setup.exe installer and update packages in the ./releases folder.
.PARAMETER Version
    Semantic version for this release (default: reads from .csproj Version property).
#>
param(
    [string]$Version
)

$ErrorActionPreference = "Stop"

$projectDir = "$PSScriptRoot/rightBright"
$publishDir = "$PSScriptRoot/publish/win-x64"
$releasesDir = "$PSScriptRoot/releases"
$packId = "rightBright"
$mainExe = "rightBright.exe"

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
    -o $publishDir

if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish failed"; exit 1 }

Write-Host "`n--- vpk pack ---" -ForegroundColor Yellow
vpk pack `
    --packId $packId `
    --packVersion $Version `
    --packDir $publishDir `
    --mainExe $mainExe `
    --outputDir $releasesDir

if ($LASTEXITCODE -ne 0) { Write-Error "vpk pack failed"; exit 1 }

Write-Host "`n=== Done! ===" -ForegroundColor Green
Write-Host "Installer and update files are in: $releasesDir"
