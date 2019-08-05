IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 1279
Tools\Version\bin\Release\Version.exe .               6.19.1279.4 6.19.1279.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                    -Tools
Tools\Touch\bin\Release\Touch.exe Tools    2009-07-29 7:29

Tools\Touch\bin\Release\Touch.exe .                   6:19                    -Tools
COPY QuranCode1433.zip        ..\Backup\QuranCode1433_6.19.1279.zip
COPY QuranCode1433.Source.zip ..\Backup\QuranCode1433_6.19.1279.Source.zip
COPY QuranCode1433.zip        ..\Backup\
COPY QuranCode1433.Source.zip ..\Backup\
