using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{

    [BsonIgnoreExtraElements]
    public class Topic
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sensor { get; set; }
        public string Type { get; set; }
        public DateTime? ValueTimeStamp { get; set; }
        public string Value { get; set; }
        public Decimal Min { get; set; }
        public Decimal Max { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DeviceId { get; set; }
        public string ClientId { get; set; }
        public DateTime? LastSeen { get; set; }
        public List<Topic> Topics { get; set; }
    }
}
