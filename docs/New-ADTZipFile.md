---
external help file: PSAppDeployToolkit-help.xml
Module Name: PSAppDeployToolkit
online version: https://psappdeploytoolkit.com
schema: 2.0.0
---

# New-ADTZipFile

## SYNOPSIS
Create a new zip archive or add content to an existing archive.

## SYNTAX

### Path
```
New-ADTZipFile -Path <String[]> -DestinationPath <String> [-CompressionLevel <String>] [-Update] [-Force]
 [-RemoveSourceAfterArchiving] [<CommonParameters>]
```

### LiteralPath
```
New-ADTZipFile -LiteralPath <String[]> -DestinationPath <String> [-CompressionLevel <String>] [-Update]
 [-Force] [-RemoveSourceAfterArchiving] [<CommonParameters>]
```

## DESCRIPTION
Create a new zip archive or add content to an existing archive by using PowerShell's Compress-Archive.

## EXAMPLES

### EXAMPLE 1
```
New-ADTZipFile -SourceDirectory 'E:\Testing\Logs' -DestinationPath 'E:\Testing\TestingLogs.zip'
```

## PARAMETERS

### -Path
One or more paths to compress.
Supports wildcards.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
One or more literal paths to compress.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DestinationPath
The file path for where the zip file should be created.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionLevel
The level of compression to apply to the zip file.

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

### -Update
Specifies whether to update an existing zip file or not.

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

### -Force
Specifies whether an existing zip file should be overwritten.

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

### -RemoveSourceAfterArchiving
Remove the source path after successfully archiving the content.

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
This is an internal script function and should typically not be called directly.

## RELATED LINKS

[https://psappdeploytoolkit.com](https://psappdeploytoolkit.com)
