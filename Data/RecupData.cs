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

        /// <summary>
        /// Liste des symboles Yahoo correpondant à nos sous-jacent
        /// </summary>
        private List<string> Symbols; 
        /// <summary>
        /// List des données brutes récupérés de Yahoo pour chaque sous-jacent
        /// </summary>
        private List<string> RawData; //Données brutes récupérés de Yahoo
        /// <summary>
        /// Date de début de récupération des données (pas celle du début du produit)
        /// </summary>
        private DateTime dateDebut;
        /// <summary>
        /// Date de fin de récup des données
        /// </summary> 
        private DateTime dateFin;
        /// <summary>
        /// Structure de données pour le stockage
        /// Les données pour un sous-jacent est stocké dans un dictionnaire (date,valeur)
        /// On fait ensuite une liste de ces dictionnaires pour avoir le total des sous-jacents
        /// </summary>
        private List<Dictionary<DateTime, double>> data;
        bool done = false;
        string cour;
        List<DateTime> dateSem;

        private void InitializeDateSem()
        {
            dateSem = new List<DateTime>();
            dateSem.Add(new DateTime(2014, 12, 19));
            dateSem.Add(new DateTime(2015, 06, 18));
            dateSem.Add(new DateTime(2015, 12, 18));
            dateSem.Add(new DateTime(2016, 06, 20));
            dateSem.Add(new DateTime(2016, 12, 19));
            dateSem.Add(new DateTime(2017, 06, 17));
            dateSem.Add(new DateTime(2017, 12, 18));
            dateSem.Add(new DateTime(2018, 06, 18));
            dateSem.Add(new DateTime(2018, 12, 18));
            dateSem.Add(new DateTime(2019, 06, 17));
            dateSem.Add(new DateTime(2019, 12, 18));
            dateSem.Add(new DateTime(2020, 06, 18));
            dateSem.Add(new DateTime(2020, 12, 18));
            dateSem.Add(new DateTime(2021, 06, 18));
            dateSem.Add(new DateTime(2021, 12, 20));
            dateSem.Add(new DateTime(2022, 06, 20));
            dateSem.Add(new DateTime(2022, 12, 14));
        }

        public DateTime getDebutData()
        {
            return this.dateDebut;
        }

        public DateTime getFinData()
        {
            return this.dateFin;
        }

        public List<Dictionary<DateTime, double>> getWholeData()
        {
            return this.data;
        }

        public List<String> getSymbols()
        {
            return this.Symbols;
        }

        public Dictionary<DateTime, double> getData(String symbol)
        {
            int Index = Symbols.IndexOf(symbol);
            return data[Index];
        }

        public void PrintData(String symbol)
        {
            Dictionary<DateTime, double> dico = getData(symbol);
            foreach(KeyValuePair<DateTime,double> entry in dico)
            {
                Console.WriteLine("<" + symbol + ";"+ entry.Key.ToString("dd/MM/yyyy") + ";" + entry.Value + ">");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dateDebut">Date de début des donées (avant le début du produit c'est mieux)</param>
        /// <param name="dateFin">Date de fin de récup des données (max : Aujourd'hui)</param>
        public RecupData(DateTime dateDebut, DateTime dateFin)
        {
            Symbols = new List<String>();
            Symbols.Add("^STOXX50E");
            Symbols.Add("^GSPC");
            Symbols.Add("^AXJO");
            Symbols.Add("USDEUR=X");
            Symbols.Add("AUDEUR=X");

            RawData = new List<string>();

            this.dateDebut = dateDebut;
            this.dateFin = dateFin;

            if (dateDebut > dateFin)
            {
                throw new Exception("[Erreur] Les dates sont incohérentes (Date de début postérieure à date de fin)");
            }
        }

        /// <summary>
        /// Convertit une date en double sur la grille de discrétisation du pricer
        /// t est en année depuis le début du produit 
        /// notre porduit dure 8 ans, donc t<8
        /// </summary>
        /// <param name="debutProduit">DateTime Début du produit</param>
        /// <param name="t">double instant sur la grille du pricer</param>
        /// <param name="finProduit">DateTime fin du produit</param>
        /// <returns>Datetime correspondant a t(double) en entrée</returns>
        public DateTime DoubleToDate(DateTime debutProduit, double t, DateTime finProduit)
        {
            if (debutProduit > finProduit)
            {
                throw new Exception("[ERREUR]Date de début et fin de produit incohérentes ! (debut > fin)");
            }
            double joursTotaux = (finProduit - debutProduit).TotalDays;
            joursTotaux = t / 8 * joursTotaux;
            DateTime res = debutProduit.AddDays(joursTotaux);
            return res;
        }

        public int GetNbTimeSteps(DateTime debutProduit, DateTime finProduit, int freq)
        {
            if (debutProduit > finProduit)
            {
                throw new Exception("[ERREUR]Date de début et fin de produit incohérentes ! (debut > fin)");
            }
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
            // Pas optimal
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
            if (debutProduit > finProduit)
            {
                throw new Exception("[ERREUR]Date de début et fin de produit incohérentes ! (debut > fin)");
            }
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

        private Dictionary<DateTime, double> ParseStringCSV(string CSVstring)
        {
            var stream = new MemoryStream();
            var bytes = System.Text.Encoding.Default.GetBytes(CSVstring);
            List<string> AllDonnees = new List<string>();
            Dictionary<DateTime, double> mapData = new Dictionary<DateTime, double>();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    foreach (string field in fields)
                    {
                        AllDonnees.Add(field);
                    }
                }
            }

            double token;
            DateTime dateCour;

            for (int i = 7; i < AllDonnees.Count; i = i + 7)
            {
                if (double.TryParse(AllDonnees[i + 4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out token))
                {
                    dateCour = DateTime.Parse(AllDonnees[i]);
                    mapData[dateCour] = double.Parse(AllDonnees[i + 4], CultureInfo.InvariantCulture);
                }
            }
            return mapData;
        }

        private List<Dictionary<DateTime, double>> ParseAllStringCSV()
        {
            List<Dictionary<DateTime, double>> res = new List<Dictionary<DateTime, double>>();
            for (int i = 0; i < Symbols.Count; i++)
            {
                res.Add(ParseStringCSV(RawData[i]));
            }
            return res;
        }

        public double freqToH(int freq, DateTime debutProd, DateTime finProd)
        {
            return 8 * freq / (finProd - debutProd).TotalDays;
        }

        public double[,] exportPastSemestre(double t, DateTime debutProduit, DateTime finProduit)
        {
            InitializeDateSem();
            DateTime date = DoubleToDate(debutProduit, t, finProduit);
            List<DateTime> toPutInPast = new List<DateTime>();
            bool trouve = false;
            foreach(DateTime d in dateSem)
            {
                if (date.Date > d.Date)
                {
                    toPutInPast.Add(d);
                }
                if (date.Date == d.Date)
                {
                    trouve = true;
                    toPutInPast.Add(d);
                }
            }
            if (!trouve)
            {
                toPutInPast.Add(date);
            }
            toPutInPast.Sort();
            double[,] res;
            res = new double[toPutInPast.Count,this.Symbols.Count];
            for (int i=0; i<toPutInPast.Count; i++)
            {
                for (int j=0; j<this.Symbols.Count; j++)
                {
                    res[i, j] = GetClosestData(toPutInPast[i])[j];
                    Console.Write(res[i,j] + " ");
                }
                Console.WriteLine();
            }
            return res;
        }

        public double[,] exportMoyPast(double t, int freq, DateTime debutProd, DateTime finProd) 
        {
            double[,] res = exportPast(t,freq,debutProd,finProd);
            for (int i = 0; i<(Symbols.Count-2); i++)
            {
                res[0, i] = GetClosestData(new DateTime(2014, 12, 18))[i] + GetClosestData(new DateTime(2014, 12, 19))[i] + GetClosestData(new DateTime(2014, 12, 22))[i];
                res[0, i] = res[0, i] / 3.0;
            }
            return res;
        }

        public double[,] exportMoyPastSemestre(double t, DateTime debutProd, DateTime finProd)
        {
            double[,] res = exportPastSemestre(t, debutProd, finProd);
            for (int i = 0; i < (Symbols.Count - 2); i++)
            {
                res[0, i] = GetClosestData(new DateTime(2014, 12, 18))[i] + GetClosestData(new DateTime(2014, 12, 19))[i] + GetClosestData(new DateTime(2014, 12, 22))[i];
                res[0, i] = res[0, i] / 3.0;
            }
            return res;
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
            toPutInPast.Sort();
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
                        Console.Write(Symbols[j] + " ");
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
                Console.WriteLine(Math.Round(res[i]*100.0,2));
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

        public double[,] exportFakeCov(DateTime debut, DateTime fin)
        {
            double[,] fakeCov = exportCov(debut, fin);
            for (int i=0; i<5; i++)
            {
                //for (int j=0; j<5; j++)
                //{
                    fakeCov[i, i] = 0.000000000001;
                //}
            }
            return fakeCov;
        }

        public double[,] exportCov(DateTime debut, DateTime fin)
        {
            return Stats.CovMatrix(Restreindre(debut,fin));
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

        public RecupData DataCommon()
        {
            RecupData res = new RecupData(this.dateDebut, this.dateFin);
            res.Symbols = new List<String>(this.Symbols);
            return null;
        }

        public void Fetch()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            GetYahooCSV();
            while (RawData.Count< this.Symbols.Count)
            {
                System.Threading.Thread.Sleep(25);
            }
            sw.Stop();
            Console.WriteLine("Fichiers récupérés de Yahoo en " + sw.Elapsed.Seconds + " secondes");
            Console.WriteLine("Mise en forme des données ...");
            this.data = ParseAllStringCSV();
            return;
        }

        public List<Dictionary<DateTime,double>> Restreindre(DateTime debut, DateTime fin)
        {
            List<Dictionary<DateTime, double>> DataRestr = new List<Dictionary<DateTime, double>>();
            for (int i=0; i<this.data.Count; i++)
            {
                DataRestr.Add(RestreindreDico(debut,fin,data[i]));
            }
            return DataRestr;
        }

        static private Dictionary<DateTime,double> RestreindreDico(DateTime debut, DateTime fin, Dictionary<DateTime,double> data)
        {
            if (fin < debut)
            {
                throw new Exception("[ERREUR] Date de début et fin incohérentes (fin < début)");
            }
            Dictionary<DateTime, double> DataRestr = new Dictionary<DateTime, double>();
            foreach(KeyValuePair<DateTime,double> entry in data)
            {
                if (entry.Key <= fin && entry.Key >= debut)
                {
                    DataRestr[entry.Key] = entry.Value;
                    //Console.WriteLine("<" + entry.Key.ToString("dd/MM/yyyy") + ";" + entry.Value + ">");
                }
            }
            return DataRestr;
        }

        public double[] GetEurostralHisto(DateTime debut, int freq, DateTime fin)
        {
            Console.WriteLine("Fetch eurostral ...");
            FetchEurostralHisto(debut, freq, fin);
            while (!done)
            {
                System.Threading.Thread.Sleep(25);
            }
            Dictionary<DateTime, double> dico;
            dico = ParseStringCSV(cour);
            List<DateTime> toReturn = new List<DateTime>();
            for (DateTime date = debut; date < fin; date = date.AddDays(freq))
            {
                toReturn.Add(date);
            }
            toReturn.Add(fin);
            double[] res = new double[toReturn.Count];
            for (int i =0; i<toReturn.Count; i++)
            {
                res[i] = GetClosestData(toReturn[i],dico);
            }
            return res;
        }

        public async void FetchEurostralHisto(DateTime debut, int freq, DateTime fin)
        {
            done = false;
            string symbolEurostral = "FR0012034023.PA";
            while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
            {
                await Token.RefreshAsync().ConfigureAwait(false);
            }
            cour = await Historical.GetRawAsync(symbolEurostral, dateDebut, dateFin).ConfigureAwait(false);
            cour = cour.Replace(",", ";");
            done = true;
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
                RawData.Add(csvdata);
                //System.IO.File.WriteAllText(Files[i], csvdata);
            }
        }

    }
}
