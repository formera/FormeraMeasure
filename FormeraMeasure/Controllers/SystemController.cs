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
    [Route("api/[controller]/{systemId}")]
    public class SystemController : Controller
    {
        private IDeviceRepository devices;
        private IUserRepository users;

        public SystemController( IDeviceRepository deviceRepo, IUserRepository userRepo)
        {
            devices = deviceRepo;
            users = userRepo;
        }


        [HttpGet("devices")]
        public async Task<IActionResult> Get(string systemId)
        {
            string authSys = ((ClaimsIdentity)User.Identity).FindFirst("sys").Value;
            if (systemId != authSys && authSys != "*")
                return Forbid();

            var result = await devices.GetAllByClientId(systemId);
            return Ok(result);
        }
        

        [HttpGet("devices/{id}")]
        public async Task<IActionResult> Get(string systemId, string id)
        {
            string authSys = ((ClaimsIdentity)User.Identity).FindFirst("sys").Value;
            if (systemId != authSys && authSys != "*")
                return Forbid();

            var result = await devices.GetByID(id);

            //TODO: Do find by system and device id instead and return not found?
            if(result.ClientId!=systemId)
                return Forbid();

            return Ok(result);
        }
/*
        // POST api/values
        [HttpPost]
        public void Post(string systemId, [FromBody]Device entity)
        {
            //var user= users.GetByID(User.Identity.Name);
            devices.Insert(entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string systemId, [FromBody]Device entity)
        {
            devices.Update(entity);
        }
        
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            devices.Delete(id);
        }
        */
    }
}
