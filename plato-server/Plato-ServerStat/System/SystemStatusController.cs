using Plato.System.Stats.Cpu;
using Plato.System.Stats.Memory;

namespace Plato.System.Stats.System
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
