﻿using System;
using Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapper;
using Data;

namespace WebApplication.Models
{
    public class Test
    {
        public double displayPrice()
        {
            DateTime date = new DateTime(2014,12,18);
            DateTime debutProduit = new DateTime(2014, 12, 18);
            DateTime finProduit = new DateTime(2022, 12, 08);
            Data.RecupData recup = new RecupData(new DateTime(2000, 1, 1), date);

            for (int i=0; i<5; i++)
            {
                recup.Files[i] = "C:\\Users\\thame\\Peps1\\Projet_Peps_2018\\WebApplication\\bin\\" + recup.Files[i];
            }
            recup.Fetch();

            double t = recup.DateToDouble(debutProduit, date, finProduit);
            double[,] covLogR = recup.exportCov();
            double[,] pastDelta = recup.exportPast(t, 182, debutProduit, finProduit); 
            double[,] pastPrice = recup.exportPast(t, 182, debutProduit, finProduit);



            //int size, double r, double* VarHis, double* spot, double* trend, double fdStep, int nbSamples, double strike, double T1, int nbTimeSteps1, double* lambdas1
            double r_eu = 0.002;
            double r_aus = 0.025;
            double r_us = 0.00025;
            int size = 5;
            double r = r_eu;
           
           

            double[] spots = new double[5];
            for (int i = 0; i < 3; i++)
            {
                spots[i] = pastPrice[0,i];
            }
           
            

            double[] trends = new double[5];
            trends[0] = r_eu;
            trends[1] = r_us-covLogR[1,3];
            trends[2] = r_aus-covLogR[2,4];
            trends[3] = r_eu - r_us;
            trends[4] = r_eu - r_aus;

            double[] lambdas = new double[5];
            for (int i = 0; i < 5; i++)
            {
                lambdas[i] = 0.05;
            }
            
            WrapperClass wc = new WrapperClass(size, r, covLogR, spots, trends, 0.1, 50000, 100, 8.0, 16, lambdas);
            double[] delta = new double[5];
            double H = 416;
            return wc.getPriceEurostral();
            //return wc.getPriceEurostral(t, pastPrice);
            //return wc.getDeltaEurostral(pastPrice, t, H)[0];
            

        }
        public double[] displayDelta0()
        {


           
            double[] delta0 = new double[5];
            //wc.getDeltaEurostral(spots, 0.0, delta0, 0.1);
            return delta0;
            

        }

    }
}