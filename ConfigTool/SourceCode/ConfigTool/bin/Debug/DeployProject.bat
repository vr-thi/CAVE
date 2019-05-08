@echo off
echo ###################################################################################################
echo BEGIN
echo ###################################################################################################

set /a startstamp=%time:~0,2%*3600000 + %time:~3,2%*60000 + %time:~6,2%*1000 + %time:~9,2%*10 

set CurrPath=%~dp0
set PrjPath=%1
set PrjName=%2
set IsOnlyUpdate=%3

echo CurrPath     = %CurrPath%
echo PrjPath      = %PrjPath:~1,-1%
echo PrjName      = %PrjName:~1,-1%
echo IsOnlyUpdate = %IsOnlyUpdate%

set /a startstamp=%time:~0,2%*3600000 + %time:~3,2%*60000 + %time:~6,2%*1000 + %time:~9,2%*10 

echo.
for /l %%i in (1, 1, 8) do (
	echo ***** Deploy/Update project on the client \\ic-%%i **************************************************
	if "%IsOnlyUpdate%" NEQ "OnlyUpdate" (
		echo --- Remove old project from \\ic-%%i\CAVE_UNITY -----------------------------------------------------
		DEL /Q /F \\ic-%%i\CAVE_UNITY\
		FOR /F "Tokens=*" %%a IN ('DIR /B /A:D \\ic-%%i\CAVE_UNITY\') DO RMDIR /Q /S "\\ic-%%i\CAVE_UNITY\%%a"
	)
	echo --- Copy files to \\ic-%%i\CAVE_UNITY ---------------------------------------------------------------
	XCOPY /E /Y /D %PrjPath%%PrjName%.exe \\ic-%%i\CAVE_UNITY\
	XCOPY /E /Y /D %PrjPath%%PrjName%_Data \\ic-%%i\CAVE_UNITY\%PrjName%_Data\
	echo ***** DONE ****************************************************************************************
	echo.
)

set /a stopstamp=%time:~0,2%*3600000 + %time:~3,2%*60000 + %time:~6,2%*1000 + %time:~9,2%*10
set /a diff=(%stopstamp% - %startstamp%)
set /a tSek=%diff%/1000
echo. 
echo ###################################################################################################
echo END. Duration: %tSek%,%diff:~-3,-1% sek.
echo ###################################################################################################
echo.