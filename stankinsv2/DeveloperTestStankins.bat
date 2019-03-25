cls
ECHO please be aware of absolute path here %cd%
docker build -t stankins_test_image -f DeveloperTestStankins.txt .
docker-compose -f solution/StankinsV2/StankinsTestXUnit/Docker/docker-sqlserver-instance-linux.yaml up -d 
echo start powershell
Start powershell ./all.ps1
docker run -it --name stankins_test_container --rm stankins_test_image  dotnet watch -p StankinsTestXUnit/StankinsTestXUnit.csproj test
docker-compose -f solution/StankinsV2/StankinsTestXUnit/Docker/docker-sqlserver-instance-linux.yaml down 