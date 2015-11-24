@echo off

for /f %%i in ('c:\work\sources\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\CSharpRes.cs -m read -v data') do set OLD_VER=%%i

c:\work\sources\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\CSharpRes.cs -m update -v normal -ver "#.#.123.456"

for /f %%i in ('c:\work\sources\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\CSharpRes.cs -m read -v data') do set NEW_VER=%%i
echo version changed from %OLD_VER% to %NEW_VER%
pause