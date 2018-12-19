cls
call ng build versions-netcore-angular
start cmd /c ng serve --proxy-config proxy.conf.js --open