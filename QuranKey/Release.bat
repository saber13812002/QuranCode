@echo off

"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranKey.Source.zip *.bat
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranKey.Source.zip *.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranKey.Source.zip *.sln
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranKey.Source.zip *.suo
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Globals\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Utilities\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Model\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip DataAccess\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Server\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Client\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip Common\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.Source.zip QuranKey\*.*

CD Build\Release
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranKey.zip *.*
MOVE QuranKey.zip ..\..\QuranKey.zip
CD ..\..

CALL Version.bat
