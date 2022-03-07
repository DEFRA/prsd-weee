# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$resultsPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$resultsFile = $null
)

$xmlFile = dir $resultsPath | where { $_.Name -like "$resultsFile" } | foreach { $_.FullName }

if ($xmlFile -eq $null) {
    write-host "No results file - failing build"

    exit 1
}

write-host $xmlFile

[Xml]$xml = Get-Content -Path $xmlFile
$xml."test-run"

if ($xml -eq $null) {
    write-host "nunit results file could not be parsed - failing build"

    exit 1
}

if ($xml."test-run".total -eq 0) {
    write-host "nunit results file contains no results - failing build"

    exit 1
}

[DateTime]$endDate = $xml."test-run"."end-time"

if ($endDate -lt (Get-Date).AddMinutes(-10)) {
    write-host "nunit test results has end time that is not within tolerance - check test run - failing build"

    exit 1
}


if ($xml."test-run".result -eq "Failed") {
    write-host "nunit test results has $($xml."test-run".failed) failed tests - failing build"

    exit 1
}


write host "nunit test run succeeded"