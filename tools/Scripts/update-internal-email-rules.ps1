# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [string]$configPath = $null,

    [Parameter(Mandatory=$true)]
    [string]$isEnabled = 'false',

    [Parameter(Mandatory=$false)]
    [string[]]$add = $null,

    [Parameter(Mandatory=$false)]
    [string[]]$remove = $null
)

$doc = (gc $configPath) -as [xml]
$rulesNode = $doc.SelectSingleNode("//testInternalUserEmailDomains")

if($rulesNode.Attributes["userTestModeEnabled"] -ne $null)
{
    Write "Setting default email rule to $isEnabled"
    $rulesNode.Attributes["userTestModeEnabled"].InnerText = $isEnabled
}

if ($add -ne $null){
    
    $add.GetEnumerator() | % {
        Write "Adding allowed test email $($_)"

        $rule = $doc.CreateElement("add")
        $rule.SetAttribute('value',$($_))
        $rulesNode.AppendChild($rule)
    }
}

if ($remove -ne $null){
    $remove.GetEnumerator() | % {
        Write "Removing allowed test email '$($_)'"
        Foreach ($node in $rulesNode.ChildNodes) {

            $allowedEmail = $node.Attributes["value"]    
            if ($allowedEmail -ne $null -and $allowedEmail.Value -eq $_) {
                $rulesNode.RemoveChild($node)
            }  
        }
    }
}

$doc.Save($configPath)