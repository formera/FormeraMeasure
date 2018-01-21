using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FormeraMeasure.Models;

namespace FormeraMeasure
{

    public interface IDataRepository
    {
        IEnumerable<DataDocument> Get();
        DataDocument GetByID(string id);
        void Insert(DataDocument entity);
        void Delete(string id);
        Task Update(DataDocument entity);
        Task<bool> Upsert(DateTime timeStampHour, string ownerId, string deviceId, string topic, int minute, string value);
        Task<IEnumerable<DataDocument>> Get(string deviceId, string ownerId, string topic, DateTime startUtc, DateTime stopUtc);
    }

}

