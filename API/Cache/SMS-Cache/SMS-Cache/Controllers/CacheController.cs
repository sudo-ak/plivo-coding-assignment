using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SMSCache.Model;

namespace SMSCache.Controllers
{
    [Produces("application/json")]
    [Route("api/Cache")]
    public class CacheController : Controller
    {
        IMemoryCache _memoryCache;

        public CacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [Route("Inbound/{from}/{to}")]
        [HttpGet]
        public ActionResult GetInbound(string from, string to)
        {
            Response response = new Response();
            string value = string.Empty;
            string key = from + "-" + to;

            try
            {
                if (!_memoryCache.TryGetValue(key, out value))
                {
                    value = "Cache is expired or not available";
                }

                response.Success = true;
                response.Message = value;
                response.Error = "";
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "";
                response.Error = ex.Message;
            }

            return CreatedAtAction("GetInbound", response);
        }

        [Route("Outbound/{from}")]
        [HttpGet]
        public ActionResult GetOutboundSMSCountCache(string from)
        {
            Response response = new Response();
            var key = from;
            int outboundSMSCount = 0;

            try
            {
                if (!_memoryCache.TryGetValue(key, out outboundSMSCount))
                {
                    outboundSMSCount = 0;
                }

                response.Success = true;
                response.Message = outboundSMSCount.ToString();
                response.Error = "";
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "";
                response.Error = ex.Message;
            }

            return CreatedAtAction("GetOutboundSMSCountCache", response);
        }

        [Route("Inbound")]
        [HttpPost]
        public ActionResult SetInboundSMSCache([FromBody] SetInboundCache inboundCache)
        {
            Response response = new Response();
            var modelErrors = new List<string>();

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                response.Success = false;
                response.Message = "";
                response.Error = string.Join(",", modelErrors);
            }
            else
            {
                var key = inboundCache.From + "-" + inboundCache.To;
                try
                {
                    MemoryCacheEntryOptions cacheOption = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = (DateTime.Now.AddMinutes(240) - DateTime.Now)
                    };

                    _memoryCache.Set(key, cacheOption.AbsoluteExpirationRelativeToNow.ToString(), cacheOption);

                    response.Success = true;
                    response.Message = "Cache entry inserted successfully for key: " + key;
                    response.Error = "";
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "";
                    response.Error = ex.Message;
                }
            }

            return CreatedAtAction("SetInboundSMSCache", response);
        }

        [Route("Outbound")]
        [HttpPost]
        public ActionResult SetOutboundSMSCountCache([FromBody] SetOutboundCache outboundCache)
        {
            Response response = new Response();
            var modelErrors = new List<string>();

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                response.Success = false;
                response.Message = "";
                response.Error = string.Join(",", modelErrors);
            }
            else
            {
                var key = outboundCache.From;
                int outboundSMSCount = 0;

                try
                {
                    if (!_memoryCache.TryGetValue(key, out outboundSMSCount))
                    {

                    }

                    if (outboundSMSCount == 0)
                    {
                        MemoryCacheEntryOptions cacheOption = new MemoryCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = (DateTime.Now.AddMinutes(1440) - DateTime.Now)
                        };

                        _memoryCache.Set(key, 1, cacheOption);
                    }
                    else
                    {
                        int messageCount = outboundSMSCount + 1;
                        _memoryCache.Set(key, messageCount);
                    }

                    response.Success = true;
                    response.Message = "SMS Count for cache entry updated successfully for key: " + key;
                    response.Error = "";
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "";
                    response.Error = ex.Message;
                }
            }

            return CreatedAtAction("SetOutboundSMSCountCache", response);
        }

        [Route("Inbound/IsPresent/{from}/{to}")]
        [HttpGet]
        public ActionResult IsKeyPresentInInboundCache(string from, string to)
        {
            Response response = new Response();
            string value = string.Empty;
            string key = from + "-" + to;
            try
            {
                if (!_memoryCache.TryGetValue(key, out value))
                {

                }

                if (string.IsNullOrEmpty(value))
                {
                    response.Message = "Not Present";
                }
                else
                {
                    response.Message = "Present";
                }

                response.Success = true;
                response.Error = "";
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "";
                response.Error = ex.Message;
            }

            return CreatedAtAction("IsKeyPresentInInboundCache", response);
        }
    }
}