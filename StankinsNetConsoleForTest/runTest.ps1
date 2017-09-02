$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = "StankinsNetConsoleForTest.exe"
$pinfo.RedirectStandardError = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
#$pinfo.Arguments = "localhost"
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo
$p.Start() | Out-Null
$p.WaitForExit()
$stdout = $p.StandardOutput.ReadToEnd()
$stderr = $p.StandardError.ReadToEnd()

Write-Host "Process finished with return code: "+ $p.ExitCode
if($p.ExitCode -ne 0){
	Write-Host $stdout
	Write-Host $stderr
	throw "error in test"
}