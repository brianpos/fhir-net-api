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
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Snapshot;

namespace Hl7.Fhir.Specification
{
    public class QuestionnaireProcessing
    {
        // Selection of the elements in the slice (create a fhirpath expression with a where to find all the children)
        // these can be removed from the set, leaving all the other non sliced content

        public OperationOutcome Validate(Base context, StructureDefinition sd)
        {
            StructureItem parent = CreateStructureTree(sd, null);
            return parent.Validate(new PocoNavigator(context), new PocoNavigator(context));
        }

        internal void CreateStructureChildren(StructureItem parent, ElementDefinitionNavigator nav, IResourceResolver source, string replaceRoot = null)
        {
            do
            {
                // if this is the extension property, and it has't been sliced, then we will skip them
                // as we don't want to permit processing items that aren't defined in the structure definition
                if (!nav.Current.IsExtension() || nav.Current.IsMappedExtension())
                {
                    // Process this child item
                    var item = new StructureItem() { id = nav.Current.ElementId, code = nav.Current.Code?.FirstOrDefault()?.Code, Path = nav.Path, FhirpathExpression = nav.PathName, ed = nav.Current };
                    if (replaceRoot != null)
                        item.Path = replaceRoot + item.Path.Substring(item.Path.IndexOf("."));
                    if (nav.Current.IsMappedExtension())
                    {
                        item.ExtensionUrl = nav.Current.PrimaryTypeProfile();
                        item.Path += ":" + nav.Current.Name;
                    }

                    if (nav.Current.PrimaryTypeCode() == FHIRDefinedType.Narrative)
                    {
                        // don't support setting any part of the narrative using this technique
                        continue;
                    }
                    if (nav.Current.Max == "0")
                    {
                        System.Diagnostics.Debug.WriteLine($"skipping {item.Path} ({nav.PathName}) {item.ExtensionUrl}");
                        continue;
                    }
                    if (item.ed.Name != null)
                    {
                        Debug.WriteLine($"  Name: {item.ed.Name}");
                        if (!nav.Current.IsMappedExtension()) // as this would already be there
                            item.Path += ":" + nav.Current.Name; // and add this into the property names
                    }
                    System.Diagnostics.Debug.WriteLine($"{item.Path} ({nav.PathName}) {item.ExtensionUrl} [{nav.Current.PrimaryTypeCode()}]{(nav.Current.Fixed != null ? " fixed value" : "")}");

                    if (item.ed.Slicing != null)
                    {
                        Debug.WriteLine($"  Discriminator: {String.Join(", ", item.ed.Slicing.Discriminator)}");
                    }

                    parent.Children.Add(item);
                    if (nav.HasChildren)
                    {
                        // retrieve the type of this property
                        var pm = parent.ClassMapping.FindMappedElementByName(nav.PathName);
                        if (pm != null)
                        {
                            // Check for the available type(s)
                            if (pm.Choice == ChoiceType.DatatypeChoice)
                            {
                                Type t = ModelInfo.FhirTypeToCsType[nav.Current.PrimaryTypeCode().GetLiteral()];
                                item.ClassMapping = ClassMapping.Create(t);
                            }
                            else if (pm.Choice == ChoiceType.ResourceChoice)
                            {
                                if (pm.ElementType == typeof(Resource))
                                {
                                    // This is not a constrained set of choices
                                    continue;
                                }
                            }
                            else
                            {
                                // Note that ReturnType would have the type of the collection
                                // where the ElementType is the type of the item in the collection
                                // or where not a collection, both are the same value
                                item.ClassMapping = ClassMapping.Create(pm.ElementType);
                            }
                        }

                        // Now process all the children
                        var st = nav.CloneSubtree();
                        st.MoveToFirstChild();
                        CreateStructureChildren(item, st, source, replaceRoot);
                    }
                    else
                    {
                        // this is likely a DataType - so check it for children too
                        if (nav.Current.PrimaryTypeCode().HasValue
                            && nav.Current.PrimaryTypeCode() != FHIRDefinedType.Resource
                            && nav.Current.PrimaryTypeCode() != FHIRDefinedType.Narrative)
                        {
                            // Need to actually get the Profile and expand that instead of just the base type
                            string profile = nav.Current.PrimaryTypeProfile(); // care needs to be applied from STU3 and the targetProfile
                            if (!string.IsNullOrEmpty(profile) && nav.Current.PrimaryTypeCode() != FHIRDefinedType.Reference && nav.Current.PrimaryTypeCode() != FHIRDefinedType.Uri)
                            {
                                StructureDefinition sdDataType = source.FindStructureDefinition(profile);
                                StructureItem dataType = CreateStructureTree(sdDataType, source, item.Path);
                                item.Children.AddRange(dataType.Children);
                                item.ClassMapping = dataType.ClassMapping;
                            }
                            else
                            {
                                StructureDefinition sdDataType = source.GetStructureDefinitionForTypeCode(new Code<FHIRDefinedType>(nav.Current.PrimaryTypeCode()));
                                StructureItem dataType = CreateStructureTree(sdDataType, source, item.Path);
                                item.Children.AddRange(dataType.Children);
                                item.ClassMapping = dataType.ClassMapping;
                            }
                        }
                    }
                }
            }
            while (nav.MoveToNext());
        }

