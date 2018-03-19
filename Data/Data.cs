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
            DateTime dateDebut = new DateTime(2010,1,1);
            DateTime dateFin = DateTime.Today;
            RecupData data = new RecupData(dateDebut, dateFin);
            data.RecupCSV(4);
            List<List<double>> test = data.ParseAll();
            List<double> cour;
            for (int i=0; i<data.Files.Count; i++)
            {
                cour = test[i];
                Console.WriteLine("Vol histo = " + Stats.volHisto(cour));
                Console.WriteLine("Vol std log = " + Stats.volStd(cour));
            }

            double[,] corMatrix = Stats.CorMatrix(test);
            for (int i = 0; i<5; i++)
            {
                for (int j=0; j<5; j++)
                {
                    Console.Write(corMatrix[i, j]+ "  ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Fin du programme ...");
            Console.ReadLine();
            data.deleteFiles();
        }
    }   
}
