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
$allFiles = Get-ChildItem -Path $publishDirFull -Recurse -File |
    Sort-Object { $_.FullName.Substring($publishDirFull.Length + 1) }

function Get-SafeId([string]$value) {
    return ($value -replace '[^a-zA-Z0-9_]', '_')
}

$sb = [System.Text.StringBuilder]::new()
$componentIds = [System.Collections.Generic.List[string]]::new()
$currentDirParts = @()

[void]$sb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
[void]$sb.AppendLine('<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">')
[void]$sb.AppendLine('  <Fragment>')
[void]$sb.AppendLine('    <DirectoryRef Id="INSTALLDIR">')

foreach ($f in $allFiles) {
    $relPath  = $f.FullName.Substring($publishDirFull.Length + 1)
    $relDir   = Split-Path $relPath -Parent
    $newParts = if ($relDir) { @($relDir -split '\\') } else { @() }

    # Compute how much of the current directory stack is still valid
    $commonLen = 0
    while ($commonLen -lt $currentDirParts.Length -and
           $commonLen -lt $newParts.Length -and
           $currentDirParts[$commonLen] -eq $newParts[$commonLen]) {
        $commonLen++
    }

    # Close directories we've left
    for ($i = $currentDirParts.Length - 1; $i -ge $commonLen; $i--) {
        $indent = '      ' + ('  ' * $i)
        [void]$sb.AppendLine("$indent</Directory>")
    }

    # Open directories we've entered
    for ($i = $commonLen; $i -lt $newParts.Length; $i++) {
        $dirPath = ($newParts[0..$i] -join '\')
        $dirId   = "dir_$(Get-SafeId $dirPath)"
        $indent  = '      ' + ('  ' * $i)
        [void]$sb.AppendLine("$indent<Directory Id=`"$dirId`" Name=`"$($newParts[$i])`">")
    }

    $currentDirParts = $newParts

    # Write the Component + File element
    $cmpId  = "cmp_$(Get-SafeId $relPath)"
    $depth  = $newParts.Length
    $indent = '      ' + ('  ' * $depth)

    $fileIdAttr = ''
    if ($f.Name -ieq 'rightBright.exe') { $fileIdAttr = ' Id="RightBrightExe"' }

    [void]$sb.AppendLine("$indent<Component Id=`"$cmpId`">")
    [void]$sb.AppendLine("$indent  <File$fileIdAttr Source=`"$relPath`" />")
    [void]$sb.AppendLine("$indent</Component>")

    $componentIds.Add($cmpId)
}

# Close any directories still open
for ($i = $currentDirParts.Length - 1; $i -ge 0; $i--) {
    $indent = '      ' + ('  ' * $i)
    [void]$sb.AppendLine("$indent</Directory>")
}

[void]$sb.AppendLine('    </DirectoryRef>')
[void]$sb.AppendLine('  </Fragment>')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('  <Fragment>')
[void]$sb.AppendLine('    <ComponentGroup Id="PublishedComponents">')

foreach ($id in $componentIds) {
    [void]$sb.AppendLine("      <ComponentRef Id=`"$id`" />")
}

[void]$sb.AppendLine('    </ComponentGroup>')
[void]$sb.AppendLine('  </Fragment>')
[void]$sb.AppendLine('</Wix>')

$sb.ToString() | Set-Content -Path $OutFile -Encoding UTF8
Write-Host "Generated $($allFiles.Count) file entries in $($componentIds.Count) components" -ForegroundColor Green
