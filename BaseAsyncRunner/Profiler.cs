namespace BaseAsyncRunner
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Implements a simple profiler that logs the number of milliseconds between each log statement.
    /// </summary>
    public class Profiler
    {
        private readonly StringBuilder trace = new StringBuilder();
        private readonly Stopwatch watch = new Stopwatch();
        private TimeSpan total = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="Profiler"/> class.
        /// </summary>
        /// <param name="message">The initial message.</param>
        public Profiler(string message)
        {
            this.watch.Start();

            if (string.IsNullOrEmpty(message) == false)
            {
                this.trace.Append(message + "\r\n");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Profiler"/> class.
        /// </summary>
        public Profiler()
            : this(null)
        {
        }

        /// <summary>
        /// Logs the specified message together with a timestamp of the duration since last logging.
        /// </summary>
        /// <param name="message">The message.</param>
        public Profiler Log(string message)
        {
            this.watch.Stop();
            var split = this.watch.Elapsed;
            this.total = this.total.Add(split);
            this.trace.AppendFormat(CultureInfo.InvariantCulture, "{0:HH:mm:ss}: ({1} ms)", DateTime.Now, split.TotalMilliseconds);

            if (string.IsNullOrEmpty(message) == false)
            {
                this.trace.Append(": " + message + "\r\n");
            }
            else
            {
                this.trace.Append("\r\n");
            }

            this.watch.Restart();
            return this;
        }

        /// <summary>
        /// Stops the profiling and logs out the total processing time.
        /// </summary>
        public void Stop()
        {
            this.watch.Stop();
            var split = this.watch.Elapsed;
            this.total = this.total.Add(split);
            this.trace.AppendFormat(CultureInfo.InvariantCulture, "{0:HH:mm:ss}: Total processing time {1} ms", DateTime.Now, this.total.TotalMilliseconds);
        }

        /// <summary>
        /// Writes contents of profiler to stdout.
        /// </summary>
        public void Dump()
        {
            Console.WriteLine(this);
            this.trace.Clear();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.trace.ToString();
        }
    }
}
