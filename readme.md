## High-Performance DotNet CRON Jobs

This is code to go along with an article I wrote. Each separate .NET project loosely represents one step in the progression of basic->advanced ways to build a CRON job system.

Here are the instructions for getting this working on your machine:

## Databases

Make sure you have Docker installed.

Run `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@assword" -p 1433:1433 --name sql1 --hostname sql1 -d mcr.microsoft.com/mssql/server:2022-latest`

That will install and run an SQLServer docker image/container.

I used [this .backpac of the WideWorldImporters sample database.](https://github.com/Microsoft/sql-server-samples/releases/download/wide-world-importers-v1.0/WideWorldImporters-Full.bacpac). You can import this .bacpack using Azure Data Studio, SQL Management Studio, DataGrip, etc.

## Code

Clone the repo, make sure the SQLServer docker container is running, and run each project's docker file!

To run each projector, run the following command from the root of a given project: `docker-compose up --build --remove-orphans --force-recreate`

