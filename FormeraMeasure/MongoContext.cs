using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using FormeraMeasure.Models;
using FormeraMeasure.Options;

namespace FormeraMeasure
{
    public class MongoContext
    {
        public readonly IMongoDatabase _database = null;

        public MongoContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("User");
            }
        }

        public IMongoCollection<Client> Clients
        {
            get
            {
                return _database.GetCollection<Client>("Clients");
            }
        }


        public IMongoCollection<Device> Devices
        {
            get
            {
                return _database.GetCollection<Device>("Devices");
            }
        }

        public IMongoCollection<DataDocument> DataDocuments
        {
            get
            {
                return _database.GetCollection<DataDocument>("DataDocuments");
            }
        }

        public IMongoCollection<Alert> Alerts
        {
            get
            {
                return _database.GetCollection<Alert>("Alerts");
            }
        }
        /*
        public IMongoCollection<AlertHistory> AlertHistory
        {
            get
            {
                return _database.GetCollection<Alert>("Alerts");
            }
        }
        */
    }
}
