using System;
using System.Diagnostics;

namespace Plato.System.Stats.Memory
{
    public class MemoryMetricsController
    {
        public static MemoryMetrics GetMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"free -m\"";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics(
                double.Parse(memory[1]), 
                double.Parse(memory[2]), 
                double.Parse(memory[3])
                );

            return metrics;
        }
    }
}