        /// <summary>
        /// http://hl7.org/fhir/elementdefinition.html#ElementDefinition
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public StructureItem CreateStructureTree(StructureDefinition sd, IResourceResolver source, string replaceRoot = null)
        {
            StructureItem parent = new StructureItem();
            Type t = ModelInfo.GetTypeForFhirType(sd.ConstrainedType.HasValue ? sd.ConstrainedType.GetLiteral() : sd.Name);
            parent.ClassMapping = ClassMapping.Create(t);

            // Build the snapshot if it doesn't already exist
            if (!sd.HasSnapshot)
            {
                SnapshotGenerator sg = new SnapshotGenerator(source);
                sg.Update(sd);
            }

            // position the Navigator on the first element in the parent's collection of elements
            var nav = ElementDefinitionNavigator.ForSnapshot(sd);
            nav.MoveToFirstChild(); // move to root
            nav.MoveToFirstChild(); // move to first child
            CreateStructureChildren(parent, nav, source, replaceRoot);

            return parent;
        }

        /// <summary>
        /// Prune the StructureTree based on a questionnaire definition.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="questionnaire"></param>
        /// <remarks>
        /// This will remove all nodes in the tree that don't either contribute to 
        /// fixed values, or potential answers in the questionnaire
        /// </remarks>
        /// <returns></returns>
        public StructureItem PruneTree(StructureItem si, Questionnaire questionnaire)
        {
            StructureItem item = new StructureItem()
            {
                ClassMapping = si.ClassMapping,
                code = si.code,
                ed = si.ed,
                FhirpathExpression = si.FhirpathExpression,
                id = si.id,
                Path = si.Path,
                ValidationRules = si.ValidationRules
            };
            foreach (var child in si.Children)
            {
                var newChild = PruneTree(child, questionnaire.Group);
                if (newChild != null)
                    item.Children.Add(newChild);
            }
            return item;
        }

        private StructureItem PruneTree(StructureItem si, Questionnaire.GroupComponent group)
        {
            if (HasFixedValueInChild(si))
            {
                StructureItem item = new StructureItem()
                {
                    ClassMapping = si.ClassMapping,
                    code = si.code,
                    ed = si.ed,
                    FhirpathExpression = si.FhirpathExpression,
                    id = si.id,
                    Path = si.Path,
                    ValidationRules = si.ValidationRules
                };
                foreach (var child in si.Children)
                {
                    var newChild = PruneTree(child, group);
                    if (newChild != null)
                        item.Children.Add(newChild);
                }
                return item;
            }

            // check to see if this item is used the questionnaire
            return null;
        }

        private bool HasFixedValueInChild(StructureItem si)
        {
            if (si.ed.Fixed != null)
                return true;
            foreach (var item in si.Children)
            {
                if (HasFixedValueInChild(item))
                    return true;
            }
            return false;
        }

