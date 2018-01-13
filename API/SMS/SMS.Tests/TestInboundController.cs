using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SMS.Tests
{
    [TestClass]
    public class TestInboundController
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private HttpClient _cacheClient;
        private HttpResponseMessage _cacheResponse;
        private string _token = "plivo1:20S0KPNOIM";
        private string _from = "4924195509196";
        private string _to = "4924195509198";
        private string _text = "Hello World!";
        private string _api = "api/inbound/sms";
        private const string _serviceBaseURL = "http://plivosms-dev.us-west-2.elasticbeanstalk.com";
        private const string _cacheServiceBaseURL = "http://smscache-dev.us-west-2.elasticbeanstalk.com";

        [TestInitialize]
        public void Setup()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_serviceBaseURL);   
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _cacheClient = new HttpClient();
            _cacheClient.BaseAddress = new Uri(_cacheServiceBaseURL);
            _cacheClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [TestMethod]
        [Description("POST call with invalid credentials")]
        public void TestAuthorization_Inbound_ShouldReturn403()
        {
            _token = "plivo2:DFAAGAADG";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            Assert.AreEqual(403,(int) _response.StatusCode);
        }

        [TestMethod]
        [Description("GET call with valid credentials")]
        public void TestAuthorization_Inbound_WithValidToken_ShouldReturn405()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            _response = _client.GetAsync(_api).Result;
            Assert.AreEqual(405, (int)_response.StatusCode);
        }

        [TestMethod]
        [Description("GET call with invalid credentials")]
        public void TestAuthorization_Inbound_WithInValidToken_ShouldReturn405()
        {
            _token = "plivo2:DFAAGAADG";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            _response = _client.GetAsync(_api).Result;
            Assert.AreEqual(405, (int)_response.StatusCode);
        }

        [TestMethod]
        [Description("POST call with required parameter missing")]
        public void TestRequiredParameterMissing_Inbound()
        {
            string inputJSON = "{'To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("from is missing",jobject.GetValue("error"));
        }

        [TestMethod]
        [Description("POST call with invalid parameter")]
        public void TestInvalidParameter_Inbound()
        {
            _from = "2345";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("from is invalid", jobject.GetValue("error"));
        }

        [TestMethod]
        [Description("POST call with To Parameter not present in phone number table")]
        public void TestTOParameterNotPresent_Inbound()
        {
            _to = "49241955091456";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("to parameter not found", jobject.GetValue("error"));
        }

        [TestMethod]
        [Description("POST call with all valid parameters")]
        public void TestAllValidParameters_Inbound()
        {
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("inbound sms ok", jobject.GetValue("message"));
        }

        [TestMethod]
        [Description("Check if the inbound entry is stored in cache")]
        public void TestInboundCache()
        {
            _from = "4924195509193";
            _to = "4924195509195";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("inbound sms ok", jobject.GetValue("message"));

            string _cacheAPI = "api/cache/inbound/" + _from +"/" + _to;
            _cacheResponse = _cacheClient.GetAsync(_cacheAPI).Result;
            JObject cacheObject = JObject.Parse(_cacheResponse.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("04:00:00", cacheObject.GetValue("message"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client = null;
            _cacheClient = null;
            _response = null;
            _cacheResponse = null;
            _token = null;
            _from = null;
            _to = null;
            _text = null;
        }
    }
}
