using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercialSF_APILinker
{
    public class cMain
    {
        static Form1 frmMain;

        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmMain = new Form1();
            Application.Run(frmMain);
        }

    }
}
