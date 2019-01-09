RD /S /Q NET2
"%PROGRAMFILES%\7-Zip\7z.exe" x Tools\NET2.zip
echo .csproj > exclude.txt
XCOPY /E /Y /EXCLUDE:exclude.txt Globals\*.* NET2\Globals\
XCOPY /E /Y /EXCLUDE:exclude.txt Utilities\*.* NET2\Utilities\
XCOPY /E /Y /EXCLUDE:exclude.txt Model\*.* NET2\Model\
XCOPY /E /Y /EXCLUDE:exclude.txt DataAccess\*.* NET2\DataAccess\
XCOPY /E /Y /EXCLUDE:exclude.txt Server\*.* NET2\Server\
XCOPY /E /Y /EXCLUDE:exclude.txt Client\*.* NET2\Client\
XCOPY /E /Y /EXCLUDE:exclude.txt Research\*.* NET2\Research\
XCOPY /E /Y /EXCLUDE:exclude.txt Common\*.* NET2\Common\
XCOPY /E /Y /EXCLUDE:exclude.txt QuranCode\*.* NET2\QuranCode\
XCOPY /E /Y /EXCLUDE:exclude.txt PrimeCalculator\*.* NET2\PrimeCalculator\
XCOPY /E /Y /EXCLUDE:exclude.txt QuranLab\*.* NET2\QuranLab\
XCOPY /E /Y /EXCLUDE:exclude.txt InitialLetters\*.* NET2\InitialLetters\
XCOPY /E /Y /EXCLUDE:exclude.txt Composites\*.* NET2\Composites\
XCOPY /E /Y /EXCLUDE:exclude.txt Numbers\*.* NET2\Numbers\
XCOPY /E /Y /EXCLUDE:exclude.txt WordGenerator\*.* NET2\WordGenerator\
XCOPY /E /Y /EXCLUDE:exclude.txt AhlulBayt\*.* NET2\AhlulBayt\

DEL exclude.txt
Tools\Replace\bin\Release\Replace.exe NET2 *.cs ((System.ComponentModel.ISupportInitialize) //((System.ComponentModel.ISupportInitialize)

Version.bat
CD NET2
CALL Version.bat
CD ..
