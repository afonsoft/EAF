using BenchmarkDotNet.Running;
using System;

namespace Eaf.Middleware.Application.Benchmarks
{
    /// <summary>
    /// Entry point for BenchmarkDotNet benchmarks.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly, args: args);
        }
    }
}
