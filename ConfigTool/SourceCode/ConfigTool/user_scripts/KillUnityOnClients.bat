if [%1] NEQ [] (
	for /l %%x in (1, 1, 8) do taskkill /S \\ic-%%x /U icuser /P icuser1 /IM %2.exe /T /F
)
::for /l %%x in (1, 1, 8) do %~dp0tools\PsExec.exe /S \\ic-%%x /U icuser /P icuser1 taskkill /IM Meshi /T /F