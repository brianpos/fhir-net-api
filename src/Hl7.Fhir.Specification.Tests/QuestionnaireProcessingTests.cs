/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hl7.Fhir.Model;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Specification.Source;
using static Hl7.Fhir.Validation.BasicValidationTests;

namespace Hl7.Fhir.Specification.Tests
{
    [TestClass]
    public class QuestionnaireProcessingTests
    {
        [TestInitialize]
        public void SetupSource()
        {
            // Ensure the FHIR extensions are registered
            Hl7.Fhir.FhirPath.PocoNavigatorExtensions.PrepareFhirSymbolTableFunctions();

            _source = new CachedResolver(
                new MultiResolver(
                    new BundleExampleResolver(@"TestData\validation"),
                    new DirectorySource(@"TestData\validation"),
                    new TestProfileArtifactSource(),
                    new ZipSource("specification.zip"),
                    new DirectorySource(@"C:\git\EHPD\Hcx.Directory.FhirApi\App_Data")
                ));

            var ctx = new ValidationSettings()
            {
                ResourceResolver = _source,
                GenerateSnapshot = true,
                EnableXsdValidation = true,
                Trace = false,
                ResolveExteralReferences = true
            };

            _validator = new Validator(ctx);
        }

        IResourceResolver _source;
        Validator _validator;

        #region << Simple Practitioner >>
        [TestMethod]
        public void QuestionnaireCreateStandardPractitioner()
        {
            var pracSd = _source.FindStructureDefinitionForCoreType(FHIRDefinedType.Practitioner);
            QuestionnaireProcessing processor = new QuestionnaireProcessing();

            //Snapshot.SnapshotGenerator sg = new Snapshot.SnapshotGenerator(_source, 
            //    new Snapshot.SnapshotGeneratorSettings() { GenerateElementIds = true, ForceRegenerateSnapshots = true });
            //sg.Update(pracSd);
            var si = processor.CreateStructureTree(pracSd, _source);

            var prac = processor.CreateResourceInstance<Practitioner>(pracSd, si, GetPractitionerQuestionnaire(), GetPractitionerQuestionnaireResponse());
            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.Text);
            // Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);
        }

        Questionnaire GetPractitionerQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "prac-demo";
            q.Group = new Questionnaire.GroupComponent();

