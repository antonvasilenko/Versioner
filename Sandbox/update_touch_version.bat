@echo off

for /f %%i in ('c:\work\binary\svn\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\Info.plist -m read -v data') do set OLD_VER=%%i

c:\work\binary\svn\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\Info.plist -m update -v normal -ver "#.#.333.444"

for /f %%i in ('c:\work\binary\svn\Versioner\Sources\Versioner\bin\Debug\Versioner.exe -f=c:\work\binary\svn\Versioner\Sandbox\Info.plist -m read -v data') do set NEW_VER=%%i
echo version changed from %OLD_VER% to %NEW_VER%
pause