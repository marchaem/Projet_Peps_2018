using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Wrapper;

namespace WebEurostral.Models
{ 
    public class EurostralModels
    {
        public ParametresModel p;
        public double t;
        public WrapperClass wc;
        public double prixEnt;

        public EurostralModels()
        {
            p = new ParametresModel();
            WrapperClass wc = new WrapperClass();
        }

        public void Pricet(double t,double[,] past)
        {
            prixEnt=wc.getPriceEurostral(t, past);
        }
    }
}