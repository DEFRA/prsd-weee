# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$configPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$validationKey = $null,

    [Parameter(Mandatory=$true)]
    [string]$decryptionKey = $null,

    [Parameter(Mandatory=$true)]
    [string]$validationMethod = $null,

    [Parameter(Mandatory=$true)]
    [string]$decryptionMethod = $null
)

$doc = (gc $configPath) -as [xml]

$systemWeb = $doc.SelectSingleNode("//system.web")
$machineKey = $systemWeb.SelectSingleNode("machineKey")

if (!$machineKey)
{
    $machineKey = $systemWeb.AppendChild($doc.CreateElement("machineKey"))
}

$machineKey.SetAttribute("validationKey", $validationKey)
$machineKey.SetAttribute("decryptionKey", $decryptionKey)
$machineKey.SetAttribute("validation", $validationMethod)
$machineKey.SetAttribute("decryption", $decryptionMethod)

$doc.Save($configPath)