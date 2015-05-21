# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$false)]
    [string[]]$appSettings = $null,

    [Parameter(Mandatory=$false)]
    [string]$connectionStringName = $null,

    [Parameter(Mandatory=$false)]
    [string]$connectionStringValue = $null,

    [Parameter(Mandatory=$true)]
    [string]$configPath = $null
)

$appSettingsTable = ConvertFrom-StringData ($appSettings | Out-String)

$doc = (gc $configPath) -as [xml]

$appSettingsTable.GetEnumerator() | % {
    Write-Verbose "Updating appSetting: $($_.key) to $($_.value)"

    $doc.SelectSingleNode("//appSettings/add[@key=""$($_.key)""]/@value").InnerText = $_.value
}

if ($connectionStringName -And $connectionStringValue)
{
    Write-Verbose "Updating connectionString: $connectionStringName to $connectionStringValue"

    $doc.SelectSingleNode("//connectionStrings/add[@name=""$connectionStringName""]/@connectionString").InnerText = $connectionStringValue
}

$doc.Save($configPath)