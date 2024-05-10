﻿#---------------------------------------------------------------------------
#
# 
#
#---------------------------------------------------------------------------

Function Copy-ContentToCache {
    <#
.SYNOPSIS
    Copies the toolkit content to a cache folder on the local machine and sets the $dirFiles and $supportFiles directory to the cache path
.DESCRIPTION
    Copies the toolkit content to a cache folder on the local machine and sets the $dirFiles and $supportFiles directory to the cache path
.PARAMETER Path
    The path to the software cache folder
.EXAMPLE
    Copy-ContentToCache -Path 'C:\Windows\Temp\PSAppDeployToolkit'
.NOTES
    This function is provided as a template to copy the toolkit content to a cache folder on the local machine and set the $dirFiles directory to the cache path.
    This can be used in the absence of an Endpoint Management solution that provides a managed cache for source files, e.g. Intune is lacking this functionality whereas ConfigMgr includes this functionality.
    Since this cache folder is effectively unmanaged, it is important to cleanup the cache in the uninstall section for the current version and potentially also in the pre-installation section for previous versions.
    This can be done using [Remove-File -Path "$Script:ADT.Config.Toolkit.CachePath\$installName" -Recurse -ContinueOnError $true]

.LINK
    https://psappdeploytoolkit.com
#>
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $false, Position = 0, HelpMessage = 'The path to the software cache folder')]
        [ValidateNotNullorEmpty()]
        [String]$Path = "$($Script:ADT.Config.Toolkit.CachePath)\$($Script:ADT.CurrentSession.GetPropertyValue('installName'))"
    )

    Begin {
        ## Get the name of this function and write header
        [String]${CmdletName} = $PSCmdlet.MyInvocation.MyCommand.Name
        Write-FunctionHeaderOrFooter -CmdletName ${CmdletName} -CmdletBoundParameters $PSBoundParameters -Header
    }
    Process {
        Try {
            ## Create the cache folder if it does not exist
            If (-not (Test-Path -LiteralPath $Path -PathType 'Container')) {
                Try {
                    Write-Log -Message "Creating cache folder [$Path]." -Source ${CmdletName}
                    $null = New-Item -Path $Path -ItemType 'Directory' -ErrorAction 'Stop'
                }
                Catch {
                    Write-Log -Message "Failed to create cache folder [$Path]. `r`n$(Resolve-Error)" -Severity 3 -Source ${CmdletName}
                    Throw "Failed to create cache folder [$Path]: $($_.Exception.Message)"
                }
            }
            Else {
                Write-Log -Message "Cache folder [$Path] already exists." -Source ${CmdletName}
            }

            ## Copy the toolkit content to the cache folder
            Write-Log -Message "Copying toolkit content to cache folder [$Path]." -Source ${CmdletName}
            Copy-File -Path (Join-Path $Script:ADT.CurrentSession.GetPropertyValue('scriptParentPath') '*') -Destination $Path -Recurse
            # Set the Files directory to the cache path
            $Script:ADT.CurrentSession.SetPropertyValue('DirFiles', "$Path\Files")
            $Script:ADT.CurrentSession.SetPropertyValue('DirFiles', "$Path\SupportFiles")
        }
        Catch {
            Write-Log -Message "Failed to copy toolkit content to cache folder [$Path]. `r`n$(Resolve-Error)" -Severity 3 -Source ${CmdletName}
            Throw "Failed to copy toolkit content to cache folder [$Path]: $($_.Exception.Message)"
        }
    }
    End {
        Write-FunctionHeaderOrFooter -CmdletName ${CmdletName} -Footer
    }
}


#---------------------------------------------------------------------------
#
# 
#
#---------------------------------------------------------------------------

Function Remove-ContentFromCache {
    <#
.SYNOPSIS
    Removes the toolkit content from the cache folder on the local machine and reverts the $dirFiles and $supportFiles directory
.DESCRIPTION
    Removes the toolkit content from the cache folder on the local machine and reverts the $dirFiles and $supportFiles directory
.PARAMETER Path
    The path to the software cache folder
.EXAMPLE
    Remove-ContentFromCache -Path 'C:\Windows\Temp\PSAppDeployToolkit'
.NOTES

.LINK
    https://psappdeploytoolkit.com
#>
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $false, Position = 0, HelpMessage = 'The path to the software cache folder')]
        [ValidateNotNullorEmpty()]
        [String]$Path = "$($Script:ADT.Config.Toolkit.CachePath)\$($Script:ADT.CurrentSession.GetPropertyValue('installName'))"
    )

    Begin {
        ## Get the name of this function and write header
        [String]${CmdletName} = $PSCmdlet.MyInvocation.MyCommand.Name
        Write-FunctionHeaderOrFooter -CmdletName ${CmdletName} -CmdletBoundParameters $PSBoundParameters -Header
    }
    Process {
        Try {
            If (Test-Path -LiteralPath $Path -PathType 'Container') {
                Try {
                    Write-Log -Message "Removing cache folder [$Path]." -Source ${CmdletName}
                    $null = Remove-Item -Path $Path -Recurse -ErrorAction 'Stop'
                    $Script:ADT.CurrentSession.SetPropertyValue('dirFiles', (Join-Path -Path $($Script:ADT.CurrentSession.GetPropertyValue('scriptParentPath')) -ChildPath 'Files'))
                    $Script:ADT.CurrentSession.SetPropertyValue('dirSupportFiles', (Join-Path -Path $($Script:ADT.CurrentSession.GetPropertyValue('scriptParentPath')) -ChildPath 'SupportFiles'))
                }
                Catch {
                    Write-Log -Message "Failed to remove cache folder [$Path]. `r`n$(Resolve-Error)" -Severity 3 -Source ${CmdletName}
                    Throw "Failed to remove cache folder [$Path]: $($_.Exception.Message)"
                }
            }
            Else {
                Write-Log -Message "Cache folder [$Path] does not exist." -Source ${CmdletName}
            }
        }
        Catch {
            Write-Log -Message "Failed to remove cache folder [$Path]. `r`n$(Resolve-Error)" -Severity 3 -Source ${CmdletName}
            Throw "Failed to remove cache folder [$Path]: $($_.Exception.Message)"
        }
    }
    End {
        Write-FunctionHeaderOrFooter -CmdletName ${CmdletName} -Footer
    }
}
