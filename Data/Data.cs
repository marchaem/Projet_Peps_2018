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
            data.Fetch();
            //data.Restreindre(new DateTime(2005,01,01), new DateTime(2005, 12, 31));
            //double[,] cov = data.exportCov(new DateTime(2007, 01, 01), new DateTime(2007, 12, 31));
            Console.WriteLine("Fin du programme ...");
            Console.ReadLine();
        }
    }   
}
