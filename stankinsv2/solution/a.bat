rem docker ps -aq
docker container prune -f
docker image rm st
docker build . -t  st
docker run -d -p 5000:5000  st 