# Enable -Verbose option
[CmdletBinding()]

param
(
    [Parameter(Mandatory=$true)]
    [uri]$url = $null,

    [Parameter(Mandatory=$true)]
    [string]$username = $null,

    [Parameter(Mandatory=$true)]
    [string]$password = $null,

    [Parameter(Mandatory=$true)]
    [string]$testEmail = $null,

    [Parameter(Mandatory=$false)]
    [bool]$ignoreSslErrors = $false
)

$failedTests = @();
$smokeTestUrl = New-Object System.Uri($url, "admin/smoke-test");
$loginUrl = New-Object System.Uri($url, "account/account/sign-in");
$testEmailUrl = New-Object System.Uri($url, "admin/test-email");
$testEmailSuccessUrl = New-Object System.Uri($url, "admin/test-email/success");
$exitCode = 0;

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

if ($ignoreSslErrors)
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
    # Supress any exceptions
}

if ($smokeTestResult -ne "True")
{
    $failedTests += "Failed admin/smoke-test";
}

### Login ###
$loginResult = Invoke-WebRequest $loginUrl.ToString() -SessionVariable session;

$loginResult.Forms[0].Fields["Email"] = $username;
$loginResult.Forms[0].Fields["Password"] = $password;

$loginPostResult = Invoke-WebRequest $loginUrl.ToString() -Method Post -Body $loginResult.Forms[0].Fields -ContentType 'application/x-www-form-urlencoded' -WebSession $session;

if (!($loginPostResult.BaseResponse.ResponseUri -ne $loginUrl -and $loginPostResult.StatusCode -eq 200))
{
    $failedTests += "Failed login";
}

### Send test email ###
$testEmailResult = Invoke-WebRequest $testEmailUrl.ToString() -WebSession $session;

if ($testEmailResult.BaseResponse.ResponseUri -ne $testEmailUrl)
{
    $failedTests += "Access denied to send test email";
}

$testEmailResult.Forms[0].Fields["EmailTo"] = $testEmail;

$testEmailPostResult = Invoke-WebRequest $testEmailUrl.ToString() -Method Post -Body $testEmailResult.Forms[0].Fields -ContentType 'application/x-www-form-urlencoded' -WebSession $session;

if ($testEmailPostResult.BaseResponse.ResponseUri -ne $testEmailSuccessUrl -and $testEmailPostResult.StatusCode -ne 200)
{
    $failedTests += "Failed to send test email";
}

### Report success or failure ###
if ($failedTests.Length -gt 0)
{
    $exitCode = -1;
    Write-Host "[FAILURE] : Smoke tests failed!";

    foreach ($failure in $failedTests)
    {
        Write-Host $failure;
    }
}
else
{
    Write-Host "[SUCCESS] : All smoke tests successful!";
}

if ($ignoreSslErrors)
{
    [SSLValidator]::RestoreValidation();
}

exit $exitCode;