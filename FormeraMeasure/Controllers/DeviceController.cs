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
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class DeviceController : Controller
    {
        private IDeviceRepository devices;
        private IUserRepository users;

        public DeviceController( IDeviceRepository deviceRepo, IUserRepository userRepo)
        {
            devices = deviceRepo;
            users = userRepo;
        }


        [HttpGet]
        public IEnumerable<Device> Get()
        {
            var result = devices.Get().ToList();
            return result;
        }

        [HttpGet("{id}")]
        public async Task<Device> Get(string id)
        {
            var result = await devices.GetByID(id);
            return result;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Device entity)
        {
            //var user= users.GetByID(User.Identity.Name);
            devices.Insert(entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put([FromBody]Device entity)
        {
            devices.Update(entity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            devices.Delete(id);
        }
    }
}
