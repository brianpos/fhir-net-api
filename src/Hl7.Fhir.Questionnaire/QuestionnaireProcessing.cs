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
    public static class QuestionnaireProcessing
    {
        public static Bundle CreateResourceInstances(Questionnaire q, QuestionnaireResponse questionnaireResponse, IResourceResolver source)
        {
            // Loop through all the groups to locate the items that are marked against a resource type
            List<QuestionnaireResponse.GroupComponent> qrg = new List<QuestionnaireResponse.GroupComponent>();
            qrg.Add(questionnaireResponse.Group);
            var result = CreateResourceInstances(q, q.Group, qrg, source);
            return result;
        }

        internal static Bundle CreateResourceInstances(Questionnaire q, Model.Questionnaire.ItemComponent qg, List<QuestionnaireResponse.ItemComponent> qrg, IResourceResolver source)
        {
            var fac = new DefaultModelFactory();
            Bundle result = new Bundle();
            result.Type = Bundle.BundleType.Batch;

            if (qg.Definition != null)
            {
                // this item has a definition, so we should process it
                var si = StructureItemTree.GetStructureTree(qg.Definition, q, source);
                if (si != null)
                {
                    foreach (var a in qrg)
                    {
                        Resource r = fac.Create(si.ClassMapping.NativeType) as Resource;
                        result.AddResourceEntry(r, null);
                        List<QuestionnaireResponse.GroupComponent> qrgItem = new List<QuestionnaireResponse.GroupComponent>();
                        qrgItem.Add(a);
                        PopulateResourceInstance(r, si, qrgItem);
                    }
                }
            }

            foreach (var qgi in qg.Group)
            {
                var items = CreateResourceInstances(q, qgi, qrg.Where(g => g.LinkId == qgi.LinkId).ToList(), source);
                result.Entry.AddRange(items.Entry);
            }

            return result;
        }

        public static T CreateResourceInstance<T>(StructureDefinition pracSd, StructureItem parent, Model.Questionnaire questionnaire, QuestionnaireResponse questionnaireResponse)
            where T : Resource, new()
        {
            System.Diagnostics.Debug.WriteLine("-----------------------------");
            T result = new T();
            List<QuestionnaireResponse.ItemComponent> groups = new List<QuestionnaireResponse.ItemComponent>();
            groups.Add(questionnaireResponse.Group);

            PopulateResourceInstance(result, parent, groups);

            return result;
        }

        internal static void PopulateResourceInstance(object instance, StructureItem parent, List<QuestionnaireResponse.ItemComponent> groups)
        {
            var fac = new DefaultModelFactory();

            // walk the structure definition (via the StructureItem)
            foreach (var item in parent.Children)
            {
                Debug.WriteLine($"{item.Path}{(item.ed.Fixed != null ? " fixed value" : "")}{(item.ed.Slicing != null ? " sliced: " + String.Join(", ", item.ed.Slicing.Discriminator) : "")}");

                if (item.ed.Slicing != null)
                {
                    Debug.WriteLine($"  Slice Name: {String.Join(", ", item.ed.Name)}");
                    Debug.WriteLine($"  Discriminator: {String.Join(", ", item.ed.Slicing.Discriminator)}");
                }

                if (item.ed.Fixed != null)
                {
                    Element fixedValue = item.ed.Fixed;
                    var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                    Type createElementType = pm.ReturnType;
                    if (pm.ReturnType == typeof(Element) && pm.Choice == ChoiceType.DatatypeChoice)
                    {
                        // check the element definition for the types
                        createElementType = ModelInfo.FhirTypeToCsType[item.ed.PrimaryTypeCode().GetLiteral()];
                    }
                    if (item.ed.Fixed.GetType() != createElementType)
                    {
                        // need to convert the data over
                        fixedValue = (Element)fac.Create(pm.ReturnType);
                        if (fixedValue is Primitive && item.ed.Fixed is Code)
                        {
                            (fixedValue as Primitive).ObjectValue = (item.ed.Fixed as Code).Value;
                        }
                    }
                    pm.SetValue(instance, fixedValue);
                    continue;
                }

                // Check the QR for this property
                if (item.Children.Count > 0)
                {
                    var filteredGroups = GetGroups(groups, item);
                    if (filteredGroups.Count > 0)
                    {
                        var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                        object value = pm.GetValue(instance);
                        if (value == null)
                        {
                            value = fac.Create(pm.ReturnType);
                            pm.SetValue(instance, value);
                        }
                        if (pm.ReturnType != pm.ElementType)
                        {
                            // this is a collection
                            IList list = value as IList;
                            foreach (var g in filteredGroups)
                            {
                                object elementValue = fac.Create(pm.ElementType);
                                List<QuestionnaireResponse.GroupComponent> g1 = new List<QuestionnaireResponse.GroupComponent>();
                                g1.Add(g);
                                PopulateResourceInstance(elementValue, item, g1);
                                list.Add(elementValue);
                            }
                        }
                        else
                        {
                            // backbone element style property, so let it flow in as normal
                            PopulateResourceInstance(value, item, filteredGroups);
                        }
                        continue;
                    }
                }
                // else
                {
                    // maybe there is an answer
                    var answers = GetAnswers(groups, item);
                    if (answers != null)
                    {
                        if (answers.Count > 0)
                        {
                            // Also need to handle repeating properties (array primitives)
                            var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                            object existingValue = pm.GetValue(instance);
                            if (answers.First().Value is Coding)
                            {
                                Coding codedValue = answers.First().Value as Coding;
                                Type createElementType = pm.ReturnType;
                                if (pm.ReturnType == typeof(Element) && pm.Choice == ChoiceType.DatatypeChoice)
                                {
                                    // check the element definition for the types
                                    createElementType = ModelInfo.FhirTypeToCsType[item.ed.PrimaryTypeCode().GetLiteral()];
                                }
                                Base prim = (Base)fac.Create(createElementType);
                                if (prim is Coding c)
                                {
                                    pm.SetValue(instance, codedValue);
                                }
                                else if (prim is CodeableConcept cc)
                                {
                                    cc.Coding.Add(codedValue);
                                    pm.SetValue(instance, cc);
                                }
                                else if (prim is Primitive p)
                                {
                                    // Still need to validate this content
                                    //if (EnumUtility.ParseLiteral(codedValue.Code, pm.ElementType) == null)
                                    //    throw Error.Format("Literal '{0}' is not a valid value for enumeration '{1}'".FormatWith(codedValue.Code, pm.ElementType.Name));
                                    p.ObjectValue = codedValue.Code;
                                    pm.SetValue(instance, p);
                                }
                                else
                                {
                                    // This is where we may consider that the coding needs to go into
                                    pm.SetValue(instance, answers.First().Value);
                                }
                            }
                            else
                            {
                                // Check for type conversion if needed
                                if (pm.ElementType == typeof(Code) && answers.First().Value is FhirString valueString)
                                    pm.SetValue(instance, new Code(valueString.Value));
                                else if (pm.ElementType == typeof(Id) && answers.First().Value is FhirString)
                                    pm.SetValue(instance, new Id(((FhirString)answers.First().Value).Value));
                                else if (pm.IsCollection)
                                {
                                    IList col = pm.GetValue(instance) as IList;
                                    if (col == null)
                                        col = fac.Create(pm.ReturnType) as IList;
                                    foreach (var itemValue in answers.Select(v => v.Value))
                                    {
                                        col.Add(itemValue);
                                    }
                                    pm.SetValue(instance, col);
                                }
                                else
                                    pm.SetValue(instance, answers.First().Value);
                            }
                        }
                        else
                        {
                            // This item is on the way to one that DOES have a value, so create it and continue
                            var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                            Type createElementType = pm.ReturnType;
                            if (pm.ReturnType == typeof(Element) && pm.Choice == ChoiceType.DatatypeChoice)
                            {
                                // check the element definition for the types
                                createElementType = ModelInfo.FhirTypeToCsType[item.ed.PrimaryTypeCode().GetLiteral()];
                            }
                            object value = pm.GetValue(instance);
                            if (value == null)
                            {
                                value = fac.Create(createElementType);
                                pm.SetValue(instance, value);
                            }
                            if (pm.IsCollection)
                            {
                                IList list = value as IList;
                                foreach (var g in groups)
                                {
                                    var arrayItem = fac.Create(pm.ElementType);
                                    if (arrayItem is Extension e && item.ExtensionUrl != null)
                                    {
                                        e.Url = item.ExtensionUrl;
                                    }
                                    List<QuestionnaireResponse.GroupComponent> g1 = new List<QuestionnaireResponse.GroupComponent>();
                                    g1.Add(g);
                                    PopulateResourceInstance(arrayItem, item, g1);
                                    list.Add(arrayItem);
                                }
                            }
                            else
                            {
                                // backbone element style property, so let it flow in as normal
                                PopulateResourceInstance(value, item, groups);
                            }
                        }
                    }
                }
            }
        }

        // this is used for repeating elements
        static private List<QuestionnaireResponse.ItemComponent> GetGroups(List<QuestionnaireResponse.ItemComponent> groups, StructureItem si)
        {
            List<QuestionnaireResponse.ItemComponent> result = new List<QuestionnaireResponse.ItemComponent>();
            foreach (var g in groups)
            {
                if (g.LinkId == si.Path)
                    result.Add(g);
                else
                {
                    foreach (var gn in g.Item.SelectMany(s => s.Answer.Select(a => a.Item)))
                    {
                        if (gn.Count > 0)
                            result.AddRange(GetGroups(gn, si));
                    }
                    if (g.Item.Count > 0)
                        result.AddRange(GetGroups(g.Item, si));
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve the list of answers relevant to the specified item ()
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="si"></param>
        /// <returns>
        /// returns null if there are no answers anywhere down, 
        /// if the immediate answers are here, returns the items, 
        /// otherwise returns an empty list
        /// </returns>
        static private List<QuestionnaireResponse.AnswerComponent> GetAnswers(List<QuestionnaireResponse.ItemComponent> gp, StructureItem si)
        {
            List<QuestionnaireResponse.AnswerComponent> result = null;
            foreach (var group in gp)
            {
                if (group.Group.Count > 0)
                {
                    var descendantAnswers = GetAnswers(group.Group, si);
                    if (descendantAnswers != null)
                    {
                        if (result == null)
                            result = new List<QuestionnaireResponse.AnswerComponent>();
                        // There is at least 1 answer somewhere in down the line
                        // (even if the collection is empty, which is different to a null result)
                        result.AddRange(descendantAnswers);
                    }
                }

                foreach (var q in group.Question)
                {
                    // Check if THIS is a question on the way to the item
                    // (need to check if the path approach will always work once we get into the extensions)
                    if (q.LinkId == si.Path)
                    {
                        if (result == null)
                            result = new List<QuestionnaireResponse.AnswerComponent>();
                        result.AddRange(q.Answer);
                    }
                    if (q.LinkId.StartsWith(si.Path) && q.LinkId != si.Path && result == null)
                        result = new List<QuestionnaireResponse.AnswerComponent>();
                }
            }
            return result;
        }
    }
}
