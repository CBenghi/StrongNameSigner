; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppID "{A89C23E7-A764-4F59-8EF3-5AB0A33E5849}"
#define MyAppName ".NET Assembly Strong-Name Signer"
#define MyAppNameNoSpaces "StrongNameSigner"
#define MyAppVersion "3.5.0.0"
#define MyAppPublisher "BrutalDev"
#define MyAppURL "https://github.com/brutaldev/StrongNameSigner"
#define MyAppExeName "StrongNameSigner.exe"

#include "scripts\products.iss"   
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"
#include "scripts\products\dotnetfx40full.iss"

[Setup]
AppId={{#MyAppID}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppPublisher}
AllowNoIcons=yes
LicenseFile=EULA.txt
OutputDir=..\..\deploy
OutputBaseFilename={#MyAppNameNoSpaces}_Setup
Compression=lzma/ultra64
SolidCompression=yes
InternalCompressLevel=ultra
AppCopyright={#MyAppPublisher}
UninstallDisplayIcon={app}\{#MyAppExeName}
MinVersion=0,5.01sp3
ArchitecturesInstallIn64BitMode=x64 ia64
ShowLanguageDialog=auto
PrivilegesRequired=admin

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2

[Files]
Source: "..\Brutal.Dev.StrongNameSigner.Docs\output\Help.chm"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Mono.Cecil.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Mono.Cecil.Mdb.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Mono.Cecil.Pdb.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Mono.Cecil.Rocks.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Brutal.Dev.StrongNameSigner.XML"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner\bin\Release\netstandard2.0\publish\Brutal.Dev.StrongNameSigner.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion
Source: "..\Brutal.Dev.StrongNameSigner.Console\bin\Release\StrongNameSigner.Console.exe"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: Console
Source: "..\Brutal.Dev.StrongNameSigner.Console\bin\Release\StrongNameSigner.Console.exe.config"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: Console
Source: "..\Brutal.Dev.StrongNameSigner.Console\bin\Release\PowerArgs.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: Console
Source: "..\Brutal.Dev.StrongNameSigner.UI\bin\Release\Microsoft.WindowsAPICodePack.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: UserInterface
Source: "..\Brutal.Dev.StrongNameSigner.UI\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: UserInterface
Source: "..\Brutal.Dev.StrongNameSigner.UI\bin\Release\StrongNameSigner.exe"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: UserInterface
Source: "..\Brutal.Dev.StrongNameSigner.UI\bin\Release\StrongNameSigner.exe.config"; DestDir: "{app}"; Flags: ignoreversion replacesameversion; Components: UserInterface

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Components: UserInterface
Name: "{group}\{#MyAppName} API Help"; Filename: "{app}\Help.chm"; Components: UserInterface
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[ThirdParty]
UseRelativePaths=True

[Components]
Name: "UserInterface"; Description: "Simple user interface to strong-name sign assemblies easily."; Types: compact full
Name: "Console"; Description: "Console version to strong-name sign from the command-line."; Types: full
                  
[Code]
/////////////////////////////////////////////////////////////////////////////////////
function InitializeSetup(): Boolean;

begin
  Result := True;

  initwinversion();
  dotnetfx40full();
end;
