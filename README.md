# stankins

Stankins want to be the general data query / transformation / export tool. 

It is an ETL tool, aimed to be easy to use by either programmers, either common people .

You can with Stankins:

- query and monitor the exchange rates

- take any part of an web site ( tables, meta )and export to excel / pdf

- take any table of database and export to excel / pdf

- make documentation for a database

- make documentation for a .sln file

- and many, many more. It will have already predefined way to do stuff , but you can define your own


## Version 2 

# Continous Integration / Deploy

[![Continous Delivery](https://dev.azure.com/ignatandrei0674/stankinsv2/_apis/build/status/ignatandrei.stankins?branchName=master)](https://dev.azure.com/ignatandrei0674/stankinsv2/_build/latest?definitionId=1?branchName=master)  [![codecov](https://codecov.io/gh/ignatandrei/stankins/branch/master/graph/badge.svg)](https://codecov.io/gh/ignatandrei/stankins) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ignatandrei_stankins&metric=alert_status)](https://sonarcloud.io/dashboard?id=ignatandrei_stankins)  <img src='https://img.shields.io/azure-devops/tests/ignatandrei0674/stankinsv2/1.svg'></a>[![CodeCoverage, Sonar, Tests](https://dev.azure.com/ignatandrei0674/stankinsv2/_apis/build/status/ignatandrei.stankins?branchName=master&jobName=FullTestOnLinux&label=FullTest)](https://dev.azure.com/ignatandrei0674/stankinsv2/_apis/build/status/ignatandrei.stankins?branchName=master&jobName=FullTestOnLinux&label=FullTest)
![Docker](https://img.shields.io/docker/pulls/ignatandrei/stankins_linux.svg?maxAge=604800)

# Demo
[Live demo https://azurestankins.azurewebsites.net](https://azurestankins.azurewebsites.net)

[Live swagger https://azurestankins.azurewebsites.net/swagger](https://azurestankins.azurewebsites.net/swagger)

# Releases

<a href='https://github.com/ignatandrei/stankins/releases'>Releases - Windows(Desktop + Console+ Site), Linux((Console+ Site),Android apk</a>

[ Docker for Windows/Linux https://hub.docker.com/u/ignatandrei](https://hub.docker.com/u/ignatandrei)

Console Global tool:![Stankins.Console](https://img.shields.io/nuget/v/stankins.console.svg?label=Stankins%20Console&style=flat)

For docker , you can just run 
docker run -p 5000:5000 ignatandrei/stankins_windows
or
docker run -p 5000:5000 ignatandrei/stankins_linux

And then access http://localhost:5000
( if error, restart docker)

Usage:

dotnet tool install --global stankins.console

stankins.console execute -o ReceiveMetadataFromDatabaseSql -a "Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;"  -o SenderDBDiagramToDot

stankins.console execute -o ReceiveMetadataFromDatabaseSql -a "Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;"  -o SenderDBDiagramHTMLDocument

stankins.Console.exe execute -o ExportDBDiagramHtmlAndDot -a "Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>" -a a.html

dotnet Stankins.Console.dll execute -o ReceiverFromSolution -a "E:\StankinsV2\StankinsV2.sln" -o SenderSolutionToHTMLDocument -a "" -o TransformerOutputStringColumnName -a "'a.html'"


<a href='https://cdn.rawgit.com/ignatandrei/stankins/74e25fbe/Documentation/Help/index.html'>Documentation</a>

<a href='https://cdn.rawgit.com/ignatandrei/stankins/74e25fbe/Documentation/Help/html/e6e8966d-f7ce-8571-98f2-b26beb8d1666.htm'>Example SQL Server to Elasticsearch</a>



