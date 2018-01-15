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
    public class TestOutboundController
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private HttpClient _cacheClient;
        private HttpResponseMessage _cacheResponse;
        private string _token = "plivo1:20S0KPNOIM";
        private string _from = "4924195509196";
        private string _to = "4924195509198";
        private string _text = "Hello World!";
        private string _api = "api/outbound/sms";
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
        public void TestAuthorization_Outbound_ShouldReturn403()
        {
            _token = "plivo2:DFAAGAADG";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            Assert.AreEqual(403, (int)_response.StatusCode);
        }

        [TestMethod]
        [Description("GET call with valid credentials")]
        public void TestAuthorization_Outbound_WithValidToken_ShouldReturn405()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            _response = _client.GetAsync(_api).Result;
            Assert.AreEqual(405, (int)_response.StatusCode);
        }

        [TestMethod]
        [Description("GET call with invalid credentials")]
        public void TestAuthorization_Outbound_WithInValidToken_ShouldReturn405()
        {
            _token = "plivo2:DFAAGAADG";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            _response = _client.GetAsync(_api).Result;
            Assert.AreEqual(405, (int)_response.StatusCode);
        }

        [TestMethod]
        [Description("POST call with required parameter missing")]
        public void TestRequiredParameterMissing_Outbound()
        {
            string inputJSON = "{'To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("from is missing", jobject.GetValue("error"));
        }

        [TestMethod]
        [Description("POST call with invalid parameter")]
        public void TestInvalidParameter_Outbound()
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
        [Description("POST call with From Parameter not present in phone number table")]
        public void TestFROMParameterNotPresent_Outbound()
        {
            _from = "49241955091456";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("from parameter not found", jobject.GetValue("error"));
        }

        [TestMethod]
        [Description("POST call with all valid parameters")]
        public void TestAllValidParameters_Outbound()
        {
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("outbound sms ok", jobject.GetValue("message"));
        }

        [TestMethod]
        [Description("Check if the outbound entry is stored in cache")]
        public void TestOutboundCache()
        {
            _from = "4924195509029";
            _to = "4924195509195";
            _text = "Hello World";
            string inputJSON = "{'From': '" + _from + "','To':'" + _to + "','Text': '" + _text + "'}";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _token);
            HttpContent content = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            _response = _client.PostAsync(_api, content).Result;
            JObject jobject = JObject.Parse(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("outbound sms ok", jobject.GetValue("message"));

            string _cacheAPI = "api/cache/outbound/" + _from;
            _cacheResponse = _cacheClient.GetAsync(_cacheAPI).Result;
            JObject cacheObject = JObject.Parse(_cacheResponse.Content.ReadAsStringAsync().Result);
            Assert.AreEqual("3", cacheObject.GetValue("message"));
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
