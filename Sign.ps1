$cert    = @(Get-ChildItem Cert:\LocalMachine\CA )[1]
$cert    = @(Get-ChildItem Cert:\LocalMachine\My -CodeSigningCert)[0]

$inbox   = "C:\PoShSigning\Inbox"
$outbox  = "C:\PoShSigning\outbox"
$repo    = "C:\PoShSigning\repository"

# Verify that certificat is in place
if ($cert -eq $null)
{
    Write-Error "Unable to retrieve certificate for script signing"
    return
}


$scripts = Get-ChildItem -Path $inbox -Filter "*.ps1"
Write-Host ("{0} scripts will be processed" -f $scripts.Count)

ForEach ($script in $scripts)
{
    try
    {
        # Read script attributes and content
        $owner = $script.GetAccessControl().Owner
        $date = $script.LastWriteTimeUtc
        $content = Get-Content -Path $script.FullName

        # Update script with author and date
        Set-Content -Path $script.FullName -Value ("# Author: {0}`r`n# Modification date: {1}`r`n" -f  $owner, $date.ToString("yyyy-MM-dd HH:mm:ss"))
        Add-Content -Path $script.FullName -Value $content

        # Sign script
        Set-AuthentiCodeSignature -Certificate $cert -FilePath $script.FullName -ErrorAction Stop

        # Copy script to repository for auditing purposes
        Copy-Item -Path $script.FullName -Destination $("{0}\{1}_{2}" -f $repo, $date.ToString("yyyyMMddHHmmss"), $script.Name) -Force

        # Move to outbox
        Move-Item -Path $script.FullName -Destination $outbox -Force
    }
    catch
    {
        Write-Error $_.Exception.Message
    }
}