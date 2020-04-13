using System;
using System.Reflection;

namespace BaseAsyncRunner
{
    public class HeadingWriter
    {
        public static void Banner(string environment)
        {
            Banner(environment, string.Empty, string.Empty, string.Empty, 55);
        }

        public static void Banner(string environment, string productName, string subTitle = "", string copyright = "", int windowWidth = 55, ConsoleColor foregroundColor = ConsoleColor.Gray)
        {
            if (string.IsNullOrEmpty(productName))
            {
                productName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            }

            Console.Title = $"{productName}{(string.IsNullOrEmpty(subTitle) ? "" : " -  " + subTitle)}";

            Console.ForegroundColor = foregroundColor;
            
            Console.WriteLine($"╔{new string('═', windowWidth - 2)}╗");
            Console.WriteLine($"║{new string(' ', windowWidth - 2)}║");
            Console.WriteLine(CenterText(productName, "║", windowWidth));
            if (!string.IsNullOrEmpty(subTitle))
            {
                Console.WriteLine(CenterText(subTitle, "║", windowWidth));
            }
            if (!string.IsNullOrEmpty(copyright))
            {
                Console.WriteLine(CenterText(copyright, "║", windowWidth));
            }
            Console.WriteLine($"║{new string(' ', windowWidth - 2)}║");
            Console.WriteLine($"╠{new string('═', windowWidth - 2)}╣");
            Console.WriteLine($"║ Environment: {environment?.PadRight(windowWidth - 16, ' ')}║");
            Console.WriteLine($"╚{new string('═', windowWidth - 2)}╝");
            Console.ResetColor();
        }

        private static string CenterText(string content, string decorationString = "",
            int windowWidth = 55)
        {
            int width = windowWidth - 2 * decorationString.Length;
            int contentRightAlign = width / 2 + content.Length / 2;
            int decorationRightAlign = width - contentRightAlign + decorationString.Length;
            //var format = decorationString +
            //             "{0," + contentRightAlign + "}" +
            //             "{1," + (width - contentRightAlign + decorationString.Length) + "}";

            //return string.Format(format, content, decorationString);

            return $"{decorationString}" +
                   $"{content.PadLeft(contentRightAlign)}" +
                   $"{decorationString.PadLeft(decorationRightAlign)}";
        }
    }
}