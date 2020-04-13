using CommandLine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAsyncRunner
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            using (var bootstrapper = new Bootstrapper())
            {
                // Will throw aggregate exception unless we unwrap exception
                var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args)
                    .MapResult(bootstrapper.App.Run, HandleParseError).GetAwaiter().GetResult();

                return Task.FromResult(result);
            }
        }

        static Task<int> HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            var result = -2;
            var errors = errs as Error[] ?? errs.ToArray();
            if (errors.IsHelp() || errors.IsVersion())
                result = -1;
            return Task.FromResult(result);
        }
    }
}
