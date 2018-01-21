using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FormeraMeasure.Models
{
    public class MissingAlert : Alert
    {

        public string DeviceId { get; set; }
        public string Topic { get; set; }

        public long Seconds { get; set; }
        
    }
}
