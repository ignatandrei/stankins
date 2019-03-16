rem echo windows
rem docker-compose -f docker-sqlserver-instance-windows.yaml up -d   
echo linux
docker-compose -f docker-sqlserver-instance-linux.yaml up -d   
docker container ls 
pause
rem docker-compose -f docker-sqlserver-instance-windows.yaml down   
docker-compose -f docker-sqlserver-instance-linux.yaml down   
            
