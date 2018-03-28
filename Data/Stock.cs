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

        public static void printPath()
        {
            string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            Console.WriteLine(wanted_path);
        }

        public List<string> createHeader()
        {
            List<string> head = new List<string>();
            head.Add("Date");
            head.Add("Delta_STOXX50");
            head.Add("Delta_SP500");
            head.Add("Delta_ASX200");
            head.Add("Delta_USDEUR");
            head.Add("Delta_AUDEUR");
            head.Add("Prix Produit");
            head.Add("Tracking error");
            return head;
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

        public List<string> ToLine(double t, double[] deltas, double tracking_error, double price)
        {
            List<string> line = new List<string>();
            line.Add(t.ToString());
            line.Add(deltas[0].ToString());
            line.Add(deltas[1].ToString());
            line.Add(deltas[2].ToString());
            line.Add(deltas[3].ToString());
            line.Add(deltas[4].ToString());
            line.Add(tracking_error.ToString());
            line.Add(price.ToString());
            return line;
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

        public Stock()
        {
            donnees = new Dictionary<double, List<double>>();
        }

        public Stock(string csvFile)
        {
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

            double token;
            double t;
            double[] deltas = new double[5];
            double prix;
            double tracking_error;

            for (int i = 8; i < AllDonnees.Count; i = i + 8)
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
            var newline = LineCSV(createHeader());
            csv.AppendLine(newline);

            foreach(var entry in donnees)
            {
                newline = LineCSV(ToLine(entry.Key, entry.Value.GetRange(0, 5).ToArray(), entry.Value[5], entry.Value[6]));
                csv.AppendLine(newline);
            }

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}
