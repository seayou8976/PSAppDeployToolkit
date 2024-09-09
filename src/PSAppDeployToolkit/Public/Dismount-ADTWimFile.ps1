﻿#-----------------------------------------------------------------------------
#
# MARK: Dismount-ADTWimFile
#
#-----------------------------------------------------------------------------

function Dismount-ADTWimFile
{
    <#
    .SYNOPSIS
        Dismounts a WIM file from the specified mount point.

    .DESCRIPTION
        The Dismount-ADTWimFile function dismounts a WIM file from the specified mount point and discards all changes. This function ensures that the specified path is a valid WIM mount point before attempting to dismount.

    .PARAMETER ImagePath
        The path to the WIM file.

    .PARAMETER Path
        The path to the WIM mount point.

    .INPUTS
        None

        You cannot pipe objects to this function.

    .OUTPUTS
        None

        This function does not return any objects.

    .EXAMPLE
        Dismount-ADTWimFile -ImagePath 'C:\Path\To\File.wim'

        This example dismounts the WIM file from all its mount points and discards all changes.

    .EXAMPLE
        Dismount-ADTWimFile -Path 'C:\Mount\WIM'

        This example dismounts the WIM file from the specified mount point and discards all changes.

    .NOTES
        An active ADT session is NOT required to use this function.

        Tags: psadt
        Website: https://psappdeploytoolkit.com
        Copyright: (c) 2024 PSAppDeployToolkit Team, licensed under LGPLv3
        License: https://opensource.org/license/lgpl-3-0

    .LINK
        https://psappdeploytoolkit.com
    #>

    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ParameterSetName = 'ImagePath')]
        [ValidateNotNullOrEmpty()]
        [System.IO.FileInfo[]]$ImagePath,

        [Parameter(Mandatory = $true, ParameterSetName = 'Path')]
        [ValidateNotNullOrEmpty()]
        [System.IO.DirectoryInfo[]]$Path
    )

    begin
    {
        & $Script:CommandTable.'Initialize-ADTFunction' -Cmdlet $PSCmdlet -SessionState $ExecutionContext.SessionState
    }

    process
    {
        # Loop through all found mounted images.
        foreach ($wimFile in (& $Script:CommandTable.'Get-ADTMountedWimFile' @PSBoundParameters))
        {
            # Announce commencement.
            & $Script:CommandTable.'Write-ADTLogEntry' -Message "Dismounting WIM file at path [$($wimFile.Path)]."
            try
            {
                try
                {
                    # Perform the dismount and discard all changes.
                    try
                    {
                        $null = & $Script:CommandTable.'Invoke-ADTCommandWithRetries' -Command Dismount-WindowsImage -Path $wimFile.Path -Discard
                    }
                    catch
                    {
                        # Re-throw if this error is anything other than a file-locked error.
                        if (!$_.Exception.ErrorCode.Equals(-1052638953))
                        {
                            throw
                        }

                        # Get all open file handles for our path.
                        & $Script:CommandTable.'Write-ADTLogEntry' -Message "The directory could not be completely unmounted. Checking for any open file handles that can be closed."
                        $exeHandle = "$Script:PSScriptRoot\bin\$([System.Environment]::GetEnvironmentVariable('PROCESSOR_ARCHITECTURE'))\handle\handle.exe"
                        $pathRegex = "^$([System.Text.RegularExpressions.Regex]::Escape($($wimFile.Path)))"
                        $pathHandles = & $Script:CommandTable.'Get-ADTProcessHandles' | & { process { if ($_.Name -match $pathRegex) { return $_ } } }

                        # Throw if we have no handles to close, it means we don't know why the WIM didn't dismount.
                        if (!$pathHandles)
                        {
                            throw
                        }

                        # Close all open file handles.
                        foreach ($handle in $pathHandles)
                        {
                            # Close handle using handle.exe. An exit code of 0 is considered successful.
                            & $Script:CommandTable.'Write-ADTLogEntry' -Message "$(($msg = "Closing handle [$($handle.Handle)] for process [$($handle.Process) ($($handle.PID))]"))."
                            $handleResult = & $exeHandle -nobanner -c $handle.Handle -p $handle.PID -y
                            if ($LASTEXITCODE.Equals(0))
                            {
                                continue
                            }

                            # If we're here, we had a bad exit code.
                            & $Script:CommandTable.'Write-ADTLogEntry' -Message ($msg = "$msg failed with exit code [$LASTEXITCODE]: $handleResult") -Severity 3
                            $naerParams = @{
                                Exception = [System.ApplicationException]::new($msg)
                                Category = [System.Management.Automation.ErrorCategory]::InvalidResult
                                ErrorId = 'HandleClosureFailure'
                                TargetObject = $handleResult
                                RecommendedAction = "Please review the result in this error's TargetObject property and try again."
                            }
                            throw (& $Script:CommandTable.'New-ADTErrorRecord' @naerParams)
                        }

                        # Attempt the dismount again.
                        $null = & $Script:CommandTable.'Invoke-ADTCommandWithRetries' -Command Dismount-WindowsImage -Path $wimFile.Path -Discard
                    }
                    & $Script:CommandTable.'Write-ADTLogEntry' -Message "Successfully dismounted WIM file."
                    & $Script:CommandTable.'Remove-Item' -LiteralPath $wimFile.Path -Force -Confirm:$false
                }
                catch
                {
                    & $Script:CommandTable.'Write-Error' -ErrorRecord $_
                }
            }
            catch
            {
                & $Script:CommandTable.'Invoke-ADTFunctionErrorHandler' -Cmdlet $PSCmdlet -SessionState $ExecutionContext.SessionState -ErrorRecord $_ -LogMessage 'Error occurred while attempting to dismount WIM file.' -ErrorAction SilentlyContinue
            }
        }
    }

    end
    {
        & $Script:CommandTable.'Complete-ADTFunction' -Cmdlet $PSCmdlet
    }
}
