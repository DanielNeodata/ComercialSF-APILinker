using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComercialSF_APILinker
{
    public class cPost
    {
        public int id_client { get; set; }
        public int id_profile { get; set; }
        public string fullpath { get; set; }
        public string filename { get; set; }
        public string base64String { get; set; }
        public DateTime created { get; set; }
    }
}
