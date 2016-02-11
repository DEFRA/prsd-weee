# EXAMPLE USAGE: .\update-competent-authority-emails.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -EAMail 'a@b.c' -SEPAMail 'a@b.c' -NIEAMail 'a@b.c' -NRWMail 'a@b.c'

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [string]$EAMail = $null,
    
    [Parameter(Mandatory=$true)]
    [string]$SEPAMail = $null,

    [Parameter(Mandatory=$true)]
    [string]$NIEAMail = $null,

    [Parameter(Mandatory=$true)]
    [string]$NRWMail = $null
)

. "$PSScriptRoot\sql\RunQuery.ps1"

Run-Query -ConnectionString $ConnectionString -QueryFile $PSScriptRoot\sql\UpdateCAEmails.sql -Parameters "@EAMail=$EAMail", "@SEPAMail=$SEPAMail", "@NIEAMail=$NIEAMail", "@NRWMail=$NRWMail"