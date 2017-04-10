using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Hl7.Fhir.Validation;
using Hl7.FhirPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.Specification.Validation
{
    public class QuickValidator
    {
        public OperationOutcome Validate(Base context, StructureDefinition sd)
        {
            ValidationItem parent = CreateValidationTree(sd);
            return parent.Validate(new PocoNavigator(context), new PocoNavigator(context));
        }

        /// <summary>
        /// http://hl7.org/fhir/elementdefinition.html#ElementDefinition
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public ValidationItem CreateValidationTree(StructureDefinition sd)
        {
            ValidationItem parent = new ValidationItem();

            // Just run through the snapshot to create all the rules
            // (yes it depends on the snapshot being complete - Thanks Michel)
            Stack<ValidationItem> processingItem = new Stack<ValidationItem>();
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
                if (!string.IsNullOrEmpty(elem.SliceName))
                {
                    discriminator = new Dictionary<string, string>();
                    if (elem.Slicing != null)
                    {
                        foreach (var disc in elem.Slicing.Discriminator)
                        {
                            discriminator.Add(disc.Path, null);
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
                        var item = new ValidationItem() { Path = elem.Path, FhirpathExpression = elem.Path.Replace(processingItem.Peek().Path + ".", "") };
                        processingItem.Peek().Children.Add(item);
                        item.ed = elem;
                        item.ValidationRules.Add(elem);
                    }
                    else
                    {
                        // this is a child element
                        var item = new ValidationItem() { Path = elem.Path, FhirpathExpression = elem.Path.Replace(processingItem.Peek().Path + ".", "") };
                        processingItem.Peek().Children.Add(item);
                        item.ed = elem;
                        item.ValidationRules.Add(elem);
                    }
                }
            }
            return parent;
        }
    }

    /// <summary>
    /// This is a subset of the ElementDefinition class with only enough
    /// information for the validation system to work with
    /// (and that we can quickly serialize to a binary form)
    /// </summary>
    public class MinimalElementDefinition
    {
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

    public class ValidationItem
    {
        /// <summary>
        /// Path is only here for the testing
        /// </summary>
        public string Path { get; set; }
        public String FhirpathExpression { get; set; }

        public ElementDefinition ed { get; set; }

        public List<ValidationItem> Children { get; set; } = new List<ValidationItem>();

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
                        result.Include(childValidations.Validate(item, ContainerContext));
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
                if (!string.IsNullOrEmpty(inv.Expression))
                {
                    // test this invariant rule
                    foreach (var item in values)
                    {
                        if (!item.Predicate(inv.Expression, ParentContext))
                        {
                            result.AddIssue(new OperationOutcome.IssueComponent()
                            {
                                Details = new CodeableConcept(null, inv.Key, inv.Human, inv.Human),
                                Severity = OperationOutcome.IssueSeverity.Error,
                                Diagnostics = inv.Expression,
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
