## Getting Started (Development)

Developed on Windows using Visual Studio, WSL and Docker

### Prequisites

- Visual Studio 2026 (Community or Professional Edition)
- Git for Windows
- Docker Desktop
- WSL - Ubuntu ???
- Sql Server Management Studio (SSMS)

## Source Repositories

- [Growney/NaeRaces](https://github.com/Growney/NaeRaces)
- [Growney/EventDbLite](https://github.com/Growney/EventDbLite)

## Setting up

Clone [Growney/NaeRaces](https://github.com/Growney/NaeRaces) and initialise submodules

```
git clone --recurse-submodules https://github.com/Growney/NaeRaces
```

Run the required docker containers for SqlServer and kurrentdb using the commands from:
- [dev-database-init.sh](./dev-database-init.sh)

In Visual Studio make NaeRaces.WebAPI the startup application then open the Package Manager Console
(Tools -> Nuget Package Manager -> Package Manager Console) and run the following:

```
cd NaeRaces.WebAPI
update-database -Context NaeRacesQueryDbContext
```

## Debugging without containers...

Run NaeRaces.WebAPI and NaeRaces.BlazorWebApp

## Developing and Debugging with containers...

Build and run the containers for NaeRaces.WebAPI and NaeRaces.BlazorWebApp....

## Source Code concepts and structure

See [NaeRaces Project Summary](./NaeRaces_Project_Summary.md)