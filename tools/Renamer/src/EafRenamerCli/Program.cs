using Eaf.Renamer.Lib;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace EafRenamerCli
{
    public static class Program
    {
        private static string NL = Environment.NewLine; // shortcut
        private static string RED = "[red]";
        private static string GREEN = "[green]";
        private static string YELLOW = "[yellow]";
        private static string CYAN = "[cyan]";
        private static string GREY = "[grey]";
        private static string END = "[/]";

        private static ConsoleColor oldColor;

        private static bool isRunning = false;
        private static bool isRunningRename = false;
        private static bool isRunninginstall = false;

        private static string UI = "eaf-projectname-ui";
        private static string API = "Eaf.ProjectName";
        private static string DIR = Environment.CurrentDirectory;
        private static bool DL = false;
        private static bool DEPS = false;

        private static RenameTemplate template;

        public static void Main(string[] args)
        {
            oldColor = Console.ForegroundColor;

            EafLogo();

            if (args == null || args.Length <= 0)
            {
                Help();
                return;
            }
            var options = new Arguments(args);
            ProcessArgs(options);
        }

        private static void WriteLine(string value)
        {
            AnsiConsole.MarkupLine(value);
        }

        private static void Write(string value)
        {
            AnsiConsole.Markup(value);
        }

        private static void ProcessArgs(Arguments options)
        {
            ResetOptions();
            ReadOptions(options);
            PrintOptions();

            if (DL)
                WriteLine($"{CYAN}Download {END}{RED}NotImplementedException{END}");

            InitializeTemplate();
            RunTemplateWithStatus();
        }

        private static void ResetOptions()
        {
            UI = "eaf-projectname-ui";
            API = "Eaf.ProjectName";
            DIR = Environment.CurrentDirectory;
            DL = false;
            DEPS = false;
        }

        private static void ReadOptions(Arguments options)
        {
            if (!string.IsNullOrEmpty(options["u"]))
                UI = options["u"];
            if (!string.IsNullOrEmpty(options["i"]))
                API = options["i"];
            if (!string.IsNullOrEmpty(options["d"]))
                DIR = options["d"];
            if (!string.IsNullOrEmpty(options["dl"]))
                DL = true;
            if (!string.IsNullOrEmpty(options["deps"]))
                DEPS = true;
        }

        private static void PrintOptions()
        {
            WriteLine($"{NL}----------------------");
            WriteLine($"{YELLOW} /i:{END}{CYAN}{API}{END}");
            WriteLine($"{YELLOW} /u:{END}{CYAN}{UI}{END}");
            WriteLine($"{YELLOW} /d:{END}{CYAN}{DIR}{END}");
            if (DL)
                WriteLine($"{YELLOW} /dl{END}");
            if (DEPS)
                WriteLine($"{YELLOW} /deps{END}");
            WriteLine($"");
            Console.ForegroundColor = oldColor;
        }

        private static void InitializeTemplate()
        {
            template = new RenameTemplate();
            Eaf.Renamer.Lib.Arguments arg = new Eaf.Renamer.Lib.Arguments
            {
                ChangeAreaName = true,
                RootDir = DIR,
                NewProjectName = UI,
                NewCompanyName = API
            };

            isRunning = true;
            isRunningRename = true;
            isRunninginstall = true;
            template.Log += (_, e) => Template_Log(e);
            template.WorkerCompleted += Template_WorkerCompleted;
            template.ProgressChanged += Template_ProgressChanged;

            template.RunWorker(arg);
        }

        private static void RunTemplateWithStatus()
        {
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Circle)
                .SpinnerStyle(Style.Parse("yellow bold"))
                .Start("[yellow bold]Start process...[/]", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Circle);
                    ctx.SpinnerStyle(Style.Parse("yellow bold"));

                    while (isRunning)
                    {
                        Thread.Sleep(500);
                        ctx.Refresh();
                        while (isRunningRename)
                        {
                            ctx.Status = "[yellow bold]Rename files...[/]";
                            Thread.Sleep(1000);
                            ctx.Refresh();
                        }
                        while (isRunninginstall)
                        {
                            ctx.Status = "[yellow bold]Installing the dependencies..[/]";
                            Thread.Sleep(1000);
                            ctx.Refresh();
                        }
                    }
                });
        }

        private static void Template_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Console.ForegroundColor = oldColor;
        }

        private static void Template_WorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Console.ForegroundColor = oldColor;
            WriteLine($"");
            isRunningRename = false;
            Thread.Sleep(1000);
            if (e.Cancelled)
            {
                isRunning = false;
                isRunningRename = false;
                isRunninginstall = false;
                WriteLine($"{RED}Task cancelled{END}");
            }
            else
            {
                if (e.Error != null)
                {
                    isRunning = false;
                    isRunningRename = false;
                    isRunninginstall = false;
                    WriteLine($"{CYAN}Internal error{END} {RED}{e.Error.Message}{END}");
                    throw e.Error;
                }

                isRunning = true;
                isRunninginstall = true;
                isRunningRename = false;
                WriteLine($"");
                WriteLine($"{CYAN}Task terminated{END}");

                string pathUi = template.GetPathUI;
                string pathApi = template.GetPathAPI;
                WriteLine($"");
                WriteLine($"{CYAN}UI path {END}{YELLOW}'{pathUi}'{END}");
                WriteLine($"{CYAN}API path {END}{YELLOW}'{pathApi}'{END}");
                WriteLine($"");

                Thread.Sleep(1000);
                if (DEPS)
                {
                    RunCommand("npm install -g @angular/cli@12 --force", pathUi);
                    RunCommand("npm install -g npm@8.5.5 --force", pathUi);
                    RunCommand("npm install --legacy-peer-deps --force", pathUi, true);
                    Thread.Sleep(1000);
                    WriteLine($"");
                    WriteLine($"{CYAN}Task terminated{END}");
                    WriteLine($"");
                    WriteLine($"{CYAN}UI path {END}{YELLOW}'{pathUi}'{END}");
                    WriteLine($"{CYAN}API path {END}{YELLOW}'{pathApi}'{END}");
                    WriteLine($"");
                }
                isRunning = false;
                isRunninginstall = false;
            }

            Console.ForegroundColor = oldColor;
            WriteLine($"");

            Environment.Exit(0);
        }

        private static void Template_Log(string e)
        {
            Console.ForegroundColor = oldColor;
            Write(e);
        }

        #region help

        private static void Help()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            WriteLine($"{NL}----------------------");
            WriteLine($"{GREEN}Usage:{END}");
            WriteLine($"{RED}  Eaf.Cli {END}{CYAN}<options>{END}");
            WriteLine($"{NL}");

            Console.ForegroundColor = ConsoleColor.Gray;

            WriteLine($"{YELLOW}    options  {END}{GREY}  Usage                           {END}Info. {NL}");

            WriteLine($"{YELLOW}   /i: or -i:   {END}{GREY}<name of api>                 {END}{GREY}Rename from Eaf.ProjectName to name selected in API {END}");
            WriteLine($"{YELLOW}   /u: or -u:   {END}{GREY}<name of ui>                  {END}{GREY}Rename from eaf-projectname-ui to name selected in Angular {END}");
            WriteLine($"{YELLOW}   /d: or -d:   {END}{GREY}<path of project>             {END}{GREY}Directory where the template is or the location of the execution{END}");
            WriteLine($"{YELLOW}   /dl or -dl   {END}{GREY}                              {END}{GREY}For download templante and rename it {END}");
            WriteLine($"{YELLOW}   /dep or -dep {END}{GREY}                              {END}{GREY}For install Angular dependencies {END}");

            WriteLine($"{NL}");
            Console.ForegroundColor = oldColor;
        }

        #endregion help

        #region Logo

        private static void EafLogo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            WriteLine("");
            WriteLine("     ███████╗ █████╗ ███████╗ ");
            WriteLine("     ██╔════╝██╔══██╗██╔════╝ ");
            WriteLine("     █████╗  ███████║█████╗   ");
            WriteLine("     ██╔══╝  ██╔══██║██╔══╝   ");
            WriteLine("     ███████╗██║  ██║██║      ");
            WriteLine("     ╚══════╝╚═╝  ╚═╝╚═╝      ");
            WriteLine("");

            Console.ForegroundColor = oldColor;
            var versionString = Assembly.GetEntryAssembly()?
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                        .InformationalVersion
                                        .ToString();

            WriteLine($"{GREY}Version: {END}{CYAN}{versionString}{END}");
            WriteLine($"{GREY}Architecture {END}{YELLOW}{System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}{END}");
            WriteLine($"{GREY}Runtime Identifier: {END}{YELLOW}{System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier}{END}");
        }

        #endregion Logo

        private static void RunCommand(string commandToRun, string workingDirectory = null, bool output = false)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDirectory
            };

            var process = Process.Start(processStartInfo);

            if (process == null)
            {
                throw new InvalidOperationException("Process should not be null.");
            }

            process.StandardInput.WriteLine($"{commandToRun} & exit");
            WriteLine($"{YELLOW}command:[/] {GREEN}{commandToRun}[/]");
            if (output)
            {
                WriteLine("");
                Console.SetIn(process.StandardOutput);
            }
            process.WaitForExit();
            WriteLine("");
        }
    }
}