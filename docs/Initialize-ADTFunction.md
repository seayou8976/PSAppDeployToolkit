---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Initialize-ADTFunction

## SYNOPSIS
Initializes the ADT function environment.

## SYNTAX

```
Initialize-ADTFunction [-Cmdlet] <PSCmdlet> [[-SessionState] <SessionState>] [<CommonParameters>]
```

## DESCRIPTION
Initializes the ADT function environment by setting up necessary variables and logging function start details.
It ensures that the function always stops on errors and handles verbose logging.

## EXAMPLES

### EXAMPLE 1
```
Initialize-ADTFunction -Cmdlet $PSCmdlet
```

Initializes the ADT function environment for the given cmdlet.

## PARAMETERS

### -Cmdlet
The cmdlet that is being initialized.

```yaml
Type: PSCmdlet
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SessionState
The session state of the cmdlet.

```yaml
Type: SessionState
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
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
### This function does not return any output.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
