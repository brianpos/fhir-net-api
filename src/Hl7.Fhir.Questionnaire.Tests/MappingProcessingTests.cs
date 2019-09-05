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
    public class MappingProcessingTests
    {
        [TestMethod]
        public void QuestionnaireCreateTCMPatientDefinition()
        {
            string xml = System.IO.File.ReadAllText(
                @"C:\Temp\TCM Patient.dstu2.structuredefinition.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            // System.Diagnostics.Debug.WriteLine("======================================");
            // DumpTree(si);
            System.Diagnostics.Debug.WriteLine("======================================");
            StructureItemTree.PruneTreeForMapping(si, "tcm");
            TestHelpers.DumpTree(si);
        }
    }
}
