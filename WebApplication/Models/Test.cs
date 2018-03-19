using System;
using Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapper;

namespace WebApplication.Models
{
    public class Test
    {
        public double displayPrice()
        {
            

            //int size, double r, double* VarHis, double* spot, double* trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double* lambdas1
           
                int size = 5;
                double r = 0.1;
                double[] varHis = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    varHis[i] = 0.15;
                }

                double[] spots = new double[5];
                for (int i = 0; i < 3; i++)
                {
                    spots[i] = 100;
                }

                for (int i = 3; i < 5; i++)
                {
                    spots[i] = 1;
                }

                double[] trends = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    trends[i] = 0.05;
                }

                double[] lambdas = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    lambdas[i] = 0.05;
                }
            return 0.0;
           // WrapperClass wc = new WrapperClass(size, r, varHis, spots, trends, 0.1, 50000, 100, 8.0, 100000, lambdas);
           // return wc.getPrice(size, r, varHis, spots, trends, 0.1, 50000, 100, 8.0, 100000, lambdas);

        }


    }
}