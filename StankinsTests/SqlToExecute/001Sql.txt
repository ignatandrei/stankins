IF OBJECT_ID('dbo.TestAndrei', 'U') IS NOT NULL
DROP TABLE dbo.TestAndrei;
GO
create table TestAndrei(
        ID int,
        FirstName varchar(64) not null
             );