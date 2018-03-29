using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using Wrapper;
using Data;
using System.Runtime.Serialization;

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
        public double[] deltaD;
        public double pocketD;
        public double[] pp;
        public double PL;
        public double v;
        public Stock stock;
        public int nb_iterations { get; set; }
        public List<DataPoint> dataPoints_GSPC;
        public List<DataPoint> dataPoints_STOXX50E;
        public List<DataPoint> dataPoints_AXJO;
        public List<DataPoint> dataPoints_EURUSD;
        public List<DataPoint> dataPoints_EURAUD;
        public List<DataPoint> dataPoints_trackingError;
        public List<DataPoint> dataPoints_PfC;
        public List<DataPoint> dataPoints_prix;


        [DataContract]
        public class DataPoint
        {
            public DataPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "x")]
            public Nullable<double> X = null;

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "y")]
            public Nullable<double> Y = null;
        }

        [DataContract]
        public class DataPoint_JSON
        {
            public DataPoint_JSON(double x, double[] y)
            {
                this.X = x;
                this.Y = y;
            }

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "x")]
            public Nullable<double> X = null;

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "y")]
            public double[] Y = null;
        }

        [DataContract]
        public class DataPoint_date
        {
            public DataPoint_date(DateTime x, double[] y)
            {
                this.X = x;
                this.Y = y;
            }

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "x")]
            public DateTime X;

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "y")]
            public double[] Y = null;
        }

        [Required(ErrorMessage = "La date courante est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateCourante { get; set; }

        public EurostralModels()
        {

            p = new ParametresModel();
            //WrapperClass wc = new WrapperClass();
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