@ECHO OFF
IF EXIST "%WINDIR%\Microsoft.NET\Framework64\v4.*" (
COPY Files\NET4\*.* .
GOTO :END
)
IF EXIST "%WINDIR%\Microsoft.NET\Framework\v4.*" (
COPY Files\NET4\*.* .
GOTO :END
)
IF EXIST "%WINDIR%\Microsoft.NET\Framework64\v2.*" (
COPY Files\NET2\*.* .
GOTO :END
)
IF EXIST "%WINDIR%\Microsoft.NET\Framework\v2.*" (
COPY Files\NET2\*.* .
GOTO :END
)
:END
