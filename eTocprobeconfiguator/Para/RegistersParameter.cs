using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTocprobeconfiguator
{
     class RegistersParameter
    {
        public ParameterTableFile ParameterTableFile { get; set; }
    }
    public class ParameterTableFile
    {
        public string id { get; set; }
        public string data { get; set; }
        public int major { get; set; }
        public int minor { get; set; }
        public Parameters Parameters { get; set; }
    }
    public class Parameters
    {
        public List<PTSA_SystemItem> ptsa_system { get; set; }
    }
    public class PTSA_SystemItem
    {
        public string name { get; set; }
        public ushort addr { get; set; }
        public byte size { get; set; }
        public string type { get; set; }
        private string Value = "0";
        public string value
        {
            get { return Value; }
            set { Value = value; }
        }
        public PTSA_SystemItem()
        {

        }
        public PTSA_SystemItem(string Name, ushort Addr, byte Size, string Type)
        {
            name = Name;
            addr = Addr;
            size = Size;
            type = Type;
        }
    }
}
