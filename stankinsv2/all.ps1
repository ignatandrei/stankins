$watcher = New-Object System.IO.FileSystemWatcher
$pathSolution = "solution/StankinsV2/"
$watcher.Path = $pathSolution
#$watcher.Filter = "*.cs*"
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
	 switch($result.ChangeType){
        Deleted{
			Write-Host " not handling delete for " $result.Name
		}
		Renamed{
			Write-Host " not handling rename for  " $result.Name
		}
		default{
			
			
			$relativeNameIndex = $result.Name.LastIndexOf("\")
			$relativeName = $result.Name.substring(0,$relativeNameIndex+1)
			$relativeName = $relativeName.replace("\","/")
			$i = $relativeName.LastIndexOf("/bin/") + $relativeName.LastIndexOf("/debug/") + $relativeName.LastIndexOf("/obj/")
			if($i -gt 0){
				continue;
			}	
			
				Write-Host " start from " $result.Name 
				Write-Host $i
				$cmd = "docker cp " + $pathSolution + $relativeName + ". stankins_test_container:/usr/app/"+ $relativeName
				Write-Host $cmd
				Invoke-Expression -Command $cmd
				
		}
	}
	#Write-Host $relativeName
	#Write-Host $cmd
	
	
    
}
