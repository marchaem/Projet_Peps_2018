using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIFiMag;
using APIFiMag.Datas;
using APIFiMag.Importer;
using APIFiMag.Exporter;
using System.Xml.Linq;

namespace Data
{
    // On veut récupérer des taux de change sur fxtop.com,
    // on commence par construire un DataFXTop
    public class DataPricer
    {

        //taux euribor entre debut et fin
        public static void Export_Taux_EURIBOR(DateTime debut, DateTime fin)
        {
            DataIRate IRate = new DataIRate(InterestRate.EURIBOR, debut,fin);
            IRate.ImportData(new ImportEBF());
            IRate.Export(new ExportCSV("recupIRateEuribor.csv"));
        }

        //taux USEuribor entre debut et fin
        public static void Export_Taux_USDEURIBOR(DateTime debut, DateTime fin)
        {
            DataIRate IRate = new DataIRate(InterestRate.USDEURIBOR, debut, fin);
            IRate.ImportData(new ImportEBF());
            IRate.Export(new ExportCSV("recupIRateUSEuribor.csv"));
        }

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
