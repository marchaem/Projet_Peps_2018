using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceAPI;

namespace Data
{
    class RecupData
    {

        List<string> Symbols; //Liste des symboles à récupérer sur Yahoo
        List<string> Files; //Liste des noms de fichiers CSV
        DateTime dateDebut;
        DateTime dateFin;
        String file;

        public RecupData(DateTime dateDebut, DateTime dateFin)
        {
            Symbols = new List<String>();
            Symbols.Add("EURUSD=X");
            Symbols.Add("EURAUD=X");
            Symbols.Add("^GSPC");
            Symbols.Add("^STOXX50E");
            Symbols.Add("^AXJO");

            Files = new List<String>();
            Files.Add("EURUSD.csv");
            Files.Add("EURAUD.csv");
            Files.Add("SP500.csv");
            Files.Add("Eurostoxx50.csv");
            Files.Add("ASX200.csv");

            this.dateDebut = dateDebut;
            this.dateFin = dateFin;
        }

        public void RecupCSV(int time)
        {
            GetYahooCSV();
            System.Threading.Thread.Sleep(time*1000);
            return;
        }

        async Task GetYahooCSV()
        {
            for (int i = 0; i < Symbols.Count; i++)
            {
                while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
                {
                    await Token.RefreshAsync().ConfigureAwait(false);
                }
                string csvdata = await Historical.GetRawAsync(Symbols[i], dateDebut, dateFin).ConfigureAwait(false);
                csvdata = csvdata.Replace(",", ";");
                System.IO.File.WriteAllText(Files[i], csvdata);
            }
            return;
        }

        void deleteFiles()
        {
            //TODO
        }

    }
}
