# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$resultsPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$resultsFile = $null
)

write-host "Failig build"
exit 1