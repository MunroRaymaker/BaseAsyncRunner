using AutoMapper;
using BaseAsyncRunner.Domain;
using CommandLine;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAsyncRunner
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            var container = new StandardKernel(new ConfigureContainer());
            var app = container.Get<ApplicationLogic>();

            // Can collect types using reflection /plugins /Ioc container
            //var cmdAssemblies = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
            //var result = Parser.Default.ParseArguments(args, cmdAssemblies)
            //    .MapResult(app.Run, HandleParseError).Result;

            // Will throw aggregate exception unless we unwrap exception
            var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args)
                .MapResult(app.Run, HandleParseError).GetAwaiter().GetResult();

            Console.WriteLine("ExitCode=" + result);
            return Task.FromResult(result);
        }

        static Task<int> HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            var result = -2;
            if (errs.IsHelp() || errs.IsVersion())
                result = -1;
            return Task.FromResult(result);
        }
    }

    public class ApplicationLogic
    {
        private readonly ILog log;
        private readonly IConfig config;
        private readonly IMapper mapper;

        // Instantiates a Singleton of the Semaphore with a value of 1.
        // This means that only one thread can be granted access at a time.
        private static readonly SemaphoreSlim mutex = new SemaphoreSlim(1, 1);

        public ApplicationLogic(ILog log, IConfig config, IMapper mapper)
        {
            this.log = log;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<int> Run(object options)
        {
            HeadingWriter.Banner(((IOptions)options)?.Environment);
            HeadingWriter.Info();
            HeadingWriter.Copyright();

            // Async await to enter the semaphore. If no-one has been granted access to the semaphore,
            // code execution will proceed, otherwise this thread will wait here until the semaphore
            // is released.
            await mutex.WaitAsync();

            int result = 0;
            this.log.Write("Quick Start Example!");
            var builder = new StringBuilder("Reading ");
            try
            {
                await Task.Delay(100);

                switch (options)
                {
                    case HeadOptions head:

                        builder.Append(head.Lines ?? head.Bytes)
                            .AppendFormat("{0} from top", head.Lines.HasValue ? " lines" : "bytes");
                        builder.Append(Environment.NewLine);
                        builder.Append(head.Lines.HasValue
                            ? ReadLines(head.FileName, true, (int)head.Lines)
                            : ReadBytes(head.FileName, true, (int)head.Bytes));

                        break;

                    case TailOptions tail:

                        builder.Append(tail.Lines ?? tail.Bytes)
                            .AppendFormat("{0} from top", tail.Lines.HasValue ? " lines" : "bytes");
                        builder.Append(Environment.NewLine);
                        builder.Append(tail.Lines.HasValue
                            ? ReadLines(tail.FileName, false, (int)tail.Lines)
                            : ReadBytes(tail.FileName, false, (int)tail.Bytes));

                        break;
                }

                if (builder.Length > 0) Console.WriteLine(builder.ToString());

                //int i = 0;
                //int j = 100 / i;

                var cut = new Customer { FirstName = "John", LastName = "Smith" };
                var dto = this.mapper.Map<CustomerDto>(cut);
                Console.WriteLine("Mapped to dto: " + dto.FullName);
            }
            catch (Exception ex)
            {
                this.log.Write("Oops " + ex.Message);
                return -2;
            }
            finally
            {
                // When the task is ready release the semaphore, or it will lock forever.
                mutex.Release();
            }

            return result;
        }

        private static string ReadLines(string fileName, bool fromTop, int count)
        {
            var lines = File.ReadAllLines(fileName);
            return string.Join(Environment.NewLine, fromTop ? lines.Take(count) : lines.Reverse().Take(count));
        }

        private static string ReadBytes(string fileName, bool fromTop, int count)
        {
            var bytes = File.ReadAllBytes(fileName);
            return fromTop ? Encoding.UTF8.GetString(bytes, 0, count) : Encoding.UTF8.GetString(bytes, bytes.Length - count, count);
        }
    }
}
