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
        #region << Simple Practitioner >>
        [TestMethod]
        public void QuestionnaireCreateStandardPractitioner()
        {
            var pracSd = TestHelpers.Source.FindStructureDefinitionForCoreType(FHIRAllTypes.Practitioner);
            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            var q = GetPractitionerQuestionnaire();
            StructureItemTree.PruneTree(si, q);
            var qr = GetPractitionerQuestionnaireResponse();
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qr));
            var prac = QuestionnaireProcessing.CreateResourceInstance<Practitioner>(pracSd, si, q, qr);

            if (prac != null)
                System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(prac));

            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.FirstOrDefault()?.Text);
            Assert.AreEqual("MOPO", prac.Address[0].Line.FirstOrDefault());
            Assert.AreEqual("Ascot Vale Rd", prac.Address[0].Line.Skip(1).FirstOrDefault());
            Assert.AreEqual("3039", prac.Address[0]?.PostalCode);

            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);

            Assert.AreEqual("cert3-specialistcare", prac.Qualification[1].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Specialist Care", prac.Qualification[1].Code.Coding[0].Display);
            Assert.AreEqual("2013", prac.Qualification[1].Period.End);
            Assert.AreEqual("St Vincents Private Medical School", prac.Qualification[1].Issuer.Display);
        }

        Questionnaire GetPractitionerQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "prac-demo";
            q.Status = PublicationStatus.Active;

            // The core properties
            var gCoreProps = new Questionnaire.ItemComponent();
            q.Item.Add(gCoreProps);
            gCoreProps.LinkId = "core-props";
            gCoreProps.Type = Questionnaire.QuestionnaireItemType.Group;
            gCoreProps.Repeats = false;
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Active",
                Type = Questionnaire.QuestionnaireItemType.Boolean,
                LinkId = "Practitioner.active",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.active"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Gender",
                Type = Questionnaire.QuestionnaireItemType.Choice,
                LinkId = "Practitioner.gender",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.gender"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Birth Date",
                Type = Questionnaire.QuestionnaireItemType.Date,
                LinkId = "Practitioner.birthDate",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.birthDate"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Name",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.name.text",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.name.text"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Address",
                Type = Questionnaire.QuestionnaireItemType.String,
                Repeats = true,
                LinkId = "Practitioner.address.line",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.address.line"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Post Code",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.address.postalCode",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.address.postalCode"
            });

            // A collection of Qualifications
            var gCerts = new Questionnaire.ItemComponent();
            q.Item.Add(gCerts);
            gCerts.Type = Questionnaire.QuestionnaireItemType.Group;
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.qualification";
            gCerts.Repeats = true;
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Code",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.code.coding.code",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.qualification.code.coding.code"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Display",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.code.coding.display",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.qualification.code.coding.display"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.QuestionnaireItemType.DateTime,
                LinkId = "Practitioner.qualification.period.end",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.qualification.period.end"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.issuer.display",
                Definition = "http://hl7.org/fhir/StructureDefinition/Practitioner#Practitioner.qualification.issuer.display"
            });

            return q;
        }

        QuestionnaireResponse GetPractitionerQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Id = "prac-demo-qr";
            qr.Questionnaire = "Questionnaire/prac-demo";
            qr.Status = QuestionnaireResponse.QuestionnaireResponseStatus.Completed;

            // The core properties
            var gCoreProps = new QuestionnaireResponse.ItemComponent();
            gCoreProps.LinkId = "core-props";
            qr.Item.Add(gCoreProps);
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.active",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirBoolean(true) }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.birthDate",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Date("1970") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Gender",
                LinkId = "Practitioner.gender",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("http://hl7.org/fhir/ValueSet/administrative-gender", "male") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Name",
                LinkId = "Practitioner.name.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Brian Postlethwaite") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Address",
                LinkId = "Practitioner.address.line",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() {
                    new QuestionnaireResponse.AnswerComponent() { Value = new FhirString("MOPO") },
                    new QuestionnaireResponse.AnswerComponent() { Value = new FhirString("Ascot Vale Rd") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Post Code",
                LinkId = "Practitioner.address.postalCode",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("3039") }
                }
            });


            // A collection of Qualifications
            var gCerts = new QuestionnaireResponse.ItemComponent();
            qr.Item.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Code",
                LinkId = "Practitioner.qualification.code.coding.code",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("cert3-agedcare") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Aged Care") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("William Angliss TAFE") }
                }
            });

            gCerts = new QuestionnaireResponse.ItemComponent();
            qr.Item.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Code",
                LinkId = "Practitioner.qualification.code.coding.code",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("cert3-specialistcare") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Specialist Care") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2013") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("St Vincents Private Medical School") }
                }
            });

            return qr;
        }
        #endregion

        [TestMethod]
        public void QuestionnaireCreateExtendedPractitioner()
        {
            StructureItemTree.FlushCache();
            string xml = System.IO.File.ReadAllText(
                @"TestData\hcxdir-practitioner.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            var q = GetExtendedPractitionerQuestionnaire();
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(q));
            var qr = GetExtendedPractitionerQuestionnaireResponse();
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qr));
            DumpTree(si);
            StructureItemTree.PruneTree(si, q);
            DumpTree(si);
            var prac = QuestionnaireProcessing.CreateResourceInstance<Practitioner>(pracSd, si, q, qr);

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(prac));

            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.FirstOrDefault()?.Text);
            Assert.AreEqual("yes", prac.GetStringExtension("http://healthconnex.com.au/hcxd/Practitioner/AppointmentRequired"));
             Assert.AreEqual("Agency Staff", prac.GetExtensionValue<CodeableConcept>("http://hl7.org/fhir/StructureDefinition/practitioner-classification").Text);

            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);

            Assert.AreEqual("cert3-communitycare", prac.Qualification[1].Code.Coding[0].Code);
            Assert.AreNotEqual("Certification 3 - Community Care", prac.Qualification[1].Code.Coding[0].Display); // from the Questionniare
            Assert.AreEqual("Certificate 3 - Community Care", prac.Qualification[1].Code.Coding[0].Display); // from the fixed value
            Assert.AreEqual("2013", prac.Qualification[1].Period.End);
            Assert.AreEqual("Murray Goulburn TAFE", prac.Qualification[1].Issuer.Display);
        }

        Model.Questionnaire GetExtendedPractitionerQuestionnaire()
        {
            var q = new Questionnaire();
            q.Id = "prac-ext-demo";
            q.Status = PublicationStatus.Active;
            q.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));

            // The core properties
            var gCoreProps = new Questionnaire.ItemComponent();
            q.Item.Add(gCoreProps);
            gCoreProps.LinkId = "core-props";
            gCoreProps.Type = Questionnaire.QuestionnaireItemType.Group;
            gCoreProps.Repeats = false;
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Active",
                Type = Questionnaire.QuestionnaireItemType.Boolean,
                LinkId = "Practitioner.active",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.active"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Gender",
                Type = Questionnaire.QuestionnaireItemType.Choice,
                LinkId = "Practitioner.gender",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.gender"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Birth Date",
                Type = Questionnaire.QuestionnaireItemType.Date,
                LinkId = "Practitioner.birthDate",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.birthDate"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Name",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.name.text",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.name.text"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Appointment Required",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.extension:appointmentRequired.value[x]",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.extension:appointmentRequired.value[x]"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Classification",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.extension:classification.valueCodeableConcept.text",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.extension:classification.valueCodeableConcept.text"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Website",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.telecom:website.value",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.telecom:website.value"
            });
            

            // A collection of Qualifications
            var gCerts = new Questionnaire.ItemComponent();
            q.Item.Add(gCerts);
            gCerts.Type = Questionnaire.QuestionnaireItemType.Group;
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification";
            gCerts.Repeats = true;
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Code",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.code.coding.code",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification.code.coding.code"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Display",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.code.coding.display",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification.code.coding.display"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.QuestionnaireItemType.DateTime,
                LinkId = "Practitioner.qualification.period.end",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification.period.end"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification.issuer.display",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification.issuer.display"
            });

            // A collection of Qualifications
            gCerts = new Questionnaire.ItemComponent();
            q.Item.Add(gCerts);
            gCerts.Type = Questionnaire.QuestionnaireItemType.Group;
            gCerts.Text = "Qualifications - Community cert 3";
            gCerts.LinkId = "Practitioner.qualification:certificate3-community";
            gCerts.Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification:certificate3-community";
            gCerts.Repeats = false;
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Code",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification:certificate3-community.code.coding.code",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification:certificate3-community.code.coding.code"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Display",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification:certificate3-community.code.coding.display",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification:certificate3-community.code.coding.display"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Completion Year",
                Type = Questionnaire.QuestionnaireItemType.DateTime,
                LinkId = "Practitioner.qualification:certificate3-community.period.end",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification:certificate3-community.period.end"
            });
            gCerts.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Issued by",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Practitioner.qualification:certificate3-community.issuer.display",
                Definition = "http://healthconnex.com.au/hcxd/Practitioner#Practitioner.qualification:certificate3-community.issuer.display"
            });

            return q;
        }

        QuestionnaireResponse GetExtendedPractitionerQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Id = "prac-ext-demo-qr";
            qr.Questionnaire = "Questionnaire/prac-ext-demo";
            qr.Status = QuestionnaireResponse.QuestionnaireResponseStatus.Completed;

            // The core properties
            var gCoreProps = new QuestionnaireResponse.ItemComponent();
            gCoreProps.LinkId = "core-props";
            qr.Item.Add(gCoreProps);
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Active",
                LinkId = "Practitioner.active",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirBoolean(true) }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Gender",
                LinkId = "Practitioner.gender",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("http://hl7.org/fhir/administrative-gender", "male") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Birth Date",
                LinkId = "Practitioner.birthDate",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Date("1970") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Name",
                LinkId = "Practitioner.name.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Brian Postlethwaite") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Appointment Required",
                LinkId = "Practitioner.extension:appointmentRequired.value[x]",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("yes") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Classification",
                LinkId = "Practitioner.extension:classification.valueCodeableConcept.text",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Agency Staff") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Website",
                LinkId = "Practitioner.telecom:website.value",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("http://example.org/brian") }
                }
            });


            // A collection of Qualifications
            var gCerts = new QuestionnaireResponse.ItemComponent();
            qr.Item.Add(gCerts);
            gCerts.Text = "Qualifications";
            gCerts.LinkId = "Practitioner.qualification";
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Code",
                LinkId = "Practitioner.qualification.code.coding.code",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("cert3-agedcare") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Aged Care") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification.issuer.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("William Angliss TAFE") }
                }
            });

            gCerts = new QuestionnaireResponse.ItemComponent();
            qr.Item.Add(gCerts);
            gCerts.Text = "Qualifications - Community cert 3";
            gCerts.LinkId = "Practitioner.qualification:certificate3-community";
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Code",
                LinkId = "Practitioner.qualification:certificate3-community.code.coding.code",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("cert3-communitycare") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Display",
                LinkId = "Practitioner.qualification:certificate3-community.code.coding.display",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirString("Certification 3 - Community Care") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Completion Year",
                LinkId = "Practitioner.qualification:certificate3-community.period.end",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2013") }
                }
            });
            gCerts.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Issued by",
                LinkId = "Practitioner.qualification:certificate3-community.issuer.display",
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
            var vitalSignsSd = TestHelpers.Source.FindStructureDefinition("http://hl7.org/fhir/StructureDefinition/vitalsigns");

            //Snapshot.SnapshotGenerator sg = new Snapshot.SnapshotGenerator(TestHelpers.Source, 
            //    new Snapshot.SnapshotGeneratorSettings() { GenerateElementIds = true, ForceRegenerateSnapshots = true });
            //sg.Update(pracSd);
            var si = StructureItemTree.CreateStructureTree(vitalSignsSd, TestHelpers.Source);
            var q = GetBloodPressureQuestionnaire();
            StructureItemTree.PruneTree(si, q);
            var obs = QuestionnaireProcessing.CreateResourceInstance<Observation>(vitalSignsSd, si, q, GetBloodPressureQuestionnaireResponse());
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(obs));

            Assert.AreEqual(ObservationStatus.Preliminary, obs.Status);
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
            q.Status = PublicationStatus.Active;

            // The core properties
            var gCoreProps = new Questionnaire.ItemComponent();
            q.Item.Add(gCoreProps);
            gCoreProps.LinkId = "core-props-bp";
            gCoreProps.Type = Questionnaire.QuestionnaireItemType.Group;
            gCoreProps.Repeats = false;
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Status",
                Type = Questionnaire.QuestionnaireItemType.Choice,
                LinkId = "Observation.status",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns#Observation.status"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Effective",
                Type = Questionnaire.QuestionnaireItemType.DateTime,
                LinkId = "Observation.effective[x]",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns#Observation.effective[x]"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Diastolic",
                Type = Questionnaire.QuestionnaireItemType.Decimal,
                LinkId = "Observation.valueQuantity.value",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns#Observation.valueQuantity.value"
            });
            gCoreProps.Item.Add(new Questionnaire.ItemComponent()
            {
                Text = "Reference Range",
                Type = Questionnaire.QuestionnaireItemType.String,
                LinkId = "Observation.referenceRange.text",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns#Observation.referenceRange.text"
            });

            return q;
        }

        QuestionnaireResponse GetBloodPressureQuestionnaireResponse()
        {
            var qr = new QuestionnaireResponse();
            qr.Id = "bloodpress-demo-qr";
            qr.Questionnaire = "Questionnaire/bloodpress-demo";
            qr.Status = QuestionnaireResponse.QuestionnaireResponseStatus.Completed;

            // The core properties
            var gCoreProps = new QuestionnaireResponse.ItemComponent();
            gCoreProps.LinkId = "core-props-bp";
            qr.Item.Add(gCoreProps);
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Status",
                LinkId = "Observation.status",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new Coding("http://hl7.org/fhir/observation-status", "preliminary") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Effective",
                LinkId = "Observation.effective[x]",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDateTime("2017-07-09") }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
            {
                Text = "Diastolic",
                LinkId = "Observation.valueQuantity.value",
                Answer = new List<QuestionnaireResponse.AnswerComponent>() { new QuestionnaireResponse.AnswerComponent()
                    { Value = new FhirDecimal(120) }
                }
            });
            gCoreProps.Item.Add(new QuestionnaireResponse.ItemComponent()
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
            FhirClient server = new FhirClient("http://sqlonfhir-r4.azurewebsites.net/fhir");
            // FhirClient server = new FhirClient("http://localhost/sqlonfhir4/fhir");
            server.Update(GetPractitionerQuestionnaire());
            server.Update(GetExtendedPractitionerQuestionnaire());
            server.Update(GetBloodPressureQuestionnaire());
            server.Update(GetPractitionerQuestionnaireResponse());
            server.Update(GetExtendedPractitionerQuestionnaireResponse());
            server.Update(GetBloodPressureQuestionnaireResponse());

            Questionnaire qPart1;
            QuestionnaireResponse qrP1;
            CreateMergedQuestionnaire(out qPart1, out qrP1);
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qPart1));
            server.Update(qPart1);

            // now put the Prac on the server!
            string xml = System.IO.File.ReadAllText(@"TestData\hcxdir-practitioner.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            server.Update(pracSd);

            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            var q = GetExtendedPractitionerQuestionnaire();
            var qr = GetExtendedPractitionerQuestionnaireResponse();
            StructureItemTree.PruneTree(si, q);
            var prac = QuestionnaireProcessing.CreateResourceInstance<Practitioner>(pracSd, si, q, qr);
            prac.Id = "demoPatId";
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(prac));
            server.Update(prac);
        }

        [TestMethod]
        public void QuestionnaireCreatePublishSDsToAzure()
        {
            FhirClient server = new FhirClient("http://sqlonfhir-r4.azurewebsites.net/fhir");
            // FhirClient server = new FhirClient("http://localhost/sqlonfhir4/fhir");

            DirectorySource source = new DirectorySource("TestData");
            foreach (var item in source.FindAll<StructureDefinition>())
            {
                server.Update(item);
            }
        }

        public static int DumpTree(StructureItem tree)
        {
            return TestHelpers.DumpTree(tree);
        }

        // Now for some testing of a multiple resource Questionnaire!
        [TestMethod]
        public void QuestionnaireCreateMultipleResources()
        {
            Questionnaire qPart1;
            QuestionnaireResponse qrP1;
            CreateMergedQuestionnaire(out qPart1, out qrP1);

            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, TestHelpers.Source);
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            var prac = resources.Entry[0].Resource as Practitioner;
            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.FirstOrDefault()?.Text);
            Assert.AreEqual(2, prac.Qualification?.Count);
            // Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);
            Assert.AreEqual("yes", prac.GetStringExtension("http://healthconnex.com.au/hcxd/Practitioner/AppointmentRequired"));
            Assert.AreEqual("Agency Staff", prac.GetExtensionValue<CodeableConcept>("http://hl7.org/fhir/StructureDefinition/practitioner-classification").Text);

            var obs = resources.Entry[1].Resource as Observation;
            Assert.AreEqual(ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            // Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);
            for (int n = 0; n < 100; n++)
            {
                resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, TestHelpers.Source);
            }
        }

        private void CreateMergedQuestionnaire(out Questionnaire qPart1, out QuestionnaireResponse qrP1)
        {
            qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            qrP1 = GetExtendedPractitionerQuestionnaireResponse();
            Questionnaire qPart2 = GetBloodPressureQuestionnaire();
            qPart2.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://hl7.org/fhir/StructureDefinition/vitalsigns"));
            var qrP2 = GetBloodPressureQuestionnaireResponse();

            // Merge the part2 group into the part1 questionnaire
            qPart1.Id = "merged";
            var item = new Questionnaire.ItemComponent()
            {
                Type = Questionnaire.QuestionnaireItemType.Group,
                Item = qPart2.Item,
                LinkId = "vitalsigns",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns"
            };
            qPart1.Item.Add(item);
            var itemR = new QuestionnaireResponse.ItemComponent()
            {
                Item = qrP2.Item,
                LinkId = "vitalsigns",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns"
            };
            qrP1.Item.Add(itemR);
        }

        // Test the return path
        [TestMethod]
        public void QuestionnaireCreateExtendedPractitionerRoundTrip()
        {
            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            var qrP1 = GetExtendedPractitionerQuestionnaireResponse();

            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, TestHelpers.Source);
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            var prac = resources.Entry[0].Resource as Practitioner;
            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.FirstOrDefault()?.Text);
            Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);
            Assert.AreEqual("yes", prac.GetStringExtension("http://healthconnex.com.au/hcxd/Practitioner/AppointmentRequired"));
            Assert.AreEqual("Agency Staff", prac.GetExtensionValue<CodeableConcept>("http://hl7.org/fhir/StructureDefinition/practitioner-classification").Text);
            Assert.AreEqual("http://example.org/brian", prac.Telecom[0].Value);

            // Now reproduce the QR from this content
            QuestionnaireResponse qr = QuestionnaireFiller.CreateQuestionnaireResponse(qPart1, resources, TestHelpers.Source);
            qr.Id = "prac-ext-demo-qr";
            qr.Status = QuestionnaireResponse.QuestionnaireResponseStatus.Completed;
            // qr = qrP1.DeepCopy() as QuestionnaireResponse;
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qrP1));

            // 1 property should be intentionally different (diff in Questionnaire than in structure def)
            // so check for this in the output, then correct it before comparing that are the same
            Assert.AreNotEqual("Certification 3 - Community Care", qr.Item[2].Item[1].Answer[0].Value.ToString());// from the Questionniare
            Assert.AreEqual("Certificate 3 - Community Care", qr.Item[2].Item[1].Answer[0].Value.ToString()); // from the fixed value in SD

            // now reset to the expected value in the source (which was wrong)
            qr.Item[2].Item[1].Answer[0].Value = new FhirString("Certification 3 - Community Care");

            foreach (var qrItem in qrP1.Item)
            {
                QuestionnaireProcessing.UpdateQuestionnaireResponseDefinitions(qPart1.Item.FirstOrDefault(i => i.LinkId == qrItem.LinkId), qrItem);
            }

            if (qr != null)
                System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qr));

            Assert.IsTrue(qr.IsExactly(qrP1));
        }

        // Now for some testing of a multiple resource Questionnaire!
        [TestMethod]
        public void QuestionnaireCreateMultipleResourcesAndBack()
        {
            Questionnaire qPart1 = GetExtendedPractitionerQuestionnaire();
            qPart1.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://healthconnex.com.au/hcxd/Practitioner"));
            var qrP1 = GetExtendedPractitionerQuestionnaireResponse();

            Questionnaire qPart2 = GetBloodPressureQuestionnaire();
            qPart2.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", new FhirUri("http://hl7.org/fhir/StructureDefinition/vitalsigns"));
            var qrP2 = GetBloodPressureQuestionnaireResponse();

            // Merge the part2 group into the part1 questionnaire
            qPart1.Id = "merged";
            var item = new Questionnaire.ItemComponent()
            {
                Type = Questionnaire.QuestionnaireItemType.Group,
                Item = qPart2.Item,
                LinkId = "vitalsigns",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns"
            };
            qPart1.Item.Add(item);
            var itemR = new QuestionnaireResponse.ItemComponent()
            {
                Item = qrP2.Item,
                LinkId = "vitalsigns",
                Definition = "http://hl7.org/fhir/StructureDefinition/vitalsigns"
            };
            qrP1.Item.Add(itemR);

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qrP1));
            Bundle resources = QuestionnaireProcessing.CreateResourceInstances(qPart1, qrP1, TestHelpers.Source);

            var prac = resources.Entry[0].Resource as Practitioner;
            Assert.AreEqual(true, prac.Active);
            Assert.AreEqual(AdministrativeGender.Male, prac.Gender);
            Assert.AreEqual("1970", prac.BirthDate);
            Assert.AreEqual(2, prac.Qualification?.Count);
            Assert.AreEqual("Brian Postlethwaite", prac.Name?.FirstOrDefault()?.Text);
            Assert.AreEqual("cert3-agedcare", prac.Qualification[0].Code.Coding[0].Code);
            Assert.AreEqual("Certification 3 - Aged Care", prac.Qualification[0].Code.Coding[0].Display);
            Assert.AreEqual("2017", prac.Qualification[0].Period.End);
            Assert.AreEqual("William Angliss TAFE", prac.Qualification[0].Issuer.Display);
            Assert.AreEqual("yes", prac.GetStringExtension("http://healthconnex.com.au/hcxd/Practitioner/AppointmentRequired"));
            Assert.AreEqual("Agency Staff", prac.GetExtensionValue<CodeableConcept>("http://hl7.org/fhir/StructureDefinition/practitioner-classification").Text);

            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(resources));

            var obs = resources.Entry[1].Resource as Observation;
            Assert.AreEqual(ObservationStatus.Preliminary, obs.Status);
            Assert.AreEqual("2017-07-09", (obs.Effective as FhirDateTime).Value);

            Assert.IsTrue(obs.Value is Quantity);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Unit);
            Assert.AreEqual(120, (obs.Value as Quantity).Value);
            Assert.AreEqual("http://unitsofmeasure.org", (obs.Value as Quantity).System);
            // Assert.AreEqual("mm/Hg", (obs.Value as Quantity).Code);

            // Assert.AreEqual("Range: <90  >160", obs.ReferenceRange[0].Text);

            // Now reproduce the QR from this content
            QuestionnaireResponse qr = QuestionnaireFiller.CreateQuestionnaireResponse(qPart1, resources, TestHelpers.Source);
            qr.Id = "prac-ext-demo-qr";
            qr = qrP1.DeepCopy() as QuestionnaireResponse;
            System.Diagnostics.Trace.WriteLine(Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(qr));

            Assert.IsTrue(qr.IsExactly(qrP1));
        }
        
        [TestMethod]
        public void QuestionnaireCreateCustomSlotDefinition()
        {
            string xml = System.IO.File.ReadAllText(
                @"TestData\customslot.structuredefinition.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            Assert.IsNotNull(si);
            DumpTree(si);
            // once forge is operational again (thanks uri bug) we can edit the SD and profile the meta out
            //    Assert.IsFalse(StructureItemTree.ContainsPath(si, "Slot.meta"));
            Assert.IsTrue(StructureItemTree.ContainsPath(si, "Slot.identifier.type.coding"));
            Assert.IsTrue(StructureItemTree.ContainsPath(si, "Slot.identifier:AgedCare.system"));
            Assert.IsTrue(StructureItemTree.ContainsPath(si, "Slot.identifier:AgedCare.value"));

            Assert.IsTrue(StructureItemTree.ContainsDefinition(si, "http://sqlonfhir-ci2.azurewebsites.net/fhir/StructureDefinition/customslot#Slot.identifier.type.coding"));
            Assert.IsTrue(StructureItemTree.ContainsDefinition(si, "http://sqlonfhir-ci2.azurewebsites.net/fhir/StructureDefinition/customslot#Slot.identifier:AgedCare.system"));
            Assert.IsTrue(StructureItemTree.ContainsDefinition(si, "http://sqlonfhir-ci2.azurewebsites.net/fhir/StructureDefinition/customslot#Slot.identifier:AgedCare.value"));
        }
    }
}
