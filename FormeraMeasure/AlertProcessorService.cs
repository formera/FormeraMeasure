using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
/*    public class DataPoint
    {
        public string ownerId { get; set; }
        public string deviceId { get; set; }
        public string topic { get; set; }
        public DateTime timeStampHour { get; set; }
        public decimal value { get; set; }
    }*/

    public class AlertProcessorService : IEventProcessorService
    {
        private IDeviceRepository devices;
        private IAlertRepository alerts;
        private IClientRepository clients;
        private ISlackService slack;
        private IMailService mail;

        public AlertProcessorService(IClientRepository clientRepo, IDeviceRepository deviceRepo, IAlertRepository alertRepo, ISlackService slackService, IMailService mailService)
        {
            clients = clientRepo;
            alerts = alertRepo;
            devices = deviceRepo;
            mail = mailService;
            slack = slackService;
        }

        public async Task<bool> ProccessDataPoint(string clientId, string deviceId, string topic, decimal value)
        {
            var a = await alerts.GetAllByDeviceId(clientId, deviceId);
            foreach (var alert in a)
            {
                if (alert is ValueAlert)
                {
                    var valueAlert = (ValueAlert)alert;
                    bool trigger = false;
                    if (valueAlert.Topic == topic)
                    {
                        //TODO: Not here...
                        switch (valueAlert.ComparisonType)
                        {
                            case ValueAlert.ComparisonTypes.greaterThan:
                                if (value > valueAlert.Value)
                                {
                                    //Trigger
                                    trigger = true;
                                }
                                break;
                            case ValueAlert.ComparisonTypes.lessThan:
                                if (value < valueAlert.Value)
                                {
                                    //Trigger
                                    trigger = true;
                                }
                                break;
                        }
                    }
                    if(trigger)
                    {
                        if (alert.Triggered)
                        {
                            Console.WriteLine("Already triggered");
                        }
                        else
                        {
                            Console.WriteLine("Triggering alert");
                            valueAlert.Triggered = true;
                            valueAlert.TriggeredAt = DateTime.UtcNow;
                            valueAlert.TriggeredByValue = value;
                            await alerts.Update(alert);

                            //TODO: Move to service
                            var client = clients.GetByID(clientId);
                            foreach (var notifyId in alert.NotifierIds)
                            {
                                var notifier = client.NotificationSystems.Find(x => x.Id == notifyId);
                                if (notifier != null) {

                                    if (notifier is SlackNotificationSystem)
                                    {
                                        //TODO: How to correctly make this testable and use DI?
                                        var slackSettings = (SlackNotificationSystem)notifier;
                                        slack.SendTestMessge(slackSettings.Url, String.Format("Alert triggered with value {0} on {1}/{2}", value, deviceId, topic));
                                    }
                                    else if (notifier is EmailNotificationSystem)
                                    {
                                        var emailSettings = (EmailNotificationSystem)notifier;
                                        string to = String.Join(";", emailSettings.Recipients);
                                        await mail.SendAsync("no-reply@formera.net", to, "Formera Alert", String.Format("Alert triggered with value {0} on {1}/{2}", value, deviceId, topic));
                                    }
                                    //else
                                    //return StatusCode(500, "unknown notificatioh type");

                                }
                            }

                        }
                    }
                }
            }
            
            return true;
        }
    
    }
}
