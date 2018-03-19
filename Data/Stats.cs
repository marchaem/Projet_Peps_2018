using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    static class Stats
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
            int size = 0;
            if (v1.Count != v2.Count)
            {
                size = Math.Min(v1.Count, v2.Count);
            }
            else
            {
                size = v1.Count;
            }
            List<double> aux = new List<double>();
            double mean1 = v1.Average();
            double mean2 = v2.Average();
            for (int i=0; i<size; i++)
            {
                aux.Add((v1[i] - mean1) * (v2[i] - mean2));
            }
            return aux.Average();                        
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

        public static double[,] CorMatrix(List<List<double>> data)
        {
            double[,] matrix = new double[data.Count, data.Count];
            //Amélioration possible : On peut faire que la moitié des calculs, la matrice est symétrique ...
            for (int i=0; i<data.Count; i++)
            {
                for (int j=0; j<data.Count; j++)
                {
                    matrix[i, j] = Math.Round(Cor(data[i],data[j]),2);
                }
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
            return std(logRendement(data));
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

    }
}
