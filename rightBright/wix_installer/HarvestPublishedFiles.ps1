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

$publishDirFull = (Resolve-Path $PublishDir).Path.TrimEnd('\', '/')
$allFiles = Get-ChildItem -Path $publishDirFull -Recurse -File

$rootFiles = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
$subDirFiles = [ordered]@{}

foreach ($f in $allFiles) {
    $relPath = $f.FullName.Substring($publishDirFull.Length + 1)
    $relDir  = Split-Path $relPath -Parent

    if ([string]::IsNullOrEmpty($relDir)) {
        $rootFiles.Add($f)
    } else {
        if (-not $subDirFiles.Contains($relDir)) {
            $subDirFiles[$relDir] = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
        }
        $subDirFiles[$relDir].Add($f)
    }
}

function Get-WixId([string]$prefix, [string]$path) {
    $safe = $path -replace '[^a-zA-Z0-9_.]', '_'
    return "${prefix}_${safe}"
}

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
[void]$sb.AppendLine('<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">')
[void]$sb.AppendLine('  <Fragment>')
[void]$sb.AppendLine('    <ComponentGroup Id="PublishedComponents" Directory="INSTALLDIR">')

foreach ($f in $rootFiles) {
    $name   = $f.Name
    $idAttr = ""
    if ($name -ieq "rightBright.exe") { $idAttr = ' Id="RightBrightExe"' }

    [void]$sb.AppendLine("      <Component>")
    [void]$sb.AppendLine("        <File$idAttr Source=`"$name`" />")
    [void]$sb.AppendLine("      </Component>")
}

foreach ($dir in $subDirFiles.Keys) {
    $dirId = Get-WixId "dir" $dir
    foreach ($f in $subDirFiles[$dir]) {
        $relPath = $f.FullName.Substring($publishDirFull.Length + 1)
        [void]$sb.AppendLine("      <Component Directory=`"$dirId`">")
        [void]$sb.AppendLine("        <File Source=`"$relPath`" />")
        [void]$sb.AppendLine("      </Component>")
    }
}

[void]$sb.AppendLine('    </ComponentGroup>')

$sortedDirs = $subDirFiles.Keys | Sort-Object
if ($sortedDirs) {
    $allDirPaths = [ordered]@{}
    foreach ($dir in $sortedDirs) {
        $parts = $dir -split '\\'
        for ($i = 0; $i -lt $parts.Length; $i++) {
            $current = ($parts[0..$i] -join '\')
            if (-not $allDirPaths.Contains($current)) {
                $parent = if ($i -gt 0) { ($parts[0..($i - 1)] -join '\') } else { $null }
                $allDirPaths[$current] = @{
                    Name   = $parts[$i]
                    Parent = $parent
                    Id     = Get-WixId "dir" $current
                }
            }
        }
    }

    function Write-DirectoryTree([string]$parentPath, [string]$indent) {
        $children = $allDirPaths.GetEnumerator() |
            Where-Object { $_.Value.Parent -eq $parentPath } |
            Sort-Object Key
        foreach ($child in $children) {
            $info = $child.Value
            [void]$sb.AppendLine("$indent<Directory Id=`"$($info.Id)`" Name=`"$($info.Name)`">")
            Write-DirectoryTree $child.Key "$indent  "
            [void]$sb.AppendLine("$indent</Directory>")
        }
    }

    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('    <DirectoryRef Id="INSTALLDIR">')
    Write-DirectoryTree $null "      "
    [void]$sb.AppendLine('    </DirectoryRef>')
}

[void]$sb.AppendLine('  </Fragment>')
[void]$sb.AppendLine('</Wix>')

$sb.ToString() | Set-Content -Path $OutFile -Encoding UTF8
Write-Host "Generated $($allFiles.Count) file entries" -ForegroundColor Green
