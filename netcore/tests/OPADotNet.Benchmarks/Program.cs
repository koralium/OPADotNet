using BenchmarkDotNet.Running;
using System;

namespace OPADotNet.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
