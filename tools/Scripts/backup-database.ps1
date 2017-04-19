# EXAMPLE USAGE: .\backup-database.ps1 -ConnectionString 'Server=MyServer;Database=MyDB;Integrated Security=True' -DatabaseName '[EA.Weee]' -Location 'C:\'

# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = $null,

    [Parameter(Mandatory=$true)]
    [string]$DatabaseName = $null,

    [Parameter(Mandatory=$true)]
    [string]$Location = $null,

	[Parameter(Mandatory=$false)]
	[int]$Timeout = 30
)

. "$PSScriptRoot\sql\RunQuery.ps1"

$exitCode = 0;

Try {

$FileName = $DatabaseName + " " + (Get-Date).ToString("yyyy-MM-dd HH-mm") + ".bak"

Run-Query -ConnectionString $ConnectionString `
          -QueryFile $PSScriptRoot\sql\BackupDatabase.sql `
          -Parameters "@DatabaseName=$DatabaseName", "@Location=$Location", "@FileName=$FileName" `
	      -CommandTimeout $Timeout;
}
Catch
{
   Write-Error -ErrorRecord $_;
   $exitCode = -1;
}

exit $exitCode;