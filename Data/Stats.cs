using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class Stats
    {
        public static double std(List<double> data)
        {
            double ret = 0;
            if (data.Count() > 0)
            {     
                double avg = data.Average();    
                double sum = data.Sum(d => Math.Pow(d - avg, 2));     
                ret = Math.Sqrt((sum) / (data.Count() - 1));
            }
            return ret;
        }

        public static double Cov(List<double> v1, List<double> v2)
        {
            int size = v1.Count;
            if (v1.Count != v2.Count)
            {
                throw new Exception("[Erreur] Covariance de vecteurs de tailles différentes ! Arrêt");
            }
            List<double> aux = new List<double>();
            List<double> logRenta1 = new List<double>();
            List<double> logRenta2 = new List<double>();
            for (int i=0; i<(v1.Count-1); i++)
            {
                logRenta1.Add(Math.Log(v1[i+1]/v1[i]));
                logRenta2.Add(Math.Log(v2[i+1]/v2[i]));
            }
            double mean1 = logRenta1.Average();
            double mean2 = logRenta2.Average();
            for (int i=0; i<(size-1); i++)
            {
                aux.Add((logRenta1[i] - mean1) * (logRenta2[i] - mean2));
            }
            return aux.Average();                        
        }

        public static double Cov(Dictionary<DateTime, double> d1, Dictionary<DateTime, double> d2)
        {
            var data = d1.Keys.Intersect(d2.Keys);
            List<double> d1Values = new List<double>();
            List<double> d2Values = new List<double>();
            foreach (DateTime d in data)
            {
                d1Values.Add(d1[d]);
                d2Values.Add(d2[d]);
            }
            if (d1Values.Count != d2Values.Count)
            {
                throw new Exception("[ERROR] Les listes générés ne sont pas de même taille ! (" + d1Values.Count + " != " + d2Values.Count + ")");
            }
            return Cov(d1Values, d2Values);
        }

        public static double Cor(List<double> v1, List<double> v2)
        {
            return Cov(v1, v2) / (std(v1) * std(v2));
        }

        public static List<double> logRendement(List<double> data)
        {
            List<double> ret = new List<double>();
            double diff = 0;
            for (int i=1; i<data.Count; i++)
            {
                diff = data[i] / data[i - 1];
                ret.Add(Math.Log(diff));
            }
            return ret;
        }

        public static double Cor(Dictionary<DateTime,double> d1, Dictionary<DateTime, double> d2)
        {
            var data = d1.Keys.Intersect(d2.Keys);
            List<double> d1Values = new List<double>();
            List<double> d2Values = new List<double>();
            foreach (DateTime d in data)
            {
                d1Values.Add(d1[d]);
                d2Values.Add(d2[d]);
            }
            if (d1Values.Count != d2Values.Count)
            {
                throw new Exception("[ERROR] Les listes générés ne sont pas de même taille ! (" + d1Values.Count + " != " + d2Values.Count + ")");
            }
            return Cor(d1Values,d2Values);
        }

        public static double[,] CorMatrix(List<Dictionary<DateTime, double>> data)
        {
            double[,] matrix = new double[data.Count, data.Count];
            //Amélioration possible : On peut faire que la moitié des calculs, la matrice est symétrique ...
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    matrix[i, j] = Cor(data[i], data[j]);
                }
            }
            return matrix;
        }

        public static double[,] CorMatrix(List<List<double>> data)
        {
            double[,] matrix = new double[data.Count, data.Count];
            //Amélioration possible : On peut faire que la moitié des calculs, la matrice est symétrique ...
            for (int i=0; i<data.Count; i++)
            {
                for (int j=0; j<data.Count; j++)
                {
                    matrix[i, j] = Cor(data[i],data[j]);
                }
            }
            return matrix;
        }

        public static double[,] CovMatrix(List<Dictionary<DateTime, double>> data)
        {
            double[,] matrix = new double[data.Count, data.Count];
            //Amélioration possible : On peut faire que la moitié des calculs, la matrice est symétrique ...
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    matrix[i, j] = Cov(data[i], data[j]);
                    Console.Write(matrix[i,j] + " ");
                }
                Console.WriteLine();
            }
            return matrix;
        }

        public static double[,] CovMatrix(List<List<double>> data)
        {
            double[,] matrix = new double[data.Count, data.Count];
            //Amélioration possible : On peut faire que la moitié des calculs, la matrice est symétrique ...
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    matrix[i, j] = Cov(data[i], data[j]);
                }
            }
            return matrix;
        }

        public static double volStd(List<double> data)
        {
            return Math.Sqrt(252) *std(logRendement(data));
        }

        public static double volStd(Dictionary<DateTime, double> data)
        {
            List<double> val = data.Values.ToList();
            return volStd(val);
        }

        public static double volHisto(List<double> data)
        {
            if (data.Count == 0)
            {
                throw new Exception("Donées vide en entrée du calcul de la vol (Taille = 0)");
            }
            double sum = 0.0;
            for (int i=1; i<data.Count; i++)
            {
                sum += Math.Pow(Math.Log(data[i] /data[i - 1]),2);
            }
            sum = sum * (252.0 / data.Count);
            sum = Math.Sqrt(sum);
            return sum;
        }

        public static double volHisto(Dictionary<DateTime, double> data)
        {
            List<double> val = data.Values.ToList();
            return volHisto(val);
        }

    }
}
