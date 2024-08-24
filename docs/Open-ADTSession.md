---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Open-ADTSession

## SYNOPSIS
Opens a new ADT session.

## SYNTAX

```
Open-ADTSession [-SessionState] <SessionState> [[-DeploymentType] <String>] [[-DeployMode] <String>]
 [-AllowRebootPassThru] [-TerminalServerMode] [-DisableLogging] [[-AppVendor] <String>] [[-AppName] <String>]
 [[-AppVersion] <String>] [[-AppArch] <String>] [[-AppLang] <String>] [[-AppRevision] <String>]
 [[-AppExitCodes] <Int32[]>] [[-AppRebootCodes] <Int32[]>] [[-AppScriptVersion] <Version>]
 [[-AppScriptDate] <String>] [[-AppScriptAuthor] <String>] [[-InstallName] <String>] [[-InstallTitle] <String>]
 [[-DeployAppScriptFriendlyName] <String>] [[-DeployAppScriptVersion] <Version>]
 [[-DeployAppScriptDate] <String>] [[-DeployAppScriptParameters] <IDictionary>] [[-DirFiles] <String>]
 [[-DirSupportFiles] <String>] [[-DefaultMsiFile] <String>] [[-DefaultMstFile] <String>]
 [[-DefaultMspFiles] <String[]>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
This function initializes and opens a new ADT session with the specified parameters.
It handles the setup of the session environment and processes any callbacks defined for the session.
If the session fails to open, it handles the error and closes the session if necessary.

## EXAMPLES

### EXAMPLE 1
```
Open-ADTSession -SessionState $ExecutionContext.SessionState -DeploymentType "Install" -DeployMode "Interactive"
```

Opens a new ADT session with the specified parameters.

## PARAMETERS

### -SessionState
Caller's SessionState.

```yaml
Type: SessionState
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeploymentType
Deploy-Application.ps1 Parameter.
Specifies the type of deployment: Install, Uninstall, or Repair.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeployMode
Deploy-Application.ps1 Parameter.
Specifies the deployment mode: Interactive, NonInteractive, or Silent.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 3
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AllowRebootPassThru
Deploy-Application.ps1 Parameter.
Allows reboot pass-through.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -TerminalServerMode
Deploy-Application.ps1 Parameter.
Enables Terminal Server mode.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -DisableLogging
Deploy-Application.ps1 Parameter.
Disables logging for the session.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppVendor
Deploy-Application.ps1 Parameter.
Specifies the application vendor.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 4
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppName
Deploy-Application.ps1 Parameter.
Specifies the application name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 5
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppVersion
Deploy-Application.ps1 Parameter.
Specifies the application version.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 6
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppArch
Deploy-Application.ps1 Parameter.
Specifies the application architecture.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 7
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppLang
Deploy-Application.ps1 Parameter.
Specifies the application language.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 8
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppRevision
Deploy-Application.ps1 Parameter.
Specifies the application revision.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 9
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppExitCodes
Deploy-Application.ps1 Parameter.
Specifies the application exit codes.

```yaml
Type: Int32[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 10
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppRebootCodes
Deploy-Application.ps1 Parameter.
Specifies the application reboot codes.

```yaml
Type: Int32[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 11
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppScriptVersion
Deploy-Application.ps1 Parameter.
Specifies the application script version.

```yaml
Type: Version
Parameter Sets: (All)
Aliases:

Required: False
Position: 12
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppScriptDate
Deploy-Application.ps1 Parameter.
Specifies the application script date.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 13
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppScriptAuthor
Deploy-Application.ps1 Parameter.
Specifies the application script author.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 14
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InstallName
Deploy-Application.ps1 Parameter.
Specifies the install name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 15
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InstallTitle
Deploy-Application.ps1 Parameter.
Specifies the install title.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 16
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeployAppScriptFriendlyName
Deploy-Application.ps1 Parameter.
Specifies the friendly name of the deploy application script.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 17
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeployAppScriptVersion
Deploy-Application.ps1 Parameter.
Specifies the version of the deploy application script.

```yaml
Type: Version
Parameter Sets: (All)
Aliases:

Required: False
Position: 18
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeployAppScriptDate
Deploy-Application.ps1 Parameter.
Specifies the date of the deploy application script.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 19
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeployAppScriptParameters
Deploy-Application.ps1 Parameter.
Specifies the parameters for the deploy application script.

```yaml
Type: IDictionary
Parameter Sets: (All)
Aliases:

Required: False
Position: 20
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DirFiles
Deploy-Application.ps1 Parameter.
Specifies the path to Files.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 21
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DirSupportFiles
Deploy-Application.ps1 Parameter.
Specifies the path to SupportFiles.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 22
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DefaultMsiFile
Deploy-Application.ps1 Parameter.
Specifies the default MSI file.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 23
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DefaultMstFile
Deploy-Application.ps1 Parameter.
Specifies the default MST file.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 24
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DefaultMspFiles
Deploy-Application.ps1 Parameter.
Specifies the default MSP files.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 25
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Deploy-Application.ps1 Parameter.
Passes the session object through the pipeline.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
### You cannot pipe objects to this function.
## OUTPUTS

### ADTSession
### This function returns the session object if -PassThru is specified.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
