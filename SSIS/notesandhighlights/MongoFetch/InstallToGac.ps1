# from https://www.andrewcbancroft.com/2015/12/16/using-powershell-to-install-a-dll-into-the-gac/

#Note that you should be running PowerShell as an Administrator
[System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")            
$publish = New-Object System.EnterpriseServices.Internal.Publish      
$publish.GacInstall("$PSScriptRoot\MongoDB.Bson.dll")
$publish.GacInstall("$PSScriptRoot\MongoDB.Driver.Core.dll")
$publish.GacInstall("$PSScriptRoot\MongoDB.Driver.dll")
$publish.GacInstall("$PSScriptRoot\MongoFetch.dll")
$publish.GacInstall("$PSScriptRoot\System.Runtime.InteropServices.RuntimeInformation.dll")

#If installing into the GAC on a server hosting web applications in IIS, you need to restart IIS for the applications to pick up the change.
#Uncomment the next line if necessary...
#iisreset

