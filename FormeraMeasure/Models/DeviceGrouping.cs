using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{

    [BsonIgnoreExtraElements]
    public class DeviceGrouping
    {
      //  [BsonId]
      //  [BsonRepresentation(BsonType.ObjectId)]
      //  public string Id { get; set; }
        public string Name { get; set; }
      //  public string ClientId { get; set; }
        public List<TopicIdentifier> Topics { get; set; }
    }
    
}
