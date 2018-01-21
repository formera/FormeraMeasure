using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using FormeraMeasure.Options;
using FormeraMeasure.Models;
using System.Text;
using AutoMapper;


namespace FormeraMeasure.Controllers
{
    [Route("api/user/{userId}/[controller]")]
//    [Route("api/user/[controller]")]
    public class FavoritesController : Controller
    {
        private IUserRepository userRepository;
        private IClientRepository clients;
        private IDeviceRepository devices;
        private IMapper _mapper;

        public FavoritesController(IUserRepository userRepo, IMapper mapper, IClientRepository clientRepo, IDeviceRepository deviceRepo)
        {
            userRepository = userRepo;
            _mapper = mapper;
            clients = clientRepo;
            devices = deviceRepo;
        }


        [HttpGet]
        public IEnumerable<TopicIdentifier> Get(string userId)
        {
            var user = userRepository.GetByID(userId);//.Get().ToList();
            return user.Favorites;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> AddFavorite(string userId, [FromBody]TopicIdentifier entity)
        {
            var user = userRepository.GetByID(userId);
            var device = await devices.GetByID(entity.DeviceId);
            if (device == null)
            {
                return BadRequest("Device does not exists");
            }
            var topic = device.Topics.Find(x => x.Id == entity.Topic);
            if( topic==null )
            {
                return BadRequest("Topic does not exists on device");
            }

            //TODO: Check if exists
            var exists = user.Favorites.Find(x => x.DeviceId == entity.DeviceId && x.Topic == entity.Topic);
            if(exists != null)
            {
                return BadRequest("Already set as favourite");
            }
            userRepository.AddFavorite(userId, entity);
            return Ok();
        }

        [HttpDelete]
        public IActionResult RemoveFavorite(string userId, [FromBody]TopicIdentifier topic)
        {
            userRepository.DeleteFavorite(userId, topic);
            return Ok();
        }

       
    }
}
