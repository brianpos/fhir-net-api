using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Hl7.FhirPath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Navigation;
using System.Collections;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Snapshot;

namespace Hl7.Fhir.QuestionnaireServices
{
    /// <summary>
    /// Class with properties that would be consistent with STU3 extensions
    /// </summary>
    public static class QuestionnaireExtentionsForSTU3
    {
        public static FhirUri Definition(this Model.Questionnaire.QuestionComponent me)
        {
            return me.GetExtensionValue<FhirUri>("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition");
        }
        public static void Definition(this Model.Questionnaire.QuestionComponent me, FhirUri value)
        {
            me.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", value);
        }
        public static FhirUri Definition(this Model.Questionnaire.GroupComponent me)
        {
            return me.GetExtensionValue<FhirUri>("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition");
        }
        public static void Definition(this Model.Questionnaire.GroupComponent me, FhirUri value)
        {
            me.AddExtension("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition", value);
        }
    }
}
