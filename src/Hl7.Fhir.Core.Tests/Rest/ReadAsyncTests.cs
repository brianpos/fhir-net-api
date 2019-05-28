using System;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Tests.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hl7.Fhir.Core.AsyncTests
{
    [TestClass]
    public class ReadAsyncTests
    {
        private string _endpoint = FhirClientTests.testEndpoint.OriginalString; //"https://api.hspconsortium.org/rpineda/open";

        private void Client_OnBeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            Console.WriteLine($"{e.RawRequest.Method}: {e.RawRequest.RequestUri}");
        }

        private void Client_OnAfterResponse(object sender, AfterResponseEventArgs e)
        {
            Console.WriteLine($"{e.RawResponse.Method}: {e.RawResponse.ResponseUri}");
            Console.WriteLine($"{System.Text.UnicodeEncoding.UTF8.GetString(e.Body)}");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public async System.Threading.Tasks.Task Read_UsingResourceIdentity_ResultReturned()
        {
            var client = new FhirClient(_endpoint)
            {
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation
            };
            client.OnBeforeRequest += Client_OnBeforeRequest;
            client.OnAfterResponse += Client_OnAfterResponse;
            
            Patient p = await client.ReadAsync<Patient>(new ResourceIdentity("/Patient/glossy"));
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Name[0].Given);
            Assert.IsNotNull(p.Name[0].Family);
            Console.WriteLine($"NAME: {p.Name[0].Given.FirstOrDefault()} {p.Name[0].Family.FirstOrDefault()}");
            Console.WriteLine("Test Completed");
        }

        
        [TestMethod]
        [TestCategory("IntegrationTest")]
        public async System.Threading.Tasks.Task Read_UsingLocationString_ResultReturned()
        {
            var client = new FhirClient(_endpoint)
            {
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation
            };
            client.OnBeforeRequest += Client_OnBeforeRequest;
            client.OnAfterResponse += Client_OnAfterResponse;

            Patient p = await client.ReadAsync<Patient>("/Patient/glossy");
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.Name[0].Given);
            Assert.IsNotNull(p.Name[0].Family);
            Console.WriteLine($"NAME: {p.Name[0].Given.FirstOrDefault()} {p.Name[0].Family.FirstOrDefault()}");
            Console.WriteLine("Test Completed");
        }
    }
}