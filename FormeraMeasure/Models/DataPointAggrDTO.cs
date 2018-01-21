using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormeraMeasure.Models
{
    public class DataPointAggrDTO
    {
        public DateTime TimeStamp { get; set; }
        public decimal Avg { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
