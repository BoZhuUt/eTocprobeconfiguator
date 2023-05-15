using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTocprobeconfiguator.Para
{
    class ProbeRegister
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private uint addr;

        public uint Addr
        {
            get { return addr; }
            set { addr = value; }
        }
        private uint size;

        public uint Size
        {
            get { return size; }
            set { size = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string value = "0";

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public ProbeRegister()
        {

        }
        public ProbeRegister(string regName, uint regAddr, uint regSize, string regType)
        {
            name = regName;
            addr = regAddr;
            size = regSize;
            type = regType;
        }
    }
    class ProbeReegisterChange
    {
        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
