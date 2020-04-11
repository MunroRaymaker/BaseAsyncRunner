using CommandLine;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BaseAsyncRunner.Domain;
using CommandLine.Text;

namespace BaseAsyncRunner
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            Console.WriteLine(HeadingInfo.Default);
            Console.WriteLine(CopyrightInfo.Default);

            var container = new StandardKernel(new ConfigureContainer());
            var app = container.Get<ApplicationLogic>();

            //Type[] types = { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) };

            //or collect types using reflection /plugins /Ioc container
            //var cmdAssemblies = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();

            //var result = Parser.Default.ParseArguments(args, typeof(HeadOptions), typeof(TailOptions))
            //    .MapResult(app.Run, HandleParseError).Result;

            // will throw aggregate exception unless we unwrap

            var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args)
                .MapResult(app.Run, HandleParseError).GetAwaiter().GetResult();

            //var result = Parser.Default.ParseArguments<HeadOptions>(args)
            //    .MapResult(async o => await app.Run(o), HandleParseError).Result;

            Console.WriteLine("ExitCode=" + result);
            return Task.FromResult(result);
        }

        static Task<int> HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            var result = -2;
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;
            return Task.FromResult(result);
        }
    }

    public class ApplicationLogic
    {
        private readonly ILog log;
        private readonly IConfig config;
        private readonly IMapper mapper;

        public ApplicationLogic(ILog log, IConfig config, IMapper mapper)
        {
            this.log = log;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<int> Run(object options)
        {
            int result = 0;
            this.log.Write("Quick Start Example!");
            var builder = new StringBuilder("Reading ");
            try
            {
                await Task.Delay(100);

                switch (options)
                {
                    case HeadOptions head:

                        builder.Append(head.Lines ?? head.Bytes).AppendFormat("{0} from top", head.Lines.HasValue ? "lines" : "bytes");
                        builder.Append(Environment.NewLine);
                        builder.Append(head.Lines.HasValue
                            ? ReadLines(head.FileName, true, (int)head.Lines)
                            : ReadBytes(head.FileName, true, (int)head.Bytes));

                        break;

                    case TailOptions tail:

                        builder.Append(tail.Lines ?? tail.Bytes).AppendFormat("{0} from top", tail.Lines.HasValue ? "lines" : "bytes");
                        builder.Append(Environment.NewLine);
                        builder.Append(tail.Lines.HasValue
                            ? ReadLines(tail.FileName, false, (int) tail.Lines)
                            : ReadBytes(tail.FileName, false, (int) tail.Bytes));

                        break;
                }

                if (builder.Length > 0) Console.WriteLine(builder.ToString());
                
                //int i = 0;
                //int j = 100 / i;

                var cut = new Customer { FirstName = "Jens", LastName = "Nielsen" };
                var dto = this.mapper.Map<CustomerDto>(cut);
                Console.WriteLine("Mapped to dto: " + dto.FullName);
            }
            catch (Exception ex)
            {
                this.log.Write("Oops " + ex.Message);
                return -2;
            }

            return result;
        }

        private static string ReadLines(string fileName, bool fromTop, int count)
        {
            var lines = File.ReadAllLines(fileName);
            if (fromTop)
            {
                return string.Join(Environment.NewLine, lines.Take(count));
            }
            return string.Join(Environment.NewLine, lines.Reverse().Take(count));
        }

        private static string ReadBytes(string fileName, bool fromTop, int count)
        {
            var bytes = File.ReadAllBytes(fileName);
            if (fromTop)
            {
                return Encoding.UTF8.GetString(bytes, 0, count);
            }
            return Encoding.UTF8.GetString(bytes, bytes.Length - count, count);
        }
    }

}
