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
        class ItemContext
        {
            internal IEnumerable<Base> Data;
            internal StructureItem Item;
            internal Bundle Resources;
        }

        /// <summary>
        /// This is effectively a variation on the $populate operation
        /// </summary>
        /// <param name="questionnaire"></param>
        /// <param name="resources"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static QuestionnaireResponse CreateQuestionnaireResponse(Model.Questionnaire questionnaire, Bundle resources, IResourceResolver source)
        {
            QuestionnaireResponse qa = new QuestionnaireResponse();
            qa.Questionnaire = "Questionnaire/" + questionnaire.Id;

            // Get the top level data context
            ItemContext data = RetrieveContextContent(questionnaire, new ItemContext { Resources = resources }, questionnaire, source);

            CreateAndPopulateGroup(questionnaire.Item, qa.Item, data, questionnaire, source);
            return qa;
        }

        private static ItemContext RetrieveContextContent(IExtendable scopedNode, ItemContext item, Model.Questionnaire questionnaire, IResourceResolver source)
        {
            string path = null;
            string definition = null;
            string linkId = null;
            if (scopedNode is Questionnaire.ItemComponent group)
            {
                definition = group.Definition;
                linkId = group.LinkId;
            }
            if (scopedNode is Questionnaire q)
            {
                definition = q.GetExtensionValue<FhirUri>("http://hl7.org/fhir/StructureDefinition/extension-Questionnaire.item.definition")?.Value;
            }

            if (!string.IsNullOrEmpty(definition))
            {
                if (!definition.Contains("#"))
                {
                    var si = StructureItemTree.GetStructureTree(definition, questionnaire, source);
                    if (si != null)
                    {
                        // now locate the resource of this type in the bundle
                        var resource = item.Resources.GetResources().Where(i => i.GetType() == si.ClassMapping.NativeType).FirstOrDefault();
                        if (resource != null)
                        {
                            ItemContext newContext = new ItemContext() { Item = si, Data = new List<Base> { resource } ,Resources = item.Resources };
                            return newContext;
                        }
                    }
                    // just pass through the provided context - no successful change was made
                    return item;
                }
                // retrieve the referenced path
                path = definition.Substring(definition.IndexOf("#") + 1);
            }
            else
            {
                path = linkId;
            }

            // Check to see if we need to navigate into a child part of the resource itself
            if (!string.IsNullOrEmpty(path))
            {
                if (StructureItemTree.ContainsPath(item.Item, path))
                {
                    ItemContext newChildContext = new ItemContext() { Item = item.Item, Resources = item.Resources };
                    List<Base> data = new List<Base>();
                    foreach (var dataItem in item.Data)
                    {
                        StructureItem extra;
                        IEnumerable<Base> prepopulatedValues = StructureItemTree.GetValues(item.Item, dataItem, path, linkId, out extra);
                        if (prepopulatedValues.Any())
                        {
                            data.AddRange(prepopulatedValues);
                            newChildContext.Item = extra;
                        }
                    }
                    if (data.Any())
                    {
                        newChildContext.Data = data;
                        return newChildContext;
                    }
                }
            }

            // just pass through the provided context - no change was made
            return item; 
        }
        /*
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
        private static List<QuestionnaireResponse.AnswerComponent> ExtractValue(ItemContext contextData, Questionnaire.ItemComponent qsource, IResourceResolver source)
        {
            string definition = qsource.Definition;
            string linkId = qsource.LinkId;
            string path = linkId;

            if (!string.IsNullOrEmpty(definition))
            {
                // this will be the structure definition, with the elementID (or path) to the right of the #
                if (definition.Contains("#"))
                {
                    string elementId = definition.Substring(definition.IndexOf("#")+1);
                    path = elementId;
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                // Nothing to process, so bail fast
                return null;
            }
            List<Base> prepopulatedValues = new List<Base>();
            StructureItem extra = null;
            foreach (var itemData in contextData.Data)
            {
                prepopulatedValues.AddRange(StructureItemTree.GetValues(contextData.Item, itemData, path, linkId, out extra));
            }

            if (prepopulatedValues.Count() == 0)
                return null; // no content to extract

            if (prepopulatedValues.Count() > 0)
            {
                foreach (var t2 in prepopulatedValues)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", path, t2?.ToString()));
                }
            }

            var results = new List<QuestionnaireResponse.AnswerComponent>();
            switch (qsource.Type.Value)
            {
                case Questionnaire.QuestionnaireItemType.Boolean:
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
                case Questionnaire.QuestionnaireItemType.Decimal:
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
                case Questionnaire.QuestionnaireItemType.Integer:
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
                case Questionnaire.QuestionnaireItemType.Date:
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
                case Questionnaire.QuestionnaireItemType.DateTime:
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
                //case Questionnaire.QuestionnaireItemType.Instant:
                //    foreach (var item in prepopulatedValues)
                //    {
                //        if (item is Instant)
                //        {
                //            var a = new QuestionnaireResponse.AnswerComponent();
                //            results.Add(a);
                //            a.Value = item as Instant;
                //            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                //                break;
                //        }
                //    }
                //    break;
                case Questionnaire.QuestionnaireItemType.Time:
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
                case Questionnaire.QuestionnaireItemType.String:
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
                        if (item is Primitive primitive)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = new FhirString(primitive.ObjectValue as string);
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.QuestionnaireItemType.Text:
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
                case Questionnaire.QuestionnaireItemType.Url:
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
                case Questionnaire.QuestionnaireItemType.Choice:
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
                        else if (item is ISystemAndCode primitive)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = new Coding(primitive.System, primitive.Code);
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.QuestionnaireItemType.OpenChoice:
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
                        else if (item is ISystemAndCode primitive)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = new Coding(primitive.System, primitive.Code);
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                        else if (item is FhirString)
                        {
                            var a = new QuestionnaireResponse.AnswerComponent();
                            results.Add(a);
                            a.Value = item as FhirString;
                            if (!qsource.Repeats.HasValue || !qsource.Repeats.Value)
                                break;
                        }
                    }
                    break;
                case Questionnaire.QuestionnaireItemType.Attachment:
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
                case Questionnaire.QuestionnaireItemType.Reference:
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
                case Questionnaire.QuestionnaireItemType.Quantity:
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
        
        private static void CreateAndPopulateGroup(List<Questionnaire.ItemComponent> itemsQ, List<QuestionnaireResponse.ItemComponent> itemsAnswer, ItemContext contextData, Model.Questionnaire questionnaire, IResourceResolver source)
        {
            // Process any child Questions on this group
            if (itemsQ != null && itemsQ.Count > 0)
            {
                foreach (var itemQ in itemsQ)
                {
                    // TODO: Source content from DataElements if required
                    ItemContext dataForItem = RetrieveContextContent(itemQ, contextData, questionnaire, source);

                    if (itemQ.Type == Questionnaire.QuestionnaireItemType.Group)
                    {
                        if (dataForItem.Data.Any())
                        {
                            if (!itemQ.Repeats.HasValue || itemQ.Repeats.Value == false)
                                dataForItem.Data = dataForItem.Data.Take(1);
                            foreach (var data in dataForItem.Data)
                            {
                                var newg = new QuestionnaireResponse.ItemComponent();
                                newg.LinkId = itemQ.LinkId;
                                newg.Definition = itemQ.Definition;
                                newg.Text = itemQ.Text;
                                itemsAnswer.Add(newg);
                                CreateAndPopulateGroup(itemQ.Item, newg.Item, new ItemContext { Data = new[] { data }, Item = dataForItem.Item, Resources = dataForItem.Resources }, questionnaire, source);
                            }
                        }
                        else
                        {
                            // There must be at least 1 group, so lets create one.
                            var newg = new QuestionnaireResponse.ItemComponent();
                            newg.LinkId = itemQ.LinkId;
                            newg.Definition = itemQ.Definition;
                            newg.Text = itemQ.Text;
                            itemsAnswer.Add(newg);
                            CreateAndPopulateGroup(itemQ.Item, newg.Item, dataForItem, questionnaire, source);
                        }
                    }
                    else
                    {
                        var a = new QuestionnaireResponse.ItemComponent();
                        a.LinkId = itemQ.LinkId;
                        a.Definition = itemQ.Definition;
                        a.Text = itemQ.Text;
                        a.Answer = ExtractValue(dataForItem, itemQ, source);

                        // TODO: Check for a default value
                        if (a.Answer == null || a.Answer.Count == 0)
                        {
                            foreach (var e in itemQ.GetExtensions("http://hl7.org/fhir/StructureDefinition/questionnaire-defaultValue"))
                            {
                                if (a.Answer == null)
                                    a.Answer = new List<QuestionnaireResponse.AnswerComponent>();
                                a.Answer.Add(new QuestionnaireResponse.AnswerComponent() { Value = e.Value });
                                // should we actually validate the default value is of the correct type?
                            }
                        }
                        if (a.Answer != null && a.Answer.Count > 0)
                            itemsAnswer.Add(a);
                    }

                    // Process any group items that this question has attached
                }
            }
        }
    }
}
