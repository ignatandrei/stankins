param([Parameter(Mandatory=$true)][string]$sourceDirectory)

Write-Host "start modifying " $sourceDirectory

Copy-Item $sourceDirectory\LoggingNET\*.*  $sourceDirectory\Logging\ -Container -Force 
Move-Item $sourceDirectory\Logging\LoggingNet.csproj $sourceDirectory\Logging\Logging.csproj -Force 
Write-Host "end modifying " $sourceDirectory