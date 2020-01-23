[CmdletBinding()]
param
(
    [Parameter(Mandatory=$true)]
    [string]$User= $null,

    [Parameter(Mandatory=$true)]
    [string]$Password = $null,

    [Parameter(Mandatory=$true)]
    [string]$Instance = $null,
    
    [Parameter(Mandatory=$true)]
    [string]$Database = $null,

    [Parameter(Mandatory=$true)]
    [string]$Container  = $null,

    [Parameter(Mandatory=$true)]
    [string]$StorageAccount  = $null,

    [Parameter(Mandatory=$true)]
    [string]$StorageAccountKey  = $null
)

$securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
$sqlCredentials = New-Object System.Management.Automation.PSCredential ($User, $securePassword)

$targetInstance = Connect-DbaInstance -SqlInstance $Instance -SqlCredential $sqlCredentials

$storageAccountContext = New-AzStorageContext -StorageAccountName $StorageAccount -StorageAccountKey $StorageAccountKey
$storageContainer = Get-AzStorageContainer -Context $storageAccountContext -Name $Container

$ts = New-TimeSpan -Minutes 30
$sas = (New-AzStorageAccountSASToken -Service Blob -ResourceType Object -Permission "rw" -Context $storageAccountContext -ExpiryTime ((get-date) + $ts)).TrimStart('?')  

New-DbaCredential -SqlInstance $targetInstance -Name "https://$StorageAccount.blob.core.windows.net/$Container" -Identity “SHARED ACCESS SIGNATURE” -SecurePassword (ConvertTo-SecureString $sas -AsPlainText -Force) -Force

$file = $Database + " " + (Get-Date).ToString("yyyy-MM-dd HH-mm") + ".bak"
Backup-DbaDatabase -SqlInstance $targetInstance -Database $Database -AzureBaseUrl “https://$StorageAccount.blob.core.windows.net/$Container/” -BackupFileName $file -Type Full -Checksum -CopyOnly

