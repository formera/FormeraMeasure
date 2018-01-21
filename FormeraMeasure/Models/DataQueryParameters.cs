using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormeraMeasure.Models
{
    public class DataQueryParameters
    {
        public enum AggregationTypes { one_minute, five_minutes, thirty_minutes, one_hour, one_day }

        public AggregationTypes? AggregationType { get; set; }
        public int? AggregateTo { get; set; }
        public string DeviceId { get; set; }
        public string Topic { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
