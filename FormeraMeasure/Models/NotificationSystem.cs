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

    [JsonConverter(typeof(NotificationSystemConverter))]
    [BsonKnownTypes(typeof(SlackNotificationSystem), typeof(EmailNotificationSystem))]
    public class NotificationSystem
    {
        public class NotificationSystemConverter : JsonCreationConverter<NotificationSystem>
        {

            protected override NotificationSystem Create(Type objectType, Newtonsoft.Json.Linq.JObject jObject)
            {
                //TODO: read the raw JSON object through jObject to identify the type
                //e.g. here I'm reading a 'typename' property:
                //var type = jObject.Value<string>("directory_rule");
                var type = jObject.Value<string>("type");
                if ("mail".Equals(jObject.Value<string>("type")))
                    return new EmailNotificationSystem();
                else if ("slack".Equals(jObject.Value<string>("type")))
                    return new SlackNotificationSystem();
                // else
                //                 return new Device();
                //now the base class' code will populate the returned object.
                return null;
            }
        }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public NotificationSystem()
        {
             Id = ObjectId.GenerateNewId().ToString();
        }
    }

}
