﻿#-----------------------------------------------------------------------------
#
# MARK: Disable-ADTWindowCloseButton
#
#-----------------------------------------------------------------------------

function Disable-ADTWindowCloseButton
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({
                if (($null -eq $_) -or $_.Equals([System.IntPtr]::Zero))
                {
                    $PSCmdlet.ThrowTerminatingError((& $Script:CommandTable.'New-ADTValidateScriptErrorRecord' -ParameterName WindowHandle -ProvidedValue $_ -ExceptionMessage 'The provided window handle is invalid.'))
                }
                return !!$_
            })]
        [System.IntPtr]$WindowHandle
    )

    $null = if (($menuHandle = [PSADT.GUI.UiAutomation]::GetSystemMenu($WindowHandle, $false)) -and ($menuHandle -ne [System.IntPtr]::Zero))
    {
        [PSADT.GUI.UiAutomation]::EnableMenuItem($menuHandle, 0xF060, 0x00000001)
        [PSADT.GUI.UiAutomation]::DestroyMenu($menuHandle)
    }
}
