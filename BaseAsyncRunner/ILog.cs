using System;

namespace BaseAsyncRunner
{
    public interface ILog
    {
        bool Verbose { get; set; }
        void Write(string message);
    }

    public class Log : ILog
    {
        public bool Verbose { get; set; }
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}