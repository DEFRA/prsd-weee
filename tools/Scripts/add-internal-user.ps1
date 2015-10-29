# EXAMPLE USAGE: .\add-internal-user.ps1 -ServerInstance 'MYCOMPUTER\SQLEXPRESS' -Database 'WeeeDB' -Username MyUser -Password MyPassword -ScriptPath 'C:\ScriptLocation' -FirstName 'John' -Surname 'Doe' -Email 'john.doe@test.co.uk' -HashedPassword 'AM1w5UflwOQIjwFTBqS1uRIiChX2vTRoya/Ca54BwKOwpgXUQRRmpwW8y7sVw3suNQ' -SecurityStamp 'a88ee2c3-3b73-43a0-ae25-78917e3b5cc1'

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ServerInstance = $null,

    [Parameter(Mandatory=$true)]
    [string]$Database = $null,

    [Parameter(Mandatory=$true)]
    [string]$Username = $null,

    [Parameter(Mandatory=$true)]
    [string]$Password = $null,

    [Parameter(Mandatory=$true)]
    [string]$ScriptPath = $null,

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

if ($HashedPassword.Contains("="))
{
    Write-Error "The parameters passed to the stored procedure cannot contain the equals symbol (=). Remove the == from the end of the HashedPassword value. This will be reinserted by the SQL script"
}
else
{
    $sqlParams = @("FirstName = '$FirstName'", "Surname = '$Surname'", "Email = '$Email'", "HashedPassword = '$HashedPassword'", "SecurityStamp = '$SecurityStamp'")
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -Username $Username -Password $Password -InputFile "$ScriptPath\AddInternalUser.sql" -Variable $sqlParams
}