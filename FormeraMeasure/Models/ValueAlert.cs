using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FormeraMeasure.Models
{
    public class ValueAlert : Alert
    {

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ComparisonTypes { lessThan, greaterThan }

        public ComparisonTypes ComparisonType { get; set; }
        public string DeviceId { get; set; }
        public string Topic { get; set; }

        public decimal Value { get; set; }
        public decimal TriggeredByValue { get; set; }

    }
}
