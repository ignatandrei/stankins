cls
call ng build versions-netcore-angular
call ng build dashboard-simple
start cmd /c ng serve --proxy-config proxy.conf.js --open