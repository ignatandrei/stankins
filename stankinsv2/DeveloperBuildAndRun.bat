cls
ECHO please be aware of absolute path here %cd%
docker build --no-cache -t stankins_run_image -f runNetCore.txt .
docker run --rm --network docker_default -d -p 5000:5000 --mount type=bind,source=%cd%\solution\StankinsV2,target=/usr/app --name stankins_run_container stankins_run_image  
rem docker exec -it  stankins_run_container dotnet watch -p StankinsData/StankinsDataWeb.csproj run --urls=http://+:5000
docker exec -it  stankins_run_container dotnet watch -p Stankins.Console/Stankins.Console.csproj run recipes -e ExportAzurePipelinesToMermaid
rem docker exec -it  stankins_run_container dotnet watch -p StankinsData/StankinsDataWeb.csproj run
