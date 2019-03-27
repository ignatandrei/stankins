$watcher = New-Object System.IO.FileSystemWatcher
$pathSolution = "solution/StankinsV2/"
$watcher.Path = $pathSolution
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $false
$watcher.NotifyFilter = [System.IO.NotifyFilters]::LastWrite -bor [System.IO.NotifyFilters]::FileName

while($TRUE){
	$result = $watcher.WaitForChanged([System.IO.WatcherChangeTypes]::Changed -bor [System.IO.WatcherChangeTypes]::Renamed -bOr [System.IO.WatcherChangeTypes]::Created, 1000);
	if($result.TimedOut){
		continue;
	}
	# $cmd = "docker cp " + $pathSolution + ". stankins_test_container:/usr/app/"
    #Write-Host $cmd
	
    #Invoke-Expression -Command $cmd
	
	Write-Host " start from " $result.Name
	
	$relativeNameIndex = $result.Name.LastIndexOf("\")
	$relativeName = $result.Name.substring(0,$relativeNameIndex+1)
	$relativeName = $relativeName.replace("\","/")
	$cmd = "docker cp " + $pathSolution + $relativeName + ". stankins_test_container:/usr/app/"+ $relativeName
	Write-Host $cmd
	Invoke-Expression -Command $cmd
	#Write-Host $relativeName
	#Write-Host $cmd
	
	
    
}
