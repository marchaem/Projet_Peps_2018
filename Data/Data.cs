using System;
using System.Collections.Generic;
using System.IO;
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
            DateTime finProduit = new DateTime(2022, 12, 22);
            RecupData data = new RecupData(dateDebut, dateFin);
            data.Fetch();
            //data.exportPast(1,7,dateDebut,new DateTime(2022,12,22));
            //data.exportCov(new DateTime(2014, 12, 22), new DateTime(2015, 5, 22));
            //double[] euro = data.GetEurostralHisto(dateDebut, 28, DateTime.Today);
            //data.Restreindre(new DateTime(2005,01,01), new DateTime(2005, 12, 31));
            //double[,] cov = data.exportCov(new DateTime(2007, 01, 01), new DateTime(2007, 12, 31));
            //double[] vol = data.exportVol();
            //double[,] past = data.exportPast(2.5, 182, new DateTime(2014, 12, 18), DateTime.Today);
            Stock stock = new Stock(data);
            double[] deltas = new double[5];
            int taille = 1500;
            for (int i=0; i<taille; i++)
            {              
                for (int j=0; j<5; j++)
                {
                    deltas[j] = random.NextDouble()*2 - 1;
                }
                if (i!=0)
                {
                    prix = prix + 2*random.NextDouble()-1;
                } else
                {
                    prix = 100 * random.NextDouble();
                }
                tracking_error = random.NextDouble() * 5 + 2.5;
                stock.Add(i*data.DateToDouble(dateDebut, DateTime.Today,finProduit) / taille, deltas, prix, tracking_error);
            }
            stock.SaveToCSV();
            stock.remove(0.0);
            stock.SaveToCSV();
            Stock stockRead = new Stock("histo.csv", data);
            stockRead.print();
            Console.WriteLine("Fin du test ...");
            Console.ReadLine();
        }
    }   
}
