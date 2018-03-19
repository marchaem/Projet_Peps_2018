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
            DateTime dateDebut = new DateTime(2015,6,1);
            DateTime dateFin = DateTime.Today;
            RecupData data = new RecupData(dateDebut, dateFin);
            data.RecupCSV(4);
            List<double> test = data.ParseCSV(data.Files[0]);
            foreach(double d in test)
            {
                Console.Write(d);
                Console.Write("-");
            }
            Console.WriteLine("Fin du programme ...");
            Console.ReadLine();
            data.deleteFiles();
        }

    }

    
    
}
