<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile ShareableNamingSystem
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>NamingSystem</sch:title>
    <sch:rule context="f:NamingSystem">
      <sch:assert test="not(exists(f:name/@value)) or matches(f:name/@value, '[A-Z]([A-Za-z0-9_]){1,254}')">Name should be usable as an identifier for the module by machine processing applications such as code generation (inherited)</sch:assert>
      <sch:assert test="not(f:kind/@value='root' and f:uniqueId/f:type/@value='uuid')">Root systems cannot have uuid identifiers (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.meta</sch:title>
    <sch:rule context="f:NamingSystem/f:meta">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.implicitRules</sch:title>
    <sch:rule context="f:NamingSystem/f:implicitRules">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.language</sch:title>
    <sch:rule context="f:NamingSystem/f:language">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.text</sch:title>
    <sch:rule context="f:NamingSystem/f:text">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.extension</sch:title>
    <sch:rule context="f:NamingSystem/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), 'value')])">Must have either extensions or value[x], not both</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.modifierExtension</sch:title>
    <sch:rule context="f:NamingSystem/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.url</sch:title>
    <sch:rule context="f:NamingSystem/f:url">
      <sch:assert test="not(exists(@value)) or matches(@value, '([^|#])*')">URL should not contain | or # - these characters make processing canonical references problematic (inherited)</sch:assert>
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.identifier</sch:title>
    <sch:rule context="f:NamingSystem/f:identifier">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.version</sch:title>
    <sch:rule context="f:NamingSystem/f:version">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.name</sch:title>
    <sch:rule context="f:NamingSystem/f:name">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.title</sch:title>
    <sch:rule context="f:NamingSystem/f:title">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.status</sch:title>
    <sch:rule context="f:NamingSystem/f:status">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.kind</sch:title>
    <sch:rule context="f:NamingSystem/f:kind">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.experimental</sch:title>
    <sch:rule context="f:NamingSystem/f:experimental">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.date</sch:title>
    <sch:rule context="f:NamingSystem/f:date">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.publisher</sch:title>
    <sch:rule context="f:NamingSystem/f:publisher">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.contact</sch:title>
    <sch:rule context="f:NamingSystem/f:contact">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.responsible</sch:title>
    <sch:rule context="f:NamingSystem/f:responsible">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.type</sch:title>
    <sch:rule context="f:NamingSystem/f:type">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.description</sch:title>
    <sch:rule context="f:NamingSystem/f:description">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.useContext</sch:title>
    <sch:rule context="f:NamingSystem/f:useContext">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.jurisdiction</sch:title>
    <sch:rule context="f:NamingSystem/f:jurisdiction">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.purpose</sch:title>
    <sch:rule context="f:NamingSystem/f:purpose">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.copyright</sch:title>
    <sch:rule context="f:NamingSystem/f:copyright">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.approvalDate</sch:title>
    <sch:rule context="f:NamingSystem/f:approvalDate">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.lastReviewDate</sch:title>
    <sch:rule context="f:NamingSystem/f:lastReviewDate">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.effectivePeriod</sch:title>
    <sch:rule context="f:NamingSystem/f:effectivePeriod">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.topic</sch:title>
    <sch:rule context="f:NamingSystem/f:topic">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.author</sch:title>
    <sch:rule context="f:NamingSystem/f:author">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.editor</sch:title>
    <sch:rule context="f:NamingSystem/f:editor">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.reviewer</sch:title>
    <sch:rule context="f:NamingSystem/f:reviewer">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.endorser</sch:title>
    <sch:rule context="f:NamingSystem/f:endorser">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.relatedArtifact</sch:title>
    <sch:rule context="f:NamingSystem/f:relatedArtifact">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.usage</sch:title>
    <sch:rule context="f:NamingSystem/f:usage">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.extension</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:extension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.modifierExtension</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:modifierExtension">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="exists(f:extension)!=exists(f:*[starts-with(local-name(.), &quot;value&quot;)])">Must have either extensions or value[x], not both (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.type</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:type">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.value</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:value">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.preferred</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:preferred">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="not(exists(for $type in distinct-values(f:uniqueId/f:type/@value) return if (count(f:uniqueId[f:type/@value=$type and f:preferred/@value=true()])&gt;1) then $type else ()))">Can't have more than one preferred identifier for a type (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.comment</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:comment">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.period</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:period">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>NamingSystem.uniqueId.authoritative</sch:title>
    <sch:rule context="f:NamingSystem/f:uniqueId/f:authoritative">
      <sch:assert test="@value|f:*|h:div">All FHIR elements must have a @value or children (inherited)</sch:assert>
      <sch:assert test="count(f:uniqueID)=count(distinct-values(f:uniqueID/f:period/@value)) or not(exists(for $type in distinct-values(f:uniqueId/f:type/@value) return if (count(f:uniqueId[f:type/@value=$type and f:authoritative/@value=true()])&gt;1) then $type else ()))">Can't have more than one authoritative identifier for a type/period combination (only one authoritative identifier allowed at any given point of time) (inherited)</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
