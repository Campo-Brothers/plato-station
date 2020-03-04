using System.Collections.Generic;

namespace Plato.System.Stats.Cpu
{
    public class CpuMetrics
    {
        private IList<CoreMetrics> coreMetrics;
        public CpuMetrics ()
        {
            coreMetrics = new List<CoreMetrics>();
        }

        public void AddCore(CoreMetrics core)
        {
            coreMetrics.Add(core);
        }

        public IEnumerable<CoreMetrics> Cores 
        {
            get { return coreMetrics; }
        }
    }
}
