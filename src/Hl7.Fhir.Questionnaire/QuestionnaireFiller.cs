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
    public static class QuestionnaireFiller
    {
        /// <summary>
        /// This is effectively a variation on the $populate operation
        /// </summary>
        /// <param name="qPart1"></param>
        /// <param name="resources"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static QuestionnaireResponse CreateQuestionnaireResponse(Model.Questionnaire qPart1, Bundle resources, IResourceResolver source)
        {
            return null;
        }
    }
}
