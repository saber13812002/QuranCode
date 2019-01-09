@echo off
:START

cd Tools
CALL Clean
cd ..

del *.zip
del *.rar
del /F /S /Q *.tmp
del /F /S /Q *.bak
del /F /S /Q *.ini
del /F /S /Q *.user
del /F /S /Q *.pdb
del /F /S /Q *.resources
del /F /S /Q *.vshost.exe
del /F /S /Q *.vshost.exe.manifest
del /F /S /Q *.exe.config
#del /F /S /Q /AH *.suo
rd /S /Q Build\Debug

rd /S /Q Globals\obj
rd /S /Q Utilities\obj
rd /S /Q Model\obj
rd /S /Q DataAccess\obj
rd /S /Q Server\obj
rd /S /Q Client\obj
rd /S /Q Common\obj
rd /S /Q QuranKey\obj

:END
