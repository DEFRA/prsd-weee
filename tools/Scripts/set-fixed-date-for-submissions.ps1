#EXAMPLE USAGE: .\set-fixed-date-for-submissions.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -FixDateForSubmissions $True -DateForSubmissions "2016-04-01"

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [bool]$FixDateForSubmissions = $null,

    [Parameter(Mandatory=$true)]
    [string]$DateForSubmissions = $null
)

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\SetFixedDateForSubmissions.sql -Parameters "@FixDateForSubmissions=$FixDateForSubmissions", "@DateForSubmissions=$DateForSubmissions"