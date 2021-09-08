@echo off
set s_version=
set /p s_version=«Î ‰»Î∞Ê±æ∫≈£∫
if "%s_version%" neq "" set "s_version=--version-suffix %s_version%"
if not exist build md build
cd build
del *.nupkg
cd..
dotnet pack src\Wodsoft.ComBoost.Core -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Security -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.AspNetCore.Security -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.AspNetCore -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Data.Core -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Data -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.EntityFramework -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.EntityFrameworkCore -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Mock -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Mvc -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Mvc.Data -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Storage -c Release --output build --include-symbols %s_version%
dotnet pack src\Wodsoft.ComBoost.Redis -c Release --output build --include-symbols %s_version%
pause