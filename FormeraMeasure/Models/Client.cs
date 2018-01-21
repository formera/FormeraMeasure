using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{
    [BsonIgnoreExtraElements]
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }

        public List<DeviceGrouping> Groupings { get; set; } = new List<DeviceGrouping>();
        public List<NotificationSystem> NotificationSystems { get; set; } = new List<NotificationSystem>();
    }
}
