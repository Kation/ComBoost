@echo off
dotnet pack src\Wodsoft.ComBoost.Core --no-build --output build
dotnet pack src\Wodsoft.ComBoost --no-build --output build
dotnet pack src\Wodsoft.ComBoost.Security --no-build --output build
dotnet pack src\Wodsoft.ComBoost.AspNetCore.Security --no-build --output build
dotnet pack src\Wodsoft.ComBoost.Data --no-build --output build
dotnet pack src\Wodsoft.ComBoost.EntityFramework --no-build --output build
dotnet pack src\Wodsoft.ComBoost.Mvc --no-build --output build
dotnet pack src\Wodsoft.ComBoost.Mvc.Data --no-build --output build
pause