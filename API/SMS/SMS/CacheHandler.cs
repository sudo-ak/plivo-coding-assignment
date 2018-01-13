using Newtonsoft.Json.Linq;
using SMS.Model;
using SMS.Model.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class CacheHandler
    {
        HttpClient httpClient;
        public static String CacheServiceBaseUri { get; set; }

        public CacheHandler()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(CacheServiceBaseUri);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetInboundSMSCacheAsync(string from, string to)
        {
            string inputJSON = "{'From': '" + from + "','To':'" + to + "'}";
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            string api = "api/cache/inbound";
            Task<CacheHandlerResponse> cacheHandlerResponse =  PostHttpRequest(api, content);
            if(!cacheHandlerResponse.Result.Success)
            {
                throw new Exception("Cache Service Failure, Error: " + cacheHandlerResponse.Result.Error);
            }
        }

        public void SetOutboundSMSCountCache(string from)
        {
            string inputJSON = "{'From': '" + from + "'}";
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            string api = "api/cache/outbound";
            Task<CacheHandlerResponse> cacheHandlerResponse = PostHttpRequest(api, content);
            if (!cacheHandlerResponse.Result.Success)
            {
                throw new Exception("Cache Service Failure, Error: " + cacheHandlerResponse.Result.Error);
            }
        }

        public int GetOutboundSMSCountCache(string from)
        {
            var api = "api/cache/outbound/" + from;
            CacheHandlerResponse cacheHandlerResponse = GetHttpRequest(api);
            if (cacheHandlerResponse.Success)
            {
                return Convert.ToInt32(cacheHandlerResponse.Message);
            }
            else
            {
                throw new Exception("Cache Service Failure, Error: " + cacheHandlerResponse.Error);
            }
        }

        public bool IsKeyPresentInInboundCache(string from, string to)
        {
            var api = "api/cache/inbound/ispresent/" + from + "/" + to;
            CacheHandlerResponse cacheHandlerResponse = GetHttpRequest(api);
            if (cacheHandlerResponse.Success)
            {
                if (cacheHandlerResponse.Message.ToLower().Equals("present"))
                    return true;
                else
                    return false;
            }
            else
            {
                throw new Exception("Cache Service Failure, Error: " + cacheHandlerResponse.Error);
            }
        }

        private async Task<CacheHandlerResponse> PostHttpRequest(string api, HttpContent content)
        {
            CacheHandlerResponse cacheHandlerResponse = new CacheHandlerResponse();
            var response = await httpClient.PostAsync(api, content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                JObject jobject = JObject.Parse(responseString);
                cacheHandlerResponse = jobject.ToObject<CacheHandlerResponse>();
            }
            else
            {
                cacheHandlerResponse.Success = false;
                cacheHandlerResponse.Message = "";
                cacheHandlerResponse.Error = response.StatusCode.ToString();
            }

            return cacheHandlerResponse;
        }

        private CacheHandlerResponse GetHttpRequest(string api)
        {
            HttpResponseMessage response = httpClient.GetAsync(api).Result;
            if (response.IsSuccessStatusCode)
            {
                Task<string> res = response.Content.ReadAsStringAsync();
                JObject jobject = JObject.Parse(res.Result);
                CacheHandlerResponse cacheHandlerResponse = jobject.ToObject<CacheHandlerResponse>();
                return cacheHandlerResponse;
            }
            else
            {
                throw new Exception("Cache Service Failure, Status Code: " + response.StatusCode);
            }
        }
    }
}
