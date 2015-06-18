@echo off

for /f %%i in ('c:\work\binary\svn\Versioner\Sources\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\AndroidManifest.xml -m read -v data') do set OLD_VER=%%i

c:\work\binary\svn\Versioner\Sources\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\AndroidManifest.xml -m update -v normal -ver "#.#.123.456"

for /f %%i in ('c:\work\binary\svn\Versioner\Sources\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\AndroidManifest.xml -m read -v data') do set NEW_VER=%%i
echo version changed from %OLD_VER% to %NEW_VER%
pause