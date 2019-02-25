copy npm-shrinkwrap.json src/npm-shrinkwrap.json /Y
call ng build --prod --build-optimizer

(robocopy dist\StankinsDataWebAngular ..\StankinsData\wwwroot  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0

(robocopy dist\StankinsDataWebAngular ..\StankinsCordova\www\dist  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0


(robocopy dist\StankinsDataWebAngular ..\StankinsElectron\www\dist  /MIR /XD) ^& IF %ERRORLEVEL% LSS 8 SET ERRORLEVEL = 0
