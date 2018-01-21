using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{
    [BsonIgnoreExtraElements]
    public class DataDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string DeviceId { get; set; }
        public string Type { get; set; }
        public string Topic { get; set; }
        public DateTime TimeStampHour { get; set; }
        [BsonElement("data")]
        public Dictionary<string, string> Data { get; set; }
    }

}
