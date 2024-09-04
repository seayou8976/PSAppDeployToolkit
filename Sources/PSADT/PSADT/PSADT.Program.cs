using System;
using System.Linq;
using PSADT.PInvoke;
using PSADT.ConsoleEx;
using PSADT.ProcessEx;
using PSADT.CommandLine;
using System.Threading.Tasks;

namespace PSADT
{
    /// <summary>
    /// The main entry point class for the PSADT application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        /// <returns>A task representing the asynchronous operation, with the exit code as the result.</returns>
        public static async Task<int> Main(string[] args)
        {
            try
            {
                ConsoleHelper.IsHelpMode = args.Any(arg => new[] { "-Help", "--Help", "-?", "--?" }.Contains(arg, StringComparer.OrdinalIgnoreCase));
                if (ConsoleHelper.IsHelpMode)
                {
                    NativeMethods.AllocConsole();
                    DisplayHelp();
                    return 0;
                }

                ConsoleHelper.IsDebugMode = args.Any(arg => new[] { "-Debug", "--Debug", "-b", "--b" }.Contains(arg, StringComparer.OrdinalIgnoreCase));
                if (ConsoleHelper.IsDebugMode)
                {
                    // In debug mode, we AttachConsole instead of AllocConsole so we can more easily see and capture log output.
                    NativeMethods.AttachConsole(NativeMethods.ATTACH_PARENT_PROCESS);
                    ConsoleHelper.DebugWrite("Debug mode enabled. Attached to parent process console.", MessageType.Info);
                }

                var options = Arguments.Parse<LaunchOptions>(args);
                ConsoleHelper.DebugWrite($"Command-line options were successfully parsed.", MessageType.Info);

                ConsoleHelper.DebugWrite($"Initial command-line FilePath property [{options.FilePath}].", MessageType.Debug);
                ConsoleHelper.DebugWrite($"Initial command-line ArgumentList property [{String.Join(", ", options.ArgumentList)}].", MessageType.Debug);

                var launcher = new StartProcess();
                return await launcher.ExecuteAndMonitorAsync(options);
            }
            catch (Exception ex)
            {
                ConsoleHelper.DebugWrite("An error occurred during execution.", MessageType.Error, ex);
                return 1;
            }
            finally
            {
                if (ConsoleHelper.IsDebugMode || ConsoleHelper.IsHelpMode)
                {
                    await ConsoleHelper.ReadLineWithTimeout(TimeSpan.FromSeconds(30));
                    NativeMethods.FreeConsole();
                }
            }
        }

        /// <summary>
        /// Displays the help information for the application.
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("PsadtExec - Launch processes in specific Windows sessions");
            Console.WriteLine("");
            Console.WriteLine("Usage: PsadtExec.exe [options]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -f  , --FilePath <path>                 Path to the executable or script to run");
            Console.WriteLine("  -l  , --ArgumentList <args>             Arguments to pass to the executable or script");
            Console.WriteLine("  -dir, --WorkingDirectory <path>         Set the working directory for the process");
            Console.WriteLine("  -h  , --HideWindow                      Hide the process window");
            Console.WriteLine("  -pus, --PrimaryActiveUserSession        Run the process in all active user sessions");
            Console.WriteLine("  -sid, --SessionId <id>                  Specific session ID to run the process in");
            Console.WriteLine("  -aus, --AllActiveUserSessions           Run the process in all active user sessions");
            Console.WriteLine("  -adm, --UseLinkedAdminToken             Use the linked admin token when available");
            Console.WriteLine("  -pxp, --PsExecutionPolicy <policy>      Set PowerShell execution policy (default: RemoteSigned)");
            Console.WriteLine("  -bxp, --BypassPsExecutionPolicy         Bypass PowerShell execution policy");
            Console.WriteLine("  -ext, --SuccessExitCodes <codes>        List of exit codes considered successful (default: 0,3010)");
            Console.WriteLine("  -con, --ConsoleTimeoutInSeconds <secs>  Timeout for console operations (default: 30 seconds)");
            Console.WriteLine("  -red, --RedirectOutput                  Redirect process output (default: true)");
            Console.WriteLine("  -out, --OutputDirectory <path>          Directory to save redirected output");
            Console.WriteLine("  -mrg, --MergeStdErrAndStdOut            Merge stderr into stdout");
            Console.WriteLine("  -trm, --TerminateOnTimeout              Terminate process on timeout (default: true)");
            Console.WriteLine("  -iev, --InheritEnvironmentVariables     Inherit environment variables from the parent process");
            Console.WriteLine("  -e  , --Env <KEY=VALUE>                 Add an environment variable to the process");
            Console.WriteLine("  -w  , --Wait                            Wait for the process to exit before continuing");
            Console.WriteLine("  -wt , --WaitType <option>               Options for waiting on processes (WaitForAny/WaitForAll, default: WaitForAny)");
            Console.WriteLine("  -v  , --Verbose                         Enable verbose output");
            Console.WriteLine("  -d  , --Debug                           Run in debug mode");
            Console.WriteLine("  -?  , --Help                            Display this help message");
            Console.WriteLine("");
            Console.WriteLine("Process Creation Flags:");
            foreach (var mapping in Arguments._creationFlagMappings)
            {
                Console.WriteLine($"  --{mapping.LongName} (or -{mapping.ShortName})");
            }
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("  PsadtExec.exe -f notepad.exe -sid 1");
            Console.WriteLine("  PsadtExec.exe -f powershell.exe -l \"-File C:\\Scripts\\MyScript.ps1\" -aus");
            Console.WriteLine("  PsadtExec.exe -f cmd.exe -l \"/c echo Hello World > C:\\output.txt\" -w");
            Console.WriteLine("  PsadtExec.exe -f myapp.exe --Env MY_VAR=MY_VALUE --Env ANOTHER_VAR=ANOTHER_VALUE");
            Console.WriteLine("  PsadtExec.exe -f myapp.exe -adm --WaitOptions WaitForAll");
            Console.WriteLine("  PsadtExec.exe -f myapp.exe --newconsole");
        }
    }
}