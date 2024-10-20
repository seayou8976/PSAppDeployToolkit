---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: http://msdn.microsoft.com/en-us/library/aa372909(VS.85).asp
schema: 2.0.0
---

# Test-ADTMutexAvailability

## SYNOPSIS
Wait, up to a timeout value, to check if current thread is able to acquire an exclusive lock on a system mutex.

## SYNTAX

```
Test-ADTMutexAvailability [-MutexName] <String> [[-MutexWaitTime] <TimeSpan>] [<CommonParameters>]
```

## DESCRIPTION
A mutex can be used to serialize applications and prevent multiple instances from being opened at the same time.

Wait, up to a timeout (default is 1 millisecond), for the mutex to become available for an exclusive lock.

## EXAMPLES

### EXAMPLE 1
```
Test-ADTMutexAvailability -MutexName 'Global\_MSIExecute' -MutexWaitTime 5000000
```

### EXAMPLE 2
```
Test-ADTMutexAvailability -MutexName 'Global\_MSIExecute' -MutexWaitTime (New-TimeSpan -Minutes 5)
```

### EXAMPLE 3
```
Test-ADTMutexAvailability -MutexName 'Global\_MSIExecute' -MutexWaitTime (New-TimeSpan -Seconds 60)
```

## PARAMETERS

### -MutexName
The name of the system mutex.

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

### -MutexWaitTime
The number of milliseconds the current thread should wait to acquire an exclusive lock of a named mutex.
Default is: 1 millisecond.

A wait time of -1 milliseconds means to wait indefinitely.
A wait time of zero does not acquire an exclusive lock but instead tests the state of the wait handle and returns immediately.

```yaml
Type: TimeSpan
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: [System.TimeSpan]::FromMilliseconds(1)
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None. You cannot pipe objects to this function.
## OUTPUTS

### System.Boolean. Returns $true if the current thread acquires an exclusive lock on the named mutex, $false otherwise.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[http://msdn.microsoft.com/en-us/library/aa372909(VS.85).asp](http://msdn.microsoft.com/en-us/library/aa372909(VS.85).asp)

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
