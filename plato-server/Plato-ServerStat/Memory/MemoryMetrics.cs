namespace Plato_ServerStat.Memory
{
    public class MemoryMetrics
    {
        public MemoryMetrics(double total, double used, double free)
        {
            Total = total;
            Used = used;
            Free = free;
        }

        public double Total { get; }
        public double Used { get; }
        public double Free { get; }
    }
}
