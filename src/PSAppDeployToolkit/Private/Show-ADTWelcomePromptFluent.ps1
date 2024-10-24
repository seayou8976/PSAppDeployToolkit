﻿#-----------------------------------------------------------------------------
#
# MARK: Show-ADTWelcomePromptFluent
#
#-----------------------------------------------------------------------------

function Show-ADTWelcomePromptFluent
{
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', 'UnboundArguments', Justification = "This parameter is just to trap any superfluous input at the end of the function's call.")]
    [CmdletBinding()]
    [OutputType([System.String])]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [System.String]$Title,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [System.String]$DeploymentType,

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [PSADT.Types.ProcessObject[]]$ProcessObjects,

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [System.Int32]$DeferTimes,

        [Parameter(Mandatory = $false)]
        [System.Management.Automation.SwitchParameter]$NoMinimizeWindows,

        [Parameter(Mandatory = $false)]
        [System.Management.Automation.SwitchParameter]$NotTopMost,

        [Parameter(Mandatory = $false, ValueFromRemainingArguments = $true, DontShow = $true)]
        [ValidateNotNullOrEmpty()]
        [System.Collections.Generic.List[System.Object]]$UnboundArguments
    )

    # Perform initial setup.
    $adtConfig = Get-ADTConfig
    $adtStrings = Get-ADTStringTable

    # Convert the incoming ProcessObject objects into AppProcessInfo objects.
    $appsToClose = Get-ADTRunningProcesses -ProcessObjects $ProcessObjects -InformationAction SilentlyContinue | & {
        process
        {
            [PSADT.UserInterface.Services.AppProcessInfo]::new(
                $_.Name,
                $_.ProcessDescription,
                $_.Product,
                $_.Company,
                $null,
                $_.StartTime
            )
        }
    }

    # Minimize all other windows.
    if (!$NoMinimizeWindows)
    {
        $null = (Get-ADTEnvironment).ShellApp.MinimizeAll()
    }

    # Send this out to the C# code.
    $result = [PSADT.UserInterface.UnifiedADTApplication]::ShowWelcomeDialog(
        $Title,
        [System.String]::Format($adtStrings.WelcomePrompt.Fluent.Subtitle, $DeploymentType),
        !$NotTopMost,
        $DeferTimes + 1,
        $appsToClose,
        $adtConfig.Assets.Fluent.Logo,
        $adtConfig.Assets.Fluent.Banner.Light,
        $adtConfig.Assets.Fluent.Banner.Dark,
        $adtStrings.WelcomePrompt.Fluent.DialogMessage,
        $adtStrings.WelcomePrompt.Fluent.DialogMessageNoProcesses,
        $adtStrings.WelcomePrompt.Fluent.ButtonDeferRemaining,
        $adtStrings.WelcomePrompt.Fluent.ButtonLeftText,
        $adtStrings.WelcomePrompt.Fluent.ButtonRightText,
        $adtStrings.WelcomePrompt.Fluent.ButtonRightTextNoProcesses,
        $(if ($adtConfig.UI.DynamicProcessEvaluation) { [PSADT.UserInterface.Services.ProcessEvaluationService]::new() })
    )

    # Return a translated value that's compatible with the toolkit.
    switch ($result)
    {
        Continue
        {
            return 'Close'
            break
        }
        Defer
        {
            return 'Defer'
            break
        }
        default
        {
            $naerParams = @{
                Exception = [System.InvalidOperationException]::new("The returned dialog result of [$_] is invalid and cannot be processed.")
                Category = [System.Management.Automation.ErrorCategory]::InvalidResult
                ErrorId = "WelcomeDialogInvalidResult"
                TargetObject = $_
            }
            throw (New-ADTErrorRecord @naerParams)
        }
    }
}
