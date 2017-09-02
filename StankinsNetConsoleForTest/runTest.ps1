$process = (Start-Process -FilePath "StankinsNetConsoleForTest.exe" -PassThru -Wait)
Write-Host "Process finished with return code: " $process.ExitCode
if($process.ExitCode -ne 0){
	throw "error in test"
}