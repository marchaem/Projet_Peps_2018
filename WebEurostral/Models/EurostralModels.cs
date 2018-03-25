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
        public double PandL;


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
            

            nomActifs[0] = "Euro stoxx 50";
            nomActifs[1] = "S&P ASX 200";
            nomActifs[2] = " S&P 500";
            nomActifs[3] = "Eur / USD";
            nomActifs[4] = "Eur / AUD";


        }

        public void Pricet(double t,double[,] past)
        {
            prixEnt=wc.getPriceEurostral(t, past);

        }
    }
}