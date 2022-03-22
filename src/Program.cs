using CommandLine;
using DDC.Helpers;
using System.Drawing;
using Console = Colorful.Console;

class Program
{
    public class Options
    {
        [Option('p', "path", Required = false, HelpText = "Directory to check")]
        public string? Path { get; set; }
        [Option('a', "all", Required = false, HelpText = "Show all dotnet dll information")]
        public bool ShowAll { get; set; } = false;
    }

    static void Main(string[] args)
    {
        Options options = new();
        _ = Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       o.Path = Path.Combine(Environment.CurrentDirectory, o.Path ?? "");
                       options = o;
                   });

        int debugCount = 0;
        int releaseCount = 0;
        var res = DebugFileChecker.Check(options.Path);

        Console.WriteLine($"Working Directory:{options.Path}", Color.Gray);

        foreach (var (Filename, IsDebug) in res)
        {
            if (IsDebug)
            {
                debugCount++;
                Console.WriteLine($"{Filename} Debug", Color.Red);
            }
            else
            {
                if (options.ShowAll == true)
                    Console.WriteLine($"{Filename} Release", Color.Green);
                releaseCount++;
            }
        }

        Console.WriteLine($"Debug: {debugCount}", Color.Red);
        Console.WriteLine($"Release: {releaseCount}", Color.Green);
    }
}