#EXAMPLE USAGE: .\set-fixed-quarter.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -FixQuarter $True -ComplianceYear 2016 -Quarter 1

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [bool]$FixQuarter = $null,

    [Parameter(Mandatory=$true)]
    [int]$ComplianceYear = $null,

    [Parameter(Mandatory=$true)]
    [int]$Quarter = $null
)

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\SetFixedQuarter.sql -Parameters "@FixQuarter=$FixQuarter", "@ComplianceYear=$ComplianceYear", "@Quarter=$Quarter"