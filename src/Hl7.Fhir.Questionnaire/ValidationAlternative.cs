using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Hl7.Fhir.QuestionnaireServices
{
    public class ValidationProcessing
    {
        // Selection of the elements in the slice (create a fhirpath expression with a where to find all the children)
        // these can be removed from the set, leaving all the other non sliced content

        public OperationOutcome Validate(Base context, StructureDefinition sd)
        {
            StructureItem parent = StructureItemTree.CreateStructureTree(sd, null);
            return parent.Validate(context.ToTypedElement(), context.ToTypedElement());
        }
    }

    /// <summary>
    /// This is a subset of the ElementDefinition class with only enough
    /// information for the validation system to work with
    /// (and that we can quickly serialize to a binary form)
    /// </summary>
    public class MinimalElementDefinition
    {
        string id { get; set; }

        /// <summary>
        /// The value that is used for matching into the Questionnaire.(Group|Question).LinkId
        /// </summary>
        string code { get; set; }

        /// <summary>
        /// The label that can be used to display in error messages
        /// </summary>
        string shortDescription { get; set; }
        int minCardinality { get; set; }
        long maxCardinality { get; set; }
        List<Element> type { get; set; }
        Element fixedValue { get; set; }
        // pattern?
        Element minValue { get; set; }
        Element maxValue { get; set; }
        long maxStringLength { get; set; }
        // condition?
        List<ElementDefinition.ConstraintComponent> constraints { get; set; }
        string bindingStrenth { get; set; } // this is really the enumeration
        string bindingReference { get; set; }
        string bindingUri { get; set; }
    }
}
