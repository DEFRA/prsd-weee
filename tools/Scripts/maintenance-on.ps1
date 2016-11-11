# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$Username = $null,

    [Parameter(Mandatory=$true)]
    [string]$Password = $null,

    [Parameter(Mandatory=$true)]
    [string]$Server = $null,
    
    [Parameter(Mandatory=$true)]
    [string]$MainWebsite = $null,
    
    [Parameter(Mandatory=$true)]
    [string]$MaintenanceWebsite = $null
)

$exitCode = 0;

Try
{
   $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force;
   $creds = New-Object System.Management.Automation.PSCredential($Username, $securePassword);

   Invoke-Command -ComputerName $Server -Credential $creds -ErrorAction Stop -ScriptBlock { 
      \windows\system32\inetsrv\appcmd stop site $Using:MainWebsite
      \windows\system32\inetsrv\appcmd start site $Using:MaintenanceWebsite
      \windows\system32\inetsrv\appcmd set site $Using:MaintenanceWebsite /serverAutoStart:true };

   Write-Host "[SUCCESS]";
}
Catch
{
   Write-Error -ErrorRecord $_;
   $exitCode = 1;
}

exit $exitCode;