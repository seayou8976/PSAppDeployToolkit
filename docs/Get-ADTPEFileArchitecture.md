---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# Get-ADTPEFileArchitecture

## SYNOPSIS
Determine if a PE file is a 32-bit or a 64-bit file.

## SYNTAX

```
Get-ADTPEFileArchitecture [-FilePath] <FileInfo[]> [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
Determine if a PE file is a 32-bit or a 64-bit file by examining the file's image file header.

PE file extensions: .exe, .dll, .ocx, .drv, .sys, .scr, .efi, .cpl, .fon

## EXAMPLES

### EXAMPLE 1
```
Get-ADTPEFileArchitecture -FilePath "$env:windir\notepad.exe"
```

## PARAMETERS

### -FilePath
Path to the PE file to examine.

```yaml
Type: FileInfo[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -PassThru
Get the file object, attach a property indicating the file binary type, and write to pipeline.

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

### System.IO.FileInfo
### Accepts a FileInfo object from the pipeline.
## OUTPUTS

### System.String
### Returns a string indicating the file binary type.
## NOTES
An active ADT session is NOT required to use this function.

Tags: psadt
Website: https://psappdeploytoolkit.com
Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
License: https://opensource.org/license/lgpl-3-0

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
