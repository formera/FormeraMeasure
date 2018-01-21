using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{
    [BsonIgnoreExtraElements]
    public class TopicIdentifier
    {
        public string DeviceId { get; set; }
        public string Topic { get; set; }
    }
}
