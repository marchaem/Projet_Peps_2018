using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Wrapper;

namespace Web
{
    public partial class _Default : Page
    {

        //[DllImport("NativePricerConsole.dll", CharSet =CharSet.Unicode)]
        //public static extern double price();

        protected void Page_Load(object sender, EventArgs e)
        {

            
            /*
            //ds.ReadXml(Server.MapPath("~/XMLFile.xml"));
            Chart1.Titles.Add("Computer Result Analysis");
            

            Chart1.Series["Series1"].XValueMember = "name";
            Chart1.Series["Series1"].YValueMembers = "per";
            double[,] xh = new double[10,10];
            Chart1.DataSource = GetDonneesAleatoires(3, 100);
     
            Chart1.Series[0].IsValueShownAsLabel = true;
            Chart1.Series[0].Label = "#PERCENT";
            Chart1.Series[0].LegendText = "#AXISLABEL";

            this.Chart1.Legends.Add("Legend1");
            this.Chart1.Legends[0].Enabled = true;
            this.Chart1.Legends[0].Docking = Docking.Bottom;
            this.Chart1.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
            this.Chart1.Series[0].LegendText = "#VALX (#PERCENT)";
            */
        }

        private double[,] GetDonneesAleatoires(int NombreActifs, int NombreDates)
        {
            int nbVals = NombreDates;
            int nbPfe = NombreActifs;
            double[,] pfVals = new double[nbVals, nbPfe];
            Random rndGen = new Random();
            double[] prevValue = new double[] { 100.0, 100.0, 100.0 };
            for (int i = 0; i < nbVals; i++)
            {
                for (int j = 0; j < nbPfe; j++)
                {
                    pfVals[i, j] = prevValue[j] + 10 * rndGen.Next(-5, 5);
                    prevValue[j] = pfVals[i, j];
                }
            }
            return pfVals;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //WrapperClass wc = new WrapperClass();
            //Label1.Text = wc.getPrice().ToString();
            //Console.WriteLine("appuie sur le bouton");
            //Label1.Text = DateTime.Parse(from_date.Text).ToString();
            Label1.Text = Convert.ToDateTime(from_date.Text).ToString();
            Label2.Text = Convert.ToDateTime(from_date2.Text).ToString();
        }

        
    }
}