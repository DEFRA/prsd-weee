# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [uri]$Url = $null,

    [Parameter(Mandatory=$true)]
    [string]$SiteUsername = $null,

    [Parameter(Mandatory=$true)]
    [string]$SitePassword = $null,

    [Parameter(Mandatory=$false)]
    [bool]$IgnoreSslErrors = $false
)

$exitCode = 0;

Try
{
    $failedTests = @();
    $smokeTestUrl = New-Object System.Uri($Url, "admin/smoke-test");
    $loginUrl = New-Object System.Uri($Url, "admin/account/sign-in");
    $blockExitCode = 0;

    ### Ignore SSL errors ###
    Add-Type @"
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public static class SSLValidator
    {
        private static Stack<RemoteCertificateValidationCallback> funcs = new Stack<RemoteCertificateValidationCallback>();

        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void OverrideValidation()
        {
            funcs.Push(ServicePointManager.ServerCertificateValidationCallback);
            ServicePointManager.ServerCertificateValidationCallback =
                OnValidateCertificate;
        }

        public static void RestoreValidation()
        {
            if (funcs.Count > 0) {
                ServicePointManager.ServerCertificateValidationCallback = funcs.Pop();
            }
        }
    }
"@;


    if ($IgnoreSslErrors)
    {
        [SSLValidator]::OverrideValidation();
    }
      
    ### Smoke test ###
    try 
    {
        $smokeTestResult = Invoke-RestMethod $smokeTestUrl.ToString();
    }
    catch [System.Net.WebException]
    {
        # Suppress any exceptions
    }
      
    if ($smokeTestResult -ne "True")
    {
        $failedTests += "Failed admin/smoke-test";
    }
      
    ### Login ###
    $loginResult = Invoke-WebRequest $loginUrl.ToString() -SessionVariable session;
      
    $loginResult.Forms[0].Fields["Email"] = $SiteUsername;
    $loginResult.Forms[0].Fields["Password"] = $SitePassword;
      
    $loginPostResult = Invoke-WebRequest $loginUrl.ToString() -Method Post -Body $loginResult.Forms[0].Fields -ContentType 'application/x-www-form-urlencoded' -WebSession $session;

    if (!($loginPostResult.BaseResponse.ResponseUri -ne $loginUrl -and $loginPostResult.StatusCode -eq 200))
    {
        $failedTests += "Failed login";
    }
      
    ### Report success or failure ###
    if ($failedTests.Length -gt 0)
    {
        $blockExitCode = -1;
        Write-Host "[FAILURE] : External login tests failed!";
      
        foreach ($failure in $failedTests)
        {
            Write-Host $failure;
        }
    }
    else
    {
        Write-Host "[SUCCESS] : All external login tests successful!";
    }
      
    if ($IgnoreSslErrors)
    {
        [SSLValidator]::RestoreValidation();
    }

    return $blockExitCode;
}
Catch
{      
   Write-Error -ErrorRecord $_;
   $exitCode = -1;
}

exit $exitCode;