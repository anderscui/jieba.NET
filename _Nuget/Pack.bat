@ECHO OFF
SET RELAEASE_PATH=..\_bin
call:MAIN_FUNC
GOTO:EOF

:MAIN_FUNC
@ECHO OFF
rd /s /q packtmp
rd /s /q ..\_Pkgs
mkdir packtmp\lib\

:: echo d x| xcopy ..\content\Resources packtmp\content\Resources /E /Y
echo d x| xcopy ..\content\Resources packtmp\Resources /E /Y

CALL:FOR_EACH %RELAEASE_PATH% COPY_PKG_FILES

NuGet pack jieba.NET.nuspec -OutputDirectory ../_Pkgs/  -BasePath packtmp
rd /s /q packtmp

pause

GOTO:EOF

:: 显示所有命令
:COPY_PKG_FILES
:: CALL:FOR_EACH_FILE %RELAEASE_PATH%\%1 PRINT
echo f | xcopy %RELAEASE_PATH%\%1\JiebaNet.Analyser.dll packtmp\lib\%1\JiebaNet.Analyser.dll /E /Y
echo f | xcopy %RELAEASE_PATH%\%1\JiebaNet.Analyser.xml packtmp\lib\%1\JiebaNet.Analyser.xml /E /Y
echo f | xcopy %RELAEASE_PATH%\%1\JiebaNet.Segmenter.dll packtmp\lib\%1\JiebaNet.Segmenter.dll /E /Y
echo f | xcopy %RELAEASE_PATH%\%1\JiebaNet.Segmenter.xml packtmp\lib\%1\JiebaNet.Segmenter.xml /E /Y
GOTO:EOF

:PRINT
ECHO.%1
GOTO:EOF

:FOR_EACH
SETLOCAL
  FOR /F "tokens=1,* delims=" %%I IN ('DIR %1 /A /B') DO (
    CALL:%2 %%I %%J
  )
ENDLOCAL
GOTO:EOF

:FOR_EACH_FILE
SETLOCAL
  FOR /F "tokens=1,* delims=" %%I IN ('DIR %1 /A-D /B') DO (
    CALL:%2 %%I %%J
  )
ENDLOCAL
GOTO:EOF