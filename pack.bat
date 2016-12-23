@echo off
cd build
del *.nupkg
cd..
dotnet pack src\Wodsoft.ComBoost.Core --output build
dotnet pack src\Wodsoft.ComBoost --output build
dotnet pack src\Wodsoft.ComBoost.Security --output build
dotnet pack src\Wodsoft.ComBoost.AspNetCore.Security --output build
dotnet pack src\Wodsoft.ComBoost.Data --output build
dotnet pack src\Wodsoft.ComBoost.Data.Core --output build
dotnet pack src\Wodsoft.ComBoost.EntityFramework --output build
dotnet pack src\Wodsoft.ComBoost.EntityFrameworkCore --output build
dotnet pack src\Wodsoft.ComBoost.Mvc --output build
dotnet pack src\Wodsoft.ComBoost.Mvc.Data --output build
dotnet pack src\Wodsoft.ComBoost.Storage --output build
dotnet pack src\Wodsoft.ComBoost.Redis --output build
pause