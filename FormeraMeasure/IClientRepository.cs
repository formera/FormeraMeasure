using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public interface IClientRepository
    {
        IEnumerable<Client> Get();
        Client GetByID(string id);
        void Insert(Client entity);
        void Delete(string id);
        void Update(Client entity);
    }
}

