@ECHO OFF
:: %comspec% /k "D:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"
:: %comspec% /k "D:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
setlocal enableextensions enabledelayedexpansion
%~d0
cd "%~dp0"

SET "BAT_FILE=Common7\Tools\VsDevCmd.bat"

for %%D in (C D E F G H I J K L M N O P Q R S T U V W X Y Z A B) do (
	SET "PROG_FILE=%%D:\Program Files (x86)\"

	IF EXIST %%D:\ FOR %%E in (Enterprise, Professional, Community) DO (		
		SET VS_DEV_BAT="!PROG_FILE!Microsoft Visual Studio\2017\%%E\%BAT_FILE%"
		IF EXIST "!VS_DEV_BAT!" GOTO:EXEC_BAT			
	)
	
	SET VS_DEV_BAT="!PROG_FILE!Microsoft Visual Studio 14.0\%BAT_FILE%"
	IF EXIST "!VS_DEV_BAT!" GOTO:EXEC_BAT
	
	SET VS_DEV_BAT="!PROG_FILE!Microsoft Visual Studio 12.0\%BAT_FILE%"
	IF EXIST "!VS_DEV_BAT!" GOTO:EXEC_BAT
	
	FOR /F "delims=" %%E IN ('dir /b /ad "!PROG_FILE!\Microsoft Visual Studio\"') DO (
		set VS_DEV_BAT="!PROG_FILE!\Microsoft Visual Studio\%%E\%BAT_FILE%"
		IF EXIST "!VS_DEV_BAT!" GOTO:EXEC_BAT
		
		FOR /F "delims=" %%F IN ('dir /b /ad "!PROG_FILE!\Microsoft Visual Studio\%%E"') DO (
			set VS_DEV_BAT="!PROG_FILE!\Microsoft Visual Studio\%%E\%%F\%BAT_FILE%"
			IF EXIST "!VS_DEV_BAT!" GOTO:EXEC_BAT
		)
	)	
)

:EXEC_BAT
ECHO.%comspec% /k %VS_DEV_BAT%
%comspec% /k %VS_DEV_BAT%
GOTO:EOF