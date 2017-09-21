﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Introspection;
using System.Runtime.Serialization;

namespace Hl7.Fhir.Tests.Serialization
{
    [TestClass]
    public class SerializationTests
    {
        private const string metaXml = "<meta xmlns=\"http://hl7.org/fhir\"><versionId value=\"3141\" /><lastUpdated value=\"2014-12-24T16:30:56.031+01:00\" /></meta>";
        private readonly Meta metaPoco = new Meta { LastUpdated = new DateTimeOffset(2014, 12, 24, 16, 30, 56, 31, new TimeSpan(1, 0, 0)), VersionId = "3141" };

        [TestMethod]
        public void SerializeMeta()
        {
            var xml = FhirSerializer.SerializeToXml(metaPoco, root: "meta");
            Assert.AreEqual(metaXml, xml);
        }


        [TestMethod]
        public void ParseMeta()
        {
            var poco = (Meta)(new FhirXmlParser().Parse(metaXml, typeof(Meta)));
            var xml = FhirSerializer.SerializeToXml(poco, root: "meta");

            Assert.IsTrue(poco.IsExactly(metaPoco));
            Assert.AreEqual(metaXml, xml);
        }


        [TestMethod]
        public void AvoidBOMUse()
        {
            Bundle b = new Bundle();

            var data = FhirSerializer.SerializeResourceToJsonBytes(b);
            Assert.IsFalse(data[0] == Encoding.UTF8.GetPreamble()[0]);

            data = FhirSerializer.SerializeResourceToXmlBytes(b);
            Assert.IsFalse(data[0] == Encoding.UTF8.GetPreamble()[0]);

            Patient p = new Patient();

            data = FhirSerializer.SerializeResourceToJsonBytes(p);
            Assert.IsFalse(data[0] == Encoding.UTF8.GetPreamble()[0]);

            data = FhirSerializer.SerializeResourceToXmlBytes(p);
            Assert.IsFalse(data[0] == Encoding.UTF8.GetPreamble()[0]);
        }

        [TestMethod]
        public void TestProbing()
        {
            Assert.IsFalse(SerializationUtil.ProbeIsJson("this is nothing"));
            Assert.IsFalse(SerializationUtil.ProbeIsJson("  crap { "));
            Assert.IsFalse(SerializationUtil.ProbeIsJson("<element/>"));
            Assert.IsTrue(SerializationUtil.ProbeIsJson("   { x:5 }"));

            Assert.IsFalse(SerializationUtil.ProbeIsXml("this is nothing"));
            Assert.IsFalse(SerializationUtil.ProbeIsXml("  crap { "));
            Assert.IsFalse(SerializationUtil.ProbeIsXml(" < crap  "));
            Assert.IsFalse(SerializationUtil.ProbeIsXml("   { x:5 }"));
            Assert.IsTrue(SerializationUtil.ProbeIsXml("   <element/>"));
            Assert.IsTrue(SerializationUtil.ProbeIsXml("<?xml />"));
        }

