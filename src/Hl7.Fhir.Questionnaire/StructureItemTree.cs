using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;
using System;
using System.Diagnostics;
using System.Linq;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Specification.Navigation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Snapshot;
using System.Collections.Generic;

namespace Hl7.Fhir.QuestionnaireServices
{
    public class StructureItemTree
    {
        static Dictionary<string, StructureItem> _cache = new Dictionary<string, StructureItem>();
        static public void FlushCache()
        {
            _cache.Clear();
        }

        public static StructureItem GetStructureTree(string StructureDefinitionUrl, string QuestionnaireUrl, IResourceResolver source)
        {
            string key = QuestionnaireUrl + "###" + StructureDefinitionUrl;
            if (_cache.ContainsKey(key))
                return _cache[key];
            StructureDefinition sd = source.FindStructureDefinition(StructureDefinitionUrl);
            if (sd != null)
            {
                var si = StructureItemTree.CreateStructureTree(sd, source, null, true);
                Type t = ModelInfo.FhirTypeToCsType[sd.ConstrainedType.GetLiteral()];
                si.ClassMapping = ClassMapping.Create(t);

                if (!_cache.ContainsKey(key))
                    _cache.Add(key, si);
                return si;
            }
            return null;
        }

        /// <summary>
        /// http://hl7.org/fhir/elementdefinition.html#ElementDefinition
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static StructureItem CreateStructureTree(StructureDefinition sd, IResourceResolver source, string replaceRoot = null, bool skipCache = false)
        {
            if (_cache.ContainsKey(sd.Url) && !skipCache && replaceRoot == null)
                return _cache[sd.Url];

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

            if (!_cache.ContainsKey(sd.Url) && replaceRoot == null)
                _cache.Add(sd.Url, parent);
            return parent;
        }

        private static void CreateStructureChildren(StructureItem parent, ElementDefinitionNavigator nav, IResourceResolver source, string replaceRoot = null)
        {
            do
            {
                // if this is the extension property, and it hasn't been sliced, then we will skip them
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
                        Bookmark bm = nav.Bookmark();
                        nav.MoveToFirstChild();
                        CreateStructureChildren(item, nav, source, replaceRoot);
                        nav.ReturnToBookmark(bm);
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
                                StructureDefinition sdDataType = source.FindStructureDefinitionForCoreType(nav.Current.PrimaryTypeCode().Value);
                                // do not look at the cache, as the item path could be different
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
        /// Prune the StructureTree based on a questionnaire definition.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="questionnaire"></param>
        /// <remarks>
        /// This will remove all nodes in the tree that don't either contribute to 
        /// fixed values, or potential answers in the questionnaire
        /// </remarks>
        /// <returns></returns>
        public StructureItem PruneTree(StructureItem si, Model.Questionnaire questionnaire)
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

        private StructureItem PruneTree(StructureItem si, Model.Questionnaire.GroupComponent group)
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

    }
}
