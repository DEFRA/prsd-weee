# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$PackagesPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$TestDllsPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$OutputPath = $null
)

$xunit = dir $PackagesPath -recurse | where { $_.PSIsContainer -eq $false -and $_.Name -eq "xunit.console.exe" } | foreach { $_.FullName } | sort -descending
$testDlls = dir $TestDllsPath | where { $_.Name -like "*.Tests.Unit.dll" } | foreach { "`"" + $_.FullName +"`"" }

write-host "Found xunit test dlls"

# As with build the paths may contain spaces and must be enclosed by ' for the iex to work
$testDllString = ([string]::Join(" ", $testDlls))
$testOutDir = $OutputPath + "\xunit-test-results.xml"

$testConsole = $xunit

if($xunit -is [system.array])
{
    $testConsole = $xunit[0]
}

$iexTest = "& '$testConsole' '$testDllString' -parallel none -nunit '$testOutDir'"
write-host $iexTest

&iex $iexTest