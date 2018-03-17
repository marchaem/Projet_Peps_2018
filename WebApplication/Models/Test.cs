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
            WrapperClass wc = new WrapperClass();
            return wc.getPrice();

        }
    }
}