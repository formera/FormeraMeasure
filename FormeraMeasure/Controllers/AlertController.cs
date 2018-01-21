using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FormeraMeasure.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace FormeraMeasure.Controllers
{
    [Route("api/{systemId}/alerts")]
    public class AlertsController : Controller
    {
        private IDeviceRepository devices;
        private IUserRepository users;
        private IAlertRepository alerts;

        public AlertsController( IDeviceRepository deviceRepo, IUserRepository userRepo, IAlertRepository alertRepo)
        {
            devices = deviceRepo;
            users = userRepo;
            alerts = alertRepo;
        }


        [HttpGet]
        public async Task<IEnumerable<Alert>> Get(string systemId)
        {
            var result = await alerts.GetAllByClientId(systemId);
            return result;
        }

        [HttpGet("{id}")]
        public async Task<Alert> Get(string systemId, string id)
        {
            var result = await alerts.GetByID(id);
            return result;
        }

        // POST api/values
        [HttpPost]
        public void Post(string systemId, [FromBody]Alert entity)
        {
            entity.ClientId = systemId;
            //var user= users.GetByID(User.Identity.Name);
            alerts.Insert(entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string systemId, string id, [FromBody]Alert entity)
        {
            //TODO: Resource validate
            entity.ClientId = systemId;
            entity.Id = id;
            alerts.Update(entity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string systemId, string id)
        {
            alerts.Delete(id);
        }


        //TODO: Fix endpoint name
        [HttpPost("{id}/ack")]
        public async void Post(string systemId, string id)
        {
            var alert = await alerts.GetByID(id);
            alert.Triggered = false;
            await alerts.Update(alert);
            //alerts.Acknowledge(id);
        }

    }
}
