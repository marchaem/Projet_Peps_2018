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
            double r_eu = 0.002;
            double r_aus = 0.025;
            double r_us = 0.00025;
            int size = 5;
            double r = r_eu;
            double[,] varHis = new double[5,5];
            for (int i = 0; i < 5; i++)
            {
                for(int j=0; j<5; j++)
                {
                    varHis[i,j] = 0.2;
                }
                
            }

            double[] spots = new double[5];
            for (int i = 0; i < 3; i++)
            {
                spots[i] = 100;
            }
            spots[3] = 1.0;
            spots[4] = 1.0;

            

            double[] trends = new double[5];
            trends[0] = r_eu;
            trends[1] = r_us - 0.1 * 0.2 * 0.2;
            trends[2] = r_aus - 0.1 * 0.2 * 0.2;
            trends[3] = r_eu - r_us;
            trends[4] = r_eu - r_aus;

            double[] lambdas = new double[5];
            for (int i = 0; i < 5; i++)
            {
                lambdas[i] = 0.05;
            }
            
            WrapperClass wc = new WrapperClass(size, r, varHis, spots, trends, 0.1, 50, 100, 8.0, 10, lambdas);
            return wc.getPriceEurostral();

        }
        public double[] displayDelta0()
        {


            //int size, double r, double* VarHis, double* spot, double* trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double* lambdas1

            int size = 5;
            double r = 0.01;
            double[,] varHis = new double[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    varHis[i, j] = 0.15;
                }

            }

            double[] spots = new double[5];
            for (int i = 0; i < 5; i++)
            {
                spots[i] = 100;
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

            

            WrapperClass wc = new WrapperClass(size, r, varHis, spots, trends, 0.1, 50, 100, 8.0, 10, lambdas);
            double[] delta0 = new double[5];
            wc.getDeltaEurostral(spots, 0.0, delta0, 0.1);
            return delta0;
            

        }

    }
}