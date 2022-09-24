<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile ConceptMap
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>f:ConceptMap</sch:title>
    <sch:rule context="f:ConceptMap">
      <sch:assert test="count(f:url) &gt;= 1">url: minimum cardinality of 'url' is 1</sch:assert>
      <sch:assert test="count(f:version) &gt;= 1">version: minimum cardinality of 'version' is 1</sch:assert>
      <sch:assert test="count(f:title) &gt;= 1">title: minimum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:experimental) &gt;= 1">experimental: minimum cardinality of 'experimental' is 1</sch:assert>
      <sch:assert test="count(f:description) &gt;= 1">description: minimum cardinality of 'description' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap</sch:title>
    <sch:rule context="f:ConceptMap">
      <sch:assert test="not(exists(f:name/@value)) or matches(f:name/@value, '[A-Z]([A-Za-z0-9_]){1,254}')">Name should be usable as an identifier for the module by machine processing applications such as code generation (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.meta</sch:title>
    <sch:rule context="f:ConceptMap/f:meta">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.implicitRules</sch:title>
    <sch:rule context="f:ConceptMap/f:implicitRules">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.language</sch:title>
    <sch:rule context="f:ConceptMap/f:language">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.text</sch:title>
    <sch:rule context="f:ConceptMap/f:text">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), 'value')])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.url</sch:title>
    <sch:rule context="f:ConceptMap/f:url">
      <sch:assert test="not(exists(@value)) or matches(@value, '([^|#])*')">URL should not contain | or # - these characters make processing canonical references problematic (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.identifier</sch:title>
    <sch:rule context="f:ConceptMap/f:identifier">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.version</sch:title>
    <sch:rule context="f:ConceptMap/f:version">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.name</sch:title>
    <sch:rule context="f:ConceptMap/f:name">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.title</sch:title>
    <sch:rule context="f:ConceptMap/f:title">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.status</sch:title>
    <sch:rule context="f:ConceptMap/f:status">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.experimental</sch:title>
    <sch:rule context="f:ConceptMap/f:experimental">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.date</sch:title>
    <sch:rule context="f:ConceptMap/f:date">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.publisher</sch:title>
    <sch:rule context="f:ConceptMap/f:publisher">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.contact</sch:title>
    <sch:rule context="f:ConceptMap/f:contact">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.description</sch:title>
    <sch:rule context="f:ConceptMap/f:description">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.useContext</sch:title>
    <sch:rule context="f:ConceptMap/f:useContext">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.jurisdiction</sch:title>
    <sch:rule context="f:ConceptMap/f:jurisdiction">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.purpose</sch:title>
    <sch:rule context="f:ConceptMap/f:purpose">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.copyright</sch:title>
    <sch:rule context="f:ConceptMap/f:copyright">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.approvalDate</sch:title>
    <sch:rule context="f:ConceptMap/f:approvalDate">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.lastReviewDate</sch:title>
    <sch:rule context="f:ConceptMap/f:lastReviewDate">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.effectivePeriod</sch:title>
    <sch:rule context="f:ConceptMap/f:effectivePeriod">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.topic</sch:title>
    <sch:rule context="f:ConceptMap/f:topic">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.author</sch:title>
    <sch:rule context="f:ConceptMap/f:author">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.editor</sch:title>
    <sch:rule context="f:ConceptMap/f:editor">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.reviewer</sch:title>
    <sch:rule context="f:ConceptMap/f:reviewer">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.endorser</sch:title>
    <sch:rule context="f:ConceptMap/f:endorser">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.relatedArtifact</sch:title>
    <sch:rule context="f:ConceptMap/f:relatedArtifact">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.sourceScope[x] 1</sch:title>
    <sch:rule context="f:ConceptMap/f:sourceScope[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.targetScope[x] 1</sch:title>
    <sch:rule context="f:ConceptMap/f:targetScope[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group</sch:title>
    <sch:rule context="f:ConceptMap/f:group">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.source</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:source">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.target</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:target">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element">
      <sch:assert test="(exists(f:noMap) and not(exists(f:target))) or not(exists(f:noMap))">If noMap is present, target SHALL NOT be present (inherited)</sch:assert>
      <sch:assert test="(exists(f:code) and not(exists(f:valueSet))) or (not(exists(f:code)) and exists(f:valueSet))">Either code or valueSet SHALL be present but not both. (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.code</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:code">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.display</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:display">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.valueSet</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:valueSet">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.noMap</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:noMap">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target">
      <sch:assert test="exists(f:comment) or (f:status/@value = draft) or not(exists(f:relationship)) or ((f:relationship/@value != 'source-is-broader-than-target') and (f:v/@value != 'not-related-to'))">If the map is source-is-broader-than-target or not-related-to, there SHALL be some comments, unless the status is 'draft' (inherited)</sch:assert>
      <sch:assert test="(exists(f:code) and not(exists(f:valueSet))) or (not(exists(f:code)) and exists(f:valueSet))">Either code or valueSet SHALL be present but not both. (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.code</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:code">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.display</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:display">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.valueSet</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:valueSet">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.relationship</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:relationship">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.comment</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:comment">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn">
      <sch:assert test="(exists(f:value) and not(exists(f:valueSet))) or (not(exists(f:value)) and exists(f:valueSet))">One of value[x] or valueSet must exist, but not both. (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn.property</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn/f:property">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn.value[x] 1</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn/f:value[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.dependsOn.valueSet</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:dependsOn/f:valueSet">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.element.target.product</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:element/f:target/f:product">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped">
      <sch:assert test="(f:mode/@value != 'fixed') or (exists(f:code) and not(exists(f:valueSet))) or (not(exists(f:code)) and exists(f:valueSet))">If the mode is 'fixed', either a code or valueSet must be provided, but not both. (inherited)</sch:assert>
      <sch:assert test="(f:mode/@value != 'other-map') or exists(f:otherMap)">If the mode is 'other-map', a url for the other map must be provided (inherited)</sch:assert>
      <sch:assert test="(f:mode/@value = 'fixed') or not(exists(f:code) or exists(f:display) or exists(f:valueSet))">If the mode is not 'fixed', code, display and valueSet are not allowed (inherited)</sch:assert>
      <sch:assert test="(f:mode/@value = 'other-map') or exists(f:relationship)">If the mode is not 'other-map', relationship must be provided (inherited)</sch:assert>
      <sch:assert test="(f:mode/@value = 'other-map') or not(exists(f:otherMap))">If the mode is not 'other-map', otherMap is not allowed (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.extension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.modifierExtension</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.mode</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:mode">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.code</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:code">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.display</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:display">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.valueSet</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:valueSet">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.relationship</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:relationship">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>ConceptMap.group.unmapped.otherMap</sch:title>
    <sch:rule context="f:ConceptMap/f:group/f:unmapped/f:otherMap">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
