copy npm-shrinkwrap.json src/npm-shrinkwrap.json /Y
call ng build --prod --build-optimizer

(robocopy dist\StankinsAliveAngular ..\StankinsStatusWeb\wwwroot  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0
