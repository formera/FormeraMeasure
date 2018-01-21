using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FormeraMeasure.Models;
using FormeraMeasure.Options;
using System.Text;

namespace FormeraMeasure.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly PasswordSettings _passwordSettings;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly IUserRepository _users;
        private readonly IDeviceRepository _devices;

        public AuthController(IOptions<JwtIssuerOptions> jwtOptions, IDeviceRepository deviceRepo, IUserRepository userRepo, IOptions<PasswordSettings> passwordSettings, ILoggerFactory loggerFactory)
        {
            _users = userRepo;
            _devices = deviceRepo;

            _jwtOptions = jwtOptions.Value;
            _passwordSettings = passwordSettings.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<AuthController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Credentials applicationUser)
        {
            applicationUser.UserName = applicationUser.UserName.ToLower();

            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({applicationUser.UserName}) or password ({applicationUser.Password})");
                return BadRequest("Invalid credentials");
            }

            var userType = identity.FindFirst("UserType");
            bool isAdmin = false;
            if(userType != null && userType.Value == "Admin")
            {
                isAdmin = true;
            }

            var claims = new[]
            {
//                new Claim(System.Security.Claims.ClaimTypes.Name, identity.Name),
                new Claim(JwtRegisteredClaimNames.Sub, identity.Name),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("UserType"),
                identity.FindFirst("email"),
                identity.FindFirst("sys"),
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();            //TODO: This will stop microsoft from remapping the sub claim to something completetly different
            handler.OutboundClaimTypeMap.Clear();
            var encodedJwt = handler.WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                user = identity.Name,
                system = identity.FindFirst("sys").Value,
                accessToken = encodedJwt,
                expiresIn = (int)_jwtOptions.ValidFor.TotalSeconds,
                adminMode = isAdmin
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeviceToken([FromQuery] string deviceId)
        {
            var device = await _devices.GetByID(deviceId);

            var claims = new[]
            {
//              new Claim(System.Security.Claims.ClaimTypes.Name, identity.Name),
                new Claim(JwtRegisteredClaimNames.Sub, deviceId),
//                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim("type", "device" )
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
//                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.ExpirationPost,
                signingCredentials: _jwtOptions.SigningCredentials);

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();            //TODO: This will stop microsoft from remapping the sub claim to something completetly different
            handler.OutboundClaimTypeMap.Clear();
            var encodedJwt = handler.WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                user = deviceId,
                accessToken = encodedJwt,
                expiresIn = (int)_jwtOptions.ValidForPost.TotalSeconds,
                role = "DevicePost"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }


        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private Task<ClaimsIdentity> GetClaimsIdentity(Credentials credentials)
        {
            if (credentials.UserName == "root" && credentials.Password == "iamroot")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity("RootAdmin", "Token"),
                  new[]
                  {
                    new Claim("UserType", "Admin")
                  }));
            }

            var user = _users.GetByUserName(credentials.UserName);
            if (user == null)
                return Task.FromResult<ClaimsIdentity>(null);

            var salt = user.Salt;
            var pass = Encoding.ASCII.GetBytes(credentials.Password);
            var hash = Convert.ToBase64String(Hasher.GenerateHash(pass, salt, user.Iterations, _passwordSettings.HashLength));

            if (user.Hash == hash)
            {
                user.LastLogin = DateTime.Now;
                _users.Update(user);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("email", user.Email));
                if (user.Admin)
                {
                    claims.Add(new Claim("UserType", "Admin"));
                }
                if(user.ClientId == null)
                {
                    claims.Add(new Claim("sys", "*"));
                }
                else
                {
                    claims.Add(new Claim("sys", user.ClientId));
                }

                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.Id, "Token"), claims));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}