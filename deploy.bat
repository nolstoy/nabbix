

IF %1.==. GOTO Missing
NuGet.exe push Nabbix.%1.nupkg
NuGet.exe push Nabbix.%1.symbols.nupkg
GOTO End

:Missing
  ECHO Missing parameter. Correct format: deploy.bat 0.1.0.0
GOTO End

:End
