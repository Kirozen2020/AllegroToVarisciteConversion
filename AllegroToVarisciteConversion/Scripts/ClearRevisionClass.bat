@echo off

(echo using System;
    echo namespace AllegroToVarisciteConversion
    echo {
    echo     public static class Revision
    echo     {
    echo         public static string revision = "";
    echo     }
    echo }
) > ..\Revision.cs

echo C# class file (Revision.cs) Cleared successfully