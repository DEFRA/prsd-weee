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

$nunit = dir $PackagesPath -recurse | where { $_.PSIsContainer -eq $false -and $_.Name -eq "nunit3-console.exe" } | foreach { $_.FullName } | sort -descending 

$testDlls = dir $TestDllsPath -recurse | where { $_.Name -like "EA.Weee.Integration.Tests.dll" } | Get-Unique | foreach { "`"" + $_.FullName +"`"" } 

write-host $testDlls
write-host "Found nunit test dlls"

# As with build the paths may contain spaces and must be enclosed by ' for the iex to work
$testDllString = ([string]::Join(" ", $testDlls))
$testOutDir = "'" + $OutputPath + "\nunit-test-results.xml;format=nunit2'" 

$testConsole = $nunit

if($nunit -is [system.array])
{
    $testConsole = $nunit[0]
}

$iexTest = "& '$testConsole' $testDllString --result=$testOutDir"
write-host $iexTest

&iex $iexTest