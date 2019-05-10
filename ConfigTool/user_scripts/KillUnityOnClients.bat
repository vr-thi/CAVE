@echo off
setlocal
set PRJ_PATH=%1
set PRJ_NAME=%2
set USER=user_name
set PSWD=password

if [%PRJ_PATH%] NEQ [] (
	for /l %%x in (1, 1, 8) do taskkill /S \\ic-%%x /U %USER% /P %PSWD% /IM %PRJ_NAME%.exe /T /F
)
::for /l %%x in (1, 1, 8) do %~dp0tools\PsExec.exe /S \\ic-%%x /U %USER% /P %PSWD% taskkill /IM %PRJ_NAME%.exe /T /F
endlocal