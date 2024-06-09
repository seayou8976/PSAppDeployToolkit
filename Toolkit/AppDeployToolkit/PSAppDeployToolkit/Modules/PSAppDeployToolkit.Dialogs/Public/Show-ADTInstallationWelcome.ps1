﻿function Show-ADTInstallationWelcome
{
    <#

    .SYNOPSIS
    Show a welcome dialog prompting the user with information about the installation and actions to be performed before the installation can begin.

    .DESCRIPTION
    The following prompts can be included in the welcome dialog:
        a) Close the specified running applications, or optionally close the applications without showing a prompt (using the -Silent switch).
        b) Defer the installation a certain number of times, for a certain number of days or until a deadline is reached.
        c) Countdown until applications are automatically closed.
        d) Prevent users from launching the specified applications while the installation is in progress.

    .PARAMETER ProcessObjects
    Name of the process to stop (do not include the .exe). Specify multiple processes separated by a comma. Specify custom descriptions like this: @{ProcessName = 'winword'; ProcessDescription = 'Microsoft Office Word'},@{ProcessName = 'excel'; ProcessDescription = 'Microsoft Office Excel'}

    .PARAMETER Silent
    Stop processes without prompting the user.

    .PARAMETER CloseAppsCountdown
    Option to provide a countdown in seconds until the specified applications are automatically closed. This only takes effect if deferral is not allowed or has expired.

    .PARAMETER ForceCloseAppsCountdown
    Option to provide a countdown in seconds until the specified applications are automatically closed regardless of whether deferral is allowed.

    .PARAMETER PromptToSave
    Specify whether to prompt to save working documents when the user chooses to close applications by selecting the "Close Programs" button. Option does not work in SYSTEM context unless toolkit launched with "psexec.exe -s -i" to run it as an interactive process under the SYSTEM account.

    .PARAMETER PersistPrompt
    Specify whether to make the Show-ADTInstallationWelcome prompt persist in the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml. The user will have no option but to respond to the prompt. This only takes effect if deferral is not allowed or has expired.

    .PARAMETER BlockExecution
    Option to prevent the user from launching processes/applications, specified in -CloseApps, during the installation.

    .PARAMETER AllowDefer
    Enables an optional defer button to allow the user to defer the installation.

    .PARAMETER AllowDeferCloseApps
    Enables an optional defer button to allow the user to defer the installation only if there are running applications that need to be closed. This parameter automatically enables -AllowDefer

    .PARAMETER DeferTimes
    Specify the number of times the installation can be deferred.

    .PARAMETER DeferDays
    Specify the number of days since first run that the installation can be deferred. This is converted to a deadline.

    .PARAMETER DeferDeadline
    Specify the deadline date until which the installation can be deferred.

    Specify the date in the local culture if the script is intended for that same culture.

    If the script is intended to run on EN-US machines, specify the date in the format: "08/25/2013" or "08-25-2013" or "08-25-2013 18:00:00"

    If the script is intended for multiple cultures, specify the date in the universal sortable date/time format: "2013-08-22 11:51:52Z"

    The deadline date will be displayed to the user in the format of their culture.

    .PARAMETER CheckDiskSpace
    Specify whether to check if there is enough disk space for the installation to proceed.

    If this parameter is specified without the RequiredDiskSpace parameter, the required disk space is calculated automatically based on the size of the script source and associated files.

    .PARAMETER RequiredDiskSpace
    Specify required disk space in MB, used in combination with CheckDiskSpace.

    .PARAMETER MinimizeWindows
    Specifies whether to minimize other windows when displaying prompt. Default: $true.

    .PARAMETER TopMost
    Specifies whether the windows is the topmost window. Default: $true.

    .PARAMETER ForceCountdown
    Specify a countdown to display before automatically proceeding with the installation when a deferral is enabled.

    .PARAMETER CustomText
    Specify whether to display a custom message specified in the XML file. Custom message must be populated for each language section in the XML.

    .INPUTS
    None. You cannot pipe objects to this function.

    .OUTPUTS
    None. This function does not return objects.

    .EXAMPLE
    # Prompt the user to close Internet Explorer, Word and Excel.
    Show-ADTInstallationWelcome -CloseApps 'iexplore,winword,excel'

    .EXAMPLE
    # Close Word and Excel without prompting the user.
    Show-ADTInstallationWelcome -CloseApps 'winword,excel' -Silent

    .EXAMPLE
    # Close Word and Excel and prevent the user from launching the applications while the installation is in progress.
    Show-ADTInstallationWelcome -CloseApps 'winword,excel' -BlockExecution

    .EXAMPLE
    # Prompt the user to close Word and Excel, with customized descriptions for the applications and automatically close the applications after 10 minutes.
    Show-ADTInstallationWelcome -CloseApps 'winword=Microsoft Office Word,excel=Microsoft Office Excel' -CloseAppsCountdown 600

    .EXAMPLE
    # Prompt the user to close Word, MSAccess and Excel.
    # By using the PersistPrompt switch, the dialog will return to the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml, so the user cannot ignore it by dragging it aside.
    Show-ADTInstallationWelcome -CloseApps 'winword,msaccess,excel' -PersistPrompt

    .EXAMPLE
    # Allow the user to defer the installation until the deadline is reached.
    Show-ADTInstallationWelcome -AllowDefer -DeferDeadline '25/08/2013'

    .EXAMPLE
    # Close Word and Excel and prevent the user from launching the applications while the installation is in progress.
    # Allow the user to defer the installation a maximum of 10 times or until the deadline is reached, whichever happens first.
    # When deferral expires, prompt the user to close the applications and automatically close them after 10 minutes.
    Show-ADTInstallationWelcome -CloseApps 'winword,excel' -BlockExecution -AllowDefer -DeferTimes 10 -DeferDeadline '25/08/2013' -CloseAppsCountdown 600

    .NOTES
    The process descriptions are retrieved from WMI, with a fall back on the process name if no description is available. Alternatively, you can specify the description yourself with a '=' symbol - see examples.

    The dialog box will timeout after the timeout specified in the XML configuration file (default 1 hour and 55 minutes) to prevent SCCM installations from timing out and returning a failure code to SCCM. When the dialog times out, the script will exit and return a 1618 code (SCCM fast retry code).

    .LINK
    https://psappdeploytoolkit.com

    #>

    [CmdletBinding(DefaultParameterSetName = 'None')]
    param (
        [Parameter(Mandatory = $false, HelpMessage = 'Specify process names and an optional process description, e.g. @{ProcessName = "winword"; ProcessDescription = "Microsoft Word"}')]
        [ValidateNotNullOrEmpty()]
        [System.Management.Automation.PSObject[]]$ProcessObjects,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify whether to prompt user or force close the applications.')]
        [System.Management.Automation.SwitchParameter]$Silent,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify a countdown to display before automatically closing applications where deferral is not allowed or has expired.')]
        [ValidateNotNullOrEmpty()]
        [System.UInt32]$CloseAppsCountdown,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify a countdown to display before automatically closing applications whether or not deferral is allowed.')]
        [ValidateNotNullOrEmpty()]
        [System.UInt32]$ForceCloseAppsCountdown,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify whether to prompt to save working documents when the user chooses to close applications by selecting the "Close Programs" button.')]
        [System.Management.Automation.SwitchParameter]$PromptToSave,

        [Parameter(Mandatory = $false, HelpMessage = ' Specify whether to make the prompt persist in the center of the screen every couple of seconds, specified in the AppDeployToolkitConfig.xml.')]
        [System.Management.Automation.SwitchParameter]$PersistPrompt,

        [Parameter(Mandatory = $false, HelpMessage = ' Specify whether to block execution of the processes during installation.')]
        [System.Management.Automation.SwitchParameter]$BlockExecution,

        [Parameter(Mandatory = $false, HelpMessage = ' Specify whether to enable the optional defer button on the dialog box.')]
        [System.Management.Automation.SwitchParameter]$AllowDefer,

        [Parameter(Mandatory = $false, HelpMessage = ' Specify whether to enable the optional defer button on the dialog box only if an app needs to be closed.')]
        [System.Management.Automation.SwitchParameter]$AllowDeferCloseApps,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify the number of times the deferral is allowed.')]
        [ValidateNotNullorEmpty()]
        [System.Int32]$DeferTimes,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify the number of days since first run that the deferral is allowed.')]
        [ValidateNotNullorEmpty()]
        [System.UInt32]$DeferDays,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify the deadline (in format dd/mm/yyyy) for which deferral will expire as an option.')]
        [ValidateNotNullOrEmpty()]
        [System.String]$DeferDeadline,

        [Parameter(Mandatory = $true, HelpMessage = 'Specify whether to check if there is enough disk space for the installation to proceed. If this parameter is specified without the RequiredDiskSpace parameter, the required disk space is calculated automatically based on the size of the script source and associated files.', ParameterSetName = 'CheckDiskSpace')]
        [System.Management.Automation.SwitchParameter]$CheckDiskSpace,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify required disk space in MB, used in combination with $CheckDiskSpace.', ParameterSetName = 'CheckDiskSpace')]
        [ValidateNotNullOrEmpty()]
        [System.UInt32]$RequiredDiskSpace,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify whether to minimize other windows when displaying prompt.')]
        [System.Management.Automation.SwitchParameter]$NoMinimizeWindows,

        [Parameter(Mandatory = $false, HelpMessage = 'Specifies whether the window is the topmost window.')]
        [System.Management.Automation.SwitchParameter]$NotTopMost,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify a countdown to display before automatically proceeding with the installation when a deferral is enabled.')]
        [ValidateNotNullOrEmpty()]
        [System.UInt32]$ForceCountdown,

        [Parameter(Mandatory = $false, HelpMessage = 'Specify whether to display a custom message specified in the XML file. Custom message must be populated for each language section in the XML.')]
        [System.Management.Automation.SwitchParameter]$CustomText
    )

    begin {
        $adtEnv = Get-ADTEnvironment
        $adtConfig = Get-ADTConfig
        $adtSession = Get-ADTSession
        Write-ADTDebugHeader
    }

    process {
        # If running in NonInteractive mode, force the processes to close silently.
        if ($adtSession.DeployModeNonInteractive)
        {
            $Silent = $true
        }

        # If using Zero-Config MSI Deployment, append any executables found in the MSI to the CloseApps list
        if ($adtSession.GetPropertyValue('UseDefaultMsi'))
        {
            $ProcessObjects = $($ProcessObjects; $adtSession.DefaultMsiExecutablesList)
        }

        # Check disk space requirements if specified
        if ($CheckDiskSpace)
        {
            Write-ADTLogEntry -Message 'Evaluating disk space requirements.'
            if (!$RequiredDiskSpace)
            {
                try
                {
                    # Determine the size of the Files folder
                    $fso = New-Object -ComObject Scripting.FileSystemObject
                    $RequiredDiskSpace = [System.Math]::Round($fso.GetFolder($adtSession.GetPropertyValue('ScriptParentPath')).Size / 1MB)
                }
                catch
                {
                    Write-ADTLogEntry -Message "Failed to calculate disk space requirement from source files.`n$(Resolve-Error)" -Severity 3
                }
                finally
                {
                    try
                    {
                        [System.Void][System.Runtime.Interopservices.Marshal]::ReleaseComObject($fso)
                    }
                    catch
                    {
                        [System.Void]$null
                    }
                }
            }
            if (($freeDiskSpace = Get-ADTFreeDiskSpace) -lt $RequiredDiskSpace)
            {
                Write-ADTLogEntry -Message "Failed to meet minimum disk space requirement. Space Required [$RequiredDiskSpace MB], Space Available [$freeDiskSpace MB]." -Severity 3
                if (!$Silent)
                {
                    Show-ADTInstallationPrompt -Message ((Get-ADTStrings).DiskSpace.Message -f $adtSession.GetPropertyValue('installTitle'), $RequiredDiskSpace, $freeDiskSpace) -ButtonRightText OK -Icon Error
                }
                Close-ADTSession -ExitCode $adtConfig.UI.DefaultExitCode
            }
            Write-ADTLogEntry -Message 'Successfully passed minimum disk space requirement check.'
        }

        # Check Deferral history and calculate remaining deferrals.
        $deferDeadlineUniversal = $null
        if ($AllowDefer -or $AllowDeferCloseApps)
        {
            # Set $AllowDefer to true if $AllowDeferCloseApps is true.
            $AllowDefer = $true

            # Get the deferral history from the registry.
            $deferHistory = Get-RegistryKey -Key $adtSession.GetPropertyValue('RegKeyDeferHistory') -ContinueOnError $true
            $deferHistoryTimes = $deferHistory | Select-Object -ExpandProperty DeferTimesRemaining -ErrorAction Ignore
            $deferHistoryDeadline = $deferHistory | Select-Object -ExpandProperty DeferDeadline -ErrorAction Ignore

            # Reset switches.
            $checkDeferDays = $DeferDays -ne 0
            $checkDeferDeadline = !!$DeferDeadline

            if ($DeferTimes -ne 0)
            {
                $DeferTimes = if ($deferHistoryTimes -ge 0)
                {
                    Write-ADTLogEntry -Message "Defer history shows [$($deferHistory.DeferTimesRemaining)] deferrals remaining."
                    $deferHistory.DeferTimesRemaining - 1
                }
                else
                {
                    $DeferTimes - 1
                }
                Write-ADTLogEntry -Message "The user has [$DeferTimes] deferrals remaining."

                if ($DeferTimes -lt 0)
                {
                    Write-ADTLogEntry -Message 'Deferral has expired.'
                    $DeferTimes = 0
                    $AllowDefer = $false
                }
            }

            if ($checkDeferDays -and $AllowDefer)
            {
                [String]$deferDeadlineUniversal = if ($deferHistoryDeadline)
                {
                    Write-ADTLogEntry -Message "Defer history shows a deadline date of [$deferHistoryDeadline]."
                    Get-ADTUniversalDate -DateTime $deferHistoryDeadline
                }
                else
                {
                    Get-ADTUniversalDate -DateTime (Get-Date -Date ([System.DateTime]::Now.AddDays($DeferDays)) -Format $adtEnv.culture.DateTimeFormat.UniversalDateTimePattern).ToString()
                }
                Write-ADTLogEntry -Message "The user has until [$deferDeadlineUniversal] before deferral expires."

                if ((Get-ADTUniversalDate) -gt $deferDeadlineUniversal)
                {
                    Write-ADTLogEntry -Message 'Deferral has expired.'
                    $AllowDefer = $false
                }
            }

            if ($checkDeferDeadline -and $AllowDefer)
            {
                # Validate date.
                try
                {
                    [String]$deferDeadlineUniversal = Get-ADTUniversalDate -DateTime $DeferDeadline
                }
                catch
                {
                    Write-ADTLogEntry -Message "Date is not in the correct format for the current culture. Type the date in the current locale format, such as 20/08/2014 (Europe) or 08/20/2014 (United States). If the script is intended for multiple cultures, specify the date in the universal sortable date/time format, e.g. '2013-08-22 11:51:52Z'.`n$(Resolve-Error)" -Severity 3
                    throw
                }
                Write-ADTLogEntry -Message "The user has until [$deferDeadlineUniversal] remaining."

                if ((Get-ADTUniversalDate) -gt $deferDeadlineUniversal)
                {
                    Write-ADTLogEntry -Message 'Deferral has expired.'
                    $AllowDefer = $false
                }
            }
        }

        if (($deferTimes -lt 0) -and !$deferDeadlineUniversal)
        {
            $AllowDefer = $false
        }

        # Prompt the user to close running applications and optionally defer if enabled.
        if (!$adtSession.DeployModeSilent -and !$Silent)
        {
            # Keep the same variable for countdown to simplify the code.
            if ($ForceCloseAppsCountdown -gt 0)
            {
                $CloseAppsCountdown = $ForceCloseAppsCountdown
            }
            elseif ($ForceCountdown -gt 0)
            {
                $CloseAppsCountdown = $ForceCountdown
            }
            $adtSession.State.CloseAppsCountdownGlobal = $CloseAppsCountdown
            $promptResult = $null

            while (($runningProcesses = $ProcessObjects | Get-ADTRunningProcesses) -or (($promptResult -ne 'Defer') -and ($promptResult -ne 'Close')))
            {
                # Get all unique running process descriptions.
                $adtSession.State.RunningProcessDescriptions = $runningProcesses | Select-Object -ExpandProperty ProcessDescription | Sort-Object -Unique

                # Define parameters for welcome prompt.
                $promptParams = @{
                    ForceCloseAppsCountdown = !!$ForceCloseAppsCountdown
                    ForceCountdown = $ForceCountdown
                    PersistPrompt = $PersistPrompt
                    NoMinimizeWindows =$NoMinimizeWindows
                    CustomText = $CustomText
                    NotTopMost = $NotTopMost
                }
                if ($ProcessObjects) {$promptParams.Add('ProcessObjects', $ProcessObjects)}

                # Check if we need to prompt the user to defer, to defer and close apps, or not to prompt them at all
                if ($AllowDefer)
                {
                    # If there is deferral and closing apps is allowed but there are no apps to be closed, break the while loop.
                    if ($AllowDeferCloseApps -and !$adtSession.State.RunningProcessDescriptions)
                    {
                        break
                    }
                    elseif (($promptResult -ne 'Close') -or ($adtSession.State.RunningProcessDescriptions -and ($promptResult -ne 'Continue')))
                    {
                        # Otherwise, as long as the user has not selected to close the apps or the processes are still running and the user has not selected to continue, prompt user to close running processes with deferral.
                        $deferParams = @{AllowDefer = $true; DeferTimes = $deferTimes}
                        if ($deferDeadlineUniversal) {$deferParams.Add('DeferDeadline', $deferDeadlineUniversal)}
                        [String]$promptResult = Show-ADTWelcomePrompt @promptParams @deferParams
                    }
                }
                elseif ($adtSession.State.RunningProcessDescriptions -or !!$forceCountdown)
                {
                    # If there is no deferral and processes are running, prompt the user to close running processes with no deferral option.
                    [String]$promptResult = Show-ADTWelcomePrompt @promptParams
                }
                else
                {
                    # If there is no deferral and no processes running, break the while loop.
                    break
                }

                # Process the form results.
                if ($promptResult -eq 'Continue')
                {
                    # If the user has clicked OK, wait a few seconds for the process to terminate before evaluating the running processes again.
                    Write-ADTLogEntry -Message 'The user selected to continue...'
                    if (!$runningProcesses)
                    {
                        # Break the while loop if there are no processes to close and the user has clicked OK to continue.
                        break
                    }
                    [System.Threading.Thread]::Sleep(2000)
                }
                elseif ($promptResult -eq 'Close')
                {
                    # Force the applications to close.
                    Write-ADTLogEntry -Message 'The user selected to force the application(s) to close...'
                    if ($PromptToSave -and $adtEnv.SessionZero -and !$adtEnv.IsProcessUserInteractive)
                    {
                        Write-ADTLogEntry -Message 'Specified [-PromptToSave] option will not be available, because current process is running in session zero and is not interactive.' -Severity 2
                    }

                    # Update the process list right before closing, in case it changed.
                    $AllOpenWindows = Get-ADTWindowTitle -GetAllWindowTitles -DisableFunctionLogging
                    $PromptToSaveTimeout = New-TimeSpan -Seconds $adtConfig.UI.PromptToSaveTimeout
                    $PromptToSaveStopWatch = [System.Diagnostics.StopWatch]::new()
                    foreach ($runningProcess in ($runningProcesses = $ProcessObjects | Get-ADTRunningProcesses))
                    {
                        # If the PromptToSave parameter was specified and the process has a window open, then prompt the user to save work if there is work to be saved when closing window.
                        $AllOpenWindowsForRunningProcess = $AllOpenWindows | Where-Object {$_.ParentProcess -eq $runningProcess.ProcessName}
                        if ($PromptToSave -and !($adtEnv.SessionZero -and !$adtEnv.IsProcessUserInteractive) -and $AllOpenWindowsForRunningProcess -and ($runningProcess.MainWindowHandle -ne [IntPtr]::Zero))
                        {
                            foreach ($OpenWindow in $AllOpenWindowsForRunningProcess)
                            {
                                try
                                {
                                    Write-ADTLogEntry -Message "Stopping process [$($runningProcess.ProcessName)] with window title [$($OpenWindow.WindowTitle)] and prompt to save if there is work to be saved (timeout in [$($adtConfig.UI.PromptToSaveTimeout)] seconds)..."
                                    [System.Void][PSADT.UiAutomation]::BringWindowToFront($OpenWindow.WindowHandle)
                                    if (!$runningProcess.CloseMainWindow())
                                    {
                                        Write-ADTLogEntry -Message "Failed to call the CloseMainWindow() method on process [$($runningProcess.ProcessName)] with window title [$($OpenWindow.WindowTitle)] because the main window may be disabled due to a modal dialog being shown." -Severity 3
                                    }
                                    else
                                    {
                                        $PromptToSaveStopWatch.Reset()
                                        $PromptToSaveStopWatch.Start()
                                        do
                                        {
                                            if (!($IsWindowOpen = $AllOpenWindows | Where-Object {$_.WindowHandle -eq $OpenWindow.WindowHandle}))
                                            {
                                                Break
                                            }
                                            [System.Threading.Thread]::Sleep(3000)
                                        }
                                        while (($IsWindowOpen) -and ($PromptToSaveStopWatch.Elapsed -lt $PromptToSaveTimeout))

                                        if ($IsWindowOpen)
                                        {
                                            Write-ADTLogEntry -Message "Exceeded the [$($adtConfig.UI.PromptToSaveTimeout)] seconds timeout value for the user to save work associated with process [$($runningProcess.ProcessName)] with window title [$($OpenWindow.WindowTitle)]." -Severity 2
                                        }
                                        else
                                        {
                                            Write-ADTLogEntry -Message "Window [$($OpenWindow.WindowTitle)] for process [$($runningProcess.ProcessName)] was successfully closed."
                                        }
                                    }
                                }
                                catch
                                {
                                    Write-ADTLogEntry -Message "Failed to close window [$($OpenWindow.WindowTitle)] for process [$($runningProcess.ProcessName)]. `r`n$(Resolve-Error)" -Severity 3
                                }
                                finally
                                {
                                    $runningProcess.Refresh()
                                }
                            }
                        }
                        else
                        {
                            Write-ADTLogEntry -Message "Stopping process $($runningProcess.ProcessName)..."
                            Stop-Process -Name $runningProcess.ProcessName -Force -ErrorAction Ignore
                        }
                    }

                    if ($runningProcesses = $ProcessObjects | Get-ADTRunningProcesses -DisableLogging)
                    {
                        # Apps are still running, give them 2s to close. If they are still running, the Welcome Window will be displayed again.
                        Write-ADTLogEntry -Message 'Sleeping for 2 seconds because the processes are still not closed...'
                        [System.Threading.Thread]::Sleep(2000)
                    }
                }
                elseif ($promptResult -eq 'Timeout')
                {
                    # Stop the script (if not actioned before the timeout value).
                    Write-ADTLogEntry -Message 'Installation not actioned before the timeout value.'
                    $BlockExecution = $false
                    if (($deferTimes -ge 0) -or $deferDeadlineUniversal)
                    {
                        Set-ADTDeferHistory -DeferTimesRemaining $DeferTimes -DeferDeadline $deferDeadlineUniversal
                    }

                    # Dispose the welcome prompt timer here because if we dispose it within the Show-ADTWelcomePrompt function we risk resetting the timer and missing the specified timeout period.
                    if ($adtSession.State.WelcomeTimer)
                    {
                        try
                        {
                            $adtSession.State.WelcomeTimer.Dispose()
                            $adtSession.State.WelcomeTimer = $null
                        }
                        catch
                        {
                            [System.Void]$null
                        }
                    }

                    # Restore minimized windows.
                    [System.Void]$adtEnv.ShellApp.UndoMinimizeAll()
                    Close-ADTSession -ExitCode $adtConfig.UI.DefaultExitCode
                }
                elseif ($promptResult -eq 'Defer')
                {
                    #  Stop the script (user chose to defer)
                    Write-ADTLogEntry -Message 'Installation deferred by the user.'
                    $BlockExecution = $false
                    Set-ADTDeferHistory -DeferTimesRemaining $DeferTimes -DeferDeadline $deferDeadlineUniversal

                    # Restore minimized windows.
                    [System.Void]$adtEnv.ShellApp.UndoMinimizeAll()
                    Close-ADTSession -ExitCode $adtConfig.UI.DeferExitCode
                }
            }
        }

        # Force the processes to close silently, without prompting the user.
        if (($Silent -or $adtSession.DeployModeSilent) -and ($runningProcesses = $ProcessObjects | Get-ADTRunningProcesses))
        {
            Write-ADTLogEntry -Message "Force closing application(s) [$(($runningProcesses.ProcessDescription | Sort-Object -Unique) -join ',')] without prompting user."
            $runningProcesses.ProcessName | Stop-Process -Force -ErrorAction Ignore
            [System.Threading.Thread]::Sleep(2000)
        }

        # If block execution switch is true, call the function to block execution of these processes.
        if ($BlockExecution)
        {
            # Make this variable globally available so we can check whether we need to call Unblock-AppExecution
            $adtSession.State.BlockExecution = $BlockExecution
            Write-ADTLogEntry -Message '[-BlockExecution] parameter specified.'
            Block-AppExecution -ProcessName ($ProcessObjects | Select-Object -ExpandProperty ProcessName)
        }
    }

    end {
        Write-ADTDebugFooter
    }
}