        [TestMethod] // This tests that the XML serializer does actually filter out the summary properties correctly
                     // checks generation of the model (only patient), and also the serializer
        public void TestSummary()
        {
            var p = new Patient();

            p.BirthDate = "1972-11-30";     // present in both summary and full
            p.Photo = new List<Attachment>() { new Attachment() { ContentType = "text/plain" } };

            var full = FhirSerializer.SerializeResourceToXml(p);
            Assert.IsTrue(full.Contains("<birthDate"));
            Assert.IsTrue(full.Contains("<photo"));

            var summ = FhirSerializer.SerializeResourceToXml(p, summary: Fhir.Rest.SummaryType.True);
            Assert.IsTrue(summ.Contains("<birthDate"));
            Assert.IsFalse(summ.Contains("<photo"));

            var q = new Questionnaire();
            q.Text = new Narrative() { Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\">Test Questionnaire</div>" };
            q.Status = PublicationStatus.Active;
            q.Date = "2015-09-27";
            q.Title = "TITLE";
            q.Item = new List<Questionnaire.ItemComponent>();
            q.Item.Add(new Questionnaire.ItemComponent()
            {
                LinkId = "linkid",
                Text = "TEXT"
            });

            var qfull = FhirSerializer.SerializeResourceToXml(q);
            Console.WriteLine("summary: Fhir.Rest.SummaryType.False");
            Console.WriteLine(qfull);
            Assert.IsTrue(qfull.Contains("Test Questionnaire"));
            Assert.IsTrue(qfull.Contains("<status value=\"active\""));
            Assert.IsTrue(qfull.Contains("<date value=\"2015-09-27\""));
            Assert.IsTrue(qfull.Contains("<title value=\"TITLE\""));
            Assert.IsTrue(qfull.Contains("<text value=\"TEXT\""));
            Assert.IsTrue(qfull.Contains("<linkId value=\"linkid\""));

            var qSum = FhirSerializer.SerializeResourceToXml(q, summary: Fhir.Rest.SummaryType.True);
            Console.WriteLine("summary: Fhir.Rest.SummaryType.True");
            Console.WriteLine(qSum);
            Assert.IsFalse(qSum.Contains("Test Questionnaire"));
            Assert.IsTrue(qSum.Contains("<status value=\"active\""));
            Assert.IsTrue(qSum.Contains("<date value=\"2015-09-27\""));
            Assert.IsTrue(qSum.Contains("<title value=\"TITLE\""));
            Assert.IsFalse(qSum.Contains("<text value=\"TEXT\""));
            Assert.IsFalse(qSum.Contains("<linkId value=\"linkid\""));

            var qData = FhirSerializer.SerializeResourceToXml(q, summary: Fhir.Rest.SummaryType.Data);
            Console.WriteLine("summary: Fhir.Rest.SummaryType.Data");
            Console.WriteLine(qData);
            Assert.IsFalse(qData.Contains("Test Questionnaire"));
            Assert.IsTrue(qData.Contains("<meta"));
            Assert.IsFalse(qData.Contains("<text value=\"TEXT\""));
            Assert.IsTrue(qData.Contains("<status value=\"active\""));
            Assert.IsTrue(qData.Contains("<date value=\"2015-09-27\""));
            Assert.IsTrue(qData.Contains("<title value=\"TITLE\""));
            Assert.IsTrue(qData.Contains("<linkId value=\"linkid\""));

            var qText = FhirSerializer.SerializeResourceToXml(q, summary: Fhir.Rest.SummaryType.Text);
            Console.WriteLine("summary: Fhir.Rest.SummaryType.Text");
            Console.WriteLine(qText);
            Assert.IsTrue(qText.Contains("Test Questionnaire"));
            Assert.IsTrue(qText.Contains("<meta"));
            Assert.IsTrue(qText.Contains("<status value=\"active\""));
            Assert.IsFalse(qText.Contains("<text value=\"TEXT\""));
            Assert.IsFalse(qText.Contains("<date value=\"2015-09-27\""));
            Assert.IsFalse(qText.Contains("<title value=\"TITLE\""));
            Assert.IsFalse(qText.Contains("<linkId value=\"linkid\""));
            Assert.AreEqual(0, q.Meta.Tag.Where(t => t.System == "http://hl7.org/fhir/v3/ObservationValue" && t.Code == "SUBSETTED").Count(), "Subsetted Tag should not still be there.");

            // Verify that reloading the content into an object...
            var qInflate = FhirXmlParser.Parse<Questionnaire>(qText);
            Assert.AreEqual(1, qInflate.Meta.Tag.Where(t => t.System == "http://hl7.org/fhir/v3/ObservationValue" && t.Code == "SUBSETTED").Count(), "Subsetted Tag should not still be there.");
        }


        private FhirXmlParser FhirXmlParser = new FhirXmlParser();
        private FhirJsonParser FhirJsonParser = new FhirJsonParser();

