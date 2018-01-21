using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormeraMeasure.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string LogoBase64 { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }

        public DateTime LastLogin { get; set; }

        public string ClientId { get; set; }

        public byte[] Salt { get; set; }
        public string Hash { get; set; }
        public int Iterations { get; set; }

        public List<TopicIdentifier> Favorites { get; set; } = new List<TopicIdentifier>(); 
    }
}
