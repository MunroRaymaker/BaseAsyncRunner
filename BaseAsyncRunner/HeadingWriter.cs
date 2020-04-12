using CommandLine.Text;
using System;

namespace BaseAsyncRunner
{
    public class HeadingWriter
    {
        public static void Info()
        {
            Console.WriteLine(HeadingInfo.Default);
        }

        public static void Copyright()
        {
            Console.WriteLine(CopyrightInfo.Default);
        }

        public static void Banner(string environment)
        {
            Console.WriteLine("╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                     ║");
            Console.WriteLine("║                Tail Number Loader                   ║");
            Console.WriteLine("║                                                     ║");
            Console.WriteLine("╠═════════════════════════════════════════════════════╣");
            Console.WriteLine($"║ Environment: {environment.PadRight(39, ' ')}║");
            Console.WriteLine("╚═════════════════════════════════════════════════════╝");
        }
    }
}