        [TestMethod]
        public void TestBundleSummary()
        {
            var p = new Patient();

            p.BirthDate = "1972-11-30";     // present in both summary and full
            p.Photo = new List<Attachment>() { new Attachment() { ContentType = "text/plain" } };

            var b = new Bundle();
            b.AddResourceEntry(p, "http://nu.nl/fhir/Patient/1");
            b.Total = 1;

            var full = FhirSerializer.SerializeResourceToXml(b);
            Assert.IsTrue(full.Contains("<entry"));
            Assert.IsTrue(full.Contains("<birthDate"));
            Assert.IsTrue(full.Contains("<photo"));
            Assert.IsTrue(full.Contains("<total"));

            var summ = FhirSerializer.SerializeResourceToXml(b, summary: Fhir.Rest.SummaryType.True);
            Assert.IsTrue(summ.Contains("<entry"));
            Assert.IsTrue(summ.Contains("<birthDate"));
            Assert.IsFalse(summ.Contains("<photo"));
            Assert.IsTrue(summ.Contains("<total"));

            summ = FhirSerializer.SerializeResourceToXml(b, summary: Fhir.Rest.SummaryType.Count);
            Assert.IsFalse(summ.Contains("<entry"));
            Assert.IsFalse(summ.Contains("<birthDate"));
            Assert.IsFalse(summ.Contains("<photo"));
            Assert.IsTrue(summ.Contains("<total"));
        }


        [TestMethod]
        public void TestBundleWithSummaryJson()
        {
            var patientOne = new Patient
            {

                Id = "patient-one",
                Text = new Narrative { Div = "A great blues player" },
                Meta = new Meta { VersionId = "eric-clapton" },

                Name = new List<HumanName> { new HumanName { Family = "Clapton", Use = HumanName.NameUse.Official } },

                Active = true,
                BirthDate = "2015-07-09",
                Gender = AdministrativeGender.Male
            };

            var patientTwo = new Patient()
            {
                Id = "patient-two",
                Active = true,
                Text = new Narrative { Div = "<div>Another great blues player</div>" },
                Meta = new Meta { VersionId = "bb-king" },
                Name = new List<HumanName> { new HumanName { Family = "King", Use = HumanName.NameUse.Nickname } }
            };

            var bundle = new Bundle()
            {
                Id = "my-bundle",
                Total = 1803,
                Type = Bundle.BundleType.Searchset,
                Entry = new List<Bundle.EntryComponent> {
                    new Bundle.EntryComponent { Resource = patientOne, FullUrl = "http://base/Patient/patient-one", Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match } },
                    new Bundle.EntryComponent { Resource = patientTwo, FullUrl = "http://base/Patient/patient-two", Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match } }
                }
            };

            var textBundle = FhirSerializer.SerializeResourceToJson(bundle, Fhir.Rest.SummaryType.Text);
            var dataBundle = FhirSerializer.SerializeResourceToJson(bundle, Fhir.Rest.SummaryType.Data);
            var countBundle = FhirSerializer.SerializeResourceToJson(bundle, Fhir.Rest.SummaryType.Count);
            var trueBundle = FhirSerializer.SerializeResourceToJson(bundle, Fhir.Rest.SummaryType.True);
            var falseBundle = FhirSerializer.SerializeResourceToJson(bundle, Fhir.Rest.SummaryType.False);

            var shouldBeSummaryTrue = TestDataHelper.ReadTestData("summary\\bundle-summary-true.json");
            var shouldBeSummaryText = TestDataHelper.ReadTestData("summary\\bundle-summary-text.json");
            var shouldBeSummaryData = TestDataHelper.ReadTestData("summary\\bundle-summary-data.json");
            var shouldBeSummaryCount = TestDataHelper.ReadTestData("summary\\bundle-summary-count.json");
            var shouldBeSummaryFalse = TestDataHelper.ReadTestData("summary\\bundle-summary-false.json");

