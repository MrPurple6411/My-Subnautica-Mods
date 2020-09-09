# Subnautica-Mods
  This is my repository of mods that I have made as well as many that I have adopted that were open source or that I have gotten permission for.
  
  If you are a mod author returning to Subnautica and I have been updating your mod and you would like it removed from this repository then please let me know and it will be done without hesitation.



This repository now uses 4 game folders for the 4 branches of the 2 games and the folders are setup using information learned from https://forum.keenswh.com/threads/how-to-easily-switch-between-steam-branches-for-modding.7385481/.   



My Batch files are as follows.

switching to Stable:
==============================
@echo off
setlocal
set BRANCH=Stable
Set GAME=Subnautica
taskkill /t /f /im steam.exe 2>NUL
taskkill /t /f /im %GAME%.exe 2>NUL
rmdir %GAME%
mklink /j %GAME% %GAME%.%BRANCH%
:: Swap the steam manifest
del ..\appmanifest_264710.acf
mklink /h ..\appmanifest_264710.acf ..\appmanifest_264710.acf.%BRANCH%

Set GAME=SubnauticaZero
taskkill /t /f /im %GAME%.exe 2>NUL
rmdir %GAME%
mklink /j %GAME% %GAME%.%BRANCH%
:: Swap the steam manifest
del ..\appmanifest_848450.acf
mklink /h ..\appmanifest_848450.acf ..\appmanifest_848450.acf.%BRANCH%


===============================

switching to Experimental branches


@echo off
setlocal
set BRANCH=Exp
Set GAME=Subnautica
taskkill /t /f /im steam.exe 2>NUL
taskkill /t /f /im %GAME%.exe 2>NUL
rmdir %GAME%
mklink /j %GAME% %GAME%.%BRANCH%
:: Swap the steam manifest
del ..\appmanifest_264710.acf
mklink /h ..\appmanifest_264710.acf ..\appmanifest_264710.acf.%BRANCH%

Set GAME=SubnauticaZero
taskkill /t /f /im %GAME%.exe 2>NUL
rmdir %GAME%
mklink /j %GAME% %GAME%.%BRANCH%
:: Swap the steam manifest
del ..\appmanifest_848450.acf
mklink /h ..\appmanifest_848450.acf ..\appmanifest_848450.acf.%BRANCH%
