---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Start-ADTServiceAndDependencies

## SYNOPSIS
Start a Windows service and its dependencies.

## SYNTAX

```
Start-ADTServiceAndDependencies [-Service] <ServiceController> [-SkipDependentServices]
 [[-PendingStatusWait] <TimeSpan>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
This function starts a specified Windows service and its dependencies.
It provides options to skip starting dependent services, wait for a service to get out of a pending state, and return the service object.

## EXAMPLES

### EXAMPLE 1
```
Start-ADTServiceAndDependencies -Service 'wuauserv'
```

Starts the Windows Update service and its dependencies.

## PARAMETERS

### -Service
Specify the name of the service.

```yaml
Type: ServiceController
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SkipDependentServices
Choose to skip checking for and starting dependent services.
Default is: $false.

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

### -PendingStatusWait
The amount of time to wait for a service to get out of a pending state before continuing.
Default is 60 seconds.

```yaml
Type: TimeSpan
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Return the System.ServiceProcess.ServiceController service object.

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

### System.ServiceProcess.ServiceController
### Returns the service object.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
