using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FormeraMeasure.Models
{
    public class Error
    {
        public int Status { get; set; }
        public string Message { get; set; }
        //        public string Stacktrace { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
