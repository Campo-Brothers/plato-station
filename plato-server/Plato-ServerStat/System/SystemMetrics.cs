using Plato.System.Stats.Cpu;
using Plato.System.Stats.Memory;

namespace Plato.System.Stats.System
{
    public class SystemMetrics
    {
        public SystemMetrics(CpuMetrics cpu, MemoryMetrics memory)
        {
            Cpu = cpu;
            Memory = memory;
        }

        public CpuMetrics Cpu { get; }

        public MemoryMetrics Memory { get; }
    }
}
