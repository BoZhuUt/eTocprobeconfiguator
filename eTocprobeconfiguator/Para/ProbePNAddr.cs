using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTocprobeconfiguator.Para
{
    public class ProbePNAddr
    {
        public string Version { get; set; }
        public string Date { get; set; }
        public string Contact { get; set; }
        public List<ProbeItem> Probes { get; set; }
    }
    public class ProbeItem
    {
        public string name { get; set; }
        public uint PN { get; set; }
        public byte addr { get; set; }
    }
}
