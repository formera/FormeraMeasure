using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAllByClientId(string clientId);
        IEnumerable<Device> Get();
        Task<Device> GetByID(string id);
        void Insert(Device entity);
        void Delete(string id);
        Task Update(Device entity);

        Task<bool> UpdateWithLatest(string deviceId, DateTime timeStamp, List<DataPostDTO.DataPoint> dataPoints);
    }
}

