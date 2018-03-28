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
            Random random = new Random();
            double prix = 0;
            double tracking_error = 0;
            DateTime dateDebut = new DateTime(2014,12,22);
            DateTime dateFin = DateTime.Today;
            RecupData data = new RecupData(dateDebut, dateFin);
            //data.Fetch();
            //data.exportPast(1,7,dateDebut,new DateTime(2022,12,22));
            //data.exportCov(new DateTime(2014, 12, 22), new DateTime(2015, 5, 22));
            //double[] euro = data.GetEurostralHisto(dateDebut, 28, DateTime.Today);
            //data.Restreindre(new DateTime(2005,01,01), new DateTime(2005, 12, 31));
            //double[,] cov = data.exportCov(new DateTime(2007, 01, 01), new DateTime(2007, 12, 31));
            //double[] vol = data.exportVol();
            //double[,] past = data.exportPast(2.5, 182, new DateTime(2014, 12, 18), DateTime.Today);
            Stock stock = new Stock();
            for (int i=0; i<1000; i++)
            {
                double[] deltas = new double[5];
                for (int j=0; j<5; j++)
                {
                    deltas[j] = random.NextDouble()*2 - 1;
                }
                prix = random.NextDouble() * 5 + 155;
                tracking_error = random.NextDouble() * 5 + 2.5;
                stock.Add(i*8.0 / 1000, deltas, prix, tracking_error);
            }
            stock.SaveToCSV();
            Stock stockRead = new Stock("histo.csv");
            stockRead.print();
            Console.WriteLine("Fin du test ...");
            Console.ReadLine();
        }
    }   
}
