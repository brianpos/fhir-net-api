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
    public class SQLViewExtractTests
    {
        [TestMethod]
        public void QuestionnaireCreateSqlViewExtract()
        {
            string xml = System.IO.File.ReadAllText(
                @"TestData\hcxdir-practitioner.xml");

            var pracSd = new Serialization.FhirXmlParser().Parse<StructureDefinition>(xml);
            var si = StructureItemTree.CreateStructureTree(pracSd, TestHelpers.Source);
            string resourceName = si.ed.Base.Path;
            System.Diagnostics.Debug.WriteLine("======================================");
            System.Diagnostics.Debug.WriteLine("WITH XMLNAMESPACES ('http://hl7.org/fhir' AS fhir)");
            System.Diagnostics.Debug.WriteLine("select top 100");
            System.Diagnostics.Debug.WriteLine("    head.internal_id,");
            OutputChildProperties(si, null, $"fhir:{resourceName}");
            //foreach (var child in si.Children)
            //{
            //    // only interested in single cardinality properties
            //    if (child?.ed?.Max == "1")
            //    {
            //        System.Diagnostics.Debug.WriteLine($"    t1.c1.value('(fhir:{resourceName}/fhir:{child.FhirpathExpression}/@value)[1]', 'nvarchar(max)'),  -- {child.FhirpathExpression} {child.ed.Min}..{child.ed.Max} {child.Children.Count} {child.SlicedPath}");
            //        if (child.Children.Any())
            //        {
            //            OutputChildProperties(child, child.FhirpathExpression, $"fhir:{resourceName}/fhir:{child.FhirpathExpression}");
            //        }
            //    }
            //}
            System.Diagnostics.Debug.WriteLine("    head.resource_id");
            System.Diagnostics.Debug.WriteLine("from");
            System.Diagnostics.Debug.WriteLine($"fhir.{resourceName}_resource head with (nolock)");
            System.Diagnostics.Debug.WriteLine($"inner join fhir.{resourceName}_resource_history currentEntry with (nolock)");
            System.Diagnostics.Debug.WriteLine("ON head.internal_id = currentEntry.internal_id and head.current_version_id = currentEntry.version_id");
            System.Diagnostics.Debug.WriteLine("cross apply currentEntry.contentXML.nodes('declare namespace fhir = \"http://hl7.org/fhir\"; (/)') as t1(c1)");

            System.Diagnostics.Debug.WriteLine("======================================");
            TestHelpers.DumpTree(si);
        }

        private void OutputChildProperties(StructureItem parent, string path, string xpath)
        {
            foreach (var child in parent.Children)
            {
                // only interested in single cardinality properties
                if (child?.ed?.Max == "1")
                {
                    if (!string.IsNullOrEmpty(child.ExtensionUrl))
                    {
                        System.Diagnostics.Debug.WriteLine($"    t1.c1.value('({xpath}/fhir:{child.FhirpathExpression}[@url=\"{child.ExtensionUrl}\"]/@value)[1]', 'nvarchar(max)') as '{path?.Replace(".", "_")}{child.FhirpathExpression}:{child.ed.SliceName}',  -- {path}{child.FhirpathExpression} {child.ed.Min}..{child.ed.Max} {child.Children.Count} {child.SlicedPath}");
                        if (child.Children.Any())
                            OutputChildProperties(child, $"{path}{child.FhirpathExpression}:{child.ed.SliceName}.", $"{xpath}/fhir:{child.FhirpathExpression}[@url=\"{child.ExtensionUrl}\"]");
                    }
                    else if (!string.IsNullOrEmpty(child.SlicedPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"    t1.c1.value('({xpath}/fhir:{child.FhirpathExpression}[{child.SlicedPath.Replace(".", "/fhir:")}/@value=\"{child.FixedValuesInSlices.FirstOrDefault()}\"]/@value)[1]', 'nvarchar(max)') as '{path?.Replace(".", "_")}{child.FhirpathExpression}',  -- {path}{child.FhirpathExpression} {child.ed.Min}..{child.ed.Max} {child.Children.Count} {child.SlicedPath}");
                        if (child.Children.Any())
                            OutputChildProperties(child, $"{path}{child.FhirpathExpression}:{child.ed.SliceName}.", $"{xpath}/fhir:{child.FhirpathExpression}[{child.SlicedPath.Replace(".", "/fhir:")}/@value=\"{child.FixedValuesInSlices.FirstOrDefault()}\"]");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"    t1.c1.value('({xpath}/fhir:{child.FhirpathExpression}/@value)[1]', 'nvarchar(max)') as '{path?.Replace(".","_")}{child.FhirpathExpression}',  -- {path}{child.FhirpathExpression} {child.ed.Min}..{child.ed.Max} {child.Children.Count} {child.SlicedPath}");
                        if (child.Children.Any())
                            OutputChildProperties(child, $"{path}{child.FhirpathExpression}.", $"{xpath}/fhir:{child.FhirpathExpression}");
                    }
                }
            }
        }
    }
}
