using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public interface IUserRepository
    {
        /*
        IEnumerable<User> Get(
            Expression<Func<User, bool>> filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null,
            string includeProperties = ""
        );
        */

        IEnumerable<User> Get();
        IEnumerable<User> GetForClient(string clientId);
        User GetByID(string id);
        User GetByUserName(string userName);
        void Insert(User entity);
        void Delete(string id);
        //void Delete(User entity);
        void Update(User entity);

        void AddFavorite(string userId, TopicIdentifier topic);
        void DeleteFavorite(string userId, TopicIdentifier topic);
    }
}

