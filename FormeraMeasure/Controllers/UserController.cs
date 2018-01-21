using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FormeraMeasure.Options;
using FormeraMeasure.Models;
using System.Text;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel;


namespace FormeraMeasure.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IUserRepository userRepository;
        private IClientRepository clients;
        private PasswordSettings _passwordSettings;
        private IMapper _mapper;

        public UserController(IUserRepository userRepo, IOptions<PasswordSettings> passwordSettings, IMapper mapper, IClientRepository clientRepo)
        {
            userRepository = userRepo;
            _passwordSettings = passwordSettings.Value;
            _mapper = mapper;
            clients = clientRepo;
        }

        private void UpdatePasswordOnUser(User user, string password)
        {
            var salt = Hasher.GenerateSalt(_passwordSettings.SaltLength);
            var pass = Encoding.ASCII.GetBytes(password);
            var hash = Convert.ToBase64String(Hasher.GenerateHash(pass, salt, _passwordSettings.Iterations, _passwordSettings.HashLength));

            user.Iterations = _passwordSettings.Iterations;
            user.Salt = salt;
            user.Hash = hash;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IEnumerable<UserInfo> Get()
        {
            var users = userRepository.Get().ToList();

            List<UserInfo> result = new List<UserInfo>();
            foreach (User user in users)
            {
                var userInfo = _mapper.Map<User, UserInfo>(user);
                result.Add(userInfo);
            }

            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (id != User.Identity.Name)
                return Forbid();

            var user = userRepository.GetByID(id);
            return Ok(_mapper.Map<User, UserInfo>(user));
        }

        // POST api/values
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddUser([FromBody]AddUserDTO entity)
        {
            var exists = userRepository.GetByUserName(entity.UserName.ToLower()) != null;
            if(exists)
            {
                return BadRequest("User already exists");
            }

            User user = new Models.User();
            user.UserName = entity.UserName.ToLower();
            user.FirstName = entity.FirstName;
            user.LastName = entity.LastName;
            user.Email = entity.Email;
            user.Description = entity.Description;
            user.LogoBase64 = entity.LogoBase64;
            user.Admin = entity.Admin;
            user.ClientId = entity.ClientId;

            UpdatePasswordOnUser(user, entity.Password);

            userRepository.Insert(user);

            return StatusCode(201);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public void Put(string id, [FromBody]User entity)
        {
            var user = userRepository.GetByID(id);
            user.FirstName = entity.FirstName;
            user.LastName = entity.LastName;
            user.Email = entity.Email;
            user.ClientId = entity.ClientId;
            user.UserName = entity.UserName.ToLower();

            userRepository.Update(user);
        }

        // PUT api/values/5
        [HttpPut("{id}/password")]
        [Authorize]
        public IActionResult UpdatePassword(string id, [FromBody]PasswordUpdateDTO entity)
        {
            var user = userRepository.GetByID(id);
            if(user == null)
            {
                return NotFound();
            }

            UpdatePasswordOnUser(user, entity.Password);
            userRepository.Update(user);
            return Ok();
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public void Delete(string id)
        {
            userRepository.Delete(id);
        }
    }
}
