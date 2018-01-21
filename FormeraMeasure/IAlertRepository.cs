using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public interface IAlertRepository
    {
        Task<IEnumerable<Alert>> GetAllByClientId(string clientId);
        Task<IEnumerable<Alert>> GetAllByDeviceId(string clientId, string deviceId);
        IEnumerable<Alert> Get();
        Task<Alert> GetByID(string id);
        void Insert(Alert entity);
        void Delete(string id);
        Task Update(Alert entity);

        Task<bool> Acknowledge(string alertId);
        //Task<bool> TriggerValueAlert(string id, decimal value);
    }
}

