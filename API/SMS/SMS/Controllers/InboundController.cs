using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Model;
using Microsoft.Extensions.Primitives;

namespace SMS.Controllers
{
    [Produces("application/json")]
    [Route("api/Inbound")]
    public class InboundController : Controller
    {
        private readonly PlivoCodingAssignmentContext _context;
        private StringValues authorizationToken;

        public InboundController(PlivoCodingAssignmentContext context)
        {
            _context = context;
        }

        // POST: api/inbound/sms
        [Route("sms")]
        [HttpPost]
        public ActionResult Inbound([FromBody] InboundRequest inboundRequest)
        {
            var modelErrors = new List<string>();
            InboundResponse inboundResponse = new InboundResponse();
            CacheHandler cacheHandler = new CacheHandler();

            var request = Request;
            var headers = request.Headers;
            headers.TryGetValue("Authorization", out authorizationToken);
            string token = authorizationToken.FirstOrDefault().Substring("Basic ".Length).Trim();
            string userName = token.Split(':')[0];
            string authId = token.Split(':')[1];

            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            modelErrors.Add(modelError.ErrorMessage);
                        }
                    }

                    inboundResponse.Message = "";
                    inboundResponse.Error = string.Join(",", modelErrors);
                }
                else
                {
                    long accountId = _context.Account.Where(x => x.Username == userName && x.AuthId == authId).FirstOrDefault().Id;
                    PhoneNumber phoneNumber = _context.PhoneNumber.Where(x => x.Number == inboundRequest.To && x.AccountId == accountId).FirstOrDefault();
                    if (phoneNumber != null)
                    {
                        if (inboundRequest.Text.Equals("STOP") || inboundRequest.Text.Equals("STOP\n") || inboundRequest.Text.Equals("STOP\r") || inboundRequest.Text.Equals("STOP\r\n"))
                        {
                            if (!cacheHandler.IsKeyPresentInInboundCache(inboundRequest.From, inboundRequest.To))
                            {
                                cacheHandler.SetInboundSMSCacheAsync(inboundRequest.From, inboundRequest.To);
                            }
                        }

                        inboundResponse.Message = "inbound sms ok";
                        inboundResponse.Error = "";
                    }
                    else
                    {
                        inboundResponse.Message = "";
                        inboundResponse.Error = "to parameter not found";
                    }
                }
            }
            catch
            {
                inboundResponse.Message = "";
                inboundResponse.Error = "unknown failure";
            }

            return CreatedAtAction("Inbound", inboundResponse);
        }
    }
}