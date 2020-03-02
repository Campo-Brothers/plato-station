using Plato_ServerStat.Cpu;
using Plato_ServerStat.Memory;

namespace Plato_ServerStat.System
{
    public class SystemStatusController
    {
        public static SystemMetrics GetMetrics()
        {
            var cpu = CpuMetricsController.GetMetrics();
            var memory = MemoryMetricsController.GetMetrics();
            
            return new SystemMetrics(cpu, memory);
        }
    }
}
