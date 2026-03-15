using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikolaevaLib
{
    public class PartnerView
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string Phone { get; set; }
        public int? Rating { get; set; }
        public double? Discount { get; set; }
    }
}
