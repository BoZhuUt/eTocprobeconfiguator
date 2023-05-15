using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTocprobeconfiguator.Para
{
    public class ProbeTestData
    {
        public List<List<ProbesItem>> Probes { get; set; }
    }
    public class ProbesItem
    {
        public string name { get; set; }
        public uint pn { get; set; }
        public string instrument { get; set; }
        public string TestItem { get; set; }
        public string AllowableError { get; set; }
        public List<AllowableErrorsListItem> AllowableErrors { get; set; }
        public List<RegInfoItem> Data { get; set; }
        public List<ReadingItem> Reading { get; set; }
        public List<RegInfoItem> Current { get; set; }
    }
    public class ReadingItem
    {
        public string measure { get; set; }
        public RegInfoItem signal { get; set; }
        public List<RegInfoItem> para { get; set; }
    }
    public class AllowableErrorsListItem
    {
        public string Measure { get; set; }
        public List<ErrorsListItem> ErrorsList { get; set; }
    }
    public class ErrorsListItem
    {
        public string SolutionNumber { get; set; }
        public string Range { get; set; }
    }
    public class RegInfoItem
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
        private double _NumConversion = default;

        public double NumConversion
        {
            get { return _NumConversion; }
            set { _NumConversion = value; }
        }
        private string _unit = "";

        public string unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public RegInfoItem()
        {

        }
        public RegInfoItem(string Name, ushort Addr, byte Size, string Type)
        {
            name = Name;
            addr = Addr;
            size = Size;
            type = Type;
        }
    }
}
