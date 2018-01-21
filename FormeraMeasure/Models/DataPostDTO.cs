using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormeraMeasure.Models
{
    public class DataPostDTO
    {
        public class DeviceStatus
        {
            public string Type { get; set; }
            public long Uptime { get; set; }
            public string HwVer { get; set; }
            public string FwVer { get; set; }
        }

        public class DataPoint
        {
            public string Topic { get; set; }
            public string Value { get; set; }
        }

        //public string DeviceId { get; set; }
        public DateTime? DeviceTimeStamp { get; set; }
        public DeviceStatus Status { get; set; }
        public List<DataPoint> Data { get; set; }
    }

}
