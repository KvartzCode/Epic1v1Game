@echo off
echo Arguments: %*
cd /d "%~dp0"
say.exe -w "player1.wav" %*
