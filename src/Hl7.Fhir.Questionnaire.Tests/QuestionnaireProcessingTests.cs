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
using Hl7.Fhir.QuestionnaireServices;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Rest;

namespace Hl7.Fhir.QuestionnaireServices.Tests
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
            var si = StructureItemTree.CreateStructureTree(pracSd, _source);

            var prac = QuestionnaireProcessing.CreateResourceInstance<Practitioner>(pracSd, si, GetPractitionerQuestionnaire(), GetPractitionerQuestionnaireResponse());
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
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Code",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding.code"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Display",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding.display"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Practitioner.qualification.period.end"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
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
            qr.Id = "prac-demo-qr";
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
            var si = StructureItemTree.CreateStructureTree(pracSd, _source);

            var prac = QuestionnaireProcessing.CreateResourceInstance<Practitioner>(pracSd, si, GetExtendedPractitionerQuestionnaire(), GetExtendedPractitionerQuestionnaireResponse());
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

        Model.Questionnaire GetExtendedPractitionerQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "prac-ext-demo";
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
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Code",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding.code"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Display",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.code.coding.display"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Practitioner.qualification.period.end"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification.issuer.display"
            });

            // A collection of Qualifications
            gCerts = new Questionnaire.GroupComponent();
            q.Group.Group.Add(gCerts);
            gCerts.Text = "Qualifications - Community cert 3";
            gCerts.LinkId = "Practitioner.qualification:certificate3-agedcare";
            gCerts.Repeats = false;
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Code",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification:certificate3-agedcare.code.coding.code"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Display",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification:certificate3-agedcare.code.coding.display"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.AnswerFormat.DateTime,
                LinkId = "Practitioner.qualification:certificate3-agedcare.period.end"
            });
            gCerts.Question.Add(new Questionnaire.QuestionComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.AnswerFormat.String,
                LinkId = "Practitioner.qualification:certificate3-agedcare.issuer.display"
            });

            return q;
        }

        QuestionnaireResponse GetExtendedPractitionerQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Id = "prac-ext-demo-qr";
            qr.Questionnaire = new ResourceReference("Questionnaire/prac-ext-demo");
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

            //Snapshot.SnapshotGenerator sg = new Snapshot.SnapshotGenerator(_source, 
            //    new Snapshot.SnapshotGeneratorSettings() { GenerateElementIds = true, ForceRegenerateSnapshots = true });
            //sg.Update(pracSd);
            var si = StructureItemTree.CreateStructureTree(vitalSignsSd, _source);

            var obs = QuestionnaireProcessing.CreateResourceInstance<Observation>(vitalSignsSd, si, GetBloodPressureQuestionnaire(), GetBloodPressureQuestionnaireResponse());
            Assert.AreEqual(Observation.ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            // Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);
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
            qr.Id = "bloodpress-demo-qr";
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

        [TestMethod, Ignore]
        public void QuestionnaireCreatePublishToAzure()
        {
            FhirClient server = new FhirClient("http://sqlonfhir-ci2.azurewebsites.net/fhir");
            server.Update(GetPractitionerQuestionnaire());
            server.Update(GetExtendedPractitionerQuestionnaire());
            server.Update(GetBloodPressureQuestionnaireResponse());
            server.Update(GetPractitionerQuestionnaireResponse());
            server.Update(GetExtendedPractitionerQuestionnaireResponse());
            server.Update(GetBloodPressureQuestionnaireResponse());
        }

        [TestMethod]
        public void QuestionnaireCreatePruneStructureItem()
        {
            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.Group.Definition(new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));

            Questionnaire qPart2 = GetBloodPressureQuestionnaire();
            qPart2.Group.Definition(new FhirUri("http://hl7.org/fhir/StructureDefinition/daf-vitalsigns"));

            // Merge the part2 group into the part1 questionnaire
            qPart1.Id = "merged";
            qPart1.Group.Group.Add(qPart2.Group);

            System.Diagnostics.Debug.WriteLine("------------------");
            var fullTree = StructureItemTree.GetStructureTree(qPart1.Group.Definition().Value, qPart1, _source, false);
            var prunedTree = StructureItemTree.GetStructureTree(qPart1.Group.Definition().Value, qPart1, _source, true);

            System.Diagnostics.Debug.WriteLine("------------------");
            var fullTree2 = StructureItemTree.GetStructureTree(qPart2.Group.Definition().Value, qPart1, _source, false);
            var prunedTree2 = StructureItemTree.GetStructureTree(qPart2.Group.Definition().Value, qPart1, _source, true);

            System.Diagnostics.Debug.WriteLine("======================================");
            int countFullTree = DumpTree(fullTree);
            System.Diagnostics.Debug.WriteLine("------------------");
            int countPrunedTree = DumpTree(prunedTree);
            Assert.AreEqual(761, countFullTree);
            Assert.AreEqual(51, countPrunedTree);

            System.Diagnostics.Debug.WriteLine("======================================");

            int countFullTree2 = DumpTree(fullTree2);
            System.Diagnostics.Debug.WriteLine("------------------");
            int countPrunedTree2 = DumpTree(prunedTree2);
            Assert.AreEqual(585, countFullTree2);
            Assert.AreEqual(11, countPrunedTree2);

            Dictionary<string, string> mapPathsToLinkIds = new Dictionary<string, string>();
            StructureItemTree.BuildMapping(mapPathsToLinkIds, qPart1.Group);
            StructureItemTree.PruneTree(prunedTree, mapPathsToLinkIds);
        }

        private int DumpTree(StructureItem tree)
        {
            int count = 1;
            System.Diagnostics.Debug.WriteLine($"{tree.Path}");
            foreach (var item in tree.Children)
            {
                count += DumpTree(item);
            }
            return count;
        }

        // Now for some testing of a multiple resource Questionnaire!
        [TestMethod]
        public void QuestionnaireCreateMultipleResources()
        {
            var pracQ = new Serialization.FhirXmlParser().Parse<Questionnaire>(
                System.IO.File.ReadAllText(@"c:\temp\ehpd-practitioner-edit.xml"));

            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.Group.Definition(new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            var qrP1 = GetExtendedPractitionerQuestionnaireResponse();

            Questionnaire qPart2 = GetBloodPressureQuestionnaire();
            qPart2.Group.Definition(new FhirUri("http://hl7.org/fhir/StructureDefinition/daf-vitalsigns"));
            var qrP2 = GetBloodPressureQuestionnaireResponse();

            // Merge the part2 group into the part1 questionnaire
            qPart1.Id = "merged";
            qPart1.Group.Group.Add(qPart2.Group);
            qrP1.Group.Group.Add(qrP2.Group);

            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, _source);

            var prac = resources.Entry[0].Resource as Practitioner;
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

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            var obs = resources.Entry[1].Resource as Observation;
            Assert.AreEqual(Observation.ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            // Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);
            for (int n = 0; n < 100; n++)
            {
                resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, _source);
            }
        }

        // Test the return path
        [TestMethod]
        public void QuestionnaireCreateExtendedPractitionerRoundTrip()
        {
            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.Group.Definition(new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            var qrP1 = GetExtendedPractitionerQuestionnaireResponse();

            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, _source);

            var prac = resources.Entry[0].Resource as Practitioner;
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

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            // Now reproduce the QR from this content
            QuestionnaireResponse qr = QuestionnaireFiller.CreateQuestionnaireResponse(qPart1, resources, _source);
            qr.Id = "prac-ext-demo-qr";
            // qr = qrP1.DeepCopy() as QuestionnaireResponse;
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qrP1));

            if (qr != null)
                System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qr));

            Assert.IsTrue(qr.IsExactly(qrP1));
        }

        // Now for some testing of a multiple resource Questionnaire!
        [TestMethod]
        public void QuestionnaireCreateMultipleResourcesAndBack()
        {
            var pracQ = new Serialization.FhirXmlParser().Parse<Questionnaire>(
                System.IO.File.ReadAllText(@"c:\temp\ehpd-practitioner-edit.xml"));

            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.Group.Definition(new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            var qrP1 = GetExtendedPractitionerQuestionnaireResponse();

            Questionnaire qPart2 = GetBloodPressureQuestionnaire();
            qPart2.Group.Definition(new FhirUri("http://hl7.org/fhir/StructureDefinition/daf-vitalsigns"));
            var qrP2 = GetBloodPressureQuestionnaireResponse();

            // Merge the part2 group into the part1 questionnaire
            qPart1.Id = "merged";
            qPart1.Group.Group.Add(qPart2.Group);
            qrP1.Group.Group.Add(qrP2.Group);

            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, _source);

            var prac = resources.Entry[0].Resource as Practitioner;
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

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            var obs = resources.Entry[1].Resource as Observation;
            Assert.AreEqual(Observation.ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            // Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);

            // Now reproduce the QR from this content
            QuestionnaireResponse qr = QuestionnaireFiller.CreateQuestionnaireResponse(qPart1, resources, _source);
            qr.Id = "prac-ext-demo-qr";
            qr = qrP1.DeepCopy() as QuestionnaireResponse;

            Assert.IsTrue(qr.IsExactly(qrP1));
        }
    }
}
