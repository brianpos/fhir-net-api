using Hl7.Fhir.Model;

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
