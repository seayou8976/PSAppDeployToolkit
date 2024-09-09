﻿#-----------------------------------------------------------------------------
#
# MARK: Convert-ADTValuesFromRemainingArguments
#
#-----------------------------------------------------------------------------

function Convert-ADTValuesFromRemainingArguments
{
    <#

    .SYNOPSIS
    Converts the collected values from a ValueFromRemainingArguments parameter value into a dictionary or PowerShell.exe command line arguments.

    .DESCRIPTION
    This function converts the collected values from a ValueFromRemainingArguments parameter value into a dictionary or PowerShell.exe command line arguments.

    .PARAMETER RemainingArguments
    The collected values to enumerate and process into a dictionary.

    .INPUTS
    System.Object. Convert-ADTValuesFromRemainingArguments accepts one or more objects via the pipeline for processing.

    .OUTPUTS
    System.Collections.Generic.Dictionary[System.String, System.Object]. Convert-ADTValuesFromRemainingArguments returns a dictionary of the processed input.

    .NOTES
    This function can be called without an active ADT session.

    .LINK
    https://psappdeploytoolkit.com

    #>

    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseSingularNouns', '', Justification = "This function is appropriately named and we don't need PSScriptAnalyzer telling us otherwise.")]
    [CmdletBinding()]
    [OutputType([System.Collections.Generic.Dictionary[System.String, System.Object]])]
    param
    (
        [Parameter(Mandatory = $true)]
        [AllowNull()][AllowEmptyCollection()]
        [System.Collections.Generic.List[System.Object]]$RemainingArguments
    )

    # Open dictionary to hold all values, using same base type as $PSBoundParameters.
    $boundParams = [System.Collections.Generic.Dictionary[System.String, System.Object]]::new()

    # Process input into a dictionary and return it. Assume anything starting with a '-' is a new variable.
    try
    {
        $RemainingArguments | & {
            process
            {
                if ($null -eq $_)
                {
                    return
                }
                if (($_ -is [System.String]) -and ($_ -match '^-'))
                {
                    $boundParams.Add(($thisvar = $_ -replace '(^-|:$)'), [System.Management.Automation.SwitchParameter]$true)
                }
                else
                {
                    $boundParams.$thisvar = $_
                }
            }
        }
    }
    catch
    {
        $naerParams = @{
            Exception = [System.FormatException]::new("The parser was unable to process the provided arguments.", $_.Exception)
            Category = [System.Management.Automation.ErrorCategory]::InvalidData
            ErrorId = 'ArgumentsMalformedException'
            TargetObject = $RemainingArguments
            RecommendedAction = "Please ensure that only PowerShell-style arguments are provided and try again."
        }
        $PSCmdlet.ThrowTerminatingError((& $Script:CommandTable.'New-ADTErrorRecord' @naerParams))
    }

    # Return dictionary, even if its empty to match $PSBoundParameters API.
    return $boundParams
}
