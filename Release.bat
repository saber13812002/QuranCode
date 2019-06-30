@echo off

CALL Version.bat

CALL Clean.bat
CD NET2
CALL Clean.bat
CD ..

"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip LICENSE
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.md
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.bat
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.sln
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.suo
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Globals\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Utilities\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Model\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip DataAccess\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Server\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Client\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Research\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Common\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip QuranCode\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip ScriptRunner\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip PrimeCalculator\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip QuranLab\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip InitialLetters\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Composites\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Numbers\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip WordGenerator\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip AhlulBayt\*.*

CD NET2
CALL Version.bat
CD ..

CD Build\Release
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Audio\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Data\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Fonts\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Help\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Images\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Languages\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Numbers\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Rules\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Translations\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip UserText\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Values\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Scripts\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Readme.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Features.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Clean.bat
MOVE QuranCode1433.zip ..\..\QuranCode1433.zip
CD ..\..

MD Files
CD Files
MD NET2
MD NET4
CD ..
COPY /Y NET2\Build\Release\*.exe Files\NET2\
COPY /Y NET2\Build\Release\*.dll Files\NET2\
COPY /Y      Build\Release\*.exe Files\NET4\
COPY /Y      Build\Release\*.dll Files\NET4\
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Files\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Setup.bat
RD /S /Q Files

CALL Version.bat
