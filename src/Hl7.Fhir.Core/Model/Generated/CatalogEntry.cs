﻿using System;
using System.Collections.Generic;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Validation;
using System.Linq;
using System.Runtime.Serialization;
using Hl7.Fhir.Utility;

/*
  Copyright (c) 2011+, HL7, Inc.
  All rights reserved.
  
  Redistribution and use in source and binary forms, with or without modification, 
  are permitted provided that the following conditions are met:
  
   * Redistributions of source code must retain the above copyright notice, this 
     list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, 
     this list of conditions and the following disclaimer in the documentation 
     and/or other materials provided with the distribution.
   * Neither the name of HL7 nor the names of its contributors may be used to 
     endorse or promote products derived from this software without specific 
     prior written permission.
  
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
  POSSIBILITY OF SUCH DAMAGE.
  

*/

#pragma warning disable 1591 // suppress XML summary warnings 

//
// Generated for FHIR v4.1.0
//
namespace Hl7.Fhir.Model
{
    /// <summary>
    /// An entry in a catalog
    /// </summary>
    [FhirType("CatalogEntry", IsResource=true)]
    [DataContract]
    public partial class CatalogEntry : Hl7.Fhir.Model.DomainResource, System.ComponentModel.INotifyPropertyChanged
    {
        [NotMapped]
        public override ResourceType ResourceType { get { return ResourceType.CatalogEntry; } }
        [NotMapped]
        public override string TypeName { get { return "CatalogEntry"; } }
        
