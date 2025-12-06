using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComercialSF_APILinker
{
    public class cRespuesta
    {
        public string code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string table { get; set; }
        public string function { get; set; }
        public cProfile[] data { get; set; }
        public string[] items { get; set; }
        public string totalrecords { get; set; }
        public string totalpages { get; set; }
        public string page { get; set; }
    }
}
