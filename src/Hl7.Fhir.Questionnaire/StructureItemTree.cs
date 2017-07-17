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

        public static StructureItem GetStructureTree(string StructureDefinitionUrl, Questionnaire q, IResourceResolver source, bool pruneTree = true)
        {
            string key = q.Id + "###" + StructureDefinitionUrl;
            if (_cache.ContainsKey(key) && pruneTree)
                return _cache[key];
            StructureDefinition sd = source.FindStructureDefinition(StructureDefinitionUrl);
            if (sd != null)
            {
                var si = StructureItemTree.CreateStructureTree(sd, source, null, true);
                Type t = ModelInfo.FhirTypeToCsType[sd.ConstrainedType.GetLiteral()];
                si.ClassMapping = ClassMapping.Create(t);

                // Populate all the bindings
                Dictionary<string, string> mapPathsToLinkIds = new Dictionary<string, string>();
                BuildMapping(mapPathsToLinkIds, q.Group);
                PopulateBindings(si, mapPathsToLinkIds);

                // Now that we have the content, we can prune the content down too
                if (pruneTree)
                    PruneTree(si, mapPathsToLinkIds);

                // Add it into the cache for later usage
                if (!_cache.ContainsKey(key) && pruneTree)
                    _cache.Add(key, si);
                return si;
            }
            return null;
        }

        public static IEnumerable<Base> GetValues(StructureItem item, Base data, string path, string linkId)
        {
            List<Base> results = new List<Base>();
            if (item.Path == path)
            {
                results.Add(data);
                return results;
            }
            foreach (var child in item.Children)
            {
                if (ContainsPath(child, path))
                {
                    var pm = item.ClassMapping.FindMappedElementByName(child.FhirpathExpression);
                    if (pm.ReturnType != pm.ElementType)
                    {
                        // this is a collection
                        IEnumerable<Base> result = (IEnumerable<Base>)pm.GetValue(data);
                        foreach (var itemInCol in result)
                        {
                            var moreData = GetValues(child, itemInCol, path, linkId);
                            results.AddRange(moreData);
                        }
                    }
                    else
                    {
                        Base result = (Base)pm.GetValue(data);
                        var moreData = GetValues(child, result, path, linkId);
                        results.AddRange(moreData);
                    }
                    break;
                }
            }
            return results;
        }

        public static bool ContainsPath(StructureItem item, string path)
        {
            if (item.Path == path)
                return true;
            foreach (var child in item.Children)
            {
                if (ContainsPath(child, path))
                    return true;
            }
            return false;
        }

        private static void PopulateBindings(StructureItem item, Dictionary<string, string> mapPathsToLinkIds)
        {
            if (mapPathsToLinkIds.ContainsKey(item.Path))
                item.LinkId = mapPathsToLinkIds[item.Path];
            foreach (var child in item.Children)
            {
                PopulateBindings(child, mapPathsToLinkIds);
            }
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
            parent.Path = nav.Current.Path;

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
                        // System.Diagnostics.Debug.WriteLine($"skipping {item.Path} ({nav.PathName}) {item.ExtensionUrl}");
                        continue;
                    }
                    if (item.ed.Name != null)
                    {
                        // Debug.WriteLine($"  Name: {item.ed.Name}");
                        if (!nav.Current.IsMappedExtension()) // as this would already be there
                            item.Path += ":" + nav.Current.Name; // and add this into the property names
                    }
                    // System.Diagnostics.Debug.WriteLine($"{item.Path} ({nav.PathName}) {item.ExtensionUrl} [{nav.Current.PrimaryTypeCode()}]{(nav.Current.Fixed != null ? " fixed value" : "")}");

                    if (item.ed.Slicing != null)
                    {
                        Debug.WriteLine($"  Discriminator: {String.Join(", ", item.ed.Slicing.Discriminator)}");
                    }

                    // retrieve the type of this property
                    var pm = parent.ClassMapping.FindMappedElementByName(nav.PathName);
                    if (pm != null)
                    {
                        // skip over the raw values
                        if (pm.ReturnType == pm.ElementType && !pm.ElementType.CanBeTreatedAsType(typeof(Base)))
                            continue;
                    }

                    parent.Children.Add(item);
                    if (nav.HasChildren)
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

                        if (pm.ReturnType != pm.ElementType)
                            item.IsArray = true;

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
                                StructureItem dataType = CreateStructureTree(sdDataType, source, item.Path, true);
                                item.Children.AddRange(dataType.Children);
                                item.ClassMapping = dataType.ClassMapping;
                            }
                            else
                            {
                                StructureDefinition sdDataType = source.FindStructureDefinitionForCoreType(nav.Current.PrimaryTypeCode().Value);
                                // do not look at the cache, as the item path could be different
                                StructureItem dataType = CreateStructureTree(sdDataType, source, item.Path, true);
                                item.Children.AddRange(dataType.Children);
                                item.ClassMapping = dataType.ClassMapping;
                            }
                        }
                    }
                }
            }
            while (nav.MoveToNext());
        }

        internal static void BuildMapping(Dictionary<string, string> mapPathsToLinkIds, Questionnaire.GroupComponent group)
        {
            string path = group.Definition()?.Value;
            if (String.IsNullOrEmpty(path))
                path = group.LinkId;
            if (!String.IsNullOrEmpty(path))
            {
                if (mapPathsToLinkIds.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("Duplicate LinkId/Path mapping");
                }
                else
                {
                    mapPathsToLinkIds.Add(path, group.LinkId);
                }
            }
            foreach (var g in group.Group)
            {
                BuildMapping(mapPathsToLinkIds, g);
            }
            foreach (var q in group.Question)
            {
                BuildMapping(mapPathsToLinkIds, q);
            }
        }

        private static void BuildMapping(Dictionary<string, string> mapPathsToLinkIds, Questionnaire.QuestionComponent question)
        {
            string path = question.Definition()?.Value;
            if (String.IsNullOrEmpty(path))
                path = question.LinkId;
            if (!String.IsNullOrEmpty(path))
            {
                if (mapPathsToLinkIds.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("Duplicate LinkId/Path mapping");
                }
                else
                {
                    mapPathsToLinkIds.Add(path, question.LinkId);
                }
            }
            foreach (var g in question.Group)
            {
                BuildMapping(mapPathsToLinkIds, g);
            }
        }

        enum RetainDueTo { Binding, FixedValue, MandatoryProperty, Discard };

        /// <summary>
        /// Check to see if this child should be retained in the collection
        /// </summary>
        /// <param name="si"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        internal static void PruneTree(StructureItem item, Dictionary<string, string> mapPathsToLinkIds)
        {
            // do any of "my" properties have values bound as children
            bool hasBoundChild = false;
            foreach (var child in item.Children)
            {
                if (HasBoundValueInChild(child))
                {
                    hasBoundChild = true;
                    break;
                }
            }

            if (hasBoundChild)
            {
                List<StructureItem> newSet = new List<StructureItem>();
                foreach (var child in item.Children)
                {
                    if (HasBoundValueInChild(child))
                        newSet.Add(child);
                    else if (IsMandatory(child))
                        newSet.Add(child);
                    else if (HasFixedValueInChild(child))
                        newSet.Add(child);
                }
                item.Children = newSet;
            }
            else if (IsMandatory(item))
            {
                List<StructureItem> newSet = new List<StructureItem>();
                foreach (var child in item.Children)
                {
                    if (IsMandatory(child))
                        newSet.Add(child);
                    else if (HasFixedValueInChild(child))
                        newSet.Add(child);
                }
                item.Children = newSet;
            }
            else
            {
                item.Children.Clear();
            }

            // Re-Process all the remaining children
            foreach (var child in item.Children)
            {
                PruneTree(child, mapPathsToLinkIds);
            }
        }

        private static bool HasBoundValueInChild(StructureItem si)
        {
            if (!string.IsNullOrEmpty(si.LinkId))
                return true;
            foreach (var item in si.Children)
            {
                if (HasBoundValueInChild(item))
                    return true;
            }
            return false;
        }

        private static bool IsMandatory(StructureItem si)
        {
            if (si.ed.Min > 0)
                return true;
            return false;
        }

        private static bool HasFixedValueInChild(StructureItem si)
        {
            if (si.ed.Fixed != null)
            {
                System.Diagnostics.Debug.WriteLine($"    <-- fixed ({si.Path})");
                return true;
            }
            if (!si.IsArray && IsMandatory(si))
            {
                foreach (var item in si.Children)
                {
                    if (HasFixedValueInChild(item))
                        return true;
                }
            }
            return false;
        }
    }
}
