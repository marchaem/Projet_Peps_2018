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
            DateTime dateDebut = new DateTime(1900,01,01);
            DateTime dateFin = DateTime.Today;
            RecupData data = new RecupData(dateDebut, dateFin);
            DateTime debut = new DateTime(2014, 12, 18);
            DateTime fin = new DateTime(2022, 12, 18);
            data.Fetch();
            //Console.WriteLine(data.DateToDouble(debut, new DateTime(2015, 07, 6), fin));
            /*Dictionary<DateTime,double> dico = data.getData("^STOXX50E");
            List<string> symbols = data.getSymbols();
            foreach (string s in symbols)
            {
                data.PrintData(s);
            }*/
            //Console.WriteLine(data.DoubleToDate(debut,3.0,fin));
            //data.GetClosestData(new DateTime(2015,4,25));
            double[,] past = data.exportPast(3.0,7,debut,fin);
            //double[] vol = data.exportVol();
            //double[,] cor = data.exportCor();
            //double[,] cov = data.exportCov();
            Console.WriteLine("Fin du programme ...");
            Console.ReadLine();
            //data.deleteFiles();
        }
    }   
}
