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
    public class UserRepository : IUserRepository 
    {
        private MongoContext _context;

        public UserRepository(MongoContext context)
        {
            _context = context;
        }

        public IEnumerable<User> Get()
        {
            var mongoFilter = new BsonDocument();
            return _context.Users.Find(mongoFilter).ToList();
        }

        public User GetByID(string id)
        {
            //var filter = Builders<User>.Filter.Eq("_id", id);
            //return _context.Users.Find(filter).FirstOrDefault();
            return _context.Users.Find(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<User> GetForClient(string id)
        {
            return _context.Users.Find(x => x.ClientId == id).ToList();
        }

        public User GetByUserName(string username)
        {
            var filter = Builders<User>.Filter.Eq("UserName", username);
            //var result = _context.Users.AsQueryable().Where(usr => usr.UserName.ToLower().Equals(username));
            //var list = result.ToList();
            //return result.FirstOrDefault();
            return _context.Users.Find(filter).FirstOrDefault();
        }

        public void Insert(User entity)
        {
            _context.Users.InsertOneAsync(entity);
        }

        public void Delete(string id)
        {
            //var filter = new BsonDocument("_id", ObjectId.Parse(id));
            //var result = _context.Users.DeleteOne(filter);
            var result = _context.Users.DeleteOne(x => x.Id == id);
        }

        public void Update(User entity)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, entity.Id);
            _context.Users.ReplaceOne(filter, entity);
        }

        public void AddFavorite(string userId, TopicIdentifier topic)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, userId);
            var update = Builders<User>.Update.Push(x => x.Favorites, topic );
            _context.Users.UpdateOne(filter, update);
        }

        public void DeleteFavorite(string userId, TopicIdentifier topic)
        {
            var update = Builders<User>.Update.PullFilter(p => p.Favorites,
                                                 f => f.DeviceId == topic.DeviceId && f.Topic == topic.Topic);
            var result =  _context.Users
                .FindOneAndUpdate(p => p.Id == userId, update);
        }
    }
}

