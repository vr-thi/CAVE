@echo off

setlocal
set PrjPath=%1
set PrjName=%2
set StartOnMaster=%3

echo PrjPath  = %PrjPath:~1,-1%
echo PrjName  = %PrjName:~1,-1%
echo OnMaster = %StartOnMaster%
echo. 
echo ===== Start unity project on clients ==============================================================
for /l %%i in (1, 1, 8) do ( 
	echo ***** Start project on client IC-%%i!
	%~dp0tools\PsExec.exe \\ic-%%i -i -d -s c:\CAVE_UNITY\%PrjName:~1,-1%.exe
	echo       DONE 
)

echo.
timeout 1

if /i "%StartOnMaster%" == "AutoStartOnMaster" (
	echo ***** Start Unity project on Master-PC
	if exist %PrjPath% (
		call %PrjPath%
	) else (
		echo   ... #ERROR# bad path: %PrjPath%!
	)
) else (
	echo ***** The user should start the unity project on Master-PC manual !!
)
endlocal