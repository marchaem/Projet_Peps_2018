using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace Data
{
    public class Stock
    {
        Dictionary<double, List<double>> donnees;
        List<String> header;
        RecupData data;
        DateTime debutProduit = new DateTime(2014, 12, 22);
        DateTime finProduit = new DateTime(2022, 12, 22);

        public static void printPath()
        {
            string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            Console.WriteLine(wanted_path);
        }

        public List<string> getHeader()
        {
            return header;
        }

        public string LineCSV(List<string> list)
        {
            string res = "";
            for (int i=0; i<list.Count; i++)
            {
                res = res + list[i] + ";";
            }
            res = res.Remove(res.Length - 1);
            return res;
        }

        public double getDeltas(double t, int i)
        {
            return donnees[t][i + 1];
        }

        public double getPrix(double t)
        {
            return donnees[t][5];
        }

        public double getTrackingError(double t)
        {
            return donnees[t][6];
        }

        private DateTime doubleToDate(double t)
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

        public List<string> ToLine(double t, double[] deltas, double tracking_error, double price)
        {
            List<string> line = new List<string>();
            DateTime date = doubleToDate(t);
            line.Add(date.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture));
            line.Add(t.ToString());
            line.Add(deltas[0].ToString());
            line.Add(deltas[1].ToString());
            line.Add(deltas[2].ToString());
            line.Add(deltas[3].ToString());
            line.Add(deltas[4].ToString());
            line.Add(tracking_error.ToString());
            line.Add(price.ToString());
            foreach(var s in data.getSymbols())
            {
                line.Add(data.GetClosestData(date,data.getData(s)).ToString());
            }
            return line;
        }

        public void remove(double t)
        {
            donnees.Remove(t);
        }

        public void Add(double t, double[] deltas, double prix, double tracking_error)
        {
            List<double> val = new List<double>();
            if (deltas.GetLength(0) != 5)
            {
                throw new Exception("[ERREUR]Les delta n'ont pas la bonne taille !");
            }
            for (int i=0; i<deltas.GetLength(0); i++)
            {
                val.Add(deltas[i]);
            }
            val.Add(prix);
            val.Add(tracking_error);
            donnees[t] = val;
        }

        private void InitializeHeader()
        {
            header = new List<string>();
            header.Add("Date");
            header.Add("Date (Grille Pricer)");
            header.Add("Delta_STOXX50");
            header.Add("Delta_SP500");
            header.Add("Delta_ASX200");
            header.Add("Delta_USDEUR");
            header.Add("Delta_AUDEUR");
            header.Add("Prix Produit");
            header.Add("Tracking error");
            header.Add("Stoxx50");
            header.Add("SP500");
            header.Add("ASX200");
            header.Add("USD/EUR");
            header.Add("AUD/EUR");
        }

        public Stock(RecupData data)
        {
            donnees = new Dictionary<double, List<double>>();
            this.data = data;
            InitializeHeader();
        }

        public Stock(string csvFile)
        {
            this.data = data;
            InitializeHeader();
            string filePath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "//" + csvFile;
            donnees = new Dictionary<double, List<double>>();
            List<string> AllDonnees = new List<string>();
            using (TextFieldParser parser = new TextFieldParser(filePath))
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

            double t;
            double[] deltas = new double[5];
            double prix;
            double tracking_error;

            for (int i = header.Count; i < AllDonnees.Count; i = i + header.Count)
            {
                t = double.Parse(AllDonnees[i], CultureInfo.CurrentCulture);
                for (int j=0; j<5; j++)
                {
                    deltas[j] = double.Parse(AllDonnees[j+1+i], CultureInfo.CurrentCulture);
                }
                prix = double.Parse(AllDonnees[i+6], CultureInfo.CurrentCulture);
                tracking_error = double.Parse(AllDonnees[i + 7], CultureInfo.CurrentCulture);
                this.Add(t, deltas, prix, tracking_error);
            }
        }

        public void print()
        {
            foreach(var e in donnees)
            {
                Console.Write("<" + e.Key);
                foreach(double d in e.Value)
                {
                    Console.Write("; " + d);
                }
                Console.Write(">");
                Console.WriteLine();
            }
        }

        public void SaveToCSV()
        {
            string filePath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "//histo.csv";
            var csv = new StringBuilder();
            var newline = LineCSV(header);
            csv.AppendLine(newline);

            List<double> keys = donnees.Keys.ToList();
            keys.Sort();

            foreach(var key in keys)
            {
                newline = LineCSV(ToLine(key, donnees[key].GetRange(0, 5).ToArray(), donnees[key][5], donnees[key][6]));
                csv.AppendLine(newline);
            }

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}
