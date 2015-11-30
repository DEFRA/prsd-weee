# EXAMPLE USAGE: .\add-external-user.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -FirstName 'John' -Surname 'Doe' -Email 'john.doe@test.co.uk' -HashedPassword 'MyHashedPassword' -SecurityStamp 'MySecurityStamp'

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
    [string]$SecurityStamp = $null
)

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\AddExternalUser.sql -Parameters "@FirstName=$FirstName", "@Surname=$Surname", "@Email=$Email", "@HashedPassword=$HashedPassword", "@SecurityStamp=$SecurityStamp"