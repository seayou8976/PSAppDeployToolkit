---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Show-ADTInstallationWelcome

## SYNOPSIS
Show a welcome dialog prompting the user with information about the installation and actions to be performed before the installation can begin.

## SYNTAX

### None (Default)
```
Show-ADTInstallationWelcome [-ProcessObjects <ProcessObject[]>] [-Silent] [-CloseAppsCountdown <UInt32>]
 [-ForceCloseAppsCountdown <UInt32>] [-PromptToSave] [-PersistPrompt] [-BlockExecution] [-AllowDefer]
 [-AllowDeferCloseApps] [-DeferTimes <Int32>] [-DeferDays <UInt32>] [-DeferDeadline <String>]
 [-NoMinimizeWindows] [-NotTopMost] [-ForceCountdown <UInt32>] [-CustomText] [<CommonParameters>]
```

### CheckDiskSpace
```
Show-ADTInstallationWelcome [-ProcessObjects <ProcessObject[]>] [-Silent] [-CloseAppsCountdown <UInt32>]
 [-ForceCloseAppsCountdown <UInt32>] [-PromptToSave] [-PersistPrompt] [-BlockExecution] [-AllowDefer]
 [-AllowDeferCloseApps] [-DeferTimes <Int32>] [-DeferDays <UInt32>] [-DeferDeadline <String>] [-CheckDiskSpace]
 [-RequiredDiskSpace <UInt32>] [-NoMinimizeWindows] [-NotTopMost] [-ForceCountdown <UInt32>] [-CustomText]
 [<CommonParameters>]
```

## DESCRIPTION
The following prompts can be included in the welcome dialog:
    a) Close the specified running applications, or optionally close the applications without showing a prompt (using the -Silent switch).
    b) Defer the installation a certain number of times, for a certain number of days or until a deadline is reached.
    c) Countdown until applications are automatically closed.
    d) Prevent users from launching the specified applications while the installation is in progress.

## EXAMPLES

### EXAMPLE 1
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'iexplore' }, @{ Name = 'winword' }, @{ Name = 'excel' }
```

Prompt the user to close Internet Explorer, Word and Excel.

### EXAMPLE 2
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'winword' }, @{ Name = 'excel' } -Silent
```

Close Word and Excel without prompting the user.

### EXAMPLE 3
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'winword' }, @{ Name = 'excel' } -BlockExecution
```

Close Word and Excel and prevent the user from launching the applications while the installation is in progress.

### EXAMPLE 4
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'winword'; Description = 'Microsoft Office Word' }, @{ Name = 'excel'; Description = 'Microsoft Office Excel' } -CloseAppsCountdown 600
```

Prompt the user to close Word and Excel, with customized descriptions for the applications and automatically close the applications after 10 minutes.

### EXAMPLE 5
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'winword' }, @{ Name = 'msaccess' }, @{ Name = 'excel' } -PersistPrompt
```

Prompt the user to close Word, MSAccess and Excel.
By using the PersistPrompt switch, the dialog will return to the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml, so the user cannot ignore it by dragging it aside.

### EXAMPLE 6
```
Show-ADTInstallationWelcome -AllowDefer -DeferDeadline '25/08/2013'
```

Allow the user to defer the installation until the deadline is reached.

### EXAMPLE 7
```
Show-ADTInstallationWelcome -CloseApps @{ Name = 'winword' }, @{ Name = 'excel' } -BlockExecution -AllowDefer -DeferTimes 10 -DeferDeadline '25/08/2013' -CloseAppsCountdown 600
Close Word and Excel and prevent the user from launching the applications while the installation is in progress.
```


Allow the user to defer the installation a maximum of 10 times or until the deadline is reached, whichever happens first.

When deferral expires, prompt the user to close the applications and automatically close them after 10 minutes.

## PARAMETERS

### -ProcessObjects
Name of the process to stop (do not include the .exe).
Specify multiple processes separated by a comma.
Specify custom descriptions like this: @{ Name = 'winword'; Description = 'Microsoft Office Word'},@{ Name = 'excel'; Description = 'Microsoft Office Excel'}

```yaml
Type: ProcessObject[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Silent
Stop processes without prompting the user.

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

### -CloseAppsCountdown
Option to provide a countdown in seconds until the specified applications are automatically closed.
This only takes effect if deferral is not allowed or has expired.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -ForceCloseAppsCountdown
Option to provide a countdown in seconds until the specified applications are automatically closed regardless of whether deferral is allowed.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -PromptToSave
Specify whether to prompt to save working documents when the user chooses to close applications by selecting the "Close Programs" button.
Option does not work in SYSTEM context unless toolkit launched with "psexec.exe -s -i" to run it as an interactive process under the SYSTEM account.

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

### -PersistPrompt
Specify whether to make the Show-ADTInstallationWelcome prompt persist in the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml.
The user will have no option but to respond to the prompt.
This only takes effect if deferral is not allowed or has expired.

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

### -BlockExecution
Option to prevent the user from launching processes/applications, specified in -CloseApps, during the installation.

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

### -AllowDefer
Enables an optional defer button to allow the user to defer the installation.

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

### -AllowDeferCloseApps
Enables an optional defer button to allow the user to defer the installation only if there are running applications that need to be closed.
This parameter automatically enables -AllowDefer

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

### -DeferTimes
Specify the number of times the installation can be deferred.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeferDays
Specify the number of days since first run that the installation can be deferred.
This is converted to a deadline.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -DeferDeadline
Specify the deadline date until which the installation can be deferred.

Specify the date in the local culture if the script is intended for that same culture.

If the script is intended to run on EN-US machines, specify the date in the format: "08/25/2013" or "08-25-2013" or "08-25-2013 18:00:00"

If the script is intended for multiple cultures, specify the date in the universal sortable date/time format: "2013-08-22 11:51:52Z"

The deadline date will be displayed to the user in the format of their culture.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CheckDiskSpace
Specify whether to check if there is enough disk space for the installation to proceed.

If this parameter is specified without the RequiredDiskSpace parameter, the required disk space is calculated automatically based on the size of the script source and associated files.

```yaml
Type: SwitchParameter
Parameter Sets: CheckDiskSpace
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -RequiredDiskSpace
Specify required disk space in MB, used in combination with CheckDiskSpace.

```yaml
Type: UInt32
Parameter Sets: CheckDiskSpace
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoMinimizeWindows
Specify whether to minimize other windows when displaying prompt.

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

### -NotTopMost
Specifies whether the window is the topmost window.

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

### -ForceCountdown
Specify a countdown to display before automatically proceeding with the installation when a deferral is enabled.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
Accept pipeline input: False
Accept wildcard characters: False
```

### -CustomText
Specify whether to display a custom message specified in the XML file.
Custom message must be populated for each language section in the XML.

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

### None. You cannot pipe objects to this function.
## OUTPUTS

### None. This function does not return objects.
## NOTES
The process descriptions are retrieved from WMI, with a fall back on the process name if no description is available.
Alternatively, you can specify the description yourself with a '=' symbol - see examples.

The dialog box will timeout after the timeout specified in the XML configuration file (default 1 hour and 55 minutes) to prevent SCCM installations from timing out and returning a failure code to SCCM.
When the dialog times out, the script will exit and return a 1618 code (SCCM fast retry code).

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)

