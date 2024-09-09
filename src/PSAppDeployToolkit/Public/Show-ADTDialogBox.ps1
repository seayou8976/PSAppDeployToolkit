﻿#-----------------------------------------------------------------------------
#
# MARK: Show-ADTDialogBox
#
#-----------------------------------------------------------------------------

function Show-ADTDialogBox
{
    <#
    .SYNOPSIS
        Display a custom dialog box with optional title, buttons, icon, and timeout.

    .DESCRIPTION
        Display a custom dialog box with optional title, buttons, icon, and timeout. The default button is "OK", the default Icon is "None", and the default Timeout is None.

        Show-ADTInstallationPrompt is recommended over this function as it provides more customization and uses consistent branding with the other UI components.

    .PARAMETER Text
        Text in the message dialog box.

    .PARAMETER Buttons
        The button(s) to display on the dialog box.

    .PARAMETER DefaultButton
        The Default button that is selected. Options: First, Second, Third.

    .PARAMETER Icon
        Icon to display on the dialog box. Options: None, Stop, Question, Exclamation, Information.

    .PARAMETER NotTopMost
        Specifies whether the message box shouldn't be a system modal message box that appears in a topmost window.

    .INPUTS
        None

        You cannot pipe objects to this function.

    .OUTPUTS
        System.String

        Returns the text of the button that was clicked.

    .EXAMPLE
        Show-ADTDialogBox -Title 'Installation Notice' -Text 'Installation will take approximately 30 minutes. Do you wish to proceed?' -Buttons 'OKCancel' -DefaultButton 'Second' -Icon 'Exclamation' -Timeout 600 -Topmost $false

    .NOTES
        This function can be called without an active ADT session.
        Website: https://psappdeploytoolkit.com
        Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
        License: https://opensource.org/license/lgpl-3-0

    .LINK
        https://psappdeploytoolkit.com
    #>

    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, Position = 0, HelpMessage = 'Enter a message for the dialog box.')]
        [ValidateNotNullOrEmpty()]
        [System.String]$Text,

        [Parameter(Mandatory = $false)]
        [ValidateSet('OK', 'OKCancel', 'AbortRetryIgnore', 'YesNoCancel', 'YesNo', 'RetryCancel', 'CancelTryAgainContinue')]
        [System.String]$Buttons = 'OK',

        [Parameter(Mandatory = $false)]
        [ValidateSet('First', 'Second', 'Third')]
        [System.String]$DefaultButton = 'First',

        [Parameter(Mandatory = $false)]
        [ValidateSet('Exclamation', 'Information', 'None', 'Stop', 'Question')]
        [System.String]$Icon = 'None',

        [Parameter(Mandatory = $false)]
        [System.Management.Automation.SwitchParameter]$NotTopMost
    )

    dynamicparam
    {
        # Initialise the module if there's no session and it hasn't been previously initialised.
        $adtSession = & $Script:CommandTable.'Initialize-ADTModuleIfUnitialized' -Cmdlet $PSCmdlet
        $adtConfig = & $Script:CommandTable.'Get-ADTConfig'

        # Define parameter dictionary for returning at the end.
        $paramDictionary = [System.Management.Automation.RuntimeDefinedParameterDictionary]::new()

        # Add in parameters we need as mandatory when there's no active ADTSession.
        $paramDictionary.Add('Title', [System.Management.Automation.RuntimeDefinedParameter]::new(
                'Title', [System.String], $(
                    [System.Management.Automation.ParameterAttribute]@{ Mandatory = !$adtSession; HelpMessage = 'Title of the message dialog box.' }
                    [System.Management.Automation.ValidateNotNullOrEmptyAttribute]::new()
                )
            ))
        $paramDictionary.Add('Timeout', [System.Management.Automation.RuntimeDefinedParameter]::new(
                'Timeout', [System.UInt32], $(
                    [System.Management.Automation.ParameterAttribute]@{ Mandatory = $false; HelpMessage = 'Specifies how long, in seconds, to show the message prompt before aborting.' }
                    [System.Management.Automation.ValidateScriptAttribute]::new({
                            if ($_ -gt $adtConfig.UI.DefaultTimeout)
                            {
                                $PSCmdlet.ThrowTerminatingError((& $Script:CommandTable.'New-ADTValidateScriptErrorRecord' -ParameterName Timeout -ProvidedValue $_ -ExceptionMessage 'The installation UI dialog timeout cannot be longer than the timeout specified in the configuration file.'))
                            }
                            return !!$_
                        })
                )
            ))

        # Return the populated dictionary.
        return $paramDictionary
    }

    begin
    {
        # Initialise function.
        & $Script:CommandTable.'Initialize-ADTFunction' -Cmdlet $PSCmdlet -SessionState $ExecutionContext.SessionState

        # Set up defaults if not specified.
        $Title = if (!$PSBoundParameters.ContainsKey('Title'))
        {
            $adtSession.GetPropertyValue('InstallTitle')
        }
        else
        {
            $PSBoundParameters.Title
        }
        $Timeout = if (!$PSBoundParameters.ContainsKey('Timeout'))
        {
            $adtConfig.UI.DefaultTimeout
        }
        else
        {
            $PSBoundParameters.Timeout
        }
    }

    process
    {
        # Bypass if in silent mode.
        if ($adtSession -and $adtSession.IsSilent())
        {
            & $Script:CommandTable.'Write-ADTLogEntry' -Message "Bypassing $($MyInvocation.MyCommand.Name) [Mode: $($adtSession.GetPropertyValue('deployMode'))]. Text: $Text"
            return
        }

        & $Script:CommandTable.'Write-ADTLogEntry' -Message "Displaying Dialog Box with message: $Text..."
        try
        {
            try
            {
                $result = switch ([System.Activator]::CreateInstance([System.Type]::GetTypeFromProgID('WScript.Shell')).Popup($Text, $Timeout, $Title, ($Script:DialogBox.Buttons.$Buttons + $Script:DialogBox.Icons.$Icon + $Script:DialogBox.DefaultButtons.$DefaultButton + (4096 * !$NotTopMost))))
                {
                    1 { 'OK'; break }
                    2 { 'Cancel'; break }
                    3 { 'Abort'; break }
                    4 { 'Retry'; break }
                    5 { 'Ignore'; break }
                    6 { 'Yes'; break }
                    7 { 'No'; break }
                    10 { 'Try Again'; break }
                    11 { 'Continue'; break }
                    -1 { 'Timeout'; break }
                    default { 'Unknown'; break }
                }

                & $Script:CommandTable.'Write-ADTLogEntry' -Message "Dialog Box Response: $result"
                return $result
            }
            catch
            {
                & $Script:CommandTable.'Write-Error' -ErrorRecord $_
            }
        }
        catch
        {
            & $Script:CommandTable.'Invoke-ADTFunctionErrorHandler' -Cmdlet $PSCmdlet -SessionState $ExecutionContext.SessionState -ErrorRecord $_
        }
    }

    end
    {
        & $Script:CommandTable.'Complete-ADTFunction' -Cmdlet $PSCmdlet
    }
}
