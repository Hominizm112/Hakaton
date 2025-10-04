@echo off
title WebGL Server

echo Starting WebGL Server and Tunnel...
echo.

@REM cd /d "D:\UnityGames\Hakaton\Builds"

echo Starting Python server...
start "WebGL Server" python server.py

echo Waiting for server to start...
timeout /t 3 /nobreak >nul

echo Starting Tuna tunnel...
tuna http 8000

pause