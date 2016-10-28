# EXAMPLE USAGE: .\add-internal-user.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -FirstName 'John' -Surname 'Doe' -Email 'john.doe@test.co.uk' -HashedPassword 'MyHashedPassword' -SecurityStamp 'MySecurityStamp'

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [string]$FirstName = $null,

    [Parameter(Mandatory=$true)]
    [string]$Surname = $null,

    [Parameter(Mandatory=$true)]
    [string]$Email = $null,

    [Parameter(Mandatory=$true)]
    [string]$HashedPassword = $null,

    [Parameter(Mandatory=$true)]
    [string]$SecurityStamp = $null,

    [ValidateSet(0, 1, 2)] 	
	[Parameter(Mandatory=$false)]
	[string]$Status = 2
)

Write-Host $Status

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\ClaimsIdInsertOn.sql
Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\AddInternalUser.sql -Parameters "@FirstName=$FirstName", "@Surname=$Surname", "@Email=$Email", "@HashedPassword=$HashedPassword", "@SecurityStamp=$SecurityStamp", "@Status=$Status"
Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\ClaimsIdInsertOff.sql