using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutonHuoltoSovellus.Models
{
    public class Tankkaus
    {
        public int Id { get; set; }
        public DateTime Aika { get; set; }
        public double Litrat { get; set; }
        public double Kilometrit { get; set; }
    }
}
