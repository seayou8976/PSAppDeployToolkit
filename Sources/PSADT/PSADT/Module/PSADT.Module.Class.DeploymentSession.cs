using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using PSADT.Shared;
using PSADT.Types;

namespace PSADT.Module
{
    public sealed class DeploymentSession
    {
        #region Constructors.


        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentSession"/> class.
        /// </summary>
        /// <param name="adtData">The ADT data.</param>
        /// <param name="adtEnv">The ADT environment.</param>
        /// <param name="adtConfig">The ADT configuration.</param>
        /// <param name="adtStrings">The ADT strings.</param>
        /// <param name="moduleSessionState">The module session state.</param>
        /// <param name="runspaceOrigin">Indicates if the origin is a runspace.</param>
        /// <param name="callerSessionState">The caller session state.</param>
        /// <param name="parameters">All parameters from Open-ADTSession.</param>
        public DeploymentSession(PSObject adtData, OrderedDictionary adtEnv, Hashtable adtConfig, Hashtable adtStrings, SessionState moduleSessionState, bool? runspaceOrigin = null, SessionState? callerSessionState = null, Dictionary<string, object>? parameters = null)
        {
            try
            {
                #region Initialization


                // Establish start date/time first so we can accurately mark the start of execution.
                CurrentDate = CurrentDateTime.ToString("dd-MM-yyyy");
                CurrentTime = CurrentDateTime.ToString("HH:mm:ss");

                // Establish initial variable values.
                ADTEnv = adtEnv;
                ADTData = adtData;
                ADTConfig = adtConfig;
                ADTStrings = adtStrings;
                ModuleSessionState = moduleSessionState;

                // Abort if the caller isn't coming in via our module's Open-ADTSession function.
                if (!GetPowerShellCallStackFrameCommand(GetLogEntryCallerInternal()).Equals("Open-ADTSession"))
                {
                    throw new InvalidOperationException("A deployment session can only be instantiated via the Open-ADTSession function.");
                }

                // Extrapolate the Toolkit options from the config hashtable.
                Hashtable configToolkit = (Hashtable)ADTConfig["Toolkit"]!;

                // Set up other variable values based on incoming dictionary.
                if (null != runspaceOrigin)
                {
                    RunspaceOrigin = (bool)runspaceOrigin;
                }
                if (null != callerSessionState)
                {
                    CallerSessionState = callerSessionState;
                }
                if (null != parameters)
                {
                    if (parameters.ContainsKey("DeploymentType"))
                    {
                        DeploymentType = (string)parameters["DeploymentType"];
                    }
                    if (parameters.ContainsKey("DeployMode"))
                    {
                        DeployMode = (string)parameters["DeployMode"];
                    }
                    if (parameters.ContainsKey("AllowRebootPassThru"))
                    {
                        AllowRebootPassThru = (SwitchParameter)parameters["AllowRebootPassThru"];
                    }
                    if (parameters.ContainsKey("TerminalServerMode"))
                    {
                        TerminalServerMode = (SwitchParameter)parameters["TerminalServerMode"];
                    }
                    if (parameters.ContainsKey("DisableLogging"))
                    {
                        DisableLogging = (SwitchParameter)parameters["DisableLogging"];
                    }
                    if (parameters.ContainsKey("AppVendor"))
                    {
                        AppVendor = (string)parameters["AppVendor"];
                    }
                    if (parameters.ContainsKey("AppName"))
                    {
                        AppName = (string)parameters["AppName"];
                    }
                    if (parameters.ContainsKey("AppVersion"))
                    {
                        AppVersion = (string)parameters["AppVersion"];
                    }
                    if (parameters.ContainsKey("AppArch"))
                    {
                        AppArch = (string)parameters["AppArch"];
                    }
                    if (parameters.ContainsKey("AppLang"))
                    {
                        AppLang = (string)parameters["AppLang"];
                    }
                    if (parameters.ContainsKey("AppRevision"))
                    {
                        AppRevision = (string)parameters["AppRevision"];
                    }
                    if (parameters.ContainsKey("AppScriptVersion"))
                    {
                        AppScriptVersion = (Version)parameters["AppScriptVersion"];
                    }
                    if (parameters.ContainsKey("AppScriptDate"))
                    {
                        AppScriptDate = (DateTime)parameters["AppScriptDate"];
                    }
                    if (parameters.ContainsKey("AppScriptAuthor"))
                    {
                        AppScriptAuthor = (string)parameters["AppScriptAuthor"];
                    }
                    if (parameters.ContainsKey("InstallName"))
                    {
                        InstallName = (string)parameters["InstallName"];
                    }
                    if (parameters.ContainsKey("InstallTitle"))
                    {
                        InstallTitle = (string)parameters["InstallTitle"];
                    }
                    if (parameters.ContainsKey("DeployAppScriptFriendlyName"))
                    {
                        DeployAppScriptFriendlyName = (string)parameters["DeployAppScriptFriendlyName"];
                    }
                    if (parameters.ContainsKey("DeployAppScriptVersion"))
                    {
                        DeployAppScriptVersion = (Version)parameters["DeployAppScriptVersion"];
                    }
                    if (parameters.ContainsKey("DeployAppScriptParameters"))
                    {
                        DeployAppScriptParameters = (IDictionary)parameters["DeployAppScriptParameters"];
                    }
                    if (parameters.ContainsKey("AppSuccessExitCodes"))
                    {
                        AppSuccessExitCodes = (int[])parameters["AppSuccessExitCodes"];
                    }
                    if (parameters.ContainsKey("AppRebootExitCodes"))
                    {
                        AppRebootExitCodes = (int[])parameters["AppRebootExitCodes"];
                    }
                    if (parameters.ContainsKey("ScriptDirectory"))
                    {
                        ScriptDirectory = (string)parameters["ScriptDirectory"];
                    }
                    if (parameters.ContainsKey("DirFiles"))
                    {
                        DirFiles = (string)parameters["DirFiles"];
                    }
                    if (parameters.ContainsKey("DirSupportFiles"))
                    {
                        DirSupportFiles = (string)parameters["DirSupportFiles"];
                    }
                    if (parameters.ContainsKey("DefaultMsiFile"))
                    {
                        DefaultMsiFile = (string)parameters["DefaultMsiFile"];
                    }
                    if (parameters.ContainsKey("DefaultMstFile"))
                    {
                        DefaultMstFile = (string)parameters["DefaultMstFile"];
                    }
                    if (parameters.ContainsKey("DefaultMspFiles"))
                    {
                        DefaultMspFiles = (string[])parameters["DefaultMspFiles"];
                    }
                    if (parameters.ContainsKey("ForceWimDetection"))
                    {
                        ForceWimDetection = (SwitchParameter)parameters["ForceWimDetection"];
                    }
                }

                // Ensure DeploymentType is title cased for aesthetics.
                DeploymentType = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DeploymentType.ToLower());
                DeploymentTypeName = (string)((Hashtable)ADTStrings["DeploymentType"]!)[DeploymentType]!;

                // Establish script directories.
                if (null != ScriptDirectory)
                {
                    if (string.IsNullOrWhiteSpace(DirFiles) && Directory.Exists(Path.Combine(ScriptDirectory, "Files")))
                    {
                        DirFiles = Path.Combine(ScriptDirectory, "Files");
                    }
                    if (string.IsNullOrWhiteSpace(DirSupportFiles) && Directory.Exists(Path.Combine(ScriptDirectory, "SupportFiles")))
                    {
                        DirSupportFiles = Path.Combine(ScriptDirectory, "SupportFiles");
                    }
                }


                #endregion
                #region DetectDefaultWimFile


                // If the default frontend hasn't been modified, and there's not already a mounted WIM file, check for WIM files and modify the install accordingly.
                if (string.IsNullOrWhiteSpace(AppName) || ForceWimDetection)
                {
                    // Only proceed if there isn't already a mounted WIM file and we have a WIM file to use.
                    if ((MountedWimFiles.Count == 0) && !string.IsNullOrWhiteSpace(DirFiles) && (Directory.GetFiles(DirFiles, "*.wim", SearchOption.TopDirectoryOnly).FirstOrDefault() is string wimFile))
                    {
                        // Mount the WIM file and reset DirFiles to the mount point.
                        WriteZeroConfigDivider(); ZeroConfigInitiated = true;
                        WriteLogEntry($"Discovered Zero-Config WIM file [{wimFile}].");
                        string mountPath = Path.Combine(DirFiles, Path.GetRandomFileName());
                        ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("& $CommandTable.'Mount-ADTWimFile' -ImagePath $args[0] -Path $args[1] -Index 1"), wimFile, mountPath);
                        MountedWimFiles.Add(new FileInfo(wimFile));
                        DirFiles = mountPath;
                        WriteLogEntry($"Successfully mounted WIM file to [{mountPath}].");

                        // Subst the new DirFiles path to eliminate any potential path length issues.
                        string[] usedLetters = DriveInfo.GetDrives().Select(d => d.Name).ToArray();
                        if ((new string[] {"Z:\\", "Y:\\", "X:\\", "W:\\", "V:\\", "U:\\", "T:\\", "S:\\", "R:\\", "Q:\\", "P:\\", "O:\\", "N:\\", "M:\\", "L:\\", "K:\\", "J:\\", "I:\\", "H:\\", "G:\\", "F:\\", "E:\\", "D:\\", "C:\\", "B:\\", "A:\\"}).Where(l => !usedLetters.Contains(l)).FirstOrDefault() is string availLetter)
                        {
                            availLetter = availLetter.Trim('\\'); WriteLogEntry($"Creating substitution drive [{availLetter}] for [{DirFiles}].");
                            ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("& $CommandTable.'Invoke-ADTSubstOperation' -Drive $args[0] -Path $args[1]"), availLetter, DirFiles);
                            DirFiles = DirFilesSubstDrive = availLetter;
                        }
                        WriteLogEntry($"Using [{DirFiles}] as the base DirFiles directory.");
                    }
                }


