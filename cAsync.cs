using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComercialSF_APILinker
{
    public class cAsync
    {
        public int id_client { get; set; }
        public int id_profile { get; set; }
        public bool ExecuteModPre { get; set; }
        public bool ExecuteModStatus { get; set; }
        public bool ExecuteModPost { get; set; }
    }
}
