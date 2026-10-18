# COM HIJACKING DEMO:
this code is made for educational purposes only, it is not intended to be used in any illegal means.
I'm not sure if the code is functional or not nor do i care, i made it in a short amount of time just for fun and it isnt intended to be used practically.
This tool demonstrates how that resolution order can be abused to redirect a legitimate process into loading an arbitrary DLL without admin rights.

requirements:
- Windows 10/11
- .NET 6.0 SDK or later
- no admin rights required by design

how it works:
App requests CLSID > Check HKCU\Software\Classes\CLSID{xxxx} (entry planted here) > Found? > Load your DLL (running under the legitimate process) > Not found? >Check HKLM\SOFTWARE\Classes\CLSID{xxxx} (normal resolution)
