# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$configPath = $null,

    [Parameter(Mandatory=$false)]
    [bool]$allowRemoteAccess = $false
)

$doc = (gc $configPath) -as [xml]

$elmah = $doc.SelectSingleNode("//elmah")
$security = $elmah.SelectSingleNode("security")

if (!$security)
{
    $security = $elmah.AppendChild($doc.CreateElement("security"))
}

$security.SetAttribute("allowRemoteAccess", $allowRemoteAccess.ToString().ToLower())

$doc.Save($configPath)