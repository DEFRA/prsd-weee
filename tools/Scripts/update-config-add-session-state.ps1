# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$false)]
    [string]$connectionStringName = $null,

    [Parameter(Mandatory=$false)]
    [string]$connectionStringValue = $null,

    [Parameter(Mandatory=$true)]
    [string]$configPath = $null
)

$doc = (gc $configPath) -as [xml]

if ($connectionStringName -And $connectionStringValue)
{
    Write-Verbose "Adding connectionString: $connectionStringName to $connectionStringValue"

    if (!$doc.configuration.connectionStrings)
    {
        $connectionStrings = $doc.CreateElement('connectionStrings')
        $addconnection = $doc.CreateElement('add')
        $addconnection.SetAttribute('name', $connectionStringName)
        $addconnection.SetAttribute('connectionString', $connectionStringValue)
        $addconnection.SetAttribute('providerName', 'System.Data.SqlClient')

        $connectionStrings.AppendChild($addconnection)

        $doc.configuration.AppendChild($connectionStrings)
    }

    $sessionState = $doc.SelectSingleNode("//system.web/sessionState")
    
    if (!$sessionState)
    {
        $sessionState = $doc.CreateElement('sessionState')
        $sessionState.SetAttribute('mode', 'Custom')
        $sessionState.SetAttribute('customProvider', 'DefaultSessionProvider')

        $providers = $doc.CreateElement('providers')
        $addsession = $doc.CreateElement('add')
        $addsession.SetAttribute('name', 'DefaultSessionProvider')
        $addsession.SetAttribute('type', 'System.Web.Providers.DefaultSessionStateProvider, System.Web.Providers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35')
        $addsession.SetAttribute('connectionStringName', $connectionStringName)

        $providers.AppendChild($addsession)
        $sessionState.AppendChild($providers)

        $doc.SelectSingleNode("//system.web").AppendChild($sessionState)

    }
}

$doc.Save($configPath)