using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class Informations
    {
        public double t;

        [Required(ErrorMessage = "La date courante est obligatoire")]
        [DataType(DataType.Date)]
        public DateTime DateCourante { get; set; }
    }
}