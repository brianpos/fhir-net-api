using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hl7.Fhir.Core.AsyncTests
{
    [TestClass]
    public class UpdateRefreshDeleteAsyncTests
    {
        private string _endpoint = "https://api.hspconsortium.org/rpineda/open";

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public async Task UpdateDelete_UsingResourceIdentity_ResultReturned()
        {
            var client = new FhirClient(_endpoint)
            {
                PreferredFormat = ResourceFormat.Json,
                ReturnFullResource = true
            };

            var pat = new Patient()
            {
                Name = new List<HumanName>()
                {
                    new HumanName()
                    {
                        Given = new List<string>() {"test_given"},
                        Family = new List<string>() {"test_family"},
                    }
                },
                Id = "async-test-patient"
            };
            // Create the patient
            Console.WriteLine("Creating patient...");
            Patient p = await client.UpdateAsync<Patient>(pat);
            Assert.IsNotNull(p);

            // Refresh the patient
            Console.WriteLine("Refreshing patient...");
            await client.RefreshAsync(p);

            // Delete the patient
            Console.WriteLine("Deleting patient...");
            await client.DeleteAsync(p);

            Console.WriteLine("Reading patient...");
            // VERIFY //
            try
            {
                await client.ReadAsync<Patient>(new ResourceIdentity("/Patient/async-test-patient"));
                Assert.Fail("Expected the exception to be thrown that the patient isn't found");
            }
            catch (FhirOperationException ex)
            {
                // we are testing more than the exception being thrown, we also want to be sure of the type of status
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, ex.Status, "Expected the resource to have gone");
            }
            
            
            Console.WriteLine("Test Completed");
        }
        
    }
}