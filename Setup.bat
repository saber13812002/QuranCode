@echo off

setlocal

for /f "tokens=2 delims=[]" %%i in ('ver') do set VERSION=%%i
for /f "tokens=2-3 delims=. " %%i in ("%VERSION%") do set VERSION=%%i.%%j

if "%VERSION%" == "5.00" echo Windows 2000
if "%VERSION%" == "5.0" echo Windows 2000
if "%VERSION%" == "5.1" echo Windows XP
if "%VERSION%" == "5.2" echo Windows Server 2003
if "%VERSION%" == "6.0" echo Windows Vista
if "%VERSION%" == "6.1" echo Windows 7
if "%VERSION%" == "6.2" echo Windows 8
if "%VERSION%" == "6.3" echo Windows 8.1
if "%VERSION%" == "6.4" echo Windows 10
if "%VERSION%" == "10.0" echo Windows 10

if "%VERSION%" == "5.00" COPY Files\Win7\*.* .
if "%VERSION%" == "5.0" COPY Files\Win7\*.* .
if "%VERSION%" == "5.1" COPY Files\Win7\*.* .
if "%VERSION%" == "5.2" COPY Files\Win7\*.* .
if "%VERSION%" == "6.0" COPY Files\Win10\*.* .
if "%VERSION%" == "6.1" COPY Files\Win10\*.* .
if "%VERSION%" == "6.2" COPY Files\Win10\*.* .
if "%VERSION%" == "6.3" COPY Files\Win10\*.* .
if "%VERSION%" == "6.4" COPY Files\Win10\*.* .
if "%VERSION%" == "10.0" COPY Files\Win10\*.* .

endlocal
