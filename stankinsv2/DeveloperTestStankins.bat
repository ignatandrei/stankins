cls
ECHO please be aware of absolute path here %cd%
docker build --no-cache -t stankins_test_image -f DeveloperTestStankins.txt .
docker-compose -f solution/StankinsV2/StankinsTestXUnit/Docker/docker-sqlserver-instance-linux.yaml up -d 
echo start powershell
Start powershell ./all.ps1 
timeout 60 /NOBREAK 
docker-compose  exec  docker_jenkins_1 bash -c "cat /var/jenkins_home/jenkinsjob.xml | java -jar /var/jenkins_home/war/WEB-INF/jenkins-cli.jar -s http://localhost:8080 create-job newmyjob"
REM -v n
REM docker run --network docker_default -m 3G --memory-reservation 2G -it --name stankins_test_container --rm stankins_test_image  dotnet watch   -p StankinsTestXUnit/StankinsTestXUnit.csproj test -c release 
docker run --network docker_default -m 3G --memory-reservation 2G -it --name stankins_test_container --rm stankins_test_image  
docker-compose -f solution/StankinsV2/StankinsTestXUnit/Docker/docker-sqlserver-instance-linux.yaml down 