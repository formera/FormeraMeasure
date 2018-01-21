using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormeraMeasure
{

    public interface IEventProcessorService
    {
        Task<bool> ProccessDataPoint(string clientId, string deviceId, string topic, decimal value);
    }

}
