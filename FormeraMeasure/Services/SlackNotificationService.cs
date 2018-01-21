using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using FormeraMeasure.Options;

namespace FormeraMeasure
{
    public class SlackNotificationService : ISlackService
    {
        private string _webHookUrl;
        private SlackClient _slack;

        public SlackNotificationService()
        {
          //  _webHookUrl = slackSettings.Value.WebHookUrl;
            //_slack = new SlackClient(new Uri(_webHookUrl));
        }

        public async void SendTestMessge(string webHookUrl, string msg)
        {
            _webHookUrl = webHookUrl;
            _slack = new SlackClient(new Uri(_webHookUrl));
            await _slack.SendMessageAsync(msg);
        }

    }
      
}



