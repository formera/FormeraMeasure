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

    [JsonConverter(typeof(AlertConverter))]
    [BsonKnownTypes(typeof(ValueAlert))]
    public class Alert
    {
        public class AlertConverter : JsonCreationConverter<Alert>
        {

            protected override Alert Create(Type objectType, Newtonsoft.Json.Linq.JObject jObject)
            {
                //TODO: read the raw JSON object through jObject to identify the type
                //e.g. here I'm reading a 'typename' property:
                //var type = jObject.Value<string>("directory_rule");
                var type = jObject.Value<string>("type");
                if ("value".Equals(jObject.Value<string>("type")))
                    return new ValueAlert();
                // else
                //                 return new Device();
                //now the base class' code will populate the returned object.
                return null;
            }
        }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Type { get; set; }

        public bool Triggered { get; set; }
        public DateTime TriggeredAt { get; set; }

        public List<String> NotifierIds { get; set; } = new List<string>();
           
    }
}
