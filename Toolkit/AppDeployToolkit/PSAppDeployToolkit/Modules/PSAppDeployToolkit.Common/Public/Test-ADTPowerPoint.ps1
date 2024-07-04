﻿function Test-ADTPowerPoint
{
    <#

    .SYNOPSIS
    Tests whether PowerPoint is running in either fullscreen slideshow mode or presentation mode.

    .DESCRIPTION
    Tests whether someone is presenting using PowerPoint in either fullscreen slideshow mode or presentation mode.

    .INPUTS
    None. You cannot pipe objects to this function.

    .OUTPUTS
    System.Boolean. Returns $true if PowerPoint is running in either fullscreen slideshow mode or presentation mode, otherwise returns $false.

    .EXAMPLE
    Test-ADTPowerPoint

    .NOTES
    This function can only execute detection logic if the process is in interactive mode.

    There is a possiblity of a false positive if the PowerPoint filename starts with "PowerPoint Slide Show".

    .LINK
    https://psappdeploytoolkit.com

    #>

    [CmdletBinding()]
    param (
    )

    begin {
        $procName = 'POWERPNT'
        $presenting = 'Unknown'
        Initialize-ADTFunction -Cmdlet $PSCmdlet
    }

    process {
        # Return early if we're not running PowerPoint or we can't interactively check.
        Write-ADTLogEntry -Message 'Checking if PowerPoint is in either fullscreen slideshow mode or presentation mode...'
        if (!($PowerPointProcess = Get-Process -Name $procName -ErrorAction Ignore))
        {
            Write-ADTLogEntry -Message 'PowerPoint application is not running.'
            return ($presenting = $false)
        }
        if (![System.Environment]::UserInteractive)
        {
            Write-ADTLogEntry -Message 'Unable to run check to see if PowerPoint is in fullscreen mode or Presentation Mode because current process is not interactive. Configure script to run in interactive mode in your deployment tool. If using SCCM Application Model, then make sure "Allow users to view and interact with the program installation" is selected. If using SCCM Package Model, then make sure "Allow users to interact with this program" is selected.' -Severity 2
            return
        }

        # PowerPoint's running, perform our checks and balances.
        try
        {
            # Check if "POWERPNT" process has a window with a title that begins with "PowerPoint Slide Show" or "Powerpoint-" for non-English language systems.
            # There is a possiblity of a false positive if the PowerPoint filename starts with "PowerPoint Slide Show"
            if ($PowerPointWindow = Get-ADTWindowTitle -GetAllWindowTitles | Where-Object {$_.WindowTitle -match '^PowerPoint Slide Show' -or $_.WindowTitle -match '^PowerPoint-'} | Where-Object {$_.ParentProcess -eq $procName} | Select-Object -First 1)
            {
                Write-ADTLogEntry -Message "Detected that PowerPoint process [$procName] has a window with a title that beings with [PowerPoint Slide Show] or [PowerPoint-]."
                return ($presenting = $true)
            }
            Write-ADTLogEntry -Message "Detected that PowerPoint process [$procName] does not have a window with a title that beings with [PowerPoint Slide Show] or [PowerPoint-]."
            Write-ADTLogEntry -Message "PowerPoint process [$procName] has process ID(s) [$(($PowerPointProcessIDs = $PowerPointProcess.Id) -join ', ')]."

            # If previous detection method did not detect PowerPoint in fullscreen mode, then check if PowerPoint is in Presentation Mode (check only works on Windows Vista or higher).
            # Note: The below method does not detect PowerPoint presentation mode if the presentation is on a monitor that does not have current mouse input control.
            Write-ADTLogEntry -Message "Detected user notification state [$(($UserNotificationState = [PSADT.UiAutomation]::GetUserNotificationState()))]."
            switch ($UserNotificationState)
            {
                PresentationMode {
                    Write-ADTLogEntry -Message 'Detected that system is in [Presentation Mode].'
                    return ($presenting = $true)
                }
                FullScreenOrPresentationModeOrLoginScreen {
                    if ($PowerPointProcessIDs -contains [PSADT.UIAutomation]::GetWindowThreadProcessID([PSADT.UIAutomation]::GetForeGroundWindow()))
                    {
                        Write-ADTLogEntry -Message 'Detected a fullscreen foreground window matches a PowerPoint process ID.'
                        return ($presenting = $true)
                    }
                    Write-ADTLogEntry -Message 'Unable to find a fullscreen foreground window that matches a PowerPoint process ID.'
                    break
                }
            }
            return ($presenting = $false)
        }
        catch
        {
            Write-ADTLogEntry -Message "Failed check to see if PowerPoint is running in fullscreen slideshow mode.`n$(Resolve-ADTError)" -Severity 3
        }
    }

    end {
        Write-ADTLogEntry -Message "PowerPoint is running in fullscreen mode [$presenting]."
        Complete-ADTFunction -Cmdlet $PSCmdlet
    }
}
