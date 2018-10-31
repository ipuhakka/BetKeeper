@setlocal enableextensions
@cd /d "%~dp0"

start API/bin/debug/API.exe
cd Client
call npm start
