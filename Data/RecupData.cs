﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceAPI;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace Data
{
    class RecupData
    {

        List<string> Symbols; //Liste des symboles à récupérer sur Yahoo
        public List<string> Files; //Liste des noms de fichiers CSV
        DateTime dateDebut;
        DateTime dateFin;

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

        public List<double> ParseCSV(string file)
        {
            List<double> donnees = new List<double>();
            List<string> AllDonnees = new List<string>();
            List<string> donneesString = new List<string>();
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
                        // On teste si c'est un nombre et non une date ou autre chose (null ...)
                        if (field.Any(char.IsDigit) && !(field.Contains("/")) && !(field.Contains("-")))
                        {
                            AllDonnees.Add(field);
                        }
                    }
                }
            }

            // On ne garde que les cours de cloture
            for (int i = 3; i < AllDonnees.Count; i = i + 6)
            {
                donneesString.Add(AllDonnees[i]);
            }

            //On change ensuite les données de string en double 
            for (int i=0; i<donneesString.Count; i++)
            {
                Console.WriteLine(donneesString[i]);
                donnees.Add(double.Parse(donneesString[i],CultureInfo.InvariantCulture));
            }
            return donnees;
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