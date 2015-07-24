# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$configPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$defaultRule = 'Allow',

    [Parameter(Mandatory=$false)]
    [string[]]$allow = $null,

    [Parameter(Mandatory=$false)]
    [string[]]$deny = $null
)

$doc = (gc $configPath) -as [xml]
$rulesNode = $doc.SelectSingleNode("//emailRules")

if($rulesNode.Attributes["defaultAction"] -ne $null)
{
    Write "Setting default email rule to $defaultRule"
    $rulesNode.Attributes["defaultAction"].InnerText = $defaultRule
}

if ($allow -ne $null){
    
    $allow.GetEnumerator() | % {
        $allowRulesArray = $_.Split(':')
        Write "Adding email rule: Allow where $($allowRulesArray[0]) matches $($allowRulesArray[1])"

        $rule = $doc.CreateElement("add")
        $rule.SetAttribute('action','Allow')
        $rule.SetAttribute('type',$($allowRulesArray[0]))
        $rule.SetAttribute('value',$($allowRulesArray[1]))
        $rulesNode.AppendChild($rule)
    }
}

if ($deny -ne $null){
    
    $deny.GetEnumerator() | % {
        $denyRulesArray = $_.Split(':')
        Write "Adding email rule: Deny where $($denyRulesArray[0]) matches $($denyRulesArray[1])"

        $rule = $doc.CreateElement("add")
        $rule.SetAttribute('action','Deny')
        $rule.SetAttribute('type',$($denyRulesArray[0]))
        $rule.SetAttribute('value',$($denyRulesArray[1]))
        $rulesNode.AppendChild($rule)
    }
}

$doc.Save($configPath)