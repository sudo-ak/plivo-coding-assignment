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
    [Route("api/Outbound")]
    public class OutboundController : Controller
    {
        private readonly PlivoCodingAssignmentContext _context;
        private StringValues authorizationToken;

        public OutboundController(PlivoCodingAssignmentContext context)
        {
            _context = context;
        }


        // POST: api/outbound/sms
        [Route("sms")]
        [HttpPost]
        public ActionResult Outbound([FromBody] OutboundRequest outboundRequest)
        {
            var modelErrors = new List<string>();
            OutboundResponse outboundResponse = new OutboundResponse();
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

                    outboundResponse.Message = "";
                    outboundResponse.Error = string.Join(",", modelErrors);
                }
                else
                {
                    long accountId = _context.Account.Where(x => x.Username == userName && x.AuthId == authId).FirstOrDefault().Id;
                    PhoneNumber phoneNumber = _context.PhoneNumber.Where(x => x.Number == outboundRequest.From && x.AccountId == accountId).FirstOrDefault();
                    if (phoneNumber != null)
                    {
                        if (!cacheHandler.IsKeyPresentInInboundCache(outboundRequest.From, outboundRequest.To))
                        {
                            if (cacheHandler.GetOutboundSMSCountCache(outboundRequest.From) < 50)
                            {
                                cacheHandler.SetOutboundSMSCountCache(outboundRequest.From);
                                outboundResponse.Message = "outbound sms ok";
                                outboundResponse.Error = "";
                            }
                            else
                            {
                                outboundResponse.Message = "";
                                outboundResponse.Error = "limit reached for from <" + outboundRequest.From + "> ";
                            }
                        }
                        else
                        {
                            outboundResponse.Message = "";
                            outboundResponse.Error = "sms from <" + outboundRequest.From + "> to <" + outboundRequest.To + "> blocked by STOP request";
                        }
                    }
                    else
                    {
                        outboundResponse.Message = "";
                        outboundResponse.Error = "from parameter not found";
                    }
                }
            }
            catch
            {
                outboundResponse.Message = "";
                outboundResponse.Error = "unknown failure";
            }

            return CreatedAtAction("Outbound", outboundResponse);
        }
    }
}