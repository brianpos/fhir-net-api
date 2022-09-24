<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile RequestOrchestration
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>f:RequestOrchestration</sch:title>
    <sch:rule context="f:RequestOrchestration">
      <sch:assert test="count(f:identifier) &gt;= 1">identifier: minimum cardinality of 'identifier' is 1</sch:assert>
      <sch:assert test="count(f:identifier) &lt;= 1">identifier: maximum cardinality of 'identifier' is 1</sch:assert>
      <sch:assert test="count(f:instantiatesUri) &gt;= 1">instantiatesUri: minimum cardinality of 'instantiatesUri' is 1</sch:assert>
      <sch:assert test="count(f:instantiatesUri) &lt;= 1">instantiatesUri: maximum cardinality of 'instantiatesUri' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration</sch:title>
    <sch:rule context="f:RequestOrchestration">
      <sch:assert test="not(parent::f:contained and f:contained)">If the resource is contained in another resource, it SHALL NOT contain nested Resources (inherited)</sch:assert>
      <sch:assert test="not(exists(for $contained in f:contained return $contained[not(exists(parent::*/descendant::f:reference/@value=concat('#', $contained/*/f:id/@value)) or exists(descendant::f:reference[@value='#']))]))">If the resource is contained in another resource, it SHALL be referred to from elsewhere in the resource or SHALL refer to the containing resource (inherited)</sch:assert>
      <sch:assert test="not(exists(f:contained/*/f:meta/f:versionId)) and not(exists(f:contained/*/f:meta/f:lastUpdated))">If a resource is contained in another resource, it SHALL NOT have a meta.versionId or a meta.lastUpdated (inherited)</sch:assert>
      <sch:assert test="not(exists(f:contained/*/f:meta/f:security))">If a resource is contained in another resource, it SHALL NOT have a security label (inherited)</sch:assert>
      <sch:assert test="exists(f:text/h:div)">A resource should have narrative for robust management (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.meta</sch:title>
    <sch:rule context="f:RequestOrchestration/f:meta">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.implicitRules</sch:title>
    <sch:rule context="f:RequestOrchestration/f:implicitRules">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.language</sch:title>
    <sch:rule context="f:RequestOrchestration/f:language">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.text</sch:title>
    <sch:rule context="f:RequestOrchestration/f:text">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.identifier</sch:title>
    <sch:rule context="f:RequestOrchestration/f:identifier">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.instantiatesCanonical</sch:title>
    <sch:rule context="f:RequestOrchestration/f:instantiatesCanonical">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.instantiatesUri</sch:title>
    <sch:rule context="f:RequestOrchestration/f:instantiatesUri">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.basedOn</sch:title>
    <sch:rule context="f:RequestOrchestration/f:basedOn">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.replaces</sch:title>
    <sch:rule context="f:RequestOrchestration/f:replaces">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.groupIdentifier</sch:title>
    <sch:rule context="f:RequestOrchestration/f:groupIdentifier">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.status</sch:title>
    <sch:rule context="f:RequestOrchestration/f:status">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.intent</sch:title>
    <sch:rule context="f:RequestOrchestration/f:intent">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.priority</sch:title>
    <sch:rule context="f:RequestOrchestration/f:priority">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.code</sch:title>
    <sch:rule context="f:RequestOrchestration/f:code">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.subject</sch:title>
    <sch:rule context="f:RequestOrchestration/f:subject">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.encounter</sch:title>
    <sch:rule context="f:RequestOrchestration/f:encounter">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.authoredOn</sch:title>
    <sch:rule context="f:RequestOrchestration/f:authoredOn">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.author</sch:title>
    <sch:rule context="f:RequestOrchestration/f:author">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.reason</sch:title>
    <sch:rule context="f:RequestOrchestration/f:reason">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.goal</sch:title>
    <sch:rule context="f:RequestOrchestration/f:goal">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.note</sch:title>
    <sch:rule context="f:RequestOrchestration/f:note">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:resource) != exists(f:action)">Must have resource or action but not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.linkId</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:linkId">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.prefix</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:prefix">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.title</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:title">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.description</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:description">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.textEquivalent</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:textEquivalent">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.priority</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:priority">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.code</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:code">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.documentation</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:documentation">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.goal</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:goal">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.condition</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:condition">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.condition.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:condition/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.condition.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:condition/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.condition.kind</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:condition/f:kind">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.condition.expression</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:condition/f:expression">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:requirement) xor exists (f:relatedData/@value)">Input data elements must have a requirement or a relatedData, but not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input.title</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input/f:title">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input.requirement</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input/f:requirement">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.input.relatedData</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:input/f:relatedData">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:requirement) xor exists (f:relatedData/@value)">Output data element must have a requirement or a relatedData, but not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output.title</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output/f:title">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output.requirement</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output/f:requirement">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.output.relatedData</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:output/f:relatedData">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction.targetId</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction/f:targetId">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction.relationship</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction/f:relationship">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.relatedAction.offset[x] 1</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:relatedAction/f:offset[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.timing[x] 1</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:timing[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.location</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:location">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.type</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:type">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.typeCanonical</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:typeCanonical">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.typeReference</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:typeReference">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.role</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:role">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.function</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:function">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.participant.actor[x] 1</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:participant/f:actor[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.type</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:type">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.groupingBehavior</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:groupingBehavior">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.selectionBehavior</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:selectionBehavior">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.requiredBehavior</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:requiredBehavior">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.precheckBehavior</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:precheckBehavior">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.cardinalityBehavior</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:cardinalityBehavior">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.resource</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:resource">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.definition[x] 1</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:definition[x]">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.transform</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:transform">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.dynamicValue</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:dynamicValue">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.dynamicValue.extension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:dynamicValue/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.dynamicValue.modifierExtension</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:dynamicValue/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.dynamicValue.path</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:dynamicValue/f:path">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.dynamicValue.expression</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:dynamicValue/f:expression">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>RequestOrchestration.action.action</sch:title>
    <sch:rule context="f:RequestOrchestration/f:action/f:action">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
