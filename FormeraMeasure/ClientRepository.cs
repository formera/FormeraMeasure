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
    public class ClientRepository : IClientRepository 
    {
        private MongoContext _context;

        public ClientRepository(MongoContext context)
        {
            _context = context;
        }

        public IEnumerable<Client> Get()
        {
            var mongoFilter = new BsonDocument();
            return _context.Clients.Find(mongoFilter).ToList();
        }

        public Client GetByID(string id)
        {
            //var filter = Builders<User>.Filter.Eq("_id", id);
            //return _context.Users.Find(filter).FirstOrDefault();
            return _context.Clients.Find(x => x.Id == id).FirstOrDefault();
        }
        

        public void Insert(Client entity)
        {
            _context.Clients.InsertOneAsync(entity);
        }

        public void Delete(string id)
        {
            //var filter = new BsonDocument("_id", ObjectId.Parse(id));
            //var result = _context.Users.DeleteOne(filter);
            var result = _context.Clients.DeleteOne(x => x.Id == id);
        }

        public void Update(Client entity)
        {
            var filter = Builders<Client>.Filter.Eq(s => s.Id, entity.Id);
            _context.Clients.ReplaceOne(filter, entity);
        }

    }
}

