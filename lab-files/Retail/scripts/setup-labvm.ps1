param($labFilesName="")

# Install SSMS
iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1')) 
choco install sql-server-management-studio -y

# Download Lab Files
$labFilesFolder = "C:\MCW"
if ([string]::IsNullOrEmpty($labFilesName) -eq $false)
{
    Write-Host "Make sure folder exists"
    if ((Test-Path $labFilesFolder) -eq $false)
    {
        New-Item -Path $labFilesFolder -ItemType directory
    }

    Write-Host "Extract .ZIP file..."
    Expand-Archive -Path $labFilesName -DestinationPath $labFilesFolder -Force
}