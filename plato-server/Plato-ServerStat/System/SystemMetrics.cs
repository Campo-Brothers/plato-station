using Plato_ServerStat.Cpu;
using Plato_ServerStat.Memory;

namespace Plato_ServerStat.System
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
