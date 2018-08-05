

IF %1.==. GOTO Missing
dotnet nuget push .\bin\release\Nabbix.%1.nupkg -s https://nuget.org
GOTO End

:Missing
  ECHO Missing parameter. Correct format: deploy.bat 0.1.0.0
GOTO End

:End
