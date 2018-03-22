using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceAPI;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Diagnostics;

namespace Data
{
    public class RecupData
    {

        List<string> Symbols; //Liste des symboles à récupérer sur Yahoo
        public List<string> Files; //Liste des noms de fichiers CSV
        DateTime dateDebut;
        DateTime dateFin;
        List<Dictionary<DateTime, double>> data;

        public RecupData(DateTime dateDebut, DateTime dateFin)
        {
            Symbols = new List<String>();
            Symbols.Add("^STOXX50E");
            Symbols.Add("^GSPC");
            Symbols.Add("^AXJO");
            Symbols.Add("EURUSD=X");
            Symbols.Add("EURAUD=X");

            Files = new List<String>();
            Files.Add("Eurostoxx50.csv");
            Files.Add("SP500.csv");
            Files.Add("ASX200.csv");
            Files.Add("EURUSD.csv");
            Files.Add("EURAUD.csv");

            if (Files.Count !=Symbols.Count)
            {
                throw new Exception("[ERREUR] La taille des symboles et des fichiers associés sont différentes");
            }

            this.dateDebut = dateDebut;
            this.dateFin = dateFin;

            if (dateDebut > dateFin)
            {
                throw new Exception("[Erreur] Les dates sont incohérentes (Date de début postérieure à date de fin)");
            }
        }

        public DateTime DoubleToDate(DateTime debutProduit, double t, DateTime finProduit)
        {
            double joursTotaux = (finProduit - debutProduit).TotalDays;
            joursTotaux = t / 8 * joursTotaux;
            DateTime res = debutProduit.AddDays(joursTotaux);
            return res;
        }

        public int GetNbTimeSteps(DateTime debutProduit, DateTime finProduit, int freq)
        {
            double joursTotaux = (finProduit - debutProduit).TotalDays;
            return (int)joursTotaux / freq;
        }

        public List<double> GetClosestData(DateTime date)
        {
            List<double> res = new List<double>();
            foreach(Dictionary<DateTime,double> dico in this.data)
            {
                res.Add(GetClosestData(date,dico));
            }
            return res;
        }

        public double GetClosestData(DateTime date, Dictionary<DateTime, double> dico)
        {
            DateTime dateOpt = dico.Keys.ToList()[0];
            double dist = Double.MaxValue;
            double min = Double.MaxValue;
            // Pas optimal ...
            foreach (KeyValuePair<DateTime, double> entry in dico)
            {
                dist = Math.Abs((date-entry.Key).TotalDays);
                if (dist < min)
                {
                    min = dist;
                    dateOpt = entry.Key;
                }
            }
            return dico[dateOpt];
        }

        public double DateToDouble(DateTime debutProduit, DateTime date, DateTime finProduit)
        {
            double joursTotaux = (finProduit - debutProduit).TotalDays;
            double joursPasses = (date - debutProduit).TotalDays;
            return joursPasses / joursTotaux * 8.0; // Maturité = 8
        }

        private Dictionary<DateTime,double> ParseCSV(string file)
        {
            List<string> AllDonnees = new List<string>();
            Dictionary<DateTime, double> mapData = new Dictionary<DateTime, double>();
            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    foreach (string field in fields)
                    {
                        AllDonnees.Add(field);
                    }
                }
            }

            double token;
            DateTime dateCour;

            for (int i=7; i< AllDonnees.Count; i = i +7)
            {
                if (double.TryParse(AllDonnees[i + 4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out token))
                {
                    dateCour = DateTime.Parse(AllDonnees[i]);
                    mapData[dateCour] = double.Parse(AllDonnees[i + 4], CultureInfo.InvariantCulture);
                }              
            }

            return mapData;
        }

        private List<Dictionary<DateTime,double>> ParseAll()
        {
            List<Dictionary<DateTime, double>> res = new List<Dictionary<DateTime, double>>();
            for (int i=0; i<Files.Count; i++)
            {
                res.Add(ParseCSV(Files[i]));
            }
            return res;
        }

        public double freqToH(int freq, DateTime debutProd, DateTime finProd)
        {
            return 8 * freq / (finProd - debutProd).TotalDays;
        }

        public double[,] exportPast(double t, int freq, DateTime debutProd, DateTime finProd)
        {
            double H = freqToH(freq, debutProd, finProd);
            List<DateTime> toPutInPast = new List<DateTime>();
            DateTime dateActuelle = DoubleToDate(debutProd, t, finProd);
            double cour = 0;
            while (cour < t)
            {
                toPutInPast.Add(DoubleToDate(debutProd, cour, finProd));
                cour += H;
            }
            toPutInPast.Add(dateActuelle);
            double[,] res = new double[toPutInPast.Count, data.Count];
            for (int i=0; i<this.data.Count; i++)
            {
                for (int j=0; j<toPutInPast.Count; j++)
                {
                    res[j, i] = GetClosestData(toPutInPast[j], this.data[i]);                    
                }
            }

            for (int i=0; i< toPutInPast.Count; i++)
            {
                if (i == 0)
                {
                    for (int j=0; j<this.data.Count; j++)
                    {
                        Console.Write(Files[j] + " ");
                    }
                    Console.WriteLine();
                } 
                for (int j=0; j<this.data.Count; j++)
                {
                    Console.Write(res[i,j] + " ");
                }
                Console.WriteLine();
            }
                return res;
        }

        public double[] exportVol()
        {
            double[] res = new double[data.Count];
            for (int i= 0; i < this.data.Count; i++)
            {
                res[i] = Stats.volStd(this.data[i].Values.ToList());
            }
            return res;
        }

        public double[,] exportCor()
        {
            return Stats.CorMatrix(this.data);
        }

        public double[,] exportCov()
        {
            return Stats.CovMatrix(this.data);
        }

        private bool inAllData(DateTime date)
        {
            bool res = true;
            foreach(Dictionary<DateTime,double> dico in this.data)
            {
                res = res && dico.ContainsKey(date);
            }
            return res;
        }

        public void Fetch()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            GetYahooCSV();
            while (!DownloadFinished())
            {
                System.Threading.Thread.Sleep(25);
            }
            sw.Stop();
            Console.WriteLine("Fichiers CSV récupérés de Yahoo en " + sw.Elapsed.Seconds + " secondes");
            Console.WriteLine("Mise en forme des données ...");
            this.data = ParseAll();
            deleteFiles();
            return;
        }

        private bool DownloadFinished()
        {
            bool res = true;
            for (int i=0; i<Files.Count; i++)
            {
                string file = Files[i];
                res = res && File.Exists(file);
            }
            return res;
        }

        private async Task GetYahooCSV()
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
        }

        public void deleteFiles()
        {
            foreach (string s in Files)
            {
                if (File.Exists(s))
                {
                    File.Delete(s);
                }
            }
        }

    }
}