        /// <summary>
        /// Types of resources that can be attached to catalog entries.
        /// (url: http://hl7.org/fhir/ValueSet/catalogentry-type)
        /// </summary>
        [FhirEnumeration("CatalogEntryType")]
        public enum CatalogEntryType
        {
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("ActivityDefinition", "http://hl7.org/fhir/catalogentry-type"), Description("ActivityDefinition")]
            ActivityDefinition,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("PlanDefinition", "http://hl7.org/fhir/catalogentry-type"), Description("PlanDefinition")]
            PlanDefinition,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("SpecimenDefinition", "http://hl7.org/fhir/catalogentry-type"), Description("SpecimenDefinition")]
            SpecimenDefinition,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("ObservationDefinition", "http://hl7.org/fhir/catalogentry-type"), Description("ObservationDefinition")]
            ObservationDefinition,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("DeviceDefinition", "http://hl7.org/fhir/catalogentry-type"), Description("DeviceDefinition")]
            DeviceDefinition,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("Organization", "http://hl7.org/fhir/catalogentry-type"), Description("Organization")]
            Organization,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("Practitioner", "http://hl7.org/fhir/catalogentry-type"), Description("Practitioner")]
            Practitioner,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("PractitionerRole", "http://hl7.org/fhir/catalogentry-type"), Description("PractitionerRole")]
            PractitionerRole,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("HealthcareService", "http://hl7.org/fhir/catalogentry-type"), Description("HealthcareService")]
            HealthcareService,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("MedicationKnowledge", "http://hl7.org/fhir/catalogentry-type"), Description("MedicationKnowledge")]
            MedicationKnowledge,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("Medication", "http://hl7.org/fhir/catalogentry-type"), Description("Medication")]
            Medication,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("Substance", "http://hl7.org/fhir/catalogentry-type"), Description("Substance")]
            Substance,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-type)
            /// </summary>
            [EnumLiteral("Location", "http://hl7.org/fhir/catalogentry-type"), Description("Location")]
            Location,
        }

        /// <summary>
        /// Public usability statuses for catalog entries.
        /// (url: http://hl7.org/fhir/ValueSet/catalogentry-status)
        /// </summary>
        [FhirEnumeration("CatalogEntryStatus")]
        public enum CatalogEntryStatus
        {
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-status)
            /// </summary>
            [EnumLiteral("draft", "http://hl7.org/fhir/catalogentry-status"), Description("Draft")]
            Draft,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-status)
            /// </summary>
            [EnumLiteral("active", "http://hl7.org/fhir/catalogentry-status"), Description("Active")]
            Active,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-status)
            /// </summary>
            [EnumLiteral("retired", "http://hl7.org/fhir/catalogentry-status"), Description("Retired")]
            Retired,
        }

        /// <summary>
        /// Types of relationships between entries.
        /// (url: http://hl7.org/fhir/ValueSet/catalogentry-relation-type)
        /// </summary>
        [FhirEnumeration("CatalogEntryRelationType")]
        public enum CatalogEntryRelationType
        {
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-relation-type)
            /// </summary>
            [EnumLiteral("triggers", "http://hl7.org/fhir/catalogentry-relation-type"), Description("Triggers")]
            Triggers,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-relation-type)
            /// </summary>
            [EnumLiteral("is-replaced-by", "http://hl7.org/fhir/catalogentry-relation-type"), Description("Is replaced by")]
            IsReplacedBy,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-relation-type)
            /// </summary>
            [EnumLiteral("excludes", "http://hl7.org/fhir/catalogentry-relation-type"), Description("Excludes")]
            Excludes,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/catalogentry-relation-type)
            /// </summary>
            [EnumLiteral("includes", "http://hl7.org/fhir/catalogentry-relation-type"), Description("Includes")]
            Includes,
        }

        [FhirType("RelatedEntryComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class RelatedEntryComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "RelatedEntryComponent"; } }
            
            /// <summary>
            /// triggers | is-replaced-by | excludes | includes
            /// </summary>
            [FhirElement("relationship", Order=40)]
            [Cardinality(Min=1,Max=1)]
            [DataMember]
            public Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryRelationType> RelationshipElement
            {
                get { return _RelationshipElement; }
                set { _RelationshipElement = value; OnPropertyChanged("RelationshipElement"); }
            }
            
            private Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryRelationType> _RelationshipElement;
            
            /// <summary>
            /// triggers | is-replaced-by | excludes | includes
            /// </summary>
            /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
            [NotMapped]
            [IgnoreDataMemberAttribute]
            public Hl7.Fhir.Model.CatalogEntry.CatalogEntryRelationType? Relationship
            {
                get { return RelationshipElement != null ? RelationshipElement.Value : null; }
                set
                {
                    if (!value.HasValue)
                        RelationshipElement = null; 
                    else
                        RelationshipElement = new Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryRelationType>(value);
                    OnPropertyChanged("Relationship");
                }
            }
            
            /// <summary>
            /// The reference to the related entry
            /// </summary>
            [FhirElement("target", Order=50)]
            [CLSCompliant(false)]
			[References("CatalogEntry")]
            [Cardinality(Min=1,Max=1)]
            [DataMember]
            public Hl7.Fhir.Model.ResourceReference Target
            {
                get { return _Target; }
                set { _Target = value; OnPropertyChanged("Target"); }
            }
            
            private Hl7.Fhir.Model.ResourceReference _Target;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as RelatedEntryComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(RelationshipElement != null) dest.RelationshipElement = (Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryRelationType>)RelationshipElement.DeepCopy();
                    if(Target != null) dest.Target = (Hl7.Fhir.Model.ResourceReference)Target.DeepCopy();
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new RelatedEntryComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as RelatedEntryComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(RelationshipElement, otherT.RelationshipElement)) return false;
                if( !DeepComparable.Matches(Target, otherT.Target)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as RelatedEntryComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(RelationshipElement, otherT.RelationshipElement)) return false;
                if( !DeepComparable.IsExactly(Target, otherT.Target)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (RelationshipElement != null) yield return RelationshipElement;
                    if (Target != null) yield return Target;
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (RelationshipElement != null) yield return new ElementValue("relationship", RelationshipElement);
                    if (Target != null) yield return new ElementValue("target", Target);
                }
            }

            
        }
        
        
        /// <summary>
        /// Business identifier of the catalog entry
        /// </summary>
        [FhirElement("identifier", InSummary=true, Order=90)]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Hl7.Fhir.Model.Identifier> Identifier
        {
            get { if(_Identifier==null) _Identifier = new List<Hl7.Fhir.Model.Identifier>(); return _Identifier; }
            set { _Identifier = value; OnPropertyChanged("Identifier"); }
        }
        
        private List<Hl7.Fhir.Model.Identifier> _Identifier;
        
        /// <summary>
        /// Displayable name assigned to the catalog entry
        /// </summary>
        [FhirElement("name", InSummary=true, Order=100)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString NameElement
        {
            get { return _NameElement; }
            set { _NameElement = value; OnPropertyChanged("NameElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _NameElement;
        
        /// <summary>
        /// Displayable name assigned to the catalog entry
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string Name
        {
            get { return NameElement != null ? NameElement.Value : null; }
            set
            {
                if (value == null)
                  NameElement = null; 
                else
                  NameElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("Name");
            }
        }
        
        /// <summary>
        /// ActivityDefinition | PlanDefinition | SpecimenDefinition | ObservationDefinition | DeviceDefinition | Organization | Practitioner | PractitionerRole | HealthcareService | MedicationKnowledge | Medication | Substance | Location
        /// </summary>
        [FhirElement("type", InSummary=true, Order=110)]
        [DataMember]
        public Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryType> TypeElement
        {
            get { return _TypeElement; }
            set { _TypeElement = value; OnPropertyChanged("TypeElement"); }
        }
        
        private Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryType> _TypeElement;
        
        /// <summary>
        /// ActivityDefinition | PlanDefinition | SpecimenDefinition | ObservationDefinition | DeviceDefinition | Organization | Practitioner | PractitionerRole | HealthcareService | MedicationKnowledge | Medication | Substance | Location
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public Hl7.Fhir.Model.CatalogEntry.CatalogEntryType? Type
        {
            get { return TypeElement != null ? TypeElement.Value : null; }
            set
            {
                if (!value.HasValue)
                  TypeElement = null; 
                else
                  TypeElement = new Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryType>(value);
                OnPropertyChanged("Type");
            }
        }
        
        /// <summary>
        /// draft | active | retired
        /// </summary>
        [FhirElement("status", InSummary=true, Order=120)]
        [DataMember]
        public Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryStatus> StatusElement
        {
            get { return _StatusElement; }
            set { _StatusElement = value; OnPropertyChanged("StatusElement"); }
        }
        
        private Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryStatus> _StatusElement;
        
        /// <summary>
        /// draft | active | retired
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public Hl7.Fhir.Model.CatalogEntry.CatalogEntryStatus? Status
        {
            get { return StatusElement != null ? StatusElement.Value : null; }
            set
            {
                if (!value.HasValue)
                  StatusElement = null; 
                else
                  StatusElement = new Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryStatus>(value);
                OnPropertyChanged("Status");
            }
        }
        
        /// <summary>
        /// When this catalog entry is expected to be active
        /// </summary>
        [FhirElement("effectivePeriod", Order=130)]
        [DataMember]
        public Hl7.Fhir.Model.Period EffectivePeriod
        {
            get { return _EffectivePeriod; }
            set { _EffectivePeriod = value; OnPropertyChanged("EffectivePeriod"); }
        }
        
        private Hl7.Fhir.Model.Period _EffectivePeriod;
        
        /// <summary>
        /// Is orderable
        /// </summary>
        [FhirElement("orderable", InSummary=true, Order=140)]
        [Cardinality(Min=1,Max=1)]
        [DataMember]
        public Hl7.Fhir.Model.FhirBoolean OrderableElement
        {
            get { return _OrderableElement; }
            set { _OrderableElement = value; OnPropertyChanged("OrderableElement"); }
        }
        
        private Hl7.Fhir.Model.FhirBoolean _OrderableElement;
        
        /// <summary>
        /// Is orderable
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public bool? Orderable
        {
            get { return OrderableElement != null ? OrderableElement.Value : null; }
            set
            {
                if (!value.HasValue)
                  OrderableElement = null; 
                else
                  OrderableElement = new Hl7.Fhir.Model.FhirBoolean(value);
                OnPropertyChanged("Orderable");
            }
        }
        
        /// <summary>
        /// Item attached to this entry of the catalog
        /// </summary>
        [FhirElement("referencedItem", InSummary=true, Order=150)]
        [CLSCompliant(false)]
		[References("DeviceDefinition","Organization","Practitioner","PractitionerRole","HealthcareService","ActivityDefinition","PlanDefinition","SpecimenDefinition","ObservationDefinition","MedicationKnowledge","Substance","Location")]
        [Cardinality(Min=1,Max=1)]
        [DataMember]
        public Hl7.Fhir.Model.ResourceReference ReferencedItem
        {
            get { return _ReferencedItem; }
            set { _ReferencedItem = value; OnPropertyChanged("ReferencedItem"); }
        }
        
        private Hl7.Fhir.Model.ResourceReference _ReferencedItem;
        
        /// <summary>
        /// Another entry of the catalog related to this one
        /// </summary>
        [FhirElement("relatedEntry", Order=160)]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Hl7.Fhir.Model.CatalogEntry.RelatedEntryComponent> RelatedEntry
        {
            get { if(_RelatedEntry==null) _RelatedEntry = new List<Hl7.Fhir.Model.CatalogEntry.RelatedEntryComponent>(); return _RelatedEntry; }
            set { _RelatedEntry = value; OnPropertyChanged("RelatedEntry"); }
        }
        
        private List<Hl7.Fhir.Model.CatalogEntry.RelatedEntryComponent> _RelatedEntry;
        
        /// <summary>
        /// Last updater of this catalog entry
        /// </summary>
        [FhirElement("updatedBy", Order=170)]
        [CLSCompliant(false)]
		[References("Person","Device")]
        [DataMember]
        public Hl7.Fhir.Model.ResourceReference UpdatedBy
        {
            get { return _UpdatedBy; }
            set { _UpdatedBy = value; OnPropertyChanged("UpdatedBy"); }
        }
        
        private Hl7.Fhir.Model.ResourceReference _UpdatedBy;
        
        /// <summary>
        /// Notes and comments about this catalog entry
        /// </summary>
        [FhirElement("note", Order=180)]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Hl7.Fhir.Model.Annotation> Note
        {
            get { if(_Note==null) _Note = new List<Hl7.Fhir.Model.Annotation>(); return _Note; }
            set { _Note = value; OnPropertyChanged("Note"); }
        }
        
        private List<Hl7.Fhir.Model.Annotation> _Note;
        
        /// <summary>
        /// Billing code in the context of this catalog entry
        /// </summary>
        [FhirElement("billingCode", InSummary=true, Order=190)]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Hl7.Fhir.Model.CodeableConcept> BillingCode
        {
            get { if(_BillingCode==null) _BillingCode = new List<Hl7.Fhir.Model.CodeableConcept>(); return _BillingCode; }
            set { _BillingCode = value; OnPropertyChanged("BillingCode"); }
        }
        
        private List<Hl7.Fhir.Model.CodeableConcept> _BillingCode;
        
        /// <summary>
        /// Billing summary in the context of this catalog entry
        /// </summary>
        [FhirElement("billingSummary", InSummary=true, Order=200)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString BillingSummaryElement
        {
            get { return _BillingSummaryElement; }
            set { _BillingSummaryElement = value; OnPropertyChanged("BillingSummaryElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _BillingSummaryElement;
        
        /// <summary>
        /// Billing summary in the context of this catalog entry
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string BillingSummary
        {
            get { return BillingSummaryElement != null ? BillingSummaryElement.Value : null; }
            set
            {
                if (value == null)
                  BillingSummaryElement = null; 
                else
                  BillingSummaryElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("BillingSummary");
            }
        }
        
        /// <summary>
        /// Schedule summary for the catalog entry
        /// </summary>
        [FhirElement("scheduleSummary", Order=210)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString ScheduleSummaryElement
        {
            get { return _ScheduleSummaryElement; }
            set { _ScheduleSummaryElement = value; OnPropertyChanged("ScheduleSummaryElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _ScheduleSummaryElement;
        
        /// <summary>
        /// Schedule summary for the catalog entry
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string ScheduleSummary
        {
            get { return ScheduleSummaryElement != null ? ScheduleSummaryElement.Value : null; }
            set
            {
                if (value == null)
                  ScheduleSummaryElement = null; 
                else
                  ScheduleSummaryElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("ScheduleSummary");
            }
        }
        
        /// <summary>
        /// Summary of limitations for the catalog entry
        /// </summary>
        [FhirElement("limitationSummary", Order=220)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString LimitationSummaryElement
        {
            get { return _LimitationSummaryElement; }
            set { _LimitationSummaryElement = value; OnPropertyChanged("LimitationSummaryElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _LimitationSummaryElement;
        
        /// <summary>
        /// Summary of limitations for the catalog entry
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string LimitationSummary
        {
            get { return LimitationSummaryElement != null ? LimitationSummaryElement.Value : null; }
            set
            {
                if (value == null)
                  LimitationSummaryElement = null; 
                else
                  LimitationSummaryElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("LimitationSummary");
            }
        }
        
        /// <summary>
        /// Regulatory  summary for the catalog entry
        /// </summary>
        [FhirElement("regulatorySummary", Order=230)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString RegulatorySummaryElement
        {
            get { return _RegulatorySummaryElement; }
            set { _RegulatorySummaryElement = value; OnPropertyChanged("RegulatorySummaryElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _RegulatorySummaryElement;
        
        /// <summary>
        /// Regulatory  summary for the catalog entry
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string RegulatorySummary
        {
            get { return RegulatorySummaryElement != null ? RegulatorySummaryElement.Value : null; }
            set
            {
                if (value == null)
                  RegulatorySummaryElement = null; 
                else
                  RegulatorySummaryElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("RegulatorySummary");
            }
        }
        

        public override void AddDefaultConstraints()
        {
            base.AddDefaultConstraints();

        }

        public override IDeepCopyable CopyTo(IDeepCopyable other)
        {
            var dest = other as CatalogEntry;
            
            if (dest != null)
            {
                base.CopyTo(dest);
                if(Identifier != null) dest.Identifier = new List<Hl7.Fhir.Model.Identifier>(Identifier.DeepCopy());
                if(NameElement != null) dest.NameElement = (Hl7.Fhir.Model.FhirString)NameElement.DeepCopy();
                if(TypeElement != null) dest.TypeElement = (Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryType>)TypeElement.DeepCopy();
                if(StatusElement != null) dest.StatusElement = (Code<Hl7.Fhir.Model.CatalogEntry.CatalogEntryStatus>)StatusElement.DeepCopy();
                if(EffectivePeriod != null) dest.EffectivePeriod = (Hl7.Fhir.Model.Period)EffectivePeriod.DeepCopy();
                if(OrderableElement != null) dest.OrderableElement = (Hl7.Fhir.Model.FhirBoolean)OrderableElement.DeepCopy();
                if(ReferencedItem != null) dest.ReferencedItem = (Hl7.Fhir.Model.ResourceReference)ReferencedItem.DeepCopy();
                if(RelatedEntry != null) dest.RelatedEntry = new List<Hl7.Fhir.Model.CatalogEntry.RelatedEntryComponent>(RelatedEntry.DeepCopy());
                if(UpdatedBy != null) dest.UpdatedBy = (Hl7.Fhir.Model.ResourceReference)UpdatedBy.DeepCopy();
                if(Note != null) dest.Note = new List<Hl7.Fhir.Model.Annotation>(Note.DeepCopy());
                if(BillingCode != null) dest.BillingCode = new List<Hl7.Fhir.Model.CodeableConcept>(BillingCode.DeepCopy());
                if(BillingSummaryElement != null) dest.BillingSummaryElement = (Hl7.Fhir.Model.FhirString)BillingSummaryElement.DeepCopy();
                if(ScheduleSummaryElement != null) dest.ScheduleSummaryElement = (Hl7.Fhir.Model.FhirString)ScheduleSummaryElement.DeepCopy();
                if(LimitationSummaryElement != null) dest.LimitationSummaryElement = (Hl7.Fhir.Model.FhirString)LimitationSummaryElement.DeepCopy();
                if(RegulatorySummaryElement != null) dest.RegulatorySummaryElement = (Hl7.Fhir.Model.FhirString)RegulatorySummaryElement.DeepCopy();
                return dest;
            }
            else
            	throw new ArgumentException("Can only copy to an object of the same type", "other");
        }
        
        public override IDeepCopyable DeepCopy()
        {
            return CopyTo(new CatalogEntry());
        }
        
        public override bool Matches(IDeepComparable other)
        {
            var otherT = other as CatalogEntry;
            if(otherT == null) return false;
            
            if(!base.Matches(otherT)) return false;
            if( !DeepComparable.Matches(Identifier, otherT.Identifier)) return false;
            if( !DeepComparable.Matches(NameElement, otherT.NameElement)) return false;
            if( !DeepComparable.Matches(TypeElement, otherT.TypeElement)) return false;
            if( !DeepComparable.Matches(StatusElement, otherT.StatusElement)) return false;
            if( !DeepComparable.Matches(EffectivePeriod, otherT.EffectivePeriod)) return false;
            if( !DeepComparable.Matches(OrderableElement, otherT.OrderableElement)) return false;
            if( !DeepComparable.Matches(ReferencedItem, otherT.ReferencedItem)) return false;
            if( !DeepComparable.Matches(RelatedEntry, otherT.RelatedEntry)) return false;
            if( !DeepComparable.Matches(UpdatedBy, otherT.UpdatedBy)) return false;
            if( !DeepComparable.Matches(Note, otherT.Note)) return false;
            if( !DeepComparable.Matches(BillingCode, otherT.BillingCode)) return false;
            if( !DeepComparable.Matches(BillingSummaryElement, otherT.BillingSummaryElement)) return false;
            if( !DeepComparable.Matches(ScheduleSummaryElement, otherT.ScheduleSummaryElement)) return false;
            if( !DeepComparable.Matches(LimitationSummaryElement, otherT.LimitationSummaryElement)) return false;
            if( !DeepComparable.Matches(RegulatorySummaryElement, otherT.RegulatorySummaryElement)) return false;
            
            return true;
        }
        
        public override bool IsExactly(IDeepComparable other)
        {
            var otherT = other as CatalogEntry;
            if(otherT == null) return false;
            
            if(!base.IsExactly(otherT)) return false;
            if( !DeepComparable.IsExactly(Identifier, otherT.Identifier)) return false;
            if( !DeepComparable.IsExactly(NameElement, otherT.NameElement)) return false;
            if( !DeepComparable.IsExactly(TypeElement, otherT.TypeElement)) return false;
            if( !DeepComparable.IsExactly(StatusElement, otherT.StatusElement)) return false;
            if( !DeepComparable.IsExactly(EffectivePeriod, otherT.EffectivePeriod)) return false;
            if( !DeepComparable.IsExactly(OrderableElement, otherT.OrderableElement)) return false;
            if( !DeepComparable.IsExactly(ReferencedItem, otherT.ReferencedItem)) return false;
            if( !DeepComparable.IsExactly(RelatedEntry, otherT.RelatedEntry)) return false;
            if( !DeepComparable.IsExactly(UpdatedBy, otherT.UpdatedBy)) return false;
            if( !DeepComparable.IsExactly(Note, otherT.Note)) return false;
            if( !DeepComparable.IsExactly(BillingCode, otherT.BillingCode)) return false;
            if( !DeepComparable.IsExactly(BillingSummaryElement, otherT.BillingSummaryElement)) return false;
            if( !DeepComparable.IsExactly(ScheduleSummaryElement, otherT.ScheduleSummaryElement)) return false;
            if( !DeepComparable.IsExactly(LimitationSummaryElement, otherT.LimitationSummaryElement)) return false;
            if( !DeepComparable.IsExactly(RegulatorySummaryElement, otherT.RegulatorySummaryElement)) return false;
            
            return true;
        }

        [NotMapped]
        public override IEnumerable<Base> Children
        {
            get
            {
                foreach (var item in base.Children) yield return item;
				foreach (var elem in Identifier) { if (elem != null) yield return elem; }
				if (NameElement != null) yield return NameElement;
				if (TypeElement != null) yield return TypeElement;
				if (StatusElement != null) yield return StatusElement;
				if (EffectivePeriod != null) yield return EffectivePeriod;
				if (OrderableElement != null) yield return OrderableElement;
				if (ReferencedItem != null) yield return ReferencedItem;
				foreach (var elem in RelatedEntry) { if (elem != null) yield return elem; }
				if (UpdatedBy != null) yield return UpdatedBy;
				foreach (var elem in Note) { if (elem != null) yield return elem; }
				foreach (var elem in BillingCode) { if (elem != null) yield return elem; }
				if (BillingSummaryElement != null) yield return BillingSummaryElement;
				if (ScheduleSummaryElement != null) yield return ScheduleSummaryElement;
				if (LimitationSummaryElement != null) yield return LimitationSummaryElement;
				if (RegulatorySummaryElement != null) yield return RegulatorySummaryElement;
            }
        }

        [NotMapped]
        internal override IEnumerable<ElementValue> NamedChildren
        {
            get
            {
                foreach (var item in base.NamedChildren) yield return item;
                foreach (var elem in Identifier) { if (elem != null) yield return new ElementValue("identifier", elem); }
                if (NameElement != null) yield return new ElementValue("name", NameElement);
                if (TypeElement != null) yield return new ElementValue("type", TypeElement);
                if (StatusElement != null) yield return new ElementValue("status", StatusElement);
                if (EffectivePeriod != null) yield return new ElementValue("effectivePeriod", EffectivePeriod);
                if (OrderableElement != null) yield return new ElementValue("orderable", OrderableElement);
                if (ReferencedItem != null) yield return new ElementValue("referencedItem", ReferencedItem);
                foreach (var elem in RelatedEntry) { if (elem != null) yield return new ElementValue("relatedEntry", elem); }
                if (UpdatedBy != null) yield return new ElementValue("updatedBy", UpdatedBy);
                foreach (var elem in Note) { if (elem != null) yield return new ElementValue("note", elem); }
                foreach (var elem in BillingCode) { if (elem != null) yield return new ElementValue("billingCode", elem); }
                if (BillingSummaryElement != null) yield return new ElementValue("billingSummary", BillingSummaryElement);
                if (ScheduleSummaryElement != null) yield return new ElementValue("scheduleSummary", ScheduleSummaryElement);
                if (LimitationSummaryElement != null) yield return new ElementValue("limitationSummary", LimitationSummaryElement);
                if (RegulatorySummaryElement != null) yield return new ElementValue("regulatorySummary", RegulatorySummaryElement);
            }
        }

    }
    
}
