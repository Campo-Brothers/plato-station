using System;
using System.Diagnostics;

namespace Plato_ServerStat.Cpu
{
    public class CpuMetricsController
    {
        public static CpuMetrics GetMetrics()
        {
            var output = "";
            var cpuMetrics = new CpuMetrics();

            var info = new ProcessStartInfo("mpstat -P ALL");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"mpstat -P ALL\"";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Split("\n");
            
            for(int line = 3; line <=7; line++)
            {
                var coreData = lines[line].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                cpuMetrics.AddCore(new CoreMetrics(
                    line == 3 ? "CPU" : $"core {coreData[1]}",
                    double.Parse(coreData[2]),
                    double.Parse(coreData[3]),
                    double.Parse(coreData[4]),
                    double.Parse(coreData[5]),
                    double.Parse(coreData[8]),
                    double.Parse(coreData[11])
                    ));
            }

            return cpuMetrics;
        }
    }
}