        public T CreateResourceInstance<T>(StructureDefinition pracSd, StructureItem parent, Questionnaire questionnaire, QuestionnaireResponse questionnaireResponse)
            where T : Resource, new()
        {
            System.Diagnostics.Debug.WriteLine("-----------------------------");
            T result = new T();
            List<QuestionnaireResponse.GroupComponent> groups = new List<QuestionnaireResponse.GroupComponent>();
            groups.Add(questionnaireResponse.Group);

            PopulateResourceInstance(result, parent, groups);

            return result;
        }

        internal void PopulateResourceInstance(object instance, StructureItem parent, List<QuestionnaireResponse.GroupComponent> groups)
        {
            var fac = new DefaultModelFactory();

            // walk the structure definition (via the StructureItem)
            foreach (var item in parent.Children)
            {
                Debug.WriteLine($"{item.Path}{(item.ed.Fixed != null ? " fixed value" : "")}{(item.ed.Slicing != null ? " sliced:" + String.Join(", ", item.ed.Slicing.Discriminator) : "")}");

                if (item.ed.Slicing != null)
                {
                    Debug.WriteLine($"  Slice Name: {String.Join(", ", item.ed.Name)}");
                    Debug.WriteLine($"  Discriminator{String.Join(", ", item.ed.Slicing.Discriminator)}");
                }

                if (item.ed.Fixed != null)
                {
                    var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                    pm.SetValue(instance, item.ed.Fixed);
                    continue;
                }

                // Check the QR for this property
                if (item.Children.Count > 0)
                {
                    var filteredGroups = GetGroups(groups, item);
                    if (filteredGroups.Count > 0)
                    {
                        var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                        object value = fac.Create(pm.ReturnType);
                        pm.SetValue(instance, value);
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
                            if (answers.First().Value is Coding)
                            {
                                Coding codedValue = answers.First().Value as Coding;
                                Base prim = (Base)fac.Create(pm.ReturnType);
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
                                    pm.SetValue(instance, new Code(((FhirString)answers.First().Value).Value));
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
                            object value = fac.Create(createElementType);
                            pm.SetValue(instance, value);
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
        private List<QuestionnaireResponse.GroupComponent> GetGroups(List<QuestionnaireResponse.GroupComponent> groups, StructureItem si)
        {
            List<QuestionnaireResponse.GroupComponent> result = new List<QuestionnaireResponse.GroupComponent>();
            foreach (var g in groups)
            {
                if (g.LinkId == si.Path)
                    result.Add(g);
                else
                {
                    foreach (var gn in g.Question.SelectMany(s => s.Answer.Select(a => a.Group)))
                    {
                        if (gn.Count > 0)
                            result.AddRange(GetGroups(gn, si));
                    }
                    if (g.Group.Count > 0)
                        result.AddRange(GetGroups(g.Group, si));
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
        private List<QuestionnaireResponse.AnswerComponent> GetAnswers(List<QuestionnaireResponse.GroupComponent> gp, StructureItem si)
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
                    if (q.LinkId.StartsWith(si.Path) && result == null)
                        result = new List<QuestionnaireResponse.AnswerComponent>();
                }
            }
            return result;
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

    [System.Diagnostics.DebuggerDisplay(@"Path = {Path}")] // http://blogs.msdn.com/b/jaredpar/archive/2011/03/18/debuggerdisplay-attribute-best-practices.aspx

    public class StructureItem
    {
        public string id { get; set; }

        /// <summary>
        /// The value that is used for matching into the Questionnaire.(Group|Question).LinkId
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// The URL for the extension (where this property represents an extension)
        /// </summary>
        public string ExtensionUrl { get; set; }

        public ClassMapping ClassMapping { get; set; }

        /// <summary>
        /// Path is only here for the testing
        /// </summary>
        public string Path { get; set; }

        public String FhirpathExpression { get; set; }

        public ElementDefinition ed { get; set; }

        public List<StructureItem> Children { get; set; } = new List<StructureItem>();

        public List<ElementDefinition> ValidationRules { get; set; } = new List<ElementDefinition>();

        public OperationOutcome Validate(IElementNavigator ParentContext, IElementNavigator ContainerContext)
        {
            OperationOutcome result = new OperationOutcome();
            IEnumerable<IElementNavigator> values;
            if (!string.IsNullOrEmpty(FhirpathExpression))
                values = new Hl7.FhirPath.FhirPathCompiler().Compile(FhirpathExpression).Invoke(ParentContext, ContainerContext);
            else
                values = new List<IElementNavigator>(new[] { ParentContext });
            if (ValidationRules != null)
            {
                foreach (var rule in ValidationRules)
                {
                    ValidateRule(rule, ParentContext, values, result);
                }
            }
            // Now validate the children
            if (Children != null)
            {
                foreach (var item in values)
                {
                    foreach (var childValidations in Children)
                    {
                        result.Issue.AddRange(childValidations.Validate(item, ContainerContext).Issue);
                    }
                }
            }
            return result;
        }

        static private void ValidateRule(ElementDefinition rule, IElementNavigator ParentContext, IEnumerable<IElementNavigator> values, OperationOutcome result)
        {
            // Does not need to look at the slicing section as that is covered via the context tree and fhirpath expression

            // Check that this rule is satisfied by all the values provided
            if (rule?.Min > 0)
            {
                if (!values.Any())
                {
                    // this is an error (no need to check how many there are without performance of counting them each time)
                    result.AddIssue("Need to have one of these! " + rule.Path,
                        new Issue() { Code = 12, Severity = OperationOutcome.IssueSeverity.Error, Type = OperationOutcome.IssueType.Value },
                        ParentContext);
                }
                else if (values.Count() < rule.Min)
                {
                    result.AddIssue("Need to have at least " + rule.Min.ToString() + " of these! " + rule.Path,
                    new Issue() { Code = 12, Severity = OperationOutcome.IssueSeverity.Error, Type = OperationOutcome.IssueType.Value },
                    ParentContext);
                }
            }
            if (rule?.Max == "0")
            {
                if (values.Any())
                {
                    // this is an error (no need to check how many there are without performance of counting them each time)
                    result.AddIssue("Should not contain one of these!" + rule.Path,
                        new Issue() { Code = 12, Severity = OperationOutcome.IssueSeverity.Error, Type = OperationOutcome.IssueType.Value },
                        ParentContext);
                }
            }
            else if (!string.IsNullOrEmpty(rule?.Max))
            {
                int max = 0;
                if (int.TryParse(rule.Max, out max))
                {
                    // this doesn't need to count all of them, just know that there is at least 1
                    if (values.Skip(max).Any())
                    {
                        // this is an error (no need to check how many there are without performance of counting them each time)
                        result.AddIssue("Should only have " + rule.Max + " one of these!" + rule.Path,
                            new Issue() { Code = 12, Severity = OperationOutcome.IssueSeverity.Error, Type = OperationOutcome.IssueType.Value },
                            ParentContext);
                    }
                }
            }
            if (rule.Fixed != null)
            {
                foreach (PocoNavigator item in values)
                {
                    if (!item.FhirValue.IsExactly(rule.Fixed))
                    {
                        result.AddIssue("Should be a fixed value " + rule.Fixed.ToString() + " !" + rule.Path,
                            Issue.CONTENT_DOES_NOT_MATCH_FIXED_VALUE,
                            ParentContext);
                    }
                }
            }
            // ...

            // Validate the invariants defined here
            foreach (var inv in rule.Constraint)
            {
                string expression = inv.Expression;
                if (!string.IsNullOrEmpty(expression))
                {
                    // test this invariant rule
                    foreach (var item in values)
                    {
                        if (!item.Predicate(expression, ParentContext))
                        {
                            result.AddIssue(new OperationOutcome.IssueComponent()
                            {
                                Details = new CodeableConcept(null, inv.Key, inv.Human, inv.Human),
                                Severity = OperationOutcome.IssueSeverity.Error,
                                Diagnostics = expression,
                                Code = OperationOutcome.IssueType.Invariant,
                                Location = new[] { (item as PocoNavigator).ShortPath }
                            });
                        }
                    }
                }
            }
        }
    }
}
