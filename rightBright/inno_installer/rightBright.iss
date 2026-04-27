; rightBright Inno Setup script
; Compile with: iscc /DAppVersion=x.y.z rightBright.iss

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif

[Setup]
AppId={{8C3D5A77-1F2B-4D5A-BE62-6B0D1C0C7A5B}
AppName=rightBright
AppVersion={#AppVersion}
AppPublisher=rightBright
DefaultDirName={autopf}\rightBright
DefaultGroupName=rightBright
DisableProgramGroupPage=yes
OutputDir=..\releases
OutputBaseFilename=rightBrightSetup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
SetupIconFile=..\rightBright\Assets\app_icon_new.ico
UninstallDisplayIcon={app}\rightBright.exe
WizardStyle=classic
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
MinVersion=10.0
CloseApplications=force
CloseApplicationsFilter=rightBright.exe
RestartApplications=no

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; \
  Flags: recursesubdirs ignoreversion createallsubdirs

[Icons]
Name: "{group}\rightBright"; Filename: "{app}\rightBright.exe"; \
  IconFilename: "{app}\rightBright.exe"
Name: "{group}\Uninstall rightBright"; Filename: "{uninstallexe}"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; \
  ValueType: string; ValueName: "rightBright"; \
  ValueData: """{app}\rightBright.exe"""; Flags: uninsdeletevalue

[Run]
Filename: "{app}\rightBright.exe"; \
  Flags: nowait postinstall skipifsilent; \
  Description: "Launch rightBright"

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM rightBright.exe"; \
  Flags: runhidden; RunOnceId: "KillApp"

[Code]
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  NeedsRestart := False;
  Exec('taskkill', '/F /IM rightBright.exe', '', SW_HIDE,
       ewWaitUntilTerminated, ResultCode);
  Sleep(500);
end;
