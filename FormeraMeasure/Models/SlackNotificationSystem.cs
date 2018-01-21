using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FormeraMeasure.Models
{

    public class SlackNotificationSystem : NotificationSystem
    {
        public string Url { get; set; }
    }
}
