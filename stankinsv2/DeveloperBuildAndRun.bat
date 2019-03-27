cls
ECHO please be aware of absolute path here %cd%
docker build --no-cache -t stankins_run_image -f runNetCore.txt .
docker run --network docker_default -d -p 5000:5000 --mount type=bind,source=%cd%\solution\StankinsV2,target=/usr/app --name stankins_run_container stankins_run_image  
docker exec -it  stankins_run_container dotnet watch -p StankinsData/StankinsDataWeb.csproj run --urls=http://+:5000
