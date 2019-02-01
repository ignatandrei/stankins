# stankins


## Version 2 

# Continous Deploy
[![Build Status](https://dev.azure.com/ignatandrei0674/stankinsv2/_apis/build/status/ignatandrei.stankins?branchName=master)](https://dev.azure.com/ignatandrei0674/stankinsv2/_build/latest?definitionId=1?branchName=master)

[![codecov](https://codecov.io/gh/ignatandrei/stankins/branch/master/graph/badge.svg)](https://codecov.io/gh/ignatandrei/stankins)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ignatandrei_stankins&metric=alert_status)](https://sonarcloud.io/dashboard?id=ignatandrei_stankins)

[![DepShield Badge](https://depshield.sonatype.org/badges/ignatandrei/stankins/depshield.svg)](https://depshield.github.io)


# Demo
[Live demo https://azurestankins.azurewebsites.net](https://azurestankins.azurewebsites.net)



[Live swagger https://azurestankins.azurewebsites.net/swagger](https://azurestankins.azurewebsites.net/swagger)


[Pipeline Azure Devops https://dev.azure.com/ignatandrei0674/stankinsv2/_build?definitionId=1](https://dev.azure.com/ignatandrei0674/stankinsv2/_build?definitionId=1)


<a href='https://github.com/ignatandrei/stankins/releases'>Releases - Windows(Desktop + Console+ Site), Linux((Console+ Site),Android apk</a>

[ Docker for Windows/Linux https://hub.docker.com/u/ignatandrei](https://hub.docker.com/u/ignatandrei)

<a href='https://cdn.rawgit.com/ignatandrei/stankins/74e25fbe/Documentation/Help/index.html'>Documentation</a>

<a href='https://cdn.rawgit.com/ignatandrei/stankins/74e25fbe/Documentation/Help/html/e6e8966d-f7ce-8571-98f2-b26beb8d1666.htm'>Example SQL Server to Elasticsearch</a>


Console Global tool:![Stankins.Console](https://img.shields.io/nuget/v/stankins.console.svg?label=Stankins%20Console&style=flat)

dotnet tool install --global stankins.console

stankins execute -o ReceiveMetadataFromDatabaseSql -a "Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;"  -o SenderDBDiagramToDot


stankins.Console.dll execute -o ReceiveMetadataFromDatabaseSql -a "Server=(local);Database=tests;User Id=SA;Password = <YourStrong!Passw0rd>;"  -o SenderDBDiagramHTMLDocument

