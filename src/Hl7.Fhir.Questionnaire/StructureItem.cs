using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Hl7.FhirPath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hl7.Fhir.QuestionnaireServices
{
    [System.Diagnostics.DebuggerDisplay(@"Path = {Path}")] // http://blogs.msdn.com/b/jaredpar/archive/2011/03/18/debuggerdisplay-attribute-best-practices.aspx

    public class StructureItem
    {
        public StructureItem ShallowClone()
        {
            StructureItem item = new StructureItem()
            {
                ClassMapping = this.ClassMapping,
                code = this.code,
                ed = this.ed,
                FhirpathExpression = this.FhirpathExpression,
                id = this.id,
                IsArray = this.IsArray,
                Path = this.Path,
                ValidationRules = this.ValidationRules,
                ExtensionUrl = this.ExtensionUrl,
                LinkId = this.LinkId
            };
            return item;
        }

        public string id { get; set; }

        /// <summary>
        /// The value that is used for matching into the Questionnaire.(Group|Question).LinkId
        /// </summary>
        public string code { get; set; }

        public bool IsArray { get; set; }

        /// <summary>
        /// The URL for the extension (where this property represents an extension)
        /// </summary>
        public string ExtensionUrl { get; set; }

        /// <summary>
        /// This does not come from the StructureDefinition, but is populated from 
        /// the Questionnaire when it is mapped to speed the efficiency of processing
        /// the content into the resource (permits iteration over the resource, not the questionnaire)
        /// </summary>
        public string LinkId { get; set; }

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
