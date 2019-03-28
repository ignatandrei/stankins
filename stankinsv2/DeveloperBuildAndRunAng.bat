cls
ECHO please be aware of absolute path here %cd%
docker build --tag stankins_angular_image --file DeveloperBuildAndRunAng.txt .
docker run --rm --network docker_default -d -p 4200:4200 -p 49153:49153 --mount type=bind,source=%cd%\solution\StankinsV2\StankinsDataWebAngular,target=/usr/app --name stankinsAngularContainer stankins_angular_image
docker exec -it stankinsAngularContainer  npm i
docker exec -it stankinsAngularContainer  ./runDocker.bat