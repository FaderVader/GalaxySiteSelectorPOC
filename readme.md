# Galaxy Site Selector Proof-of-concept #

A POC based on .NET Core ASP.NET Blazor template, this project explores running a Kestrel-based web & REST API as a windows service.

### Installing as windows service: ###

The project should be **published** as single-exe, non-framework dependent build. 
Install service at win command prompt: sc.exe {servicename} {complete path to exe}. After install, set LogOnAs to current windows-user. Otherwise the service is not allowed to interact with the system.

### Logs and configuration ###
Copy appsettings.json to C:\ProgramData\GalaxyWeb
Also folder wwwroot






