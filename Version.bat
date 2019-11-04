COPY Readme.txt Build\Release\
COPY Features.txt Build\Release\

IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat      0000 0019
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat    00:00 00:19

Tools\Version\bin\Release\Version.exe .                 7.29.199.0019 7.29.199.8319 -Tools

Tools\Touch\bin\Release\Touch.exe Build                         00:19               -Tools
Tools\Touch\bin\Release\Touch.exe .                             00:19               -Tools
Tools\Touch\bin\Release\Touch.exe Tools    2009-07-29  07:29

COPY QuranCode1433.zip          ..\Backup\QuranCode1433_7.29.199.0019.zip
COPY QuranCode1433.Source.zip   ..\Backup\QuranCode1433_7.29.199.0019.Source.zip
COPY QuranCode1433.zip          ..\Backup\
COPY QuranCode1433.Source.zip   ..\Backup\
