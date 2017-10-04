param([Parameter(Mandatory=$true)][string]$sourceDirectory)

Write-Host "start modifying " $sourceDirectory

Copy-Item $sourceDirectory\LoggingNET  $sourceDirectory\Logging -Container -Force -recurse

Write-Host "end modifying " $sourceDirectory