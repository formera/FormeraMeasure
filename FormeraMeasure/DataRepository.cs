using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public class DataRepository : IDataRepository 
    {
        private MongoContext _context;

        public DataRepository(MongoContext context)
        {
            _context = context;
        }

        public IEnumerable<DataDocument> Get()
        {
            var mongoFilter = new BsonDocument();
            return _context.DataDocuments.Find(mongoFilter).ToList();
        }

        public DataDocument GetByID(string id)
        {
            //var filter = Builders<User>.Filter.Eq("_id", id);
            //return _context.Users.Find(filter).FirstOrDefault();
            return _context.DataDocuments.Find(x => x.Id == id).FirstOrDefault();
        }
        

        public void Insert(DataDocument entity)
        {
            _context.DataDocuments.InsertOneAsync(entity);
        }

        public void Delete(string id)
        {
            var result = _context.DataDocuments.DeleteOne(x => x.Id == id);
        }

        public async Task Update(DataDocument entity)
        {
            var filter = Builders<DataDocument>.Filter.Eq(s => s.Id, entity.Id);
            await _context.DataDocuments.ReplaceOneAsync(filter, entity);
        }


        public async Task<bool> Upsert(DateTime timeStampHour, string ownerId, string deviceId, string topic, int minute, string value)
        {
            var collection = _context.DataDocuments; // _context._database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<DataDocument>.Filter.Eq("TimeStampHour", timeStampHour) &
                         Builders<DataDocument>.Filter.Eq("OwnerId", ownerId) &
                         Builders<DataDocument>.Filter.Eq("DeviceId", deviceId) &
                         Builders<DataDocument>.Filter.Eq("Topic", topic );

            var dataItem = "data." + minute.ToString(); 
            var update = Builders<DataDocument>.Update.Set(dataItem, value);
            var result = await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true } );

            return true;
        }




        public async Task<IEnumerable<DataDocument>> Get(string deviceId, string ownerId, string topic, DateTime startUtc, DateTime stopUtc)
        {
            var q = _context.DataDocuments.AsQueryable();
            var query = q.Where(x =>
                 x.DeviceId == deviceId  &&
                 x.OwnerId == ownerId &&
                 x.Topic == topic &&
                 x.TimeStampHour >= startUtc &&
                 x.TimeStampHour <= stopUtc
            ).OrderBy(x => x.TimeStampHour);
            
            return await query.ToListAsync();
        }


        /*
                public async Task<long> UpdateTest(DataDocument data)
                {
                    var collection = _context.DataDocuments;
                    var filter = Builders<DataDocument>.Filter.Eq("name", "Juni");
                    var update = Builders<DataDocument>.Update
                        .Set("test", "test (New)")
                        .CurrentDate("lastModified");
                    var result = await collection.UpdateOneAsync(filter, update);
                    return result.ModifiedCount;
                }
        */
    }
}

