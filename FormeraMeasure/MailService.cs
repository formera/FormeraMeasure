using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FormeraMeasure.Options;


namespace FormeraMeasure
{

    public class MailGunService : IMailService
    {
        public MailGunService(IOptions<MailGunSettings> settings)
        {
            _domain = settings.Value.Domain;
            _apiKey = settings.Value.API_Key;
        }

        private string _domain;
        private string _apiKey;

        public async Task<bool> SendAsync(string from, string to, string subject, string message)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes("api" + ":" + _apiKey)));

            var form = new Dictionary<string, string>();
            form["from"] = from;
            form["to"] = to;
            form["subject"] = subject;
            form["text"] = message;

            var response = await client.PostAsync("https://api.mailgun.net/v3/" + _domain + "/messages", new FormUrlEncodedContent(form));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Debug.WriteLine("Success");
                return true;
            }
            else
            {
                Debug.WriteLine("StatusCode: " + response.StatusCode);
                Debug.WriteLine("ReasonPhrase: " + response.ReasonPhrase);
                return false;
            }
        }
    }
}
