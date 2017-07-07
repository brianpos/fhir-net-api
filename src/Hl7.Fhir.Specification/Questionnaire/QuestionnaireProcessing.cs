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

namespace Hl7.Fhir.Specification
{
    public class QuestionnaireProcessing
    {
        // Selection of the elements in the slice (create a fhirpath expression with a where to find all the children)
        // these can be removed from the set, leaving all the other non sliced content

        public OperationOutcome Validate(Base context, StructureDefinition sd)
        {
            StructureItem parent = CreateStructureTree(sd);
            return parent.Validate(new PocoNavigator(context), new PocoNavigator(context));
        }

        /// <summary>
        /// http://hl7.org/fhir/elementdefinition.html#ElementDefinition
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public StructureItem CreateStructureTree(StructureDefinition sd)
        {
            StructureItem parent = new StructureItem();
            Type t = ModelInfo.GetTypeForFhirType(sd.ConstrainedType.HasValue ? sd.ConstrainedType.ToString() : sd.Name);
            parent.ClassMapping = ClassMapping.Create(t);

            // Just run through the snapshot to create all the rules
            // (yes it depends on the snapshot being complete - Thanks Michel)
            Stack<StructureItem> processingItem = new Stack<StructureItem>();
            processingItem.Push(parent);
            Dictionary<string, string> discriminator = new Dictionary<string, string>();
            foreach (var elem in sd.Snapshot.Element)
            {
                // if this is slicing, create the slicing context
                Console.WriteLine($"Path: {elem.Path}");

                if (processingItem.Peek().Path == null)
                {
                    // this is the root item
                    processingItem.Peek().Path = elem.Path;
                    processingItem.Peek().ed = elem;
                    processingItem.Peek().ValidationRules.Add(elem);
                    continue;
                }

                // Handle a slicing introduction element
                if (!string.IsNullOrEmpty(elem.Name))
                {
                    discriminator = new Dictionary<string, string>();
                    if (elem.Slicing != null)
                    {
                        foreach (var disc in elem.Slicing.Discriminator)
                        {
                            discriminator.Add(disc, null);
                        }
                    }
                    //  continue;
                }

                while (processingItem.Count > 0 && !elem.Path.Contains(processingItem.Peek().Path + "."))
                {
                    // this is a new item, so pop 
                    processingItem.Pop();
                }
                // there is nothing to process (or SD is corrupt)
                if (processingItem.Count == 0)
                    break;

                string parentPath = processingItem.Peek().Path;
                if (elem.Path.Contains(parentPath + "."))
                {
                    string thisPath = elem.Path.Replace(parentPath + ".", "");
                    if (thisPath.Contains("."))
                    {
                        // this is a new child to the previous item
                        var newParent = processingItem.Peek().Children.Last();
                        processingItem.Push(newParent);
                        var item = new StructureItem() { id = elem.ElementId, code = elem.Code?.FirstOrDefault()?.Code, Path = elem.Path, FhirpathExpression = elem.Path.Replace(processingItem.Peek().Path + ".", "") };
                        processingItem.Peek().Children.Add(item);
                        item.ed = elem;
                        item.ValidationRules.Add(elem);
                    }
                    else
                    {
                        // this is a child element
                        var item = new StructureItem() { id = elem.ElementId, code = elem.Code?.FirstOrDefault()?.Code, Path = elem.Path, FhirpathExpression = elem.Path.Replace(processingItem.Peek().Path + ".", "") };
                        processingItem.Peek().Children.Add(item);
                        item.ed = elem;
                        item.ValidationRules.Add(elem);
                    }
                }
            }
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
            T result = new T();
            List<QuestionnaireResponse.GroupComponent> groups = new List<QuestionnaireResponse.GroupComponent>();
            groups.Add(questionnaireResponse.Group);

            var edn = ElementDefinitionNavigator.ForSnapshot(pracSd);
            edn.MoveToFirstChild();
            PopulateResourceInstance(result, parent, groups);

            return result;
        }

        internal void PopulateResourceInstance(object instance, StructureItem parent, List<QuestionnaireResponse.GroupComponent> groups)
        {
            // walk the structure definition (via the StructureItem)
            foreach (var item in parent.Children)
            {
                Debug.WriteLine($"{item.FhirpathExpression} children: {item.Children.Count}");

                // Check the QR for this property
                if (item.Children.Count > 0)
                {
                    var filteredGroups = GetGroups(groups, item);
                    var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                    var fac = new DefaultModelFactory();
                    object value = fac.Create(pm.ReturnType);
                    PopulateResourceInstance(value, item, filteredGroups);
                    pm.SetValue(instance, value);
                }
                else
                {
                    // maybe there is an answer
                    var answers = GetAnswers(groups, item);
                    if (answers.Count > 0)
                    {
                        // Also need to handle repeating properties (array primitives)
                        var pm = parent.ClassMapping.FindMappedElementByName(item.FhirpathExpression);
                        if (answers.First().Value is Coding)
                        {
                            Coding codedValue = answers.First().Value as Coding;
                            if (pm.RepresentsValueElement && pm.ElementType.IsEnum())
                            {
                                Primitive prim = (Primitive)pm.GetValue(instance);
                                if (EnumUtility.ParseLiteral(codedValue.Code, pm.ElementType) == null)
                                    throw Error.Format("Literal '{0}' is not a valid value for enumeration '{1}'".FormatWith(codedValue.Code, pm.ElementType.Name));
                                prim.ObjectValue = codedValue.Code;
                            }
                            else
                            {
                                pm.SetValue(instance, answers.First().Value);
                            }
                        }
                        else
                        {
                            pm.SetValue(instance, answers.First().Value);
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

        private List<QuestionnaireResponse.AnswerComponent> GetAnswers(List<QuestionnaireResponse.GroupComponent> gp, StructureItem si)
        {
            List<QuestionnaireResponse.AnswerComponent> result = new List<QuestionnaireResponse.AnswerComponent>();
            foreach (var group in gp)
            {
                if (group.Group.Count > 0)
                {
                    result.AddRange(GetAnswers(group.Group, si));
                }
                foreach (var q in group.Question)
                {
                    if (q.LinkId == si.Path)
                        result.AddRange(q.Answer);
                }
            }
            // support for nesting is still needed here (this only covers 1 level of depth)
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

    public class StructureItem
    {
        public string id { get; set; }

        /// <summary>
        /// The value that is used for matching into the Questionnaire.(Group|Question).LinkId
        /// </summary>
        public string code { get; set; }

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
