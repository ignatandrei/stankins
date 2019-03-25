$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = "."
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $false
$watcher.NotifyFilter = [System.IO.NotifyFilters]::LastWrite -bor [System.IO.NotifyFilters]::FileName

while($TRUE){
	$result = $watcher.WaitForChanged([System.IO.WatcherChangeTypes]::Changed -bor [System.IO.WatcherChangeTypes]::Renamed -bOr [System.IO.WatcherChangeTypes]::Created, 1000);
	if($result.TimedOut){
		continue;
	}
    switch($result.ChangeType){
        Changed{
        #TODO: see if folder or file
            $fullName = [System.IO.Path]::Combine($watcher.Path ,$result.Name)          
            $cmd= "docker cp $fullName stankins_test_container:/usr/app/"+$result.Name.Replace("\","/")
            Write-Host $cmd
            Invoke-Expression -Command $cmd
            

        }
        default{
        	write-host "not handled " $result.ChangeType + " Change in "  + $result.Name
        }
    }

    
	
}