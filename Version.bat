COPY Readme.txt Build\Release\
COPY Features.txt Build\Release\

IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat    0000 0002
Tools\Version\bin\Release\Version.exe .               7.29.139.0002 7.29.139.0002 -Tools
Tools\Touch\bin\Release\Touch.exe Build                    16:53                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools         2009-07-29 16:53
Tools\Touch\bin\Release\Touch.exe .                        16:53                  -Tools

COPY QuranCode1433.zip        ..\Backup\QuranCode1433_7.29.139.0002.zip
COPY QuranCode1433.Source.zip ..\Backup\QuranCode1433_7.29.139.0002.Source.zip
COPY QuranCode1433.zip        ..\Backup\
COPY QuranCode1433.Source.zip ..\Backup\