            Assert.AreEqual(shouldBeSummaryTrue, trueBundle);
            Assert.AreEqual(shouldBeSummaryData, dataBundle);
            Assert.AreEqual(shouldBeSummaryText, textBundle);
            Assert.AreEqual(shouldBeSummaryCount, countBundle);
            Assert.AreEqual(shouldBeSummaryFalse, falseBundle);
        }

        [TestMethod]
        public void TestBundleWithSummaryXML()
        {
            var patientOne = new Patient
            {
                Id = "patient-one",
                Text = new Narrative { Div = "<div>A great blues player</div>", Status = Narrative.NarrativeStatus.Additional },
                Meta = new Meta { VersionId = "eric-clapton" },

                Name = new List<HumanName> { new HumanName { Family = "Clapton", Use = HumanName.NameUse.Official } },

                Active = true,
                BirthDate = "2015-07-09",
                Gender = AdministrativeGender.Male
            };

            var patientTwo = new Patient()
            {
                Id = "patient-two",
                Active = true,
                Text = new Narrative { Div = "<div>Another great blues player</div>", Status = Narrative.NarrativeStatus.Additional },
                Meta = new Meta { VersionId = "bb-king" },
                Name = new List<HumanName> { new HumanName { Family = "King", Use = HumanName.NameUse.Nickname } }
            };

            var bundle = new Bundle()
            {
                Id = "my-bundle",
                Total = 1803,
                Type = Bundle.BundleType.Searchset,
                Entry = new List<Bundle.EntryComponent> {
                    new Bundle.EntryComponent { Resource = patientOne, FullUrl = "http://base/Patient/patient-one", Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match } },
                    new Bundle.EntryComponent { Resource = patientTwo, FullUrl = "http://base/Patient/patient-two", Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match } }
                }
            };

            var textBundle = FhirSerializer.SerializeResourceToXml(bundle, Fhir.Rest.SummaryType.Text);
            var dataBundle = FhirSerializer.SerializeResourceToXml(bundle, Fhir.Rest.SummaryType.Data);
            var countBundle = FhirSerializer.SerializeResourceToXml(bundle, Fhir.Rest.SummaryType.Count);
            var trueBundle = FhirSerializer.SerializeResourceToXml(bundle, Fhir.Rest.SummaryType.True);
            var falseBundle = FhirSerializer.SerializeResourceToXml(bundle, Fhir.Rest.SummaryType.False);

            var shouldBeSummaryText = TestDataHelper.ReadTestData("summary\\bundle-summary-text.xml");
            var shouldBeSummaryData = TestDataHelper.ReadTestData("summary\\bundle-summary-data.xml");
            var shouldBeSummaryCount = TestDataHelper.ReadTestData("summary\\bundle-summary-count.xml");
            var shouldBeSummaryTrue = TestDataHelper.ReadTestData("summary\\bundle-summary-true.xml");
            var shouldBeSummaryFalse = TestDataHelper.ReadTestData("summary\\bundle-summary-false.xml");

            Assert.AreEqual(falseBundle, shouldBeSummaryFalse);
            Assert.AreEqual(trueBundle, shouldBeSummaryTrue);
            Assert.AreEqual(dataBundle, shouldBeSummaryData);
            Assert.AreEqual(countBundle, shouldBeSummaryCount);
            Assert.AreEqual(textBundle, shouldBeSummaryText);

        }

        [TestMethod]
        public void TestResourceWithSummaryJson()
        {
            var patientOne = new Patient
            {

                Id = "patient-one",
                Text = new Narrative { Div = "A great blues player" },
                Meta = new Meta { VersionId = "1234" },

                Name = new List<HumanName> { new HumanName { Family = "Clapton", Use = HumanName.NameUse.Official } },

                Active = true,
                BirthDate = "2015-07-09",
                Gender = AdministrativeGender.Male
            };

            // Properties with IsSummary == true -> Id, Meta, Active, BirthDate, Gender, Name

            var summaryTrue = FhirSerializer.SerializeResourceToJson(patientOne, Fhir.Rest.SummaryType.True);
            var summaryText = FhirSerializer.SerializeResourceToJson(patientOne, Fhir.Rest.SummaryType.Text);
            var summaryData = FhirSerializer.SerializeResourceToJson(patientOne, Fhir.Rest.SummaryType.Data);
            var summaryFalse = FhirSerializer.SerializeResourceToJson(patientOne, Fhir.Rest.SummaryType.False);
            /* It doesn't make sense to use SummaryType.Count on a single resource hence why I'm not testing it here. */

            var shouldBePatientOneTrue =
                "{\"resourceType\":\"Patient\",\"id\":\"patient-one\",\"meta\":{\"versionId\":\"1234\",\"tag\":[{\"system\":\"http://hl7.org/fhir/v3/ObservationValue\",\"code\":\"SUBSETTED\"}]},\"active\":true,\"name\":[{\"use\":\"official\",\"family\":\"Clapton\"}],\"gender\":\"male\",\"birthDate\":\"2015-07-09\"}";

            var shouldBePatientOneText =
                "{\"resourceType\":\"Patient\",\"id\":\"patient-one\",\"meta\":{\"versionId\":\"1234\",\"tag\":[{\"system\":\"http://hl7.org/fhir/v3/ObservationValue\",\"code\":\"SUBSETTED\"}]},\"text\":{\"div\":\"A great blues player\"}}";

            var shouldBePationeOneData =
                "{\"resourceType\":\"Patient\",\"id\":\"patient-one\",\"meta\":{\"versionId\":\"1234\",\"tag\":[{\"system\":\"http://hl7.org/fhir/v3/ObservationValue\",\"code\":\"SUBSETTED\"}]},\"active\":true,\"name\":[{\"use\":\"official\",\"family\":\"Clapton\"}],\"gender\":\"male\",\"birthDate\":\"2015-07-09\"}";

            var shouldBePatientOneFalse = "{\"resourceType\":\"Patient\",\"id\":\"patient-one\",\"meta\":{\"versionId\":\"1234\"},\"text\":{\"div\":\"A great blues player\"},\"active\":true,\"name\":[{\"use\":\"official\",\"family\":\"Clapton\"}],\"gender\":\"male\",\"birthDate\":\"2015-07-09\"}";

            Assert.AreEqual(summaryTrue, shouldBePatientOneTrue);
            Assert.AreEqual(summaryText, shouldBePatientOneText);
            Assert.AreEqual(summaryData, shouldBePationeOneData);
            Assert.AreEqual(summaryFalse, shouldBePatientOneFalse);
        }

        [TestMethod]
        public void TestResourceWithSummaryXML()
        {
            var patientOne = new Patient
            {

                Id = "patient-one",
                Text = new Narrative { Status = Narrative.NarrativeStatus.Generated, Div = "<div>A great blues player</div>" },
                Meta = new Meta { ElementId = "eric-clapton", VersionId = "1234" },

                Name = new List<HumanName> { new HumanName { Family = "Clapton", Use = HumanName.NameUse.Official } },

                Active = true,
                BirthDate = "2015-07-09",
                Gender = AdministrativeGender.Male
            };

            // Properties with IsSummary == true -> Id, Meta, Active, BirthDate, Gender, Name

            var summaryTrue = FhirSerializer.SerializeResourceToXml(patientOne, Fhir.Rest.SummaryType.True);
            var summaryText = FhirSerializer.SerializeResourceToXml(patientOne, Fhir.Rest.SummaryType.Text);
            var summaryData = FhirSerializer.SerializeResourceToXml(patientOne, Fhir.Rest.SummaryType.Data);
            var summaryFalse = FhirSerializer.SerializeResourceToXml(patientOne, Fhir.Rest.SummaryType.False);

            var shouldBeSummaryTrue = TestDataHelper.ReadTestData("summary\\summary-true.xml");
            var shouldBeSummaryText = TestDataHelper.ReadTestData("summary\\summary-text.xml");
            var shouldBeSummaryData = TestDataHelper.ReadTestData("summary\\summary-data.xml");
            var shouldBeSummaryFalse = TestDataHelper.ReadTestData("summary\\summary-false.xml");

            Assert.AreEqual(shouldBeSummaryTrue, summaryTrue);
            Assert.AreEqual(shouldBeSummaryText, summaryText);
            Assert.AreEqual(shouldBeSummaryData, summaryData);
            Assert.AreEqual(shouldBeSummaryFalse, summaryFalse);
        }

        [TestMethod]
        public void HandleCommentsJson()
        {
            string json = TestDataHelper.ReadTestData("TestPatient.json");

            var pat = FhirJsonParser.Parse<Patient>(json);

            Assert.AreEqual(1, pat.Telecom[0].FhirCommentsElement.Count);
            Assert.AreEqual("   home communication details aren't known   ", pat.Telecom[0].FhirComments.First());

            pat.Telecom[0].FhirCommentsElement.Add(new FhirString("A second line"));

            json = FhirSerializer.SerializeResourceToJson(pat);
            pat = FhirJsonParser.Parse<Patient>(json);

            Assert.AreEqual(2, pat.Telecom[0].FhirCommentsElement.Count);
            Assert.AreEqual("   home communication details aren't known   ", pat.Telecom[0].FhirComments.First());
            Assert.AreEqual("A second line", pat.Telecom[0].FhirComments.Skip(1).First());
        }

        [TestMethod, Ignore]
        public void HandleCommentsXml()
        {
            string xml = TestDataHelper.ReadTestData("TestPatient.xml");

            var pat = FhirXmlParser.Parse<Patient>(xml);

            Assert.AreEqual(1, pat.Name[0].FhirCommentsElement.Count);
            Assert.AreEqual("See if this is roundtripped", pat.Name[0].FhirComments.First());

            pat.Name[0].FhirCommentsElement.Add(new FhirString("A second line"));

            xml = FhirSerializer.SerializeResourceToXml(pat);
            pat = FhirXmlParser.Parse<Patient>(xml);

            Assert.AreEqual(2, pat.Name[0].FhirCommentsElement.Count);
            Assert.AreEqual("See if this is roundtripped", pat.Name[0].FhirComments.First());
            Assert.AreEqual("A second line", pat.Name[0].FhirComments.Skip(1).First());
        }


        [TestMethod]
        public void BundleLinksUnaltered()
        {
            var b = new Bundle();

            b.NextLink = new Uri("Organization/123456/_history/123456", UriKind.Relative);

            var xml = FhirSerializer.SerializeToXml(b);

            b = FhirXmlParser.Parse<Bundle>(xml);

            Assert.IsTrue(!b.NextLink.ToString().EndsWith("/"));
        }

        [TestMethod]
        public void TestIdInSummary()
        {
            var p = new Patient();
            p.Text = new Narrative();
            p.Text.Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\">Some test narrative</div>";
            p.Meta = new Meta();
            p.Contained = new List<Resource>();
            p.Contained.Add(new Organization() { Id = "temp", Name = "temp org", Active = true });
            p.AddExtension("http://example.org/ext", new FhirString("dud"));
            p.Id = "test-id-1";
            p.BirthDate = "1972-11-30";     // present in both summary and full
            p.Photo = new List<Attachment>() { new Attachment() { ContentType = "text/plain", Creation = "45" } };
            p.ManagingOrganization = new ResourceReference() { Display = "temp org", Reference = "#temp" };

            var full = FhirSerializer.SerializeResourceToXml(p);
            Assert.IsTrue(full.Contains("narrative"));
            Assert.IsTrue(full.Contains("dud"));
            Assert.IsTrue(full.Contains("temp org"));
            Assert.IsTrue(full.Contains("<id value="));
            Assert.IsTrue(full.Contains("<birthDate"));
            Assert.IsTrue(full.Contains("<photo"));
            Assert.IsTrue(full.Contains("text/plain"));

            full = FhirSerializer.SerializeResourceToXml(p, summary: Hl7.Fhir.Rest.SummaryType.False);
            Assert.IsTrue(full.Contains("narrative"));
            Assert.IsTrue(full.Contains("dud"));
            Assert.IsTrue(full.Contains("temp org"));
            Assert.IsTrue(full.Contains("contain"));
            Assert.IsTrue(full.Contains("<id value="));
            Assert.IsTrue(full.Contains("<birthDate"));
            Assert.IsTrue(full.Contains("<photo"));
            Assert.IsTrue(full.Contains("text/plain"));

            var summ = FhirSerializer.SerializeResourceToXml(p, summary: Fhir.Rest.SummaryType.True);
            Assert.IsFalse(summ.Contains("narrative"));
            Assert.IsFalse(summ.Contains("dud"));
            Assert.IsFalse(summ.Contains("contain"));
            Assert.IsTrue(summ.Contains("temp org"));
            Assert.IsTrue(summ.Contains("<id value="));
            Assert.IsTrue(summ.Contains("<birthDate"));
            Assert.IsFalse(summ.Contains("<photo"));

            var data = FhirSerializer.SerializeResourceToXml(p, summary: Hl7.Fhir.Rest.SummaryType.Data);
            Assert.IsFalse(data.Contains("narrative"));
            Assert.IsTrue(data.Contains("contain"));
            Assert.IsTrue(data.Contains("dud"));
            Assert.IsTrue(data.Contains("temp org"));
            Assert.IsTrue(data.Contains("<id value="));
            Assert.IsTrue(data.Contains("<birthDate"));
            Assert.IsTrue(data.Contains("<photo"));
        }

        [TestMethod]
        public void TestDecimalPrecisionSerializationInJson()
        {
            var dec6 = 6m;
            var dec60 = 6.0m;

            var obs = new Observation { Value = new FhirDecimal(dec6) };
            var json = FhirSerializer.SerializeResourceToJson(obs);
            var obs2 = FhirJsonParser.Parse<Observation>(json);
            Assert.AreEqual("6", ((FhirDecimal)obs2.Value).Value.Value.ToString(CultureInfo.InvariantCulture));

            obs = new Observation { Value = new FhirDecimal(dec60) };
            json = FhirSerializer.SerializeResourceToJson(obs);
            obs2 = FhirJsonParser.Parse<Observation>(json);
            Assert.AreEqual("6.0", ((FhirDecimal)obs2.Value).Value.Value.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void TestLongDecimalSerialization()
        {
            var dec = 3.1415926535897932384626433833m;
            var obs = new Observation { Value = new FhirDecimal(dec) };
            var json = FhirSerializer.SerializeResourceToJson(obs);
            var obs2 = FhirJsonParser.Parse<Observation>(json);
            Assert.AreEqual(dec.ToString(CultureInfo.InvariantCulture), ((FhirDecimal)obs2.Value).Value.Value.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void TryScriptInject()
        {
            var x = new Patient();

            x.Name.Add(HumanName.ForFamily("<script language='javascript'></script>"));

            var xml = FhirSerializer.SerializeResourceToXml(x);
            Assert.IsFalse(xml.Contains("<script"));
        }


        [TestMethod]
        public void TryXXEExploit()
        {
            var input =
                "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n" +
                "<!DOCTYPE foo [  \n" +
                "<!ELEMENT foo ANY >\n" +
                "<!ENTITY xxe SYSTEM \"file:///etc/passwd\" >]>" +
                "<Patient xmlns=\"http://hl7.org/fhir\">" +
                    "<text>" +
                        "<div xmlns=\"http://www.w3.org/1999/xhtml\">TEXT &xxe; TEXT</div>\n" +
                    "</text>" +
                    "<address>" +
                        "<line value=\"FOO\"/>" +
                    "</address>" +
                "</Patient>";

            try
            {
                FhirXmlParser.Parse<Resource>(input);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("DTD is prohibited"));
            }
        }

        [TestMethod]
        public void SerializeUnknownEnums()
        {
            string xml = TestDataHelper.ReadTestData("TestPatient.xml");
            var pser = new FhirXmlParser();
            var p = pser.Parse<Patient>(xml);
            string outp = FhirSerializer.SerializeResourceToXml(p);
            Assert.IsTrue(outp.Contains("\"male\""));

            // Pollute the data with an incorrect administrative gender
            p.GenderElement.ObjectValue = "superman";

            outp = FhirSerializer.SerializeResourceToXml(p);
            Assert.IsFalse(outp.Contains("\"male\""));
            Assert.IsTrue(outp.Contains("\"superman\""));
        }


        [TestMethod]
        public void TestNullExtensionRemoval()
        {
            var p = new Patient
            {
                Extension = new List<Extension>
                {
                    new Extension("http://hl7.org/fhir/Profile/iso-21090#qualifier", new Code("VV")),
                    null
                },

                Contact = new List<Patient.ContactComponent>
                {
                    null,
                    new Patient.ContactComponent { Name = HumanName.ForFamily("Kramer") },
                }
            };

            var xml = FhirSerializer.SerializeResourceToXml(p);

            var p2 = (new FhirXmlParser()).Parse<Patient>(xml);
            Assert.AreEqual(1, p2.Extension.Count);
            Assert.AreEqual(1, p2.Contact.Count);
        }

        [TestMethod]
        public void SerializeEmptyParams()
        {
            var par = new Parameters();
            var xml = FhirSerializer.SerializeResourceToXml(par);

            var par2 = (new FhirXmlParser()).Parse<Parameters>(xml);
            Assert.AreEqual(0, par2.Parameter.Count);
        }

        // [WMR 20161222] Richard Kavanagh: serializing ValueSet (to XML) throws an exception...?
        // Cause: { ... "text" { ... "div" = "removed" } ... }
        // => "removed" is not valid Xhtml contents (no root)! Should be e.g. "<p>removed</p>"
        // However: http://www.hl7.org/implement/standards/fhir/narrative.html#Narrative
        // => div SHOULD accept plain text!
        [TestMethod]
        public void SerializeValueSet()
        {
            // var res = new ValueSet() { Url = "http://example.org/fhir/ValueSet/MyValueSetExample" };

            string json = TestDataHelper.ReadTestData(@"valueset-v2-0717.json");
            Assert.IsNotNull(json);
            var parser = new FhirJsonParser();
            var vs = parser.Parse<ValueSet>(json);
            Assert.IsNotNull(vs);

            var xml = FhirSerializer.SerializeResourceToXml(vs);
            Assert.IsNotNull(xml);
        }

        [TestMethod]
        public void TestClaimJsonSerialization()
        {
            var c = new Claim();
            c.Payee = new Claim.PayeeComponent();
            c.Payee.Type = new CodeableConcept(null, "test");
            c.Payee.ResourceType = new Coding(null, "test2");
            c.Payee.Party = new ResourceReference("Practitioner/example", "Example, Dr John");

            string json = FhirSerializer.SerializeResourceToJson(c);
            var c2 = new FhirJsonParser().Parse<Claim>(json);
            Assert.AreEqual("test", c2.Payee.Type.Coding[0].Code);
            Assert.AreEqual("test2", c2.Payee.ResourceType.Code);
            Assert.AreEqual("Practitioner/example", c2.Payee.Party.Reference);
        }

        [FhirType("Bundle", IsResource = true)]
        //[DataContract]
        public class CustomBundle : Bundle
        {
            public CustomBundle() : base() { }
        }

        // [WMR 20170825] Richard Kavanagh: runtime exception while serializating derived PoCo classes
        // Workaround: add the FhirType attribute to derived class
        [TestMethod]
        public void TestDerivedPoCoSerialization()
        {
            var bundle = new CustomBundle()
            {
                Type = Bundle.BundleType.Collection,
                Id = "MyBundle"
            };

            var xml = FhirSerializer.SerializeResourceToXml(bundle);
            Assert.IsNotNull(xml);

            var json = FhirSerializer.SerializeResourceToJson(bundle);
            Assert.IsNotNull(json);
        }
    }
}
