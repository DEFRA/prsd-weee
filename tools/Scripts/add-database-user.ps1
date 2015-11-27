# EXAMPLE USAGE: .\add-database-user.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -Username 'WeeeTestUser' -Login 'WeeeTestUserLogin' -Role 'weee_application'

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [string]$Username = $null,
    
    [Parameter(Mandatory=$true)]
    [string]$Login = $null,

    [Parameter(Mandatory=$true)]
    [string]$Role = $null
)

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\AddDatabaseUser.sql -Parameters "@Username=$Username", "@Login=$Login", "@Role=$Role"