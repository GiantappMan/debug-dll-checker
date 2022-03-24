using CommandLine;
using DDC.Helpers;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Console = Colorful.Console;

class Program
{
    private static string _runDir = string.Empty;
    public class Options
    {
        [Option('p', "path", Required = false, HelpText = "Directory to check")]
        public string? Path { get; set; }
        [Option('a', "all", Required = false, HelpText = "Show all dotnet dll information")]
        public bool ShowAll { get; set; } = false;
    }

    static void Main(string[] args)
    {
        _runDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        Options options = new();
        _ = Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       if (!Directory.Exists(o.Path))
                       {
                           o.Path = Path.Combine(Environment.CurrentDirectory, o.Path ?? "");
                       }
                       options = o;
                   });

        int debugCount = 0;
        int releaseCount = 0;
        int failedCount = 0;
        var res = Check(options.Path);

        Console.WriteLine($"Working Directory:{options.Path}", Color.Gray);
        Console.WriteLine($"Running Directory:{_runDir}", Color.Gray);

        foreach (var (Filename, IsDebug, Arch) in res)
        {
            if (IsDebug == true)
            {
                debugCount++;
                Console.WriteLine($"{Filename} Debug {Arch}", Color.Red);
            }
            else if (IsDebug == false)
            {
                if (options.ShowAll == true)
                    Console.WriteLine($"{Filename} Release {Arch}", Color.Green);
                releaseCount++;
            }
            else
            {
                failedCount++;
                Console.WriteLine($"{Filename} Failed {Arch}", Color.Yellow);
            }
        }

        Console.WriteLine($"Debug: {debugCount}", Color.Red);
        Console.WriteLine($"Release: {releaseCount}", Color.Green);
        Console.WriteLine($"Failed: {failedCount}", Color.Yellow);
    }
    internal static IEnumerable<(string Filename, bool? IsDebug, string Arch)> Check(string? path)
    {
        if (path == null)
            yield break;

        string[] searchExtensions = { ".dll", ".exe", ".lib" };
        var files = Directory
                    .GetFiles(path!, "*.*", SearchOption.AllDirectories)
                    .Where(file => searchExtensions.Any(ext => file.ToLower().Contains(ext)))
                    .ToList();
        foreach (var file in files)
        {
            if (!DebugFileChecker.IsAssembly(file))
                continue;

            bool? isDebug = null;
            string arch = "x64";
            try
            {
                if (DebugFileChecker.IsInDebugMode(file))
                {
                    isDebug = true;
                }
                else
                {
                    isDebug = false;
                }
            }
            catch (Exception)
            {

            }

            if (isDebug == null)
            {
                arch = "x86";
                //try x86
                Process p = new();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.Arguments = $"\"{file}\"";
                p.StartInfo.FileName = Path.Combine(_runDir, "ddc_x86/DDC_X86.exe");
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                bool isOk = bool.TryParse(output, out bool tmpIsDebug);
                if (isOk)
                    isDebug = tmpIsDebug;
            }

            yield return (file, isDebug, arch);
        }
    }

}