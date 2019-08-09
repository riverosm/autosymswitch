@echo OFF
echo Stopping old service version...
net stop "[YOUR SERVICE NAME]"
echo Uninstalling old service version...
sc delete "[YOUR SERVICE NAME]"

echo Installing service...
rem DO NOT remove the space after "binpath="!
sc create "[YOUR SERVICE NAME]" binpath= "[PATH_TO_YOUR_SERVICE_EXE]" start= auto displayname= "VAI"
echo Starting server complete
pause