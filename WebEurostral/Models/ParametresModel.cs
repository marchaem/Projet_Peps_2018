using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebEurostral.Models
{
    public class ParametresModel
    {
        public double t;
        public double r_eu= 0.002;
        public double r_aus = 0.025;
        public double r_us = 0.00025;
        public int size = 5;
        public double r = 0.002;

        [Required(ErrorMessage = "La date courante est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateCourante { get; set; }

        public ParametresModel()
        {

        }

    }
}