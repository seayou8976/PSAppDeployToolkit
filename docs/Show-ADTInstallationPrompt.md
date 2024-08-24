---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Show-ADTInstallationPrompt

## SYNOPSIS
Displays a custom installation prompt with the toolkit branding and optional buttons.

## SYNTAX

```
Show-ADTInstallationPrompt [-Message] <String> [[-MessageAlignment] <String>] [[-ButtonRightText] <String>]
 [[-ButtonLeftText] <String>] [[-ButtonMiddleText] <String>] [[-Icon] <String>] [-NoWait] [-PersistPrompt]
 [-MinimizeWindows] [-NoExitOnTimeout] [-NotTopMost] [<CommonParameters>]
```

## DESCRIPTION
Displays a custom installation prompt with the toolkit branding and optional buttons.
Any combination of Left, Middle, or Right buttons can be displayed.
The return value of the button clicked by the user is the button text specified.
The prompt can also display a system icon and be configured to persist, minimize other windows, or timeout after a specified period.

## EXAMPLES

### EXAMPLE 1
```
Show-ADTInstallationPrompt -Message 'Do you want to proceed with the installation?' -ButtonRightText 'Yes' -ButtonLeftText 'No'
```

### EXAMPLE 2
```
Show-ADTInstallationPrompt -Title 'Funny Prompt' -Message 'How are you feeling today?' -ButtonRightText 'Good' -ButtonLeftText 'Bad' -ButtonMiddleText 'Indifferent'
```

### EXAMPLE 3
```
Show-ADTInstallationPrompt -Message 'You can customize text to appear at the end of an install, or remove it completely for unattended installations.' -Icon Information -NoWait
```

## PARAMETERS

### -Message
The message text to be displayed on the prompt.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MessageAlignment
Alignment of the message text.
Options: Left, Center, Right.
Default: Center.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: Center
Accept pipeline input: False
Accept wildcard characters: False
```

### -ButtonRightText
Show a button on the right of the prompt with the specified text.

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

### -ButtonLeftText
Show a button on the left of the prompt with the specified text.

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

### -ButtonMiddleText
Show a button in the middle of the prompt with the specified text.

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

### -Icon
Show a system icon in the prompt.
Options: Application, Asterisk, Error, Exclamation, Hand, Information, None, Question, Shield, Warning, WinLogo.
Default: None.

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

### -NoWait
Presents the dialog in a separate, independent thread so that the main process isn't stalled waiting for a response.

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
Specify whether to make the prompt persist in the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml.
The user will have no option but to respond to the prompt - resistance is futile!

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

### -MinimizeWindows
Specifies whether to minimize other windows when displaying prompt.

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

### -NoExitOnTimeout
Specifies whether to not exit the script if the UI times out.

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
Specifies whether the prompt shouldn't be topmost, above all other windows.

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

### None
### This function does not generate any output.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