            // The core properties
            var gCoreProps = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCoreProps);
            gCoreProps.Repeats = false;
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Active",
                Type = Questionnaire.AnswerFormat.Boolean,
                LinkId = "Practitioner.active"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Gender",
                Type = Questionnaire.AnswerFormat.Choice,
                LinkId = "Practitioner.gender"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Birth Date",
                Type = Questionnaire.AnswerFormat.Date,
                LinkId = "Practitioner.birthDate"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Name",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.name.text"
            });

            // A collection of Qualifications
            var gCerts = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Repeats = true;
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Code",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Display",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.display"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Practitioner.qualification.period.end"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.issuer.display"
            });

            return q;
        }

        QuestionnaireResponse GetPractitionerQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Questionnaire = new ResourceReference("Questionnaire/prac-demo");
            qr.Group = new QuestionnaireResponse.GroupComponent();

            // The core properties
            var gCoreProps = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCoreProps);
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.active",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirBoolean(true) }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.birthDate",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Date("1970") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Gender",
                LinkId = "Practitioner.gender",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("http://hl7.org/fhir/ValueSet/administrative-gender", "male") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Name",
                LinkId = "Practitioner.name.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Brian Postlethwaite") }
                }
            });

            // A collection of Qualifications
            var gCerts = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            //gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            //{
            //    Text = "Code",
            //    LinkId = "Practitioner.qualification.code.coding.code",
            //    Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
            //        { Value = new FhirString("cert3-agedcare") }
            //    }
            //});
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Aged Care") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("William Angliss TAFE") }
                }
            });

            gCerts = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            //gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            //{
            //    Text = "Code",
            //    LinkId = "Practitioner.qualification.code.coding.code",
            //    Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
            //        { Value = new FhirString("cert3-communitycare") }
            //    }
            //});
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Community Care") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2013") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Murray Goulburn TAFE") }
                }
            });

            return qr;
        }
        #endregion

        [TestMethod]
        public void QuestionnaireCreateExtendedPractitioner()
        {
            string xml = System.IO.File.ReadAllText(
                @"C:\git\EHPD\Hcx.Directory.FhirApi\App_Data\hcxdir-practitioner.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            QuestionnaireProcessing processor = new QuestionnaireProcessing();
            var si = processor.CreateStructureTree(pracSd, _source);

            var prac = processor.CreateResourceInstance<Practitioner>(pracSd, si, GetExtendedPractitionerQuestionnaire(), GetExtendedPractitionerQuestionnaireResponse());
            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.Text);
            // Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);
            Assert.AreEqual("yes", prac.GetStringExtension("http://healthconnex.com.au/hcxd/Practitioner/AppointmentRequired"));
        }

        Questionnaire GetExtendedPractitionerQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "prac-demo";
            q.Group = new Questionnaire.GroupComponent();

            // The core properties
            var gCoreProps = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCoreProps);
            gCoreProps.Repeats = false;
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Active",
                Type = Questionnaire.AnswerFormat.Boolean,
                LinkId = "Practitioner.active"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Gender",
                Type = Questionnaire.AnswerFormat.Choice,
                LinkId = "Practitioner.gender"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Birth Date",
                Type = Questionnaire.AnswerFormat.Date,
                LinkId = "Practitioner.birthDate"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Name",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.name.text"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Appointment Required",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.extension:appointment Required.value[x]"
            });

            // A collection of Qualifications
            var gCerts = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Repeats = true;
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Code",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Display",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.display"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Practitioner.qualification.period.end"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.issuer.display"
            });

            return q;
        }

        QuestionnaireResponse GetExtendedPractitionerQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Questionnaire = new ResourceReference("Questionnaire/prac-demo");
            qr.Group = new QuestionnaireResponse.GroupComponent();

            // The core properties
            var gCoreProps = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCoreProps);
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.active",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirBoolean(true) }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.birthDate",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Date("1970") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Gender",
                LinkId = "Practitioner.gender",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("http://hl7.org/fhir/ValueSet/administrative-gender", "male") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Name",
                LinkId = "Practitioner.name.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Brian Postlethwaite") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Appointment Required",
                LinkId = "Practitioner.extension:appointment Required.value[x]",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("yes") }
                }
            });

            // A collection of Qualifications
            var gCerts = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            //gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            //{
            //    Text = "Code",
            //    LinkId = "Practitioner.qualification.code.coding.code",
            //    Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
            //        { Value = new FhirString("cert3-agedcare") }
            //    }
            //});
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Aged Care") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("William Angliss TAFE") }
                }
            });

            gCerts = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            //gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            //{
            //    Text = "Code",
            //    LinkId = "Practitioner.qualification.code.coding.code",
            //    Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
            //        { Value = new FhirString("cert3-communitycare") }
            //    }
            //});
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Community Care") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2013") }
                }
            });
            gCerts.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Murray Goulburn TAFE") }
                }
            });

            return qr;
        }

        #region << Blood Pressure Example >>
        [TestMethod]
        public void QuestionnaireCreateBloodPressionObservation()
        {
            var vitalSignsSd = _source.FindStructureDefinition("http://hl7.org/fhir/StructureDefinition/daf-vitalsigns");
            QuestionnaireProcessing processor = new QuestionnaireProcessing();

            //Snapshot.SnapshotGenerator sg = new Snapshot.SnapshotGenerator(_source, 
            //    new Snapshot.SnapshotGeneratorSettings() { GenerateElementIds = true, ForceRegenerateSnapshots = true });
            //sg.Update(pracSd);
            var si = processor.CreateStructureTree(vitalSignsSd, _source);

            var obs = processor.CreateResourceInstance<Observation>(vitalSignsSd, si, GetBloodPressureQuestionnaire(), GetBloodPressureQuestionnaireResponse());
            Assert.AreEqual(Observation.ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);
        }

        Questionnaire GetBloodPressureQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "bloodpress-demo";
            q.Group = new Questionnaire.GroupComponent();

            // The core properties
            var gCoreProps = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCoreProps);
            gCoreProps.Repeats = false;
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Status",
                Type = Questionnaire.AnswerFormat.Choice,
                LinkId = "Observation.status"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Effective",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Observation.effective[x]"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Diastolic",
                Type = Questionnaire.AnswerFormat.Decimal,
                LinkId = "Observation.valueQuantity.value"
            });
            gCoreProps.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Reference Range",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Observation.referenceRange.text"
            });

            return q;
        }

        QuestionnaireResponse GetBloodPressureQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Questionnaire = new ResourceReference("Questionnaire/bloodpress-demo");
            qr.Group = new QuestionnaireResponse.GroupComponent();

            // The core properties
            var gCoreProps = new QuestionnaireResponse.GroupComponent();
            qr.Group.Group.Add(gCoreProps);
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Status",
                LinkId = "Observation.status",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("", "preliminary") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Effective",
                LinkId = "Observation.effective[x]",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017-07-09") }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Diastolic",
                LinkId = "Observation.valueQuantity.value",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDecimal(120) }
                }
            });
            gCoreProps.Question.Add(new QuestionnaireResponse.QuestionComponent()
            {
                Text = "Reference Range",
                LinkId = "Observation.referenceRange.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Range: <90  >160") }
                }
            });
            return qr;
        }
        #endregion
    }
}
