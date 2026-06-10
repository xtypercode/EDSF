@echo off
title EDSF

echo ========================================
echo  A iniciar EDSF.API + EDSF.App
echo ========================================

start "EDSF.Api" cmd /k dotnet run --project src\EDSF.Api\EDSF.Api.csproj
timeout /t 3 /nobreak >nul
start "EDSF.App" cmd /k dotnet run --project src\EDSF.App\EDSF.App.csproj -f net10.0-windows10.0.19041.0

echo.
echo API a correr em http://localhost:5285
echo App a iniciar...
echo.
echo Para parar tudo, feche as janelas ou prima Ctrl+C
