using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class _Default : Page
    {

        [DllImport("Pricer.dll", SetLastError = true)]
        public static extern double price();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //WrapperClass wc = new WrapperClass();
            //Label1.Text = wc.getPrice().ToString();
            Console.WriteLine("appuie sur le bouton");
        }
    }
}