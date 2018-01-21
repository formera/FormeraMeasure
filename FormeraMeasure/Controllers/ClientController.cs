using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FormeraMeasure.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace FormeraMeasure.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private IClientRepository clients;
        private IUserRepository users;
        private IMapper _mapper;

        public ClientController(IClientRepository clientRepo, IUserRepository userRepo, IMapper mapper, ILoggerFactory loggerFactory)
        {
            clients = clientRepo;
            users = userRepo;
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<Client> Get()
        {
            var result = clients.Get().ToList();
            return result;
        }


        [HttpGet("{id}")]
        public Client Get(string id)
        {
            var result = clients.GetByID(id);
            return result;
        }


        [HttpPost]
        public void Post([FromBody]Client entity)
        {
            clients.Insert(entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put([FromBody]Client entity)
        {
            clients.Update(entity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            clients.Delete(id);
        }

    }
}
