COPY Readme.txt   Build\Release\
COPY Features.txt Build\Release\

Tools\Version\bin\Release\Version.exe .            7.29.139.0101 7.29.139.8317  -Tools

Tools\Touch\bin\Release\Touch.exe Build                         01:01           -Tools
Tools\Touch\bin\Release\Touch.exe .                             01:01           -Tools
Tools\Touch\bin\Release\Touch.exe Tools      2009-07-29         07:29

IF NOT EXIST ..\Backup\ MD ..\Backup
COPY QuranCode1433.zip                   ..\Backup\QuranCode1433_7.29.139.0101.zip
COPY QuranCode1433.Source.zip            ..\Backup\QuranCode1433_7.29.139.0101.Source.zip
COPY QuranCode1433.zip                   ..\Backup\
COPY QuranCode1433.Source.zip            ..\Backup\
