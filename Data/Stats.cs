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

        public static double[,] cov()
        {
            return null;
        }
    }
}
