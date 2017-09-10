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
        private string _endpoint = "https://sqlonfhir-stu3.azurewebsites.net/fhir";

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public async System.Threading.Tasks.Task UpdateDelete_UsingResourceIdentity_ResultReturned()
        {
            var client = new FhirClient(_endpoint)
            {
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation
            };

            var pat = new Patient()
            {
                Name = new List<HumanName>()
                {
                    new HumanName()
                    {
                        Given = new List<string>() {"test_given"},
                        Family = "test_family",
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

            // Delete a patient that doesn't exist
            await client.DeleteAsync("Patient/xxxxxxx");

            Console.WriteLine("Reading patient...");
            try
            {
                var patGone = await client.ReadAsync<Patient>(new ResourceIdentity("/Patient/async-test-patient"));
                System.Diagnostics.Trace.WriteLine(pat.Id);
                Assert.Fail("Patient should have been deleted from the server");
            }
            catch(FhirOperationException ex)
            {
                Assert.AreEqual(System.Net.HttpStatusCode.Gone, ex.Status, "Expected a gone status code");
            }
            
            
            Console.WriteLine("Test Completed");
        }
        
    }
}