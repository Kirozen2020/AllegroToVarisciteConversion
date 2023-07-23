@echo off

REM Check if the current directory is a Git repository
git rev-parse --is-inside-work-tree > nul 2>&1
if %ERRORLEVEL% == 1 (
    echo Not a Git repository. Please run this script from the root of your C# project.
    exit /b 1
)

REM Get the short commit ID
for /f %%i in ('git rev-parse --short HEAD') do set "commit_id=%%i"

REM Check if there are uncommitted changes
git diff --quiet
if %ERRORLEVEL% == 0 (
    set "revision=%commit_id%"
) else (
    git diff-index HEAD
    set "revision=%commit_id%-dirty"
)

REM Create the C# class file
(
    echo using System;
    echo namespace AllegroToVarisciteConversion
    echo {
    echo     public static class Revision
    echo     {
    echo         public static string revision = "%revision%";
    echo     }
    echo }
) > ..\Revision.cs

echo C# class file (Revision.cs) created successfully with revision: %revision%
