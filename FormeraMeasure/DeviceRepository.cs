using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public class DeviceRepository : IDeviceRepository 
    {
        private MongoContext _context;

        public DeviceRepository(MongoContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Device>> GetAllByClientId(string clientId)
        {
            var result = await _context.Devices.FindAsync(x => x.ClientId == clientId);
            return result.ToList();
        }

        public IEnumerable<Device> Get()
        {
            var mongoFilter = new BsonDocument();
            return _context.Devices.Find(mongoFilter).ToList();
        }

        public async Task<Device> GetByID(string id)
        {
            //var filter = Builders<User>.Filter.Eq("_id", id);
            //return _context.Users.Find(filter).FirstOrDefault();
            return await _context.Devices.FindSync(x => x.Id == id).FirstAsync();
        }
        

        public void Insert(Device entity)
        {
            _context.Devices.InsertOneAsync(entity);
        }


        public void Delete(string id)
        {
            //var filter = new BsonDocument("_id", ObjectId.Parse(id));
            //var result = _context.Users.DeleteOne(filter);
            var result = _context.Devices.DeleteOne(x => x.Id == id);
        }


        public async Task Update(Device entity)
        {
            var filter = Builders<Device>.Filter.Eq(s => s.Id, entity.Id);
            await _context.Devices.ReplaceOneAsync(filter, entity);
        }


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



    }
}

