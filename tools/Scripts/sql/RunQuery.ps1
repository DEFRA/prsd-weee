function Run-Query
{
    param (
        [Parameter(Mandatory=$true)]
        [string]$ConnectionString,

        [Parameter(Mandatory=$true)]
        [string]$QueryFile,

        [Parameter(Mandatory=$false)]
        [string[]]$Parameters = $null
    )

    $ParametersTable = ConvertFrom-StringData ($Parameters | Out-String)

    $SqlConnection = New-Object System.Data.SqlClient.SqlConnection
    $SqlConnection.ConnectionString = $ConnectionString
    $SqlConnection.Open()
    $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
    $Query = get-content $QueryFile
    $SqlCmd.CommandText = $Query
    $SqlCmd.Connection = $SqlConnection
    
    $ParametersTable.GetEnumerator() | % {
        Write-Verbose "Setting parameter $($_.key) as $($_.value)"

        $SqlCmd.Parameters.AddWithValue($($_.key), $($_.value))
    }

    $SqlCmd.ExecuteScalar()
    $SqlConnection.Close()
    $SqlConnection.Dispose()
}