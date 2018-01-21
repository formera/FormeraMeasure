using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public class AlertRepository : IAlertRepository 
    {
        private MongoContext _context;

        public AlertRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alert>> GetAllByClientId(string clientId)
        {
            var result = await _context.Alerts.FindAsync(x => x.ClientId == clientId);
            return result.ToList();
        }

        public async Task<IEnumerable<Alert>> GetAllByDeviceId(string clientId, string deviceId)
        {
            //var filter = Builders<ValueAlert>.Filter.Where(x => x.ClientId == clientId && x.DeviceId == deviceId);
            var col = _context.Alerts.AsQueryable<Alert>().OfType<ValueAlert>();
            var result = col.Where(x=>x.ClientId == clientId && x.DeviceId == deviceId);
            return await result.ToListAsync();
        }

        public IEnumerable<Alert> Get()
        {
            var mongoFilter = new BsonDocument();
            return _context.Alerts.Find(mongoFilter).ToList();
        }

        public async Task<Alert> GetByID(string id)
        {
            //var filter = Builders<User>.Filter.Eq("_id", id);
            //return _context.Users.Find(filter).FirstOrDefault();
            return await _context.Alerts.FindSync(x => x.Id == id).FirstAsync();
        }
        

        public void Insert(Alert entity)
        {
            _context.Alerts.InsertOneAsync(entity);
        }


        public void Delete(string id)
        {
            //var filter = new BsonDocument("_id", ObjectId.Parse(id));
            //var result = _context.Users.DeleteOne(filter);
            var result = _context.Alerts.DeleteOne(x => x.Id == id);
        }


        public async Task Update(Alert entity)
        {
            var filter = Builders<Alert>.Filter.Eq(s => s.Id, entity.Id);
            await _context.Alerts.ReplaceOneAsync(filter, entity);
        }

        public async Task<bool> Acknowledge(string alertId)
        {
            var collection = _context.Alerts;
            var filter = Builders<Alert>.Filter.Eq("_id", alertId);
            var update = Builders<Alert>.Update
                .Set(x => x.Triggered, false);
            var result = await collection.UpdateOneAsync(filter, update);;
            return true;
        }
        /*
        public async Task<bool> TriggerValueAlert(string id, decimal value)
        {
            var collection = _context.Alerts;
            var filter = Builders<ValueAlert>.Filter.Eq("_id", id);
            var update = Builders<ValueAlert>.Update
                .Set(x => x.Triggered, true)
                .Set(x => x.TriggeredByValue, value)
                .Set(x => x.TriggeredAt, DateTime.UtcNow);
            var updateResult = collection.UpdateOneAsync(filter, update).Result;
        }
        */

        /*
                public async Task<bool> UpdateWithLatest(string deviceId, DateTime timeStamp, List<DataPostDTO.DataPoint> dataPoints)
                {
                    var collection = _context.Devices;
                    var filter = Builders<Device>.Filter.Eq("DeviceId", deviceId);
                    var update = Builders<Device>.Update
                        .Set("LastSeen", timeStamp);

                    var result = await collection.UpdateOneAsync(filter, update);

                    //TODO: fix this so we only do one update...
                    foreach (var dataPoint in dataPoints)
                    {
                        filter = Builders<Device>.Filter.Where(x => x.DeviceId == deviceId && x.Topics.Any(i => i.Id == dataPoint.Topic));
                        update = Builders<Device>.Update
                            .Set(x => x.Topics[-1].Value, dataPoint.Value)
                            .Set(x => x.Topics[-1].ValueTimeStamp, timeStamp);
                        var updateResult = collection.UpdateOneAsync(filter, update).Result;
                    }

                    return true;
                }
        */


    }
}

