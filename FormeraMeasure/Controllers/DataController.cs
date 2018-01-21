using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FormeraMeasure.Models;
using System.Security.Claims;
using System.IdentityModel;

namespace FormeraMeasure.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        //   private IDataRepository devices;
        private IDataRepository dataDocuments;
        private IDeviceRepository devices;
        private IUserRepository users;
        private IEventProcessorService eventService;

        public DataController(IDataRepository dataRepo, IDeviceRepository deviceRepo, IUserRepository userRepo, IEventProcessorService eventProcessor)
        {
            dataDocuments = dataRepo;
            devices = deviceRepo;
            users = userRepo;
            eventService = eventProcessor;
        }

        [HttpGet]
        public async Task<List<DataPointDTO>> Get([FromQuery] DataQueryParameters query)
        {
            //TODO: Check model valid
            var device = await devices.GetByID(query.DeviceId);

            if (query.AggregationType == null)
            {
                query.AggregationType = DataQueryParameters.AggregationTypes.one_minute;
            }

            DateTime from = query.From.ToUniversalTime();
            DateTime to = DateTime.UtcNow;
            if (query.To != null)
            {
                to = ((DateTime)query.To).ToUniversalTime();
            }

            //Get all documents needed to cover time span
            var docHour = from.Date.AddHours(from.Hour);
            var docs = await dataDocuments.Get(device.Id, device.ClientId, query.Topic, docHour, to);
            var result = new List<DataPointDTO>();
            foreach (var doc in docs)
            {
                var points = doc.Data.Where(x => x.Value.ToLower() != "nan")
                    .Select(x => new DataPointDTO
                    {
                        TimeStamp = doc.TimeStampHour.AddMinutes(int.Parse(x.Key)),
                        Value = Decimal.Parse(x.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo)
                    }
                );

                result = result.Concat(points).ToList();
            }
            //Filter documents on time span
            var dataPoints = result.Where(x => x.TimeStamp >= from && x.TimeStamp < to);
            return dataPoints.ToList();
        }


        [HttpGet("aggr")]
        public async Task<List<DataPointAggrDTO>> Aggr([FromQuery] DataQueryParameters query)
        {
            //TODO: Check model valid
            var device = await devices.GetByID(query.DeviceId);

            if (query.AggregationType == null)
            {
                query.AggregationType = DataQueryParameters.AggregationTypes.one_minute;
            }

            DateTime from = query.From.ToUniversalTime();
            DateTime to = DateTime.UtcNow;
            if (query.To != null)
            {
                to = ((DateTime)query.To).ToUniversalTime();
            }

            //Get all documents needed to cover time span
            var docHour = from.Date.AddHours(from.Hour);
            var docs = await dataDocuments.Get(device.Id, device.ClientId, query.Topic, docHour, to);
            var result = new List<DataPointDTO>();
            foreach (var doc in docs)
            {
                var points = doc.Data.Where(x=>x.Value.ToLower() != "nan")
                    .Select(x => new DataPointDTO
                        {
                            TimeStamp = doc.TimeStampHour.AddMinutes(int.Parse(x.Key)),
                            Value = Decimal.Parse(x.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo)
                        }
                );

                result = result.Concat(points).ToList();
            }
            //Filter documents on time span
            var dataPoints = result.Where(x => x.TimeStamp >= from && x.TimeStamp < to);

            int rounding = 1;
            if (query.AggregateTo != null)
            {
                rounding = (int)query.AggregateTo;
            }
            
            var aggregated = dataPoints.GroupBy(s => GetShiftedTimeStamp(s.TimeStamp, rounding))
                .Select(g => new DataPointAggrDTO
                {
                    TimeStamp = g.Key,
                    Avg = Math.Round(g.Average(x => x.Value), 2),
                    Min = Math.Round(g.Min(x => x.Value),2),
                    Max = Math.Round(g.Max(x => x.Value),2)
                });

            return aggregated.ToList();
        }


        public DateTime GetShiftedTimeStamp(DateTime timeStamp, int minutes)
        {
            return
                new DateTime(
                    Convert.ToInt64(
                        Math.Round(timeStamp.Ticks / (decimal)TimeSpan.FromMinutes(minutes).Ticks, 0, MidpointRounding.AwayFromZero)
                            * TimeSpan.FromMinutes(minutes).Ticks));
        }


        [HttpPost]
        [Authorize(Policy = "PostData")]
        public async Task<IActionResult> Post([FromBody]DataPostDTO entity)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var tokenId = User.Identity.Name;
            //string sub = ihdentity.FindFirst(identity.NameClaimType).Value;
            //UserManager
            /*if (entity.DeviceId != tokenId)
            {
                return BadRequest("Post data does not match token");
            }*/

            var device = await devices.GetByID(tokenId);
            var now = DateTime.UtcNow;
            var timeStampHour = now.Date.AddHours(now.Hour);

            Console.WriteLine("Post data from deviceId {0}", tokenId);
            foreach (var datapoint in entity.Data)
            {
                Console.WriteLine(" - {0} : {1}", datapoint.Topic, datapoint.Value);
                await dataDocuments.Upsert(timeStampHour, device.ClientId, tokenId, datapoint.Topic, now.Minute, datapoint.Value);

                foreach(var topic in device.Topics.FindAll(x => x.Id == datapoint.Topic))
                {
                    topic.ValueTimeStamp = now;
                    topic.Value = datapoint.Value;
                }

                try
                {
                    var value = Decimal.Parse(datapoint.Value, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
                    await eventService.ProccessDataPoint(device.ClientId, tokenId, datapoint.Topic, value);
                }
                catch (Exception) { };
            }

            device.LastSeen = now;
            await devices.Update(device);

            //TODO: We should to this using updates instead of replace
            //await devices.UpdateWithLatest(device.DeviceId, now, entity.Data);
            return Ok();
        }

    }
}
