

IF %1.==. GOTO Missing
NuGet.exe push Nabbix.Metrics.%1.nupkg
NuGet.exe push Nabbix.Metrics.%1.symbols.nupkg
GOTO End

:Missing
  ECHO Missing parameter. Correct format: metrics-deploy.bat 0.1.0.0
GOTO End

:End
