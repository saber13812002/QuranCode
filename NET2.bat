RD /S /Q NET2
XCOPY /O /X /E /H /K /Y Globals\*.* NET2\Globals\
XCOPY /O /X /E /H /K /Y Utilities\*.* NET2\Utilities\
XCOPY /O /X /E /H /K /Y Model\*.* NET2\Model\
XCOPY /O /X /E /H /K /Y DataAccess\*.* NET2\DataAccess\
XCOPY /O /X /E /H /K /Y Server\*.* NET2\Server\
XCOPY /O /X /E /H /K /Y Client\*.* NET2\Client\
XCOPY /O /X /E /H /K /Y Research\*.* NET2\Research\
XCOPY /O /X /E /H /K /Y Common\*.* NET2\Common\
XCOPY /O /X /E /H /K /Y QuranCode\*.* NET2\QuranCode\
XCOPY /O /X /E /H /K /Y ScriptRunner\*.* NET2\ScriptRunner\
XCOPY /O /X /E /H /K /Y PrimeCalculator\*.* NET2\PrimeCalculator\
XCOPY /O /X /E /H /K /Y QuranLab\*.* NET2\QuranLab\
XCOPY /O /X /E /H /K /Y InitialLetters\*.* NET2\InitialLetters\
XCOPY /O /X /E /H /K /Y Composites\*.* NET2\Composites\
XCOPY /O /X /E /H /K /Y Numbers\*.* NET2\Numbers\
XCOPY /O /X /E /H /K /Y WordGenerator\*.* NET2\WordGenerator\
XCOPY /O /X /E /H /K /Y AhlulBayt\*.* NET2\AhlulBayt\
XCOPY /O /X /E /H /K /Y Tools\NET2sln\*.* NET2\
COPY *.txt NET2\

Tools\Replace\bin\Release\Replace.exe NET2 *.cs ((System.ComponentModel.ISupportInitialize) //((System.ComponentModel.ISupportInitialize)

Version.bat
CD NET2
CALL Version.bat
CD ..
