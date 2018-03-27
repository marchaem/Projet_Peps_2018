using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Wrapper;

namespace WebEurostral.Models
{ 
    public class EurostralModels
    {
        public ParametresModel p;
        public double t;
        public WrapperClass wc;
        public double prixEnt;
        public double[] deltaEnt;
        public String[] nomActifs;
        public double[] prixActifs;
        public double[,] covLogR;
        public double[,] pastDelta;
        public double[,] pastPrice;
        public double[] PandL { get; set; }
        public DateTime[] dates;
        public int[] datesbis;
        public double[] pock;

        [Required(ErrorMessage = "La date courante est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateCourante { get; set; }

        public EurostralModels()
        {

            p = new ParametresModel();
            WrapperClass wc = new WrapperClass();
            deltaEnt = null;
            nomActifs = new string [5];
            prixActifs = new double[5];
            //PandL = new double[2];
            dates = new DateTime[2];

            nomActifs[0] = "Euro stoxx 50";
            nomActifs[1] = "S&P 500";
            nomActifs[2] = " S&P ASX 200";
            nomActifs[3] = "USD / EUR";
            nomActifs[4] = "AUD / EUR";


        }

        public void Pricet(double t,double[,] past)
        {
            prixEnt=wc.getPriceEurostral(t, past);

        }
    }
}