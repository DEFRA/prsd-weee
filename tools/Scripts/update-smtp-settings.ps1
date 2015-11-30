# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$configPath = $null,

	[Parameter(Mandatory=$false)]
    [string]$networkHost = 'localhost',

    [Parameter(Mandatory=$false)]
    [string]$networkPort = '25',

    [Parameter(Mandatory=$false)]
    [string]$networkDefaultCredentials = $null,

    [Parameter(Mandatory=$false)]
    [string]$networkUsername = $null,

    [Parameter(Mandatory=$false)]
    [string]$networkPassword = $null    
)

# Open document as xml
[xml] $doc = get-content $configPath

# Find SMTP and network settings using XPath.
$smtpSettings = $doc.SelectSingleNode("//smtp")
$networkSettings = $smtpSettings.SelectSingleNode("//network")

# Set the values
if($networkSettings.Attributes["host"] -ne $null)
{
    $networkSettings.Attributes["host"].InnerText = $networkHost
}
else
{
    $networkSettings.SetAttribute("host", $networkHost)
}

if($networkSettings.Attributes["port"] -ne $null)
{
    $networkSettings.Attributes["port"].InnerText = $networkPort
}
elseif($networkPort -ne $null -and $networkPort -ne '')
{
    $networkSettings.SetAttribute("port", $networkPort)
}

if($networkSettings.Attributes["userName"] -ne $null -and $networkUsername -ne $null)
{
    $networkSettings.Attributes["userName"].InnerText = $networkUsername 
}
elseif($networkUsername -ne $null -and $networkUsername -ne '')
{
    $networkSettings.SetAttribute("userName", $networkUsername)
}

if($networkSettings.Attributes["defaultCredentials"] -ne $null -and $networkDefaultCredentials -ne $null)
{
    $networkSettings.Attributes["defaultCredentials"].InnerText = $networkDefaultCredentials 
}
elseif($networkDefaultCredentials -ne $null -and $networkDefaultCredentials -ne '')
{
    $networkSettings.SetAttribute("defaultCredentials", $networkDefaultCredentials)
}

if($networkSettings.Attributes["password"] -ne $null -and $networkPassword -ne $null)
{
    $networkSettings.Attributes["password"].InnerText = $networkPassword 
}
elseif($networkPassword -ne $null -and $networkPassword -ne '')
{
    $networkSettings.SetAttribute("password", $networkPassword)
}
$doc.Save($configPath)