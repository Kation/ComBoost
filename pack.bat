@echo off
set s_version=
set /p s_version=«Î ‰»Î∞Ê±æ∫≈£∫
if "%s_version%" neq "" set "s_version=--version-suffix %s_version%"
if not exist build md build
cd build
del *.nupkg
cd..
dotnet pack src\Wodsoft.ComBoost.Core --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Security --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.AspNetCore.Security --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.AspNetCore --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Data.Core --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Data --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.EntityFramework --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.EntityFrameworkCore --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Mock --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Mvc --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Mvc.Data --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Storage --output build --include-source %s_version%
dotnet pack src\Wodsoft.ComBoost.Redis --output build --include-source %s_version%
pause