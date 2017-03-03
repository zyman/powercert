param(
    [Parameter(Mandatory = $false)][string]$ScriptPath = $Null
)

if ($ScriptPath -eq "") {Write-Output "Path is missing"; break}

$cert    = @(Get-ChildItem Cert:\LocalMachine\CA )[1]
$cert    = @(Get-ChildItem Cert:\LocalMachine\My -CodeSigningCert)[0]

# $cert.Thumbprint
# $cert.Subject

[void](Set-AuthentiCodeSignature -Certificate $cert -FilePath $ScriptPath -Force)

Write-Output $(Get-Content $ScriptPath)
