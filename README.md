# WorkerService.DatabaseOrganizer
This is a Worker Service that executes a SQL command every 30 days using the Quartz library.
The essence of the service is to delete unused databases, which have not been used for more than 30 days, in the MS SQL environment.
The service can be installed as a Windows service.
If you wish to do this, you must do two things:
1. Publish the project in the folder using the Package Manager Console by entering the command `dotnet publish -o d:\workerpub`.
2. Run the command line with administrator rights and run the command `sc create WorkerDatabaseOrganizer binPath=d:\workerpub\DatabaseOrganizer.exe`.

To start and stop the service you need to press the key combination `Win + R` and paste `services.msc`, then you can find the _WorkerDatabaseOrganizer_ service.

> WARNING. After starting the service in the development environment or as a Windows service, the program will immediately delete all databases that have not been used for more than 30 days.
