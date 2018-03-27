using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data
{
    public class DataPricer
    {
        static void Main()
        {
            DateTime dateDebut = new DateTime(2015,01,01);
            DateTime dateFin = DateTime.Today;
            RecupData data = new RecupData(dateDebut, dateFin);
            data.Fetch();
            double[] euro = data.GetEurostralHisto(dateDebut, 28*3, DateTime.Today);
            //data.Restreindre(new DateTime(2005,01,01), new DateTime(2005, 12, 31));
            //double[,] cov = data.exportCov(new DateTime(2007, 01, 01), new DateTime(2007, 12, 31));
            //double[] vol = data.exportVol();
            //double[,] past = data.exportPast(2.5, 182, new DateTime(2014, 12, 18), DateTime.Today);
            Console.WriteLine("Fin du programme ...");
            Console.ReadLine();
        }
    }   
}
