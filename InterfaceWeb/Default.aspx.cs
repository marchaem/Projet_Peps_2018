using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wrapper;

namespace InterfaceWeb
{
    public partial class _Default : Page
    {


        

        protected void Button1_Click(object sender, EventArgs e)
        {
            WrapperClass wc = new WrapperClass();
            Label1.Text = wc.getZero().ToString();
        }
    }
}