using DDC.Helpers;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Console.WriteLine(false);
                return;
            }
            var res = DebugFileChecker.IsInDebugMode(args[0]);
            Console.WriteLine(res);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}