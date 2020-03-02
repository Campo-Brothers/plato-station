namespace Plato_ServerStat.Cpu
{
    public class CoreMetrics
    {
        public CoreMetrics(string core, double user, double nice, double system, double ioewait, double steal, double idle)
        {
            Core = core;
            User = user;
            Nice = nice;
            System = system;
            IOWait = ioewait;
            Steal = steal;
            Idle = idle;
        }

        public string Core { get; }
        public double User { get; }
        public double Nice { get; }
        public double System { get; }
        public double IOWait { get; }
        public double Steal { get; }
        public double Idle { get; }

    }
}
