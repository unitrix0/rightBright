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

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; \
  Flags: recursesubdirs ignoreversion createallsubdirs

; #region agent log
[Code]
function JsonString(const S: String): String;
begin
  Result := S;
  StringChangeEx(Result, '\', '\\', True);
  StringChangeEx(Result, '"', '\"', True);
end;

procedure AgentNdjson(const HypothesisId, Location, Message, DataJson: String);
var
  LogPath: String;
  Line: String;
begin
  LogPath := ExpandConstant('{localappdata}\rightBright\debug-6012ca.log');
  ForceDirectories(ExtractFilePath(LogPath));
  Line :=
    '{"sessionId":"6012ca","hypothesisId":"' + HypothesisId +
    '","location":"' + Location + '","message":"' + Message +
    '","data":' + DataJson + ',"timestamp":"' +
    GetDateTimeString('yyyy-mm-ddThh:nn:ss', #0, #0) + '"}' + #10;
  SaveStringToFile(LogPath, Line, True);
end;

function InitializeSetup(): Boolean;
var
  UserName: String;
  LocalApp: String;
begin
  Result := True;
  UserName := ExpandConstant('{username}');
  LocalApp := ExpandConstant('{localappdata}');
  AgentNdjson(
    'H3',
    'rightBright.iss:InitializeSetup',
    'Setup starting',
    '{"username":"' + JsonString(UserName) + '","localAppData":"' + JsonString(LocalApp) + '"}');
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  AppDir: String;
  ExePath: String;
  RunVal: String;
  RunExists: Boolean;
  ExistsStr: String;
begin
  if CurStep = ssInstall then
  begin
    AppDir := ExpandConstant('{app}');
    ExePath := ExpandConstant('{app}\rightBright.exe');
    AgentNdjson(
      'H4',
      'rightBright.iss:CurStepChanged:ssInstall',
      'Entering install phase (files and registry)',
      '{"app":"' + JsonString(AppDir) + '","exePath":"' + JsonString(ExePath) + '"}');
  end;

  if CurStep = ssPostInstall then
  begin
    AppDir := ExpandConstant('{app}');
    ExePath := ExpandConstant('{app}\rightBright.exe');
    RunVal := '';
    RunExists :=
      RegQueryStringValue(
        HKEY_CURRENT_USER,
        'Software\Microsoft\Windows\CurrentVersion\Run',
        'rightBright',
        RunVal);
    if RunExists then
      ExistsStr := 'true'
    else
      ExistsStr := 'false';
    AgentNdjson(
      'H1_H2_H5',
      'rightBright.iss:CurStepChanged:ssPostInstall',
      'Post-install Run key probe',
      '{"runKeyExists":' + ExistsStr +
      ',"runValue":"' + JsonString(RunVal) +
      '","expectedExePath":"' + JsonString(ExePath) + '"}');
  end;
end;
; #endregion

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