                #endregion
                #region DetectDefaultMsi


                // If the default frontend hasn't been modified, check for MSI / MST and modify the install accordingly.
                if (string.IsNullOrWhiteSpace(AppName))
                {
                    // Find the first MSI file in the Files folder and use that as our install.
                    if (string.IsNullOrWhiteSpace(DefaultMsiFile))
                    {
                        // Only proceed if the Files directory is set.
                        if (!string.IsNullOrWhiteSpace(DirFiles))
                        {
                            // Get the first MSI file in the Files directory.
                            string[] msiFiles = Directory.GetFiles(DirFiles, "*.msi", SearchOption.TopDirectoryOnly);
                            if (msiFiles.Where(f => !f.EndsWith($".{ADTEnv["envOSArchitecture"]}.msi")).FirstOrDefault() is string msiFile)
                            {
                                DefaultMsiFile = new FileInfo(msiFile).FullName;
                            }
                            else if (msiFiles.Length > 0)
                            {
                                DefaultMsiFile = new FileInfo(msiFiles[0]).FullName;
                            }
                        }
                    }
                    else if (!Path.IsPathRooted(DefaultMsiFile) && !string.IsNullOrWhiteSpace(DirFiles))
                    {
                        DefaultMsiFile = Path.Combine(DirFiles, DefaultMsiFile);
                    }

                    // If we have a default MSI file, proceed further with the Zero-Config configuration.
                    if (!string.IsNullOrWhiteSpace(DefaultMsiFile))
                    {
                        WriteZeroConfigDivider(); ZeroConfigInitiated = true;
                        WriteLogEntry($"Discovered Zero-Config MSI installation file [{DefaultMsiFile}].");

                        // Discover if there is a zero-config MST file.
                        if (string.IsNullOrWhiteSpace(DefaultMstFile))
                        {
                            string mstFile = Path.ChangeExtension(DefaultMsiFile, "mst");
                            if (File.Exists(mstFile))
                            {
                                DefaultMstFile = mstFile;
                            }
                        }
                        else if (!Path.IsPathRooted(DefaultMstFile) && !string.IsNullOrWhiteSpace(DirFiles))
                        {
                            DefaultMstFile = Path.Combine(DirFiles, DefaultMstFile);
                        }
                        if (!string.IsNullOrWhiteSpace(DefaultMstFile))
                        {
                            WriteLogEntry($"Discovered Zero-Config MST installation file [{DefaultMstFile}].");
                        }

                        // Discover if there are zero-config MSP files. Name multiple MSP files in alphabetical order to control order in which they are installed.
                        if (null == DefaultMspFiles)
                        {
                            if (!string.IsNullOrWhiteSpace(DirFiles) && (Directory.GetFiles(DirFiles, "*.msp", SearchOption.TopDirectoryOnly) is string[] mspFiles))
                            {
                                DefaultMspFiles = mspFiles;
                            }
                        }
                        else if (DefaultMspFiles.Select(f => Path.IsPathRooted(f)).FirstOrDefault() && !string.IsNullOrWhiteSpace(DirFiles))
                        {
                            DefaultMspFiles = DefaultMspFiles.Where(f => !Path.IsPathRooted(f)).Select(f => Path.Combine(DirFiles, f)).ToArray();
                        }
                        if (DefaultMspFiles?.Length > 0)
                        {
                            WriteLogEntry($"Discovered Zero-Config MSP installation file(s) [{string.Join(", ", DefaultMspFiles)}].");
                        }

                        // Read the MSI and get the installation details.
                        ReadOnlyDictionary<string, object> msiProps = ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("$gmtpParams = @{ Path = $args[0] }; if ($args[1]) { $gmtpParams.Add('TransformPath', $args[1]) }; & $CommandTable.'Get-ADTMsiTableProperty' @gmtpParams -Table File"), DefaultMsiFile, DefaultMstFile).Select(d => (ReadOnlyDictionary<string, object>)d.BaseObject).First();
                        DefaultMsiExecutablesList = msiProps.Where(p => Path.GetExtension(p.Key).Equals(".exe")).Select(p => new ProcessObject(Regex.Replace(Path.GetFileNameWithoutExtension(p.Key), "^_", string.Empty))).ToArray();

                        // Generate list of MSI executables for testing later on.
                        if (null != DefaultMsiExecutablesList)
                        {
                            WriteLogEntry($"MSI Executable List [{string.Join(", ", DefaultMsiExecutablesList.Select(p => p.Name))}].");
                        }

                        // Update our app variables with new values.
                        msiProps = ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("$gmtpParams = @{ Path = $args[0] }; if ($args[1]) { $gmtpParams.Add('TransformPath', $args[1]) }; & $CommandTable.'Get-ADTMsiTableProperty' @gmtpParams -Table Property"), DefaultMsiFile, DefaultMstFile).Select(d => (ReadOnlyDictionary<string, object>)d.BaseObject).First();
                        AppName = (string)msiProps["ProductName"];
                        AppVersion = (string)msiProps["ProductVersion"];
                        WriteLogEntry($"App Vendor [{(string)msiProps["Manufacturer"]}].");
                        WriteLogEntry($"App Name [{AppName}].");
                        WriteLogEntry($"App Version [{AppVersion}].");
                        UseDefaultMsi = true;
                    }
                }


                #endregion
                #region SetAppProperties


                // Set up sample variables if Dot Sourcing the script, app details have not been specified.
                if (string.IsNullOrWhiteSpace(AppName))
                {
                    AppName = (string)ADTEnv["appDeployToolkitName"]!;

                    if (!string.IsNullOrWhiteSpace(AppVendor))
                    {
                        AppVendor = null;
                    }
                    if (string.IsNullOrWhiteSpace(AppVersion))
                    {
                        AppVersion = ADTEnv["appDeployMainScriptVersion"]!.ToString()!;
                    }
                    if (string.IsNullOrWhiteSpace(AppLang))
                    {
                        AppLang = (string)ADTEnv["currentLanguage"]!;
                    }
                    if (string.IsNullOrWhiteSpace(AppRevision))
                    {
                        AppRevision = "01";
                    }
                }

                // Sanitize the application details, as they can cause issues in the script.
                string invalidChars = string.Join(null, Path.GetInvalidFileNameChars());
                if (null != AppVendor)
                {
                    AppVendor = Regex.Replace(AppVendor, invalidChars, string.Empty);
                }
                if (null != AppName)
                {
                    AppName = Regex.Replace(AppName, invalidChars, string.Empty);
                }
                if (null != AppVersion)
                {
                    AppVersion = Regex.Replace(AppVersion, invalidChars, string.Empty);
                }
                if (null != AppArch)
                {
                    AppArch = Regex.Replace(AppArch, invalidChars, string.Empty);
                }
                if (null != AppLang)
                {
                    AppLang = Regex.Replace(AppLang, invalidChars, string.Empty);
                }
                if (null != AppRevision)
                {
                    AppRevision = Regex.Replace(AppRevision, invalidChars, string.Empty);
                }

                // If we're left with a null AppName, throw a terminating error.
                if (string.IsNullOrWhiteSpace(AppName))
                {
                    throw new ArgumentNullException("AppName", "The application name was not specified.");
                }


                #endregion
                #region SetInstallProperties


                // Build the Installation Title.
                if (string.IsNullOrWhiteSpace(InstallTitle))
                {
                    InstallTitle = $"{AppVendor} {AppName} {AppVersion}".Trim();
                }
                InstallTitle = Regex.Replace(InstallTitle, "\\s{2,}", string.Empty);

                // Build the Installation Name.
                if (string.IsNullOrWhiteSpace(InstallName))
                {
                    InstallName = $"{AppVendor}_{AppName}_{AppVersion}_{AppArch}_{AppLang}_{AppRevision}";
                }
                InstallName = Regex.Replace(Regex.Replace(InstallName, "\\s|^_|_$", string.Empty), "_+", "_");

                // Set the Defer History registry path.
                RegKeyDeferBase = $"{configToolkit["RegPath"]}\\{ADTEnv["appDeployToolkitName"]}\\DeferHistory";
                RegKeyDeferHistory = $"{RegKeyDeferBase}\\{InstallName}";


                #endregion
                #region InitLogging


                // Generate log paths from our installation properties.
                LogTempFolder = Path.Combine((string)ADTEnv["envTemp"]!, $"{InstallName}_{DeploymentType}");
                if ((bool)configToolkit["CompressLogs"]!)
                {
                    // If the temp log folder already exists from a previous ZIP operation, then delete all files in it to avoid issues.
                    if (Directory.Exists(LogTempFolder))
                    {
                        Directory.Delete(LogTempFolder, true);
                    }
                    LogPath = Directory.CreateDirectory(LogTempFolder).FullName;
                }
                else
                {
                    LogPath = Directory.CreateDirectory((string)configToolkit["LogPath"]!).FullName;
                }

                // Generate the log filename to use. Append the username to the log file name if the toolkit is not running as an administrator,
                // since users do not have the rights to modify files in the ProgramData folder that belong to other users.
                if ((bool)ADTEnv["IsAdmin"]!)
                {
                    LogName = $"{InstallName}_{ADTEnv["appDeployToolkitName"]}_{DeploymentType}.log";
                }
                else
                {
                    LogName = $"{InstallName}_{ADTEnv["appDeployToolkitName"]}_{DeploymentType}_{ADTEnv["envUserName"]}.log";
                }
                LogName = Regex.Replace(LogName, invalidChars, string.Empty);
                string logFile = Path.Combine(LogPath, LogName);
                FileInfo logFileInfo = new FileInfo(logFile);
                int logMaxSize = (int)configToolkit["LogMaxSize"]!;
                bool logFileSizeExceeded = logFileInfo.Exists && (logMaxSize > 0) && ((logFileInfo.Length / 1048576.0) > logMaxSize);

                // Check if log file needs to be rotated.
                if ((logFileInfo.Exists && !(bool)configToolkit["LogAppend"]!) || logFileSizeExceeded)
                {
                    try
                    {
                        // Get new log file path.
                        string logFileNameOnly = Path.GetFileNameWithoutExtension(LogName);
                        string logFileExtension = Path.GetExtension(LogName);
                        string logFileTimestamp = DateTime.Now.ToString("O").Split('.')[0].Replace(":", null);
                        string archiveLogFileName = $"{logFileNameOnly}_{logFileTimestamp}{logFileExtension}";
                        string archiveLogFilePath = Path.Combine(LogPath, archiveLogFileName);
                        int logMaxHistory = (int)configToolkit["LogMaxHistory"]!;

                        // Log message about archiving the log file.
                        if (logFileSizeExceeded)
                        {
                            WriteLogEntry($"Maximum log file size [{logMaxSize} MB] reached. Rename log file to [{archiveLogFileName}].", 2);
                        }

                        // Rename the file.
                        logFileInfo.MoveTo(archiveLogFilePath);

                        // Start new log file and log message about archiving the old log file.
                        if (logFileSizeExceeded)
                        {
                            WriteLogEntry($"Previous log file was renamed to [{archiveLogFileName}] because maximum log file size of [{logMaxSize} MB] was reached.", 2);
                        }

                        // Get all log files sorted by last write time.
                        var logFiles = new DirectoryInfo(LogPath).GetFiles($"{logFileNameOnly}*.log").OrderBy(f => f.LastWriteTime);

                        // Keep only the max number of log files.
                        if (logFiles.Count() > logMaxHistory)
                        {
                            logFiles.Take(logFiles.Count() - logMaxHistory).ToList().ForEach(f => f.Delete());
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLogEntry($"Failed to rotate the log file [{logFile}]: {ex.Message}", 3);
                    }
                }

                // Open log file with commencement message.
                WriteLogDivider(2);
                WriteLogEntry($"[{InstallName}] {DeploymentTypeName.ToLower()} started.");


                #endregion
                #region LogScriptInfo


                // Announce provided deployment script info.
                if (!UseDefaultMsi)
                {
                    if (null != AppScriptVersion)
                    {
                        WriteLogEntry($"[{InstallName}] script version is [{AppScriptVersion}].");
                    }
                    if (null != AppScriptDate)
                    {
                        WriteLogEntry($"[{InstallName}] script date is [{((DateTime)AppScriptDate).ToString("O").Split('T')[0]}].");
                    }
                    if (!string.IsNullOrWhiteSpace(AppScriptAuthor))
                    {
                        WriteLogEntry($"[{InstallName}] script author is [{AppScriptAuthor}].");
                    }
                }
                if (!string.IsNullOrWhiteSpace(DeployAppScriptFriendlyName))
                {
                    if (null != DeployAppScriptVersion)
                    {
                        WriteLogEntry($"[{DeployAppScriptFriendlyName}] script version is [{DeployAppScriptVersion}].");
                    }
                    if ((null != DeployAppScriptParameters) && (DeployAppScriptParameters.Count > 0))
                    {
                        WriteLogEntry($"The following parameters were passed to [${DeployAppScriptFriendlyName}]: [{Utility.ConvertDictToPowerShellArgs(DeployAppScriptParameters)}].");
                    }
                }
                PSObject adtDirectories = (PSObject)ADTData.Properties["Directories"].Value;
                PSObject adtDurations = (PSObject)ADTData.Properties["Durations"].Value;
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] module version is [{ADTEnv["appDeployMainScriptVersion"]}].");
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] module imported in [{((TimeSpan)adtDurations.Properties["ModuleImport"].Value).TotalSeconds}] seconds.");
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] module initialized in [{((TimeSpan)adtDurations.Properties["ModuleInit"].Value).TotalSeconds}] seconds.");
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] module path is [{ADTEnv["appDeployToolkitPath"]}].");
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] config path is [{adtDirectories.Properties["Config"].Value}].");
                WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] string path is [{adtDirectories.Properties["Strings"].Value}].");

                // Announce session instantiation mode.
                if (null != CallerSessionState)
                {
                    WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] session mode is [Compatibility]. This mode is for the transition of v3.x scripts and is not for new development.", 2);
                    WriteLogEntry("Information on how to migrate this script to Native mode is available at [https://psappdeploytoolkit.com/].", 2);
                }
                else
                {
                    WriteLogEntry($"[{ADTEnv["appDeployToolkitName"]}] session mode is [Native].");
                }


                #endregion
                #region LogSystemInfo


                // Report on all determined system info.
                WriteLogEntry($"Computer Name is [{ADTEnv["envComputerNameFQDN"]}].");
                WriteLogEntry($"Current User is [{ADTEnv["ProcessNTAccount"]}].");
                WriteLogEntry($"OS Version is [{ADTEnv["envOSName"]}{((ADTEnv["envOSServicePack"] is string envOSServicePack) && !string.IsNullOrWhiteSpace(envOSServicePack) ? envOSServicePack : string.Empty)} {ADTEnv["envOSArchitecture"]} {ADTEnv["envOSVersion"]}].");
                WriteLogEntry($"OS Type is [{ADTEnv["envOSProductTypeName"]}].");
                WriteLogEntry($"Hardware Platform is [{ADTEnv["envHardwareType"]}].");
                WriteLogEntry($"Current Culture is [{CultureInfo.CurrentCulture.Name}], language is [{ADTEnv["currentLanguage"]}] and UI language is [{ADTEnv["currentUILanguage"]}].");
                WriteLogEntry($"PowerShell Host is [{((PSHost)ADTEnv["envHost"]!).Name}] with version [{((PSHost)ADTEnv["envHost"]!).Version}].");
                WriteLogEntry($"PowerShell Version is [{ADTEnv["envPSVersion"]} {ADTEnv["psArchitecture"]}].");
                if (null != ADTEnv["envCLRVersion"])
                {
                    WriteLogEntry($"PowerShell CLR (.NET) version is [{ADTEnv["envCLRVersion"]}].");
                }


                #endregion
                #region LogUserInfo


                // Log details for all currently logged on users.
                WriteLogEntry($"Display session information for all logged on users:\n{ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("$args[0] | & $CommandTable.'Format-List' | & $CommandTable.'Out-String' -Width ([System.Int32]::MaxValue)"), ADTEnv["LoggedOnUserSessions"]).First().BaseObject}", false);

                // Provide detailed info about current process state.
                if (null != ADTEnv["usersLoggedOn"])
                {
                    WriteLogEntry($"The following users are logged on to the system: [{string.Join(", ", ADTEnv["usersLoggedOn"])}].");

                    // Check if the current process is running in the context of one of the logged on users
                    if (null != ADTEnv["CurrentLoggedOnUserSession"])
                    {
                        WriteLogEntry($"Current process is running with user account [{ADTEnv["ProcessNTAccount"]}] under logged on user session for [{((QueryUser.TerminalSessionInfo)ADTEnv["CurrentLoggedOnUserSession"]!).NTAccount}].");
                    }
                    else
                    {
                        WriteLogEntry($"Current process is running under a system account [{ADTEnv["ProcessNTAccount"]}].");
                    }

                    // Guard Intune detection code behind a variable.
                    if ((bool)configToolkit["OobeDetection"]! && (Environment.OSVersion.Version >= new Version(10, 0, 16299, 0)) && !Utility.IsOOBEComplete())
                    {
                        WriteLogEntry("Detected OOBE in progress, changing deployment mode to silent.");
                        DeployMode = "Silent";
                    }

                    // Display account and session details for the account running as the console user (user with control of the physical monitor, keyboard, and mouse)
                    if (null != ADTEnv["CurrentConsoleUserSession"])
                    {
                        WriteLogEntry($"The following user is the console user [{((QueryUser.TerminalSessionInfo)ADTEnv["CurrentConsoleUserSession"]!).NTAccount}] (user with control of physical monitor, keyboard, and mouse).");
                    }
                    else
                    {
                        WriteLogEntry("There is no console user logged on (user with control of physical monitor, keyboard, and mouse).");
                    }

                    // Display the account that will be used to execute commands in the user session when toolkit is running under the SYSTEM account
                    if (null != ADTEnv["RunAsActiveUser"])
                    {
                        WriteLogEntry($"The active logged on user is [{((QueryUser.TerminalSessionInfo)ADTEnv["RunAsActiveUser"]!).NTAccount}].");
                    }
                }
                else
                {
                    WriteLogEntry("No users are logged on to the system.");
                }

                // Log which language's UI messages are loaded from the config file
                WriteLogEntry($"The current execution context has a primary UI language of [{ADTEnv["currentLanguage"]}].");

                // Advise whether the UI language was overridden.
                if (((Hashtable)ADTConfig["UI"]!)["LanguageOverride"] is string languageOverride)
                {
                    WriteLogEntry($"The config file was configured to override the detected primary UI language with the following UI language: [{languageOverride}].");
                }
                WriteLogEntry($"The following UI messages were imported from the config file: [{ADTData.Properties["Language"].Value}].");


                #endregion
                #region PerformConfigMgrTests


                // Check if script is running from a SCCM Task Sequence.
                if ((bool)ADTEnv["RunningTaskSequence"]!)
                {
                    WriteLogEntry("Successfully found COM object [Microsoft.SMS.TSEnvironment]. Therefore, script is currently running from a SCCM Task Sequence.");
                }
                else
                {
                    WriteLogEntry("Unable to find COM object [Microsoft.SMS.TSEnvironment]. Therefore, script is not currently running from a SCCM Task Sequence.");
                }


                #endregion
                #region PerformSystemAccountTests


                // Return early if we're not in session 0.
                if ((bool)ADTEnv["SessionZero"]!)
                {
                    // If the script was launched with deployment mode set to NonInteractive, then continue.
                    if (DeployMode != "Interactive")
                    {
                        WriteLogEntry($"Session 0 detected but deployment mode was manually set to [{DeployMode}].");
                    }
                    else if ((bool)configToolkit["SessionDetection"]!)
                    {
                        // If the process is not able to display a UI, enable NonInteractive mode.
                        if ((bool)ADTEnv["IsProcessUserInteractive"]!)
                        {
                            DeployMode = "NonInteractive";
                            WriteLogEntry($"Session 0 detected, process not running in user interactive mode; deployment mode set to [{DeployMode}].");
                        }
                        else if (null == ADTEnv["usersLoggedOn"])
                        {
                            DeployMode = "NonInteractive";
                            WriteLogEntry($"Session 0 detected, process running in user interactive mode, no users logged on; deployment mode set to [{DeployMode}].");
                        }
                        else
                        {
                            WriteLogEntry("Session 0 detected, process running in user interactive mode, user(s) logged on.");
                        }
                    }
                    else
                    {
                        WriteLogEntry("Session 0 detected but toolkit is configured to not adjust deployment mode.");
                    }
                }
                else
                {
                    WriteLogEntry("Session 0 not detected.");
                }


                #endregion
                #region SetDeploymentProperties


                // Set Deploy Mode switches.
                WriteLogEntry($"Installation is running in [{DeployMode}] mode.");
                switch (DeployMode)
                {
                    case "Silent":
                        DeployModeNonInteractive = true;
                        DeployModeSilent = true;
                        break;
                    case "NonInteractive":
                        DeployModeNonInteractive = true;
                        break;
                }


                // Check deployment type (install/uninstall).
                WriteLogEntry($"Deployment type is [{DeploymentTypeName}].");


                #endregion
                #region TestDefaultMsi


                // Advise the caller if a zero-config MSI was found.
                if (UseDefaultMsi)
                {
                    WriteLogEntry($"Discovered Zero-Config MSI installation file [{DefaultMsiFile}].");
                }


                #endregion
                #region TestAdminRequired


                // Check current permissions and exit if not running with Administrator rights.
                if ((bool)configToolkit["RequireAdmin"]! && !(bool)ADTEnv["IsAdmin"]!)
                {
                    throw new UnauthorizedAccessException($"[{ADTEnv["appDeployToolkitName"]}] has a toolkit config option [RequireAdmin] set to [True] and the current user is not an Administrator, or PowerShell is not elevated. Please re-run the deployment script as an Administrator or change the option in the config file to not require Administrator rights.");
                }


                #endregion
                #region Finalization


                // If terminal server mode was specified, change the installation mode to support it.
                if (TerminalServerMode)
                {
                    ModuleSessionState.InvokeCommand.InvokeScript("& $CommandTable.'Enable-ADTTerminalServerInstallMode'");
                }

                // Export session's public variables to the user's scope. For these, we can't capture the Set-Variable
                // PassThru data as syntax like `$var = 'val'` constructs a new PSVariable every time.
                if (null != CallerSessionState)
                {
                    this.GetType().GetProperties(BindingFlags.Public).ToList().ForEach(p => CallerSessionState.PSVariable.Set(p.Name, p.GetValue(this)));
                }


                #endregion
            }
            catch (Exception ex)
            {
                WriteLogEntry($"Failure occurred while instantiating new deployment session: \"{ex.Message}\".", 3);
                SetExitCode(60008);
                Close();
                throw;
            }
        }


        #endregion
        #region Methods.


        /// <summary>
        /// Gets the caller of the log entry from the call stack frames.
        /// </summary>
        /// <returns>The call stack frame of the log entry caller.</returns>
        private CallStackFrame GetLogEntryCallerInternal()
        {
            return GetLogEntryCaller(ModuleSessionState.InvokeCommand.InvokeScript("& $CommandTable.'Get-PSCallStack'").Skip(1).Select(o => (CallStackFrame)o.BaseObject).ToArray());
        }

        /// <summary>
        /// Gets the caller of the log entry from the call stack frames.
        /// </summary>
        /// <param name="stackFrames">The call stack frames.</param>
        /// <returns>The call stack frame of the log entry caller.</returns>
        public static CallStackFrame GetLogEntryCaller(CallStackFrame[] stackFrames)
        {
            foreach (CallStackFrame frame in stackFrames)
            {
                // Get the command from the frame and test its validity.
                string command = GetPowerShellCallStackFrameCommand(frame);
                if (!string.IsNullOrWhiteSpace(command) && (!Regex.IsMatch(command, "^(Write-(Log|ADTLogEntry)|<ScriptBlock>(<\\w+>)?)$") || (Regex.IsMatch(command, "^(<ScriptBlock>(<\\w+>)?)$") && frame.GetScriptLocation().Equals("<No file>"))))
                {
                    return frame;
                }
            }
            return null!;
        }

        /// <summary>
        /// Gets the PowerShell call stack frame command.
        /// </summary>
        /// <param name="frame">The call stack frame.</param>
        /// <returns>The PowerShell call stack frame command.</returns>
        private static string GetPowerShellCallStackFrameCommand(CallStackFrame frame)
        {
            // We must re-create the "Command" ScriptProperty as it's only available in PowerShell.
            if (null == frame.InvocationInfo)
            {
                return frame.FunctionName;
            }
            if (null == frame.InvocationInfo.MyCommand)
            {
                return frame.InvocationInfo.InvocationName;
            }
            if (frame.InvocationInfo.MyCommand.Name != string.Empty)
            {
                return frame.InvocationInfo.MyCommand.Name;
            }
            return frame.FunctionName;
        }

        /// <summary>
        /// Closes the session and releases resources.
        /// </summary>
        /// <returns>The exit code.</returns>
        public int Close()
        {
            // Abort if the caller isn't coming in via our module's Close-ADTSession function.
            if (!(new StackFrame(1, false).GetMethod()!.Name.Equals(".ctor")) && !GetPowerShellCallStackFrameCommand(GetLogEntryCallerInternal()).Equals("Close-ADTSession"))
            {
                throw new InvalidOperationException("A deployment session can only be closed via the Close-ADTSession function.");
            }

            // Throw if this object has already been disposed.
            if (Disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "This object has already been disposed.");
            }

            // If terminal server mode was specified, revert the installation mode to support it.
            if (TerminalServerMode)
            {
                ModuleSessionState.InvokeCommand.InvokeScript("& $CommandTable.'Disable-ADTTerminalServerInstallMode'");
            }

            // Store app/deployment details string. If we're exiting before properties are set, use a generic string.
            if ((GetPropertyValue(nameof(InstallName)) is string deployString) && !string.IsNullOrWhiteSpace(deployString))
            {
                deployString = $"[{GetPropertyValue(nameof(InstallName))}] {DeploymentTypeName.ToLower()}".Trim();
            }
            else
            {
                deployString = $"{ADTEnv["appDeployToolkitName"]} deployment";
            }

            // Process resulting exit code.
            string deploymentStatus = GetDeploymentStatus();
            switch (deploymentStatus)
            {
                case "FastRetry":
                    // Just advise of the exit code with the appropriate severity.
                    WriteLogEntry($"{deployString} completed with exit code [{ExitCode}].", 2);
                    break;
                case "Error":
                    WriteLogEntry($"{deployString} completed with exit code [{ExitCode}].", 3);
                    break;
                default:
                    // Clean up app deferral history.
                    ResetDeferHistory();

                    // Handle reboot prompts on successful script completion.
                    if (deploymentStatus.Equals("RestartRequired") && (bool)GetPropertyValue(nameof(AllowRebootPassThru))!)
                    {
                        WriteLogEntry("A restart has been flagged as required.");
                    }
                    else
                    {
                        ExitCode = 0;
                    }
                    WriteLogEntry($"{deployString} completed with exit code [{ExitCode}].", 0);
                    break;
            }

            // Update the module's last tracked exit code.
            if (ExitCode != 0)
            {
                ADTData.Properties["LastExitCode"].Value = ExitCode;
            }

            // Remove any subst paths if created in the zero-config WIM code.
            if (!string.IsNullOrWhiteSpace(DirFilesSubstDrive))
            {
                ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("& $CommandTable.'Invoke-ADTSubstOperation' -Drive $args[0] -Delete"), DirFilesSubstDrive);
            }

            // Unmount any stored WIM file entries.
            if (MountedWimFiles.Count > 0)
            {
                MountedWimFiles.Reverse(); ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, ScriptBlock.Create("& $CommandTable.'Dismount-ADTWimFile' -ImagePath $args[0]"), MountedWimFiles);
                MountedWimFiles.Clear();
            }

            // Write out a log divider to indicate the end of logging.
            WriteLogDivider();
            Disposed = true;

            // Extrapolate the Toolkit options from the config hashtable.
            Hashtable configToolkit = (Hashtable)ADTConfig["Toolkit"]!;

            // Compress log files if configured to do so.
            if ((bool)configToolkit["CompressLogs"]!)
            {
                // Archive the log files to zip format and then delete the temporary logs folder.
                string destArchiveFileName = $"{GetPropertyValue(nameof(InstallName))}_{GetPropertyValue(nameof(DeploymentType))}_{0}.zip";
                try
                {
                    // Get all archive files sorted by last write time.
                    var archiveFiles = Directory.GetFiles((string)configToolkit["LogPath"]!, string.Format(destArchiveFileName, "*")).Select(f => new FileInfo(f)).OrderBy(f => f.LastWriteTime);
                    destArchiveFileName = string.Format(destArchiveFileName, DateTime.Now.ToString("O").Split('.')[0].Replace(":", null));

                    // Keep only the max number of archive files
                    int logMaxHistory = (int)configToolkit["LogMaxHistory"]!;
                    if (archiveFiles.Count() > logMaxHistory)
                    {
                        archiveFiles.Take(archiveFiles.Count() - logMaxHistory).ToList().ForEach(f => f.Delete());
                    }

                    // Compression of the log files.
                    string logTempFolder = (string)this.GetPropertyValue(nameof(LogTempFolder))!;
                    ZipFile.CreateFromDirectory(logTempFolder, destArchiveFileName, CompressionLevel.Optimal, false);
                    Directory.Delete(logTempFolder, true);
                }
                catch (Exception ex)
                {
                    WriteLogEntry($"Failed to manage archive file [$DestinationArchiveFileName]: {ex.Message}", 3);
                }
            }

            // Return the exit code to the caller.
            return ExitCode;
        }

        /// <summary>
        /// Gets the value of a property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public object GetPropertyValue(string propertyName)
        {
            // This getter exists as once the object is opened, we need to read the variable from the caller's scope.
            // We must get the variable every time as syntax like `$var = 'val'` always constructs a new PSVariable...
            if (null != CallerSessionState)
            {
                return CallerSessionState.PSVariable.Get(propertyName)!.Value;
            }
            return this.GetType().GetProperty(propertyName)!.GetValue(this)!;
        }

        /// <summary>
        /// Sets the value of a property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value to set.</param>
        public void SetPropertyValue(string propertyName, object propertyValue)
        {
            // This getter exists as once the object is opened, we need to read the variable from the caller's scope.
            // We must get the variable every time as syntax like `$var = 'val'` always constructs a new PSVariable...
            if (null != CallerSessionState)
            {
                CallerSessionState.PSVariable.Set(propertyName, propertyValue);
            }
            this.GetType().GetProperty(propertyName)!.SetValue(this, propertyValue);
        }

        /// <summary>
        /// Writes a log entry with detailed parameters.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="severity">The severity level.</param>
        /// <param name="source">The source of the log entry.</param>
        /// <param name="scriptSection">The script section.</param>
        /// <param name="writeHost">Whether to write to the host.</param>
        /// <param name="debugMessage">Whether it is a debug message.</param>
        /// <param name="logType">The type of log.</param>
        /// <param name="logFileDirectory">The log file directory.</param>
        /// <param name="logFileName">The log file name.</param>
        public void WriteLogEntry(string[] message, uint? severity, string source, string scriptSection, bool? writeHost, bool debugMessage, string logType, string logFileDirectory, string logFileName)
        {
            // Extrapolate the Toolkit options from the config hashtable.
            Hashtable configToolkit = (Hashtable)ADTConfig["Toolkit"]!;

            // Determine whether we can write to the console.
            if (null == writeHost)
            {
                writeHost = (bool)configToolkit["LogWriteToHost"]!;
            }

            // Perform early return checks before wasting time.
            if (((bool)GetPropertyValue(nameof(DisableLogging))! && (bool)!writeHost) || (debugMessage && !(bool)configToolkit["LogDebugMessage"]!))
            {
                return;
            }

            // Establish logging date/time vars.
            DateTime dateNow = DateTime.Now;
            string logTime = dateNow.ToString("HH\\:mm\\:ss.fff");
            CallStackFrame invoker = GetLogEntryCallerInternal();

            // Determine the log file name; either a proper script/function, or a caller directly from the console.
            string logFile = !string.IsNullOrWhiteSpace(invoker.ScriptName) ? invoker.ScriptName : invoker.GetScriptLocation();

            // Set up default values if not specified.
            if (null == severity)
            {
                severity = 1;
            }
            if (string.IsNullOrWhiteSpace(source))
            {
                source = GetPowerShellCallStackFrameCommand(invoker);
            }
            if (string.IsNullOrWhiteSpace(scriptSection))
            {
                scriptSection = (string)GetPropertyValue(nameof(InstallPhase))!;
            }
            if (string.IsNullOrWhiteSpace(logType))
            {
                logType = (string)configToolkit["LogStyle"]!;
            }
            if (string.IsNullOrWhiteSpace(logFileDirectory))
            {
                logFileDirectory = LogPath;
            }
            else if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }
            if (string.IsNullOrWhiteSpace(logFileName))
            {
                logFileName = (string)GetPropertyValue(nameof(LogName))!;
            }

            // Cache all data pertaining to current severity.
            OrderedDictionary logData = (OrderedDictionary)ADTData.Properties["Logging"].Value;
            OrderedDictionary logFmts = (OrderedDictionary)logData["Formats"]!;
            OrderedDictionary sevData = ((ReadOnlyCollection<OrderedDictionary>)logData["Severities"]!)[(int)severity];
            OrderedDictionary sevCols = (OrderedDictionary)sevData["Colours"]!;
            string outFile = Path.Combine(logFileDirectory ?? string.Empty, logFileName ?? string.Empty);

            // Store log string to format with message.
            StringDictionary logFormats = new StringDictionary()
            {
                { "Legacy", string.Format((string)logFmts["Legacy"]!, "{0}", dateNow.ToString("O").Split('T')[0], logTime, scriptSection, source, sevData["Name"]) },
                { "CMTrace", string.Format((string)logFmts["CMTrace"]!, "{0}", scriptSection, $"{logTime}+{CurrentTimeZoneBias.TotalMinutes}", dateNow.ToString("M-dd-yyyy"), source, severity, logFile) },
            };

            // Add this log message to the session's buffer.
            message.ToList().ForEach(msg => LogBuffer.Add(new LogEntry(dateNow, invoker, msg, (uint)severity, source, scriptSection)));

            // Write out all messages to disk if configured/permitted to do so.
            if (!string.IsNullOrWhiteSpace(outFile) && !(bool)GetPropertyValue(nameof(DisableLogging))!)
            {
                using (StreamWriter logFileWriter = new StreamWriter(outFile, true, new UTF8Encoding(true)))
                {
                    string logLine = logFormats[logType]!;
                    switch (logType)
                    {
                        case "CMTrace":
                            foreach (string msg in message)
                            {
                                if (msg.Contains("\n"))
                                {
                                    // Replace all empty lines with a space so OneTrace doesn't trim them.
                                    // When splitting the message, we want to trim all lines but not replace genuine
                                    // spaces. As such, replace all spaces and empty lines with a punctuation space.
                                    // C# identifies this character as whitespace but OneTrace does not so it works.
                                    // The empty line feed at the end is required by OneTrace to format correctly.
                                    logFileWriter.WriteLine(string.Format(logLine, string.Join("\n", msg.Replace("\r", null).Trim().Replace(' ', (char)0x2008).Split((char)10).ToList().Select(m => Regex.Replace(m, "^$", $"{(char)0x2008}"))).Replace("\n", "\r\n") + "\r\n"));
                                }
                                else
                                {
                                    logFileWriter.WriteLine(string.Format(logLine, msg));
                                }
                            }
                            break;
                        case "Legacy":
                            foreach (string msg in message)
                            {
                                logFileWriter.WriteLine(string.Format(logLine, msg));
                            }
                            break;
                    }
                }
            }

            // Write out all messages to host if configured/permitted to do so.
            if ((bool)writeHost!)
            {
                if ((bool)configToolkit["LogHostOutputToStdStreams"]!)
                {
                    // Colour the console if we're not informational.
                    if (severity != 1)
                    {
                        Console.ForegroundColor = (ConsoleColor)sevCols["ForegroundColor"]!;
                        Console.BackgroundColor = (ConsoleColor)sevCols["BackgroundColor"]!;
                    }

                    // Write errors to stderr, otherwise send everything else to stdout.
                    if (severity == 3)
                    {
                        Console.Error.WriteLine(string.Join(Environment.NewLine, message.Select(msg => string.Format(logFormats["Legacy"]!, msg))));
                    }
                    else
                    {
                        Console.WriteLine(string.Join(Environment.NewLine, message.Select(msg => string.Format(logFormats["Legacy"]!, msg))));
                    }

                    // Reset the console colours back to default.
                    Console.ResetColor();
                }
                else
                {
                    // Write the host output to PowerShell's InformationStream.
                    ModuleSessionState.InvokeCommand.InvokeScript(ModuleSessionState, WriteLogEntryDelegate, message, sevCols, source, logFormats["Legacy"]!);
                }
            }
        }

        /// <summary>
        /// Writes a log entry with a message array.
        /// </summary>
        /// <param name="message">The log message array.</param>
        public void WriteLogEntry(string[] message)
        {
            WriteLogEntry(message, null, string.Empty, string.Empty, null, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log entry with a message array and severity.
        /// </summary>
        /// <param name="message">The log message array.</param>
        /// <param name="severity">The severity level.</param>
        public void WriteLogEntry(string[] message, uint? severity)
        {
            WriteLogEntry(message, severity, string.Empty, string.Empty, null, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log entry with a message array and host write option.
        /// </summary>
        /// <param name="message">The log message array.</param>
        /// <param name="writeHost">Whether to write to the host.</param>
        public void WriteLogEntry(string[] message, bool writeHost)
        {
            WriteLogEntry(message, null, string.Empty, string.Empty, writeHost, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log entry with a single message.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void WriteLogEntry(string message)
        {
            WriteLogEntry([message], null, string.Empty, string.Empty, null, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log entry with a single message and severity.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="severity">The severity level.</param>
        public void WriteLogEntry(string message, uint? severity)
        {
            WriteLogEntry([message], severity, string.Empty, string.Empty, null, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log entry with a single message and host write option.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="writeHost">Whether to write to the host.</param>
        public void WriteLogEntry(string message, bool writeHost)
        {
            WriteLogEntry([message], null, string.Empty, string.Empty, writeHost, false, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Writes a log divider with a specified count.
        /// </summary>
        /// <param name="count">The number of dividers to write.</param>
        private void WriteLogDivider(uint count)
        {
            StringCollection dividers = []; for (uint i = 0; i < count; i++) { dividers.Add(new string('*', 79)); }
            WriteLogEntry(dividers.Cast<string>().ToArray());
        }

        /// <summary>
        /// Writes a log divider.
        /// </summary>
        private void WriteLogDivider()
        {
            WriteLogDivider(1);
        }

        /// <summary>
        /// Writes a log divider prior to a Zero-Config setup.
        /// </summary>
        private void WriteZeroConfigDivider()
        {
            // Print an extra divider when we process a Zero-Config setup before the main logging starts.
            if (!ZeroConfigInitiated)
            {
                WriteLogDivider(2);
            }
        }

        /// <summary>
        /// Tests the deferral history path.
        /// </summary>
        /// <returns>True if the deferral history path exists; otherwise, false.</returns>
        private bool TestDeferHistoryPath()
        {
            return ModuleSessionState.InvokeProvider.Item.Exists(RegKeyDeferHistory, true, true);
        }

        /// <summary>
        /// Creates the deferral history path.
        /// </summary>
        private void CreateDeferHistoryPath()
        {
            ModuleSessionState.InvokeProvider.Item.New(RegKeyDeferBase, InstallName, "None", null);
        }

        /// <summary>
        /// Gets the deferral history.
        /// </summary>
        /// <returns>The deferral history.</returns>
        public PSObject? GetDeferHistory()
        {
            if (string.IsNullOrWhiteSpace(RegKeyDeferHistory) || !TestDeferHistoryPath())
            {
                return null;
            }
            WriteLogEntry("Getting deferral history...");
            return ModuleSessionState.InvokeProvider.Property.Get(RegKeyDeferHistory, null)[0];
        }

        /// <summary>
        /// Sets the deferral history.
        /// </summary>
        /// <param name="deferDeadline">The deferral deadline.</param>
        /// <param name="deferTimesRemaining">The deferral times remaining.</param>
        public void SetDeferHistory(int? deferTimesRemaining, string deferDeadline)
        {
            if (null != deferTimesRemaining)
            {
                WriteLogEntry($"Setting deferral history: [DeferTimesRemaining = {deferTimesRemaining}].");
                if (!TestDeferHistoryPath())
                {
                    CreateDeferHistoryPath();
                }
                ModuleSessionState.InvokeProvider.Property.New([RegKeyDeferHistory], "DeferTimesRemaining", "String", deferTimesRemaining, true, true);
            }
            if (!string.IsNullOrWhiteSpace(deferDeadline))
            {
                WriteLogEntry($"Setting deferral history: [DeferDeadline = {deferDeadline}].");
                ModuleSessionState.InvokeProvider.Property.New([RegKeyDeferHistory], "DeferDeadline", "String", deferDeadline, true, true);
            }
        }

        /// <summary>
        /// Resets the deferral history.
        /// </summary>
        public void ResetDeferHistory()
        {
            if (!string.IsNullOrWhiteSpace(RegKeyDeferHistory) && TestDeferHistoryPath())
            {
                WriteLogEntry("Removing deferral history...");
                ModuleSessionState.InvokeProvider.Item.Remove(RegKeyDeferHistory, true);
            }
        }

        /// <summary>
        /// Gets the deployment status.
        /// </summary>
        /// <returns>The deployment status.</returns>
        public string GetDeploymentStatus()
        {
            // Extrapolate the UI options from the config hashtable.
            Hashtable configUI = (Hashtable)ADTConfig["UI"]!;

            if ((ExitCode == (int)configUI["DefaultExitCode"]!) || (ExitCode == (int)configUI["DeferExitCode"]!))
            {
                return "FastRetry";
            }
            else if (((int[])GetPropertyValue(nameof(AppRebootExitCodes))!).Contains(ExitCode))
            {
                return "RestartRequired";
            }
            else if (((int[])GetPropertyValue(nameof(AppSuccessExitCodes))!).Contains(ExitCode))
            {
                return "Complete";
            }
            else
            {
                return "Error";
            }
        }

        /// <summary>
        /// Gets the mounted WIM files.
        /// </summary>
        /// <returns>A read-only collection of mounted WIM files.</returns>
        public ReadOnlyCollection<FileInfo> GetMountedWimFiles()
        {
            return MountedWimFiles.AsReadOnly();
        }

        /// <summary>
        /// Gets the default MSI executables list.
        /// </summary>
        /// <returns>An array of default MSI executables.</returns>
        public ProcessObject[]? GetDefaultMsiExecutablesList()
        {
            return DefaultMsiExecutablesList;
        }

        /// <summary>
        /// Gets the deployment type name.
        /// </summary>
        /// <returns>The deployment type name.</returns>
        public string GetDeploymentTypeName()
        {
            return DeploymentTypeName;
        }

        /// <summary>
        /// Determines whether the origin is a runspace.
        /// </summary>
        /// <returns>True if the origin is a runspace; otherwise, false.</returns>
        public bool IsRunspaceOrigin()
        {
            return RunspaceOrigin;
        }

        /// <summary>
        /// Determines whether the mode is non-interactive.
        /// </summary>
        /// <returns>True if the mode is non-interactive; otherwise, false.</returns>
        public bool IsNonInteractive()
        {
            return DeployModeNonInteractive;
        }

        /// <summary>
        /// Determines whether the mode is silent.
        /// </summary>
        /// <returns>True if the mode is silent; otherwise, false.</returns>
        public bool IsSilent()
        {
            return DeployModeSilent;
        }

        /// <summary>
        /// Sets the exit code.
        /// </summary>
        /// <param name="exitCode">The exit code to set.</param>
        public void SetExitCode(int exitCode)
        {
            ExitCode = exitCode;
        }


        #endregion
        #region Internal variables.


        /// <summary>
        /// Gets the Write-LogEntry delegate script block.
        /// </summary>
        private static readonly ScriptBlock WriteLogEntryDelegate = ScriptBlock.Create("$colours = $args[1]; $args[0] | & $CommandTable.'Write-ADTLogEntryToInformationStream' @colours -Source $args[2] -Format $args[3]");

        /// <summary>
        /// Gets/sets the disposal state of this object.
        /// </summary>
        private bool Disposed { get; set; }

        /// <summary>
        /// Gets the environment table that was supplied during object instantiation.
        /// </summary>
        private PSObject ADTData { get; }

        /// <summary>
        /// Gets the environment table that was supplied during object instantiation.
        /// </summary>
        private OrderedDictionary ADTEnv { get; }

        /// <summary>
        /// Gets the config table that was supplied during object instantiation.
        /// </summary>
        private Hashtable ADTConfig { get; }

        /// <summary>
        /// Gets the string table that was supplied during object instantiation.
        /// </summary>
        private Hashtable ADTStrings { get; }

        /// <summary>
        /// Gets the module's SessionState from value that was supplied during object instantiation.
        /// </summary>
        private SessionState ModuleSessionState { get; }

        /// <summary>
        /// Gets the caller's SessionState from value that was supplied during object instantiation.
        /// </summary>
        private SessionState? CallerSessionState { get; }

        /// <summary>
        /// Gets the mounted WIM files within this session.
        /// </summary>
        private readonly List<FileInfo> MountedWimFiles = [];

        /// <summary>
        /// Gets the log entries written with this session.
        /// </summary>
        private readonly List<LogEntry> LogBuffer = [];

        /// <summary>
        /// Gets the list of executables found within a Zero-Config MSI file.
        /// </summary>
        private ProcessObject[]? DefaultMsiExecutablesList { get; }

        /// <summary>
        /// Gets whether this deployment session has finished processing the Zero-Config MSI file detection.
        /// </summary>
        private bool ZeroConfigInitiated { get; }

        /// <summary>
        /// Gets whether this deployment session was instantiated via a script or the command line.
        /// </summary>
        private bool RunspaceOrigin { get; }

        /// <summary>
        /// Gets whether this deployment session should force WIM file detection, even if AppName was defined.
        /// </summary>
        private bool ForceWimDetection { get; }

        /// <summary>
        /// Gets the drive letter used with subst during a Zero-Config WIM file mount operation.
        /// </summary>
        private string? DirFilesSubstDrive { get; }

        /// <summary>
        /// Gets the base registry path used for getting/setting deferral information.
        /// </summary>
        private string RegKeyDeferBase { get; }

        /// <summary>
        /// Gets the registry path used for getting/setting deferral information.
        /// </summary>
        private string RegKeyDeferHistory { get; }

        /// <summary>
        /// Gets whether this deployment session is in non-interactive mode.
        /// </summary>
        private bool DeployModeNonInteractive { get; }

        /// <summary>
        /// Gets whether this deployment session is in silent mode.
        /// </summary>
        private bool DeployModeSilent { get; }

        /// <summary>
        /// Gets the deployment session's filesystem log path.
        /// </summary>
        private string LogPath { get; }

        /// <summary>
        /// Gets the deployment session's closing exit code.
        /// </summary>
        private int ExitCode { get; set; }


        #endregion
        #region Frontend parameters.


        /// <summary>
        /// Gets the deployment session's deployment type.
        /// </summary>
        public string DeploymentType { get; } = "Install";

        /// <summary>
        /// Gets the deployment type name from the language string table for the given DeploymentType.
        /// </summary>
        public string DeploymentTypeName { get; }

        /// <summary>
        /// Gets the deployment session's deployment mode.
        /// </summary>
        public string DeployMode { get; } = "Interactive";

        /// <summary>
        /// Gets whether this deployment session is allowed to exit with a reboot exit code.
        /// </summary>
        public bool AllowRebootPassThru { get; }

        /// <summary>
        /// Gets whether this deployment session should enable terminal services install mode.
        /// </summary>
        public bool TerminalServerMode { get; }

        /// <summary>
        /// Gets whether this deployment session should disable logging for the operation.
        /// </summary>
        public bool DisableLogging { get; }


        #endregion
        #region Frontend variables.


        /// <summary>
        /// Gets the deployment session's application vendor.
        /// </summary>
        public string? AppVendor { get; }

        /// <summary>
        /// Gets the deployment session's application name.
        /// </summary>
        public string? AppName { get; }

        /// <summary>
        /// Gets the deployment session's application version.
        /// </summary>
        public string? AppVersion { get; }

        /// <summary>
        /// Gets the deployment session's application architecture.
        /// </summary>
        public string? AppArch { get; }

        /// <summary>
        /// Gets the deployment session's application language.
        /// </summary>
        public string? AppLang { get; }

        /// <summary>
        /// Gets the deployment session's application package revision.
        /// </summary>
        public string? AppRevision { get; }

        /// <summary>
        /// Gets the deployment session's exit code(s) to indicate a successful deployment.
        /// </summary>
        public int[] AppSuccessExitCodes { get; } = [0];

        /// <summary>
        /// Gets the deployment session's exit code(s) to indicate a reboot is required.
        /// </summary>
        public int[] AppRebootExitCodes { get; } = [1641, 3010];

        /// <summary>
        /// Gets the deployment session's application package version.
        /// </summary>
        public Version? AppScriptVersion { get; }

        /// <summary>
        /// Gets the deployment session's application package date.
        /// </summary>
        public DateTime? AppScriptDate { get; }

        /// <summary>
        /// Gets the deployment session's application package author.
        /// </summary>
        public string? AppScriptAuthor { get; }

        /// <summary>
        /// Gets an override to the deployment session's installation name.
        /// </summary>
        public string InstallName { get; }

        /// <summary>
        /// Gets an override to the deployment session's installation title.
        /// </summary>
        public string InstallTitle { get; }

        /// <summary>
        /// Gets the deployment session's frontend script name.
        /// </summary>
        public string? DeployAppScriptFriendlyName { get; }

        /// <summary>
        /// Gets the deployment session's frontend script version.
        /// </summary>
        public Version? DeployAppScriptVersion { get; }

        /// <summary>
        /// Gets the deployment session's frontend script parameters.
        /// </summary>
        public IDictionary? DeployAppScriptParameters { get; }

        /// <summary>
        /// Gets/sets the deployment session's installation phase'.
        /// </summary>
        public string InstallPhase { get; set; } = "Initialization";


        #endregion
        #region Other public variables.


        /// <summary>
        /// Gets the deployment session's starting date and time.
        /// </summary>
        public readonly DateTime CurrentDateTime = DateTime.Now;

        /// <summary>
        /// Gets the deployment session's starting date as a string.
        /// </summary>
        public string CurrentDate { get; }

        /// <summary>
        /// Gets the deployment session's starting time as a string.
        /// </summary>
        public string CurrentTime { get; }

        /// <summary>
        /// Gets the deployment session's UTC offset from GMT 0.
        /// </summary>
        public readonly TimeSpan CurrentTimeZoneBias = TimeZoneInfo.Local.BaseUtcOffset;

        /// <summary>
        /// Gets the script directory of the caller.
        /// </summary>
        public string? ScriptDirectory { get; }

        /// <summary>
        /// Gets the specified or determined path to the Files folder.
        /// </summary>
        public string? DirFiles { get; }

        /// <summary>
        /// Gets the specified or determined path to the SupportFiles folder.
        /// </summary>
        public string? DirSupportFiles { get; }

        /// <summary>
        /// Gets the deployment session's Zero-Config MSI file path.
        /// </summary>
        public string? DefaultMsiFile { get; }

        /// <summary>
        /// Gets the deployment session's Zero-Config MST file path.
        /// </summary>
        public string? DefaultMstFile { get; }

        /// <summary>
        /// Gets the deployment session's Zero-Config MSP file paths.
        /// </summary>
        public string[]? DefaultMspFiles { get; }

        /// <summary>
        /// Gets whether this deployment session found a valid Zero-Config MSI file.
        /// </summary>
        private bool UseDefaultMsi { get; }

        /// <summary>
        /// Gets the deployment session's Zero-Config MSP file paths.
        /// </summary>
        public string LogTempFolder { get; }

        /// <summary>
        /// Gets the deployment session's log filename.
        /// </summary>
        public string LogName { get; }


        #endregion
    }
}