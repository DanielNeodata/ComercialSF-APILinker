using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComercialSF_APILinker
{
    public class cProfile
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public DateTime created { get; set; }
        public DateTime verified { get; set; }
        public DateTime offline { get; set; }
        public DateTime fum { get; set; }
        public int id_user { get; set; }
        public int id_client { get; set; }
        public string unc_source { get; set; }
        public string unc_target { get; set; }
        public int mm_alive { get; set; }
        public int pdf { get; set; }
        public int tiff { get; set; }
        public int single_page { get; set; }
        public DateTime time_from { get; set; }
        public DateTime time_to { get; set; }
        public string sufix_subdirs { get; set; }
        public string db_source { get; set; }
        public string db_target { get; set; }
        public int mod_files { get; set; }
        public int mod_pages { get; set; }
        public int mod_status { get; set; }
        public string sql_status { get; set; }
        public string post_unc_source { get; set; }
        public string post_unc_target { get; set; }
        public int id_type_end { get; set; }
        public int mod_post { get; set; }
        public int mod_alert { get; set; }
        public DateTime alert_fixed_time { get; set; }
        public int alert_every_hours { get; set; }
        public DateTime alert_from_date { get; set; }
        public string alert_email { get; set; }
        public string unc_storage { get; set; }
        public int ttl_storage { get; set; }
        public int sla { get; set; }
		public string post_unc_bad { get; set; }
	}
}
