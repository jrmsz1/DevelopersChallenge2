Meu perfil
-------

**Nome completo:Oziel Vilalba Junior**   
**Data de nascimento:1983-02-26**   
**LinkedIn:https://www.linkedin.com/in/oziel-junior-6a293525/**    
**Como nos conheceu:LinkedIn**   




FIRST - You must set SERVICESAPI project like initialization project.

I used Postgres database, for use SQLServer alter connectionString and uncomment in the file Startup.cs "services.AddEntityFrameworkSqlServer()";
Run commands in Package Manager Console:
EntityFrameworkCore\Add-Migration InitialDB
EntityFrameworkCore\Update-Database

Inside folder project (Prompt command), execute DOTNET command, for instance:
C:\Users\Junior\source\repos\OFXTestOziel\SERVICESAPI\bin\Debug\netcoreapp2.2\DOTNET SERVICESAPI.dll


Check in your browser:
https://localhost:5001/api/ofx


SECOND - You must set VIEWERWEB project like initialization project.
RUN Project


Best regards.