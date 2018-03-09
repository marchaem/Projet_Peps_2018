using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIFiMag;
using APIFiMag.Datas;
using APIFiMag.Importer;
using APIFiMag.Exporter;

namespace Data
{
    // On veut récupérer des taux de change sur fxtop.com,
    // on commence par construire un DataFXTop
    public class DataPricer
    {
        public void Export_Taux_de_change(DateTime debut, DateTime fin)
        {
            List<Currency> curr = new List<Currency>();
            curr.Add(Currency.USD);
            curr.Add(Currency.AUD);
            // Ici, on s’intéresse à l’euro par rapport aux autres :
            // EUR/USD, EUR/AUD

            DataFXTop xchange = new DataFXTop(Currency.EUR,
                                              curr,
                                              debut,
                                              fin,
                                              Frequency.Daily);
            // On récupère les données, on les parse et on remplit xchange

            xchange.ImportData(new ParserFXTop());

            // On exporte les données récupérées, dans le format de notre choix, ici CSV

            xchange.Export(new ExportCSV("recupFxTop.csv"));

        }

        //taux euribor entre debut et fin
        public void Export_Taux_EURIBOR(DateTime debut, DateTime fin)
        {
            DataIRate IRate = new DataIRate(InterestRate.EURIBOR, debut,fin);
            IRate.ImportData(new ImportEBF());
            IRate.Export(new ExportCSV("recupIRateEuribor.csv"));
        }

        //taux USEuribor entre debut et fin
        public void Export_Taux_USDEURIBOR(DateTime debut, DateTime fin)
        {
            DataIRate IRate = new DataIRate(InterestRate.USDEURIBOR, debut, fin);
            IRate.ImportData(new ImportEBF());
            IRate.Export(new ExportCSV("recupIRateUSEuribor.csv"));
        }
        
    }
    
}
