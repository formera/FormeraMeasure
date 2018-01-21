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
    [Route("api/{clientId}/notifications")]
    [Authorize(Policy = "AdminOnly")]
    public class NotificationsController : Controller
    {
        private IClientRepository clients;
        private IUserRepository users;
        private ISlackService slack;
        private IMailService mail;

        public NotificationsController( IClientRepository clientRepo, IUserRepository userRepo, ISlackService slackService, IMailService mailService)
        {
            clients = clientRepo;
            users = userRepo;
            mail = mailService;
            slack = slackService;
        }

        // POST 
        [HttpPost("{id}")]
        public IActionResult Post(string clientId, string id)
        {
            var client = clients.GetByID(clientId);
            var notifier = client.NotificationSystems.Find(x => x.Id == id);
            if (notifier == null)
                return NotFound();

            if (notifier is SlackNotificationSystem)
            {
                //TODO: How to correctly make this testable and use DI?
                var slackSettings = (SlackNotificationSystem)notifier;
                slack.SendTestMessge(slackSettings.Url, "Test message from slack notification system");
                return Ok();
            }
            else if (notifier is EmailNotificationSystem)
            {
                var emailSettings = (EmailNotificationSystem)notifier;
                string to = String.Join(";", emailSettings.Recipients);
                mail.SendAsync("no-reply@formera.net", to, "Test message", "Test message from Formera Measure");
                return Ok();
            }
            else
                return StatusCode(500, "unknown notificatioh type");
        }

    }
}
