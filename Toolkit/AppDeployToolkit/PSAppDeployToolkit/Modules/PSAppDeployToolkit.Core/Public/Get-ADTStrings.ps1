﻿function Get-ADTStrings
{
    [CmdletBinding()]
    param (
    )

    # Return the string database if initialised.
    if (!($adtData = Get-ADT).Strings -or !$adtData.Strings.Count)
    {
        $naerParams = @{
            Exception = [System.InvalidOperationException]::new("Please ensure that [Open-ADTSession] is called before using any PSAppDeployToolkit functions.")
            Category = [System.Management.Automation.ErrorCategory]::InvalidOperation
            ErrorId = 'ADTStringTableNotInitialised'
            TargetObject = $adtData.Strings
            RecommendedAction = "Please ensure the module is initialised via [Open-ADTSession] and try again."
        }
        $PSCmdlet.ThrowTerminatingError((New-ADTErrorRecord @naerParams))
    }
    return $adtData.Strings
}
