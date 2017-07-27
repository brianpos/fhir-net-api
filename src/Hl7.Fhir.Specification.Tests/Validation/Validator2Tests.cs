using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Validation;
using Hl7.Fhir.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Hl7.Fhir.Specification.Tests.Validation
{
    [Trait("Category", "Validation")]
    public class Validator2Tests : IClassFixture<ValidationFixture>
    {
        private IResourceResolver _resolver;
        private Validator _validator;
        private readonly ITestOutputHelper _output;

        public Validator2Tests(ValidationFixture fixture, ITestOutputHelper output)
        {
            _resolver = fixture.Resolver;
            _validator = fixture.Validator;
            _output = output;
            Debug.Listeners.Add(new DefaultTraceListener());
        }

        // [TestMethod]
        [Fact(DisplayName = "NewValdation2Tester")]
        public void NewValdation2Tester()
        {
            FhirPath.ElementNavFhirExtensions.PrepareFhirSymbolTableFunctions();
            var sd = _resolver.FindStructureDefinition("http://example.com/StructureDefinition/patient-telecom-slice-ek");
            Assert.NotNull(sd);
            var snapgen = new SnapshotGenerator(_resolver);
            snapgen.Update(sd);

            var jsonPatient = File.ReadAllText(@"TestData\validation\patient-ck.json");
            var parser = new FhirJsonParser();
            var patient = parser.Parse<Patient>(jsonPatient);
            Assert.NotNull(patient);

            QuickValidator v = new QuickValidator();
            ValidationItem vi = v.CreateValidationTree(sd);
            DumpValidationItemToDebug(vi, _output, null);

            var result = v.Validate(patient, sd);
            DebugDumpOperationOutcome(_output, result);
            Assert.True(result.Success);

            // force a validation fail for a fixed value
            sd.Snapshot.Element[45].Fixed = new ResourceReference("Organization/1", "Walt Disney Corporation");
            result = v.Validate(patient, sd);
            DebugDumpOperationOutcome(_output, result);
            Assert.True(result.Success);

            // force a validation fail for a fixed value
            sd.Snapshot.Element[45].Fixed = new ResourceReference("Organization/1", "Walt Disney Corporation2");
            result = v.Validate(patient, sd);
            DebugDumpOperationOutcome(_output, result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors);

            // force a validation fail! (pat-1)
            patient.Contact[0].Name = null;
            patient.Contact[0].Telecom = null;
            patient.Contact[0].Address = null;
            patient.Contact[0].Organization = null;

            sd.Snapshot.Element[9].Max = "0"; // pretend that not allowed to have any identifiers
            sd.Snapshot.Element[12].Min = 1; // pretend that must have at least 1 telecom
            result = v.Validate(patient, sd);
            DebugDumpOperationOutcome(_output, result);
            Assert.False(result.Success);
            Assert.Equal(3, result.Errors);
        }

        public static void DebugDumpOperationOutcome(ITestOutputHelper output, OperationOutcome result)
        {
            output.WriteLine(result.ToString());
        }

        private void DumpValidationItemToDebug(ValidationItem vi, ITestOutputHelper output, string prefix)
        {
            if (vi.ed.SliceName != null)
                _output.WriteLine($"{prefix}{vi.FhirpathExpression} ({vi.Path}) - {vi.ed.SliceName}");
            else
                _output.WriteLine($"{prefix}{vi.FhirpathExpression} ({vi.Path})");
            foreach (var expr in vi.ValidationRules.SelectMany(s => s.Constraint))
            {
                _output.WriteLine($"{prefix}   ->{expr.Key}: {expr.Expression}");
            }
            if (vi.Children != null)
            {
                foreach (var item in vi.Children)
                {
                    DumpValidationItemToDebug(item, output, "    " + prefix);
                }
            }
        }
    }
}
