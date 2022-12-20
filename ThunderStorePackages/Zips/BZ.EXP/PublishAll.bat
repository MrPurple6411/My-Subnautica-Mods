for %%f in (.\*.zip) do ( 
IF NOT EXIST "%%f".done (
start CMD /K CALL tcli.bat %%f
)
)