﻿function Get-ADTServiceStartMode
{
    <#

    .SYNOPSIS
    Get the service startup mode.

    .DESCRIPTION
    Get the service startup mode.

    .PARAMETER Name
    Specify the name of the service.

    .PARAMETER ComputerName
    Specify the name of the computer. Default is: the local computer.

    .INPUTS
    None. You cannot pipe objects to this function.

    .OUTPUTS
    System.ServiceProcess.ServiceController. Returns the service object.

    .EXAMPLE
    Get-ADTServiceStartMode -Name 'wuauserv'

    .LINK
    https://psappdeploytoolkit.com

    #>

    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [System.String]$Name,

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [System.String]$ComputerName
    )

    begin {
        # Make this function continue on error.
        $ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
        if (!$PSBoundParameters.ContainsKey('ErrorAction'))
        {
            $PSBoundParameters.ErrorAction = [System.Management.Automation.ActionPreference]::Continue
        }
        Write-ADTDebugHeader
    }

    process {
        try
        {
            # Get the start mode and adjust it if the automatic type is delayed.
            Write-ADTLogEntry -Message "Getting the service [$Name] startup mode."
            if ((($serviceStartMode = (Get-Service @PSBoundParameters).StartType) -eq 'Automatic') -and (Get-ItemProperty -LiteralPath "Registry::HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\$Name" -ErrorAction Ignore | Select-Object -ExpandProperty DelayedAutoStart -ErrorAction Ignore) -eq 1)
            {
                $serviceStartMode = 'Automatic (Delayed Start)'
            }

            # Return startup type to the caller.
            Write-ADTLogEntry -Message "Service [$Name] startup mode is set to [$serviceStartMode]."
            return $serviceStartMode
        }
        catch
        {
            if ($PSBoundParameters.ErrorAction -notmatch '^(Ignore|SilentlyContinue)$')
            {
                Write-ADTLogEntry -Message "Failed to get the service [$Name] startup mode.`n$(Resolve-ADTError)" -Severity 3
                if ($PSBoundParameters.ErrorAction.Equals([System.Management.Automation.ActionPreference]::Stop))
                {
                    throw
                }
            }
        }
    }

    end {
        Write-ADTDebugFooter
    }
}