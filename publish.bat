@echo off
set s_version=
set /p s_version=请输入版本号：
set /p s_key=请输入密钥：
setlocal ENABLEDELAYEDEXPANSION
set projects="Wodsoft.ComBoost.Core Wodsoft.ComBoost Wodsoft.ComBoost.Security Wodsoft.ComBoost.Mock Wodsoft.ComBoost.Redis Wodsoft.ComBoost.Storage Wodsoft.ComBoost.Data.Core Wodsoft.ComBoost.Data Wodsoft.ComBoost.AspNetCore Wodsoft.ComBoost.AspNetCore.Security Wodsoft.ComBoost.EntityFramework Wodsoft.ComBoost.EntityFrameworkCore Wodsoft.ComBoost.Mvc Wodsoft.ComBoost.Mvc.Data"

call:fun_arr %projects% array len
for /l %%i in (1,1,%len%) do (  
    echo 正在发布!array[%%i]!
    dotnet nuget push build\!array[%%i]!.%s_version%.symbols.nupkg -s https://www.nuget.org -k %s_key% --skip-duplicate
)

pause

:fun_arr
    rem 用法：call:fun_arr %tmp% array len
    rem  in: %tmp%  - 空格分隔的数据
    rem out: array  - 数组名
    rem      len    - 数组长度

    set tmpstr=%1
    set tmpstr=%tmpstr:"=%
    set "arr=%2"

    rem 清空数组
    set /a n=0
    :fun_arrCls_loop
        set /a n+=1
        if "!%arr%[%n%]!" equ "" (goto:fun_arr_continue)
        set "%arr%[!n!]="
    goto:fun_arrCls_loop

    rem 数组赋值
    :fun_arr_continue
    set /a n=0
    for %%j in (%tmpstr%) do (
        set /a n+=1
        set %arr%[!n!]=%%j
    )
    set %3=%n%
goto:eof