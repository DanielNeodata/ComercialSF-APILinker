using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComercialSF_APILinker
{
    public class cStatus
    {
        public string id_file { get; set; }
        public int id_client { get; set; }
        public string filename { get; set; }
        public string status { get; set; }
        public DateTime created { get; set; }
    }
}
