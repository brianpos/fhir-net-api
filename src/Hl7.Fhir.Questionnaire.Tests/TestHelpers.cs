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
using Hl7.Fhir.FhirPath;

namespace Hl7.Fhir.QuestionnaireServices.Tests
{
    [TestClass]
    public static class TestHelpers
    {
        [AssemblyInitialize]
        public static void SetupSource(TestContext context)
        {
            // Ensure the FHIR extensions are registered
            ElementNavFhirExtensions.PrepareFhirSymbolTableFunctions();

            _source = new CachedResolver(
                new MultiResolver(
                    new ZipSource("specification.zip"),
                    new DirectorySource("TestData")
                ));
        }

        private static IResourceResolver _source;
        public static IResourceResolver Source
        {
            get
            {
                return _source;
            }
        }

        public static int DumpTree(StructureItem tree)
        {
            int count = 1;
            if (!string.IsNullOrEmpty(tree.SlicedPath))
                System.Diagnostics.Debug.WriteLine($"{tree.Path}\t\t{tree.FixedValuesInSlices?.Count}\t\t{tree.MapTo("tcm")}");
            else
                System.Diagnostics.Debug.WriteLine($"{tree.Path}\t\t-->{tree.SlicedPath}\t{tree.FixedValuesInSlices?.Count}\t{tree.MapTo("tcm")}");
            foreach (var item in tree.Children)
            {
                count += DumpTree(item);
            }
            return count;
        }
    }
}
