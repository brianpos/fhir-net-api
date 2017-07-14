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
using Hl7.Fhir.Rest;

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
        public static QuestionnaireResponse CreateQuestionnaireResponse(Model.Questionnaire questionnaire, Bundle resources, IResourceResolver source)
        {
            QuestionnaireResponse qa = new QuestionnaireResponse();
            qa.Group = new QuestionnaireResponse.GroupComponent();
            qa.Questionnaire = new ResourceReference();
            qa.Questionnaire.Reference = "Questionnaire/" + questionnaire.Id;

            // Get the top level data context
            IElementNavigator data = null; // RetrieveContextContent(questionnaire.Group, resources);

            CreateAndPopulateGroup(questionnaire.Group, qa.Group, data);
            return qa;
        }

        /*
        private static IElementNavigator RetrieveContextContent(IExtendable scopedNode, Bundle resources)
        {
            FhirString filter = scopedNode.GetExtensionValue<FhirString>("expression");
            if (filter == null || string.IsNullOrEmpty(filter.Value))
                filter = new FhirString("/");

            FhirString query = scopedNode.GetExtensionValue<FhirString>("query");
            if (query != null && !String.IsNullOrEmpty(query.Value))
            {
                // Validate that the query contains the subject in some way
                if (!query.Value.Contains("[$subject]"))
                    return parentContext;

                if (query.Value == "Patient/[$subject]")
                    query.Value = "[$subject]";

                System.Diagnostics.Debug.WriteLine(String.Format("Query: {0}\r\nFilter: {1}", query, filter));

                // remove the _format=xml if it is there
                query.Value = query.Value.Replace("_format=xml", "")
                                    .Replace("&&", "&")
                                    .Replace("?&", "?")
                                    .TrimEnd('&')
                                    .TrimEnd('?');
                IModelBase model = null;

                ResourceIdentity ri = new ResourceIdentity(subject.Reference);
                int indexSubject = query.Value.IndexOf("[$subject]");
                if (query.Value.Contains("?") && indexSubject > query.Value.IndexOf("?"))
                {
                    // this is a search execution
                    string executeQuery = query.Value.TrimStart('/').Replace("[$subject]", ri.MakeRelative().WithoutVersion().OriginalString);
                    string ResourceName = executeQuery.Substring(0, executeQuery.IndexOf('/'));
                    if (ResourceName.Contains("?"))
                        ResourceName = ResourceName.Substring(0, ResourceName.IndexOf('?'));
                    model = sqlonfhir.Controllers.StandardResourceController.GetModel(ResourceName, Inputs);
                    if (model != null)
                    {
                        Bundle result = new Bundle();
                        result.Meta = new Meta();
                        result.Id = new Uri("urn:uuid:" + Guid.NewGuid().ToString("n")).OriginalString;

                        HttpRequestMessage temp = new HttpRequestMessage();
                        temp.RequestUri = new Uri(Inputs.BaseUri + executeQuery);
                        var parameters = temp.TupledParameters(true);

                        result.ResourceBase = Inputs.BaseUri;
                        result.Type = Bundle.BundleType.Searchset;
                        // limit the query to only get 1000 entries (should be a system parameter)
                        result.Total = model.Search(parameters, result.Entry, 1000, 0, SummaryType.False);
                        IElementNavigator tree = new PocoNavigator(result);
                        return tree;
                    }
                }
                else
                {
                    // this is a pure get
                    string actualQuery = query.Value.Replace("[$subject]", ri.MakeRelative().WithoutVersion().OriginalString);

                    ResourceIdentity riQuery = new ResourceIdentity(actualQuery);
                    model = sqlonfhir.Controllers.StandardResourceController.GetModel(riQuery.ResourceType, this.Inputs);
                    // model.Request = this.Request;
                    DomainResource resource = model.Get(riQuery.Id, null, Hl7.Fhir.Rest.SummaryType.False) as DomainResource;
                    IElementNavigator tree = new PocoNavigator(resource);
                    return tree;
                }
            }
            return parentContext;
        }

        private static IElementNavigator RetrieveContextContent(IExtendable scopedNode, IElementNavigator parentContext)
        {
            FhirString filter = scopedNode.GetExtensionValue<FhirString>("expression");
            if (filter == null || string.IsNullOrEmpty(filter.Value))
                filter = new FhirString("/");

            FhirString query = scopedNode.GetExtensionValue<FhirString>("query");
            if (query != null && !String.IsNullOrEmpty(query.Value))
            {
                // Validate that the query contains the subject in some way
                if (!query.Value.Contains("[$subject]"))
                    return parentContext;

                if (query.Value == "Patient/[$subject]")
                    query.Value = "[$subject]";

                System.Diagnostics.Debug.WriteLine(String.Format("Query: {0}\r\nFilter: {1}", query, filter));

                // remove the _format=xml if it is there
                query.Value = query.Value.Replace("_format=xml", "")
                                    .Replace("&&", "&")
                                    .Replace("?&", "?")
                                    .TrimEnd('&')
                                    .TrimEnd('?');
                IModelBase model = null;

                ResourceIdentity ri = new ResourceIdentity(subject.Reference);
                int indexSubject = query.Value.IndexOf("[$subject]");
                if (query.Value.Contains("?") && indexSubject > query.Value.IndexOf("?"))
                {
                    // this is a search execution
                    string executeQuery = query.Value.TrimStart('/').Replace("[$subject]", ri.MakeRelative().WithoutVersion().OriginalString);
                    string ResourceName = executeQuery.Substring(0, executeQuery.IndexOf('/'));
                    if (ResourceName.Contains("?"))
                        ResourceName = ResourceName.Substring(0, ResourceName.IndexOf('?'));
                    model = sqlonfhir.Controllers.StandardResourceController.GetModel(ResourceName, Inputs);
                    if (model != null)
                    {
                        Bundle result = new Bundle();
                        result.Meta = new Meta();
                        result.Id = new Uri("urn:uuid:" + Guid.NewGuid().ToString("n")).OriginalString;

                        HttpRequestMessage temp = new HttpRequestMessage();
                        temp.RequestUri = new Uri(Inputs.BaseUri + executeQuery);
                        var parameters = temp.TupledParameters(true);

                        result.ResourceBase = Inputs.BaseUri;
                        result.Type = Bundle.BundleType.Searchset;
                        // limit the query to only get 1000 entries (should be a system parameter)
                        result.Total = model.Search(parameters, result.Entry, 1000, 0, SummaryType.False);
                        IElementNavigator tree = new PocoNavigator(result);
                        return tree;
                    }
                }
                else
                {
                    // this is a pure get
                    string actualQuery = query.Value.Replace("[$subject]", ri.MakeRelative().WithoutVersion().OriginalString);

                    ResourceIdentity riQuery = new ResourceIdentity(actualQuery);
                    model = sqlonfhir.Controllers.StandardResourceController.GetModel(riQuery.ResourceType, this.Inputs);
                    // model.Request = this.Request;
                    DomainResource resource = model.Get(riQuery.Id, null, Hl7.Fhir.Rest.SummaryType.False) as DomainResource;
                    IElementNavigator tree = new PocoNavigator(resource);
                    return tree;
                }
            }
            return parentContext;
        }
        */
        private static List<QuestionnaireResponse.AnswerComponent> ExtractValue(IElementNavigator contextData, Questionnaire.QuestionComponent qsource)
        {
            FhirString expression = qsource.GetExtensionValue<FhirString>("expression");
            if (expression == null || string.IsNullOrEmpty(expression.Value)) // the context data can be empty, as some expressions don't need content
            {
                // Nothing to process, so bail fast
                return null;
            }
            // Don't need to cache this, it is cached in the fhir-client
            System.Diagnostics.Debug.WriteLine(string.Format("expression: {0}", expression.Value));
            Hl7.FhirPath.CompiledExpression xps = null;// PatientModel._compiler.Compile(expression.Value);

            if (xps == null)
                return null; // this is not expected, as would throw an exception on the compilation step

            var results = new List<QuestionnaireResponse.AnswerComponent>();
            IEnumerable<Base> prepopulatedValues = xps(contextData, contextData).ToFhirValues();

            if (prepopulatedValues.Count() == 0)
                return null; // no content to extract

            if (prepopulatedValues.Count() > 0)
            {
                foreach (var t2 in prepopulatedValues)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", expression.Value, t2.ToString()));
                }
            }

            switch (qsource.Type.Value)
            {
                case Questionnaire.AnswerFormat.Boolean:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is FhirBoolean)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirBoolean;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Decimal:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is FhirDecimal)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirDecimal;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Integer:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Integer)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Integer;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Date:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Date)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Date;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                        if (item is FhirDateTime)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = new Date((item as FhirDateTime).ToDateTime().ToFhirDate());
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.DateTime:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Date)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = new FhirDateTime((item as Date).ToDateTime().ToFhirDateTime());
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                        if (item is FhirDateTime)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirDateTime;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Instant:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Instant)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Instant;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Time:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Time)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Time;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.String:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is FhirString)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirString;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Text:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is FhirString)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirString;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Url:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is FhirUri)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirUri;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Choice:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Coding)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Coding;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.OpenChoice:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Coding)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Coding;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                        if (item is FhirString)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirString;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Attachment:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Attachment)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Attachment;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Reference:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is ResourceReference)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as ResourceReference;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.AnswerFormat.Quantity:
                    foreach (var item in prepopulatedValues)
                    {
                        if (item is Quantity)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as Quantity;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            
            return results;
        }
        
        private static void CreateAndPopulateGroup(Questionnaire.GroupComponent qg, QuestionnaireResponse.GroupComponent answer_group, IElementNavigator contextData)
        {
            // Initialize the base Group values
            answer_group.LinkId = qg.LinkId;
            answer_group.Title = qg.Title;
            answer_group.Text = qg.Text;

            // Process any child Questions on this group
            if (qg.Question != null && qg.Question.Count > 0)
            {
                answer_group.Question = new List<QuestionnaireResponse.QuestionComponent>();
                foreach (var sq in qg.Question)
                {
                    IElementNavigator dataForQuestion = null; // RetrieveContextContent(sq, subject, contextData);

                    var a = new QuestionnaireResponse.QuestionComponent();
                    a.LinkId = sq.LinkId;
                    a.Text = sq.Text;
                    a.Answer = ExtractValue(contextData, sq);
                    answer_group.Question.Add(a);

                    // TODO: Check for a default value
                    if (a.Answer == null || a.Answer.Count == 0)
                    {
                        foreach (var e in sq.GetExtensions("http://hl7.org/fhir/StructureDefinition/questionnaire-defaultValue"))
                        {
                            if (a.Answer == null)
                                a.Answer = new List<QuestionnaireResponse.AnswerComponent>();
                            a.Answer.Add(new QuestionnaireResponse.AnswerComponent() { Value = e.Value });
                            // should we actually validate the default value is of the correct type?
                        }
                    }

                    // Process any group items that this question has attached
                }
            }

            // Process any child Groups on this group
            if (qg.Group != null && qg.Group.Count > 0)
            {
                answer_group.Group = new List<QuestionnaireResponse.GroupComponent>();
                foreach (var sg in qg.Group)
                {
                    // TODO: Source content from DataElements if required
                    IElementNavigator dataForGroup = null; // RetrieveContextContent(sg, subject, contextData);

                    // Check if this is a required group and no content was pre-populated.
                    // if (sg.Required.HasValue && sg.Required.Value && answer_group.Group.Count == 0)
                    {
                        // There must be at least 1 group, so lets create one.
                        var newg = new QuestionnaireResponse.GroupComponent();
                        answer_group.Group.Add(newg);
                        CreateAndPopulateGroup(sg, newg, dataForGroup);
                    }
                }
            }
        }
    }
}
