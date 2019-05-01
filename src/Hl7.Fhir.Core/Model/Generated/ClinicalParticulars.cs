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
    /// ClinicalParticulars
    /// </summary>
    [FhirType("ClinicalParticulars", IsResource=true)]
    [DataContract]
    public partial class ClinicalParticulars : Hl7.Fhir.Model.DomainResource, System.ComponentModel.INotifyPropertyChanged
    {
        [NotMapped]
        public override ResourceType ResourceType { get { return ResourceType.ClinicalParticulars; } }
        [NotMapped]
        public override string TypeName { get { return "ClinicalParticulars"; } }
        
        /// <summary>
        /// Overall defining type of this clinical particular.
        /// (url: http://hl7.org/fhir/ValueSet/clinical-particulars-type)
        /// </summary>
        [FhirEnumeration("ClinicalParticularsType")]
        public enum ClinicalParticularsType
        {
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/clinical-particulars-type)
            /// </summary>
            [EnumLiteral("indication", "http://hl7.org/fhir/clinical-particulars-type"), Description("Indication")]
            Indication,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/clinical-particulars-type)
            /// </summary>
            [EnumLiteral("contraindication", "http://hl7.org/fhir/clinical-particulars-type"), Description("Contraindication")]
            Contraindication,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/clinical-particulars-type)
            /// </summary>
            [EnumLiteral("interaction", "http://hl7.org/fhir/clinical-particulars-type"), Description("Interaction")]
            Interaction,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/clinical-particulars-type)
            /// </summary>
            [EnumLiteral("undesirable-effect", "http://hl7.org/fhir/clinical-particulars-type"), Description("Undesirable Effect")]
            UndesirableEffect,
            /// <summary>
            /// MISSING DESCRIPTION
            /// (system: http://hl7.org/fhir/clinical-particulars-type)
            /// </summary>
            [EnumLiteral("other", "http://hl7.org/fhir/clinical-particulars-type"), Description("Other")]
            Other,
        }

        [FhirType("ContraindicationComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class ContraindicationComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "ContraindicationComponent"; } }
            
            /// <summary>
            /// The disease, symptom or procedure for the contraindication
            /// </summary>
            [FhirElement("diseaseSymptomProcedure", InSummary=true, Order=40)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept DiseaseSymptomProcedure
            {
                get { return _DiseaseSymptomProcedure; }
                set { _DiseaseSymptomProcedure = value; OnPropertyChanged("DiseaseSymptomProcedure"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _DiseaseSymptomProcedure;
            
            /// <summary>
            /// The status of the disease or symptom for the contraindication
            /// </summary>
            [FhirElement("diseaseStatus", InSummary=true, Order=50)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept DiseaseStatus
            {
                get { return _DiseaseStatus; }
                set { _DiseaseStatus = value; OnPropertyChanged("DiseaseStatus"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _DiseaseStatus;
            
            /// <summary>
            /// A comorbidity (concurrent condition) or coinfection
            /// </summary>
            [FhirElement("comorbidity", InSummary=true, Order=60)]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.CodeableConcept> Comorbidity
            {
                get { if(_Comorbidity==null) _Comorbidity = new List<Hl7.Fhir.Model.CodeableConcept>(); return _Comorbidity; }
                set { _Comorbidity = value; OnPropertyChanged("Comorbidity"); }
            }
            
            private List<Hl7.Fhir.Model.CodeableConcept> _Comorbidity;
            
            /// <summary>
            /// The indication which this is a contraidication for
            /// </summary>
            [FhirElement("indication", InSummary=true, Order=70)]
            [CLSCompliant(false)]
			[References("ClinicalParticulars")]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.ResourceReference> Indication
            {
                get { if(_Indication==null) _Indication = new List<Hl7.Fhir.Model.ResourceReference>(); return _Indication; }
                set { _Indication = value; OnPropertyChanged("Indication"); }
            }
            
            private List<Hl7.Fhir.Model.ResourceReference> _Indication;
            
            /// <summary>
            /// Information about the use of the medicinal product in relation to other therapies described as part of the indication
            /// </summary>
            [FhirElement("otherTherapy", InSummary=true, Order=80)]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent> OtherTherapy
            {
                get { if(_OtherTherapy==null) _OtherTherapy = new List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent>(); return _OtherTherapy; }
                set { _OtherTherapy = value; OnPropertyChanged("OtherTherapy"); }
            }
            
            private List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent> _OtherTherapy;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as ContraindicationComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(DiseaseSymptomProcedure != null) dest.DiseaseSymptomProcedure = (Hl7.Fhir.Model.CodeableConcept)DiseaseSymptomProcedure.DeepCopy();
                    if(DiseaseStatus != null) dest.DiseaseStatus = (Hl7.Fhir.Model.CodeableConcept)DiseaseStatus.DeepCopy();
                    if(Comorbidity != null) dest.Comorbidity = new List<Hl7.Fhir.Model.CodeableConcept>(Comorbidity.DeepCopy());
                    if(Indication != null) dest.Indication = new List<Hl7.Fhir.Model.ResourceReference>(Indication.DeepCopy());
                    if(OtherTherapy != null) dest.OtherTherapy = new List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent>(OtherTherapy.DeepCopy());
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new ContraindicationComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as ContraindicationComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(DiseaseSymptomProcedure, otherT.DiseaseSymptomProcedure)) return false;
                if( !DeepComparable.Matches(DiseaseStatus, otherT.DiseaseStatus)) return false;
                if( !DeepComparable.Matches(Comorbidity, otherT.Comorbidity)) return false;
                if( !DeepComparable.Matches(Indication, otherT.Indication)) return false;
                if( !DeepComparable.Matches(OtherTherapy, otherT.OtherTherapy)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as ContraindicationComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(DiseaseSymptomProcedure, otherT.DiseaseSymptomProcedure)) return false;
                if( !DeepComparable.IsExactly(DiseaseStatus, otherT.DiseaseStatus)) return false;
                if( !DeepComparable.IsExactly(Comorbidity, otherT.Comorbidity)) return false;
                if( !DeepComparable.IsExactly(Indication, otherT.Indication)) return false;
                if( !DeepComparable.IsExactly(OtherTherapy, otherT.OtherTherapy)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (DiseaseSymptomProcedure != null) yield return DiseaseSymptomProcedure;
                    if (DiseaseStatus != null) yield return DiseaseStatus;
                    foreach (var elem in Comorbidity) { if (elem != null) yield return elem; }
                    foreach (var elem in Indication) { if (elem != null) yield return elem; }
                    foreach (var elem in OtherTherapy) { if (elem != null) yield return elem; }
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (DiseaseSymptomProcedure != null) yield return new ElementValue("diseaseSymptomProcedure", DiseaseSymptomProcedure);
                    if (DiseaseStatus != null) yield return new ElementValue("diseaseStatus", DiseaseStatus);
                    foreach (var elem in Comorbidity) { if (elem != null) yield return new ElementValue("comorbidity", elem); }
                    foreach (var elem in Indication) { if (elem != null) yield return new ElementValue("indication", elem); }
                    foreach (var elem in OtherTherapy) { if (elem != null) yield return new ElementValue("otherTherapy", elem); }
                }
            }

            
        }
        
        
        [FhirType("OtherTherapyComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class OtherTherapyComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "OtherTherapyComponent"; } }
            
            /// <summary>
            /// The type of relationship between the medicinal product indication or contraindication and another therapy
            /// </summary>
            [FhirElement("therapyRelationshipType", InSummary=true, Order=40)]
            [Cardinality(Min=1,Max=1)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept TherapyRelationshipType
            {
                get { return _TherapyRelationshipType; }
                set { _TherapyRelationshipType = value; OnPropertyChanged("TherapyRelationshipType"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _TherapyRelationshipType;
            
            /// <summary>
            /// Reference to a specific medication (active substance, medicinal product or class of products) as part of an indication or contraindication
            /// </summary>
            [FhirElement("medication", InSummary=true, Order=50, Choice=ChoiceType.DatatypeChoice)]
            [CLSCompliant(false)]
			[AllowedTypes(typeof(Hl7.Fhir.Model.CodeableConcept),typeof(Hl7.Fhir.Model.ResourceReference))]
            [Cardinality(Min=1,Max=1)]
            [DataMember]
            public Hl7.Fhir.Model.Element Medication
            {
                get { return _Medication; }
                set { _Medication = value; OnPropertyChanged("Medication"); }
            }
            
            private Hl7.Fhir.Model.Element _Medication;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as OtherTherapyComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(TherapyRelationshipType != null) dest.TherapyRelationshipType = (Hl7.Fhir.Model.CodeableConcept)TherapyRelationshipType.DeepCopy();
                    if(Medication != null) dest.Medication = (Hl7.Fhir.Model.Element)Medication.DeepCopy();
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new OtherTherapyComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as OtherTherapyComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(TherapyRelationshipType, otherT.TherapyRelationshipType)) return false;
                if( !DeepComparable.Matches(Medication, otherT.Medication)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as OtherTherapyComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(TherapyRelationshipType, otherT.TherapyRelationshipType)) return false;
                if( !DeepComparable.IsExactly(Medication, otherT.Medication)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (TherapyRelationshipType != null) yield return TherapyRelationshipType;
                    if (Medication != null) yield return Medication;
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (TherapyRelationshipType != null) yield return new ElementValue("therapyRelationshipType", TherapyRelationshipType);
                    if (Medication != null) yield return new ElementValue("medication", Medication);
                }
            }

            
        }
        
        
        [FhirType("IndicationComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class IndicationComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "IndicationComponent"; } }
            
            /// <summary>
            /// The disease, symptom or procedure for the indication
            /// </summary>
            [FhirElement("diseaseSymptomProcedure", InSummary=true, Order=40)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept DiseaseSymptomProcedure
            {
                get { return _DiseaseSymptomProcedure; }
                set { _DiseaseSymptomProcedure = value; OnPropertyChanged("DiseaseSymptomProcedure"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _DiseaseSymptomProcedure;
            
            /// <summary>
            /// The status of the disease or symptom for the indication
            /// </summary>
            [FhirElement("diseaseStatus", InSummary=true, Order=50)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept DiseaseStatus
            {
                get { return _DiseaseStatus; }
                set { _DiseaseStatus = value; OnPropertyChanged("DiseaseStatus"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _DiseaseStatus;
            
            /// <summary>
            /// A comorbidity (concurrent condition) or coinfection as part of the indication
            /// </summary>
            [FhirElement("comorbidity", InSummary=true, Order=60)]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.CodeableConcept> Comorbidity
            {
                get { if(_Comorbidity==null) _Comorbidity = new List<Hl7.Fhir.Model.CodeableConcept>(); return _Comorbidity; }
                set { _Comorbidity = value; OnPropertyChanged("Comorbidity"); }
            }
            
            private List<Hl7.Fhir.Model.CodeableConcept> _Comorbidity;
            
            /// <summary>
            /// For an indication - the intended effect, aim or strategy to be achieved
            /// </summary>
            [FhirElement("intendedEffect", InSummary=true, Order=70)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept IntendedEffect
            {
                get { return _IntendedEffect; }
                set { _IntendedEffect = value; OnPropertyChanged("IntendedEffect"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _IntendedEffect;
            
            /// <summary>
            /// For an indication - timing or duration information
            /// </summary>
            [FhirElement("duration", InSummary=true, Order=80)]
            [DataMember]
            public Quantity Duration
            {
                get { return _Duration; }
                set { _Duration = value; OnPropertyChanged("Duration"); }
            }
            
            private Quantity _Duration;
            
            /// <summary>
            /// For an indicaton - the specific undesirable effects of the medicinal product
            /// </summary>
            [FhirElement("undesirableEffect", InSummary=true, Order=90)]
            [CLSCompliant(false)]
			[References("ClinicalParticulars")]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.ResourceReference> UndesirableEffect
            {
                get { if(_UndesirableEffect==null) _UndesirableEffect = new List<Hl7.Fhir.Model.ResourceReference>(); return _UndesirableEffect; }
                set { _UndesirableEffect = value; OnPropertyChanged("UndesirableEffect"); }
            }
            
            private List<Hl7.Fhir.Model.ResourceReference> _UndesirableEffect;
            
            /// <summary>
            /// Information about the use of the medicinal product in relation to other therapies described as part of the contraindication
            /// </summary>
            [FhirElement("otherTherapy", InSummary=true, Order=100)]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent> OtherTherapy
            {
                get { if(_OtherTherapy==null) _OtherTherapy = new List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent>(); return _OtherTherapy; }
                set { _OtherTherapy = value; OnPropertyChanged("OtherTherapy"); }
            }
            
            private List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent> _OtherTherapy;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as IndicationComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(DiseaseSymptomProcedure != null) dest.DiseaseSymptomProcedure = (Hl7.Fhir.Model.CodeableConcept)DiseaseSymptomProcedure.DeepCopy();
                    if(DiseaseStatus != null) dest.DiseaseStatus = (Hl7.Fhir.Model.CodeableConcept)DiseaseStatus.DeepCopy();
                    if(Comorbidity != null) dest.Comorbidity = new List<Hl7.Fhir.Model.CodeableConcept>(Comorbidity.DeepCopy());
                    if(IntendedEffect != null) dest.IntendedEffect = (Hl7.Fhir.Model.CodeableConcept)IntendedEffect.DeepCopy();
                    if(Duration != null) dest.Duration = (Quantity)Duration.DeepCopy();
                    if(UndesirableEffect != null) dest.UndesirableEffect = new List<Hl7.Fhir.Model.ResourceReference>(UndesirableEffect.DeepCopy());
                    if(OtherTherapy != null) dest.OtherTherapy = new List<Hl7.Fhir.Model.ClinicalParticulars.OtherTherapyComponent>(OtherTherapy.DeepCopy());
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new IndicationComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as IndicationComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(DiseaseSymptomProcedure, otherT.DiseaseSymptomProcedure)) return false;
                if( !DeepComparable.Matches(DiseaseStatus, otherT.DiseaseStatus)) return false;
                if( !DeepComparable.Matches(Comorbidity, otherT.Comorbidity)) return false;
                if( !DeepComparable.Matches(IntendedEffect, otherT.IntendedEffect)) return false;
                if( !DeepComparable.Matches(Duration, otherT.Duration)) return false;
                if( !DeepComparable.Matches(UndesirableEffect, otherT.UndesirableEffect)) return false;
                if( !DeepComparable.Matches(OtherTherapy, otherT.OtherTherapy)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as IndicationComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(DiseaseSymptomProcedure, otherT.DiseaseSymptomProcedure)) return false;
                if( !DeepComparable.IsExactly(DiseaseStatus, otherT.DiseaseStatus)) return false;
                if( !DeepComparable.IsExactly(Comorbidity, otherT.Comorbidity)) return false;
                if( !DeepComparable.IsExactly(IntendedEffect, otherT.IntendedEffect)) return false;
                if( !DeepComparable.IsExactly(Duration, otherT.Duration)) return false;
                if( !DeepComparable.IsExactly(UndesirableEffect, otherT.UndesirableEffect)) return false;
                if( !DeepComparable.IsExactly(OtherTherapy, otherT.OtherTherapy)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (DiseaseSymptomProcedure != null) yield return DiseaseSymptomProcedure;
                    if (DiseaseStatus != null) yield return DiseaseStatus;
                    foreach (var elem in Comorbidity) { if (elem != null) yield return elem; }
                    if (IntendedEffect != null) yield return IntendedEffect;
                    if (Duration != null) yield return Duration;
                    foreach (var elem in UndesirableEffect) { if (elem != null) yield return elem; }
                    foreach (var elem in OtherTherapy) { if (elem != null) yield return elem; }
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (DiseaseSymptomProcedure != null) yield return new ElementValue("diseaseSymptomProcedure", DiseaseSymptomProcedure);
                    if (DiseaseStatus != null) yield return new ElementValue("diseaseStatus", DiseaseStatus);
                    foreach (var elem in Comorbidity) { if (elem != null) yield return new ElementValue("comorbidity", elem); }
                    if (IntendedEffect != null) yield return new ElementValue("intendedEffect", IntendedEffect);
                    if (Duration != null) yield return new ElementValue("duration", Duration);
                    foreach (var elem in UndesirableEffect) { if (elem != null) yield return new ElementValue("undesirableEffect", elem); }
                    foreach (var elem in OtherTherapy) { if (elem != null) yield return new ElementValue("otherTherapy", elem); }
                }
            }

            
        }
        
        
        [FhirType("InteractionComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class InteractionComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "InteractionComponent"; } }
            
            /// <summary>
            /// The specific medication, food or laboratory test that interacts
            /// </summary>
            [FhirElement("interactant", InSummary=true, Order=40)]
            [Cardinality(Min=0,Max=-1)]
            [DataMember]
            public List<Hl7.Fhir.Model.ClinicalParticulars.InteractantComponent> Interactant
            {
                get { if(_Interactant==null) _Interactant = new List<Hl7.Fhir.Model.ClinicalParticulars.InteractantComponent>(); return _Interactant; }
                set { _Interactant = value; OnPropertyChanged("Interactant"); }
            }
            
            private List<Hl7.Fhir.Model.ClinicalParticulars.InteractantComponent> _Interactant;
            
            /// <summary>
            /// The type of the interaction e.g. drug-drug interaction, drug-food interaction, drug-lab test interaction
            /// </summary>
            [FhirElement("type", InSummary=true, Order=50)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept Type
            {
                get { return _Type; }
                set { _Type = value; OnPropertyChanged("Type"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _Type;
            
            /// <summary>
            /// The effect of the interaction, for example "reduced gastric absorption of primary medication"
            /// </summary>
            [FhirElement("effect", InSummary=true, Order=60)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept Effect
            {
                get { return _Effect; }
                set { _Effect = value; OnPropertyChanged("Effect"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _Effect;
            
            /// <summary>
            /// The incidence of the interaction, e.g. theoretical, observed
            /// </summary>
            [FhirElement("incidence", InSummary=true, Order=70)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept Incidence
            {
                get { return _Incidence; }
                set { _Incidence = value; OnPropertyChanged("Incidence"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _Incidence;
            
            /// <summary>
            /// Actions for managing the interaction
            /// </summary>
            [FhirElement("management", InSummary=true, Order=80)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept Management
            {
                get { return _Management; }
                set { _Management = value; OnPropertyChanged("Management"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _Management;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as InteractionComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(Interactant != null) dest.Interactant = new List<Hl7.Fhir.Model.ClinicalParticulars.InteractantComponent>(Interactant.DeepCopy());
                    if(Type != null) dest.Type = (Hl7.Fhir.Model.CodeableConcept)Type.DeepCopy();
                    if(Effect != null) dest.Effect = (Hl7.Fhir.Model.CodeableConcept)Effect.DeepCopy();
                    if(Incidence != null) dest.Incidence = (Hl7.Fhir.Model.CodeableConcept)Incidence.DeepCopy();
                    if(Management != null) dest.Management = (Hl7.Fhir.Model.CodeableConcept)Management.DeepCopy();
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new InteractionComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as InteractionComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(Interactant, otherT.Interactant)) return false;
                if( !DeepComparable.Matches(Type, otherT.Type)) return false;
                if( !DeepComparable.Matches(Effect, otherT.Effect)) return false;
                if( !DeepComparable.Matches(Incidence, otherT.Incidence)) return false;
                if( !DeepComparable.Matches(Management, otherT.Management)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as InteractionComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(Interactant, otherT.Interactant)) return false;
                if( !DeepComparable.IsExactly(Type, otherT.Type)) return false;
                if( !DeepComparable.IsExactly(Effect, otherT.Effect)) return false;
                if( !DeepComparable.IsExactly(Incidence, otherT.Incidence)) return false;
                if( !DeepComparable.IsExactly(Management, otherT.Management)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    foreach (var elem in Interactant) { if (elem != null) yield return elem; }
                    if (Type != null) yield return Type;
                    if (Effect != null) yield return Effect;
                    if (Incidence != null) yield return Incidence;
                    if (Management != null) yield return Management;
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    foreach (var elem in Interactant) { if (elem != null) yield return new ElementValue("interactant", elem); }
                    if (Type != null) yield return new ElementValue("type", Type);
                    if (Effect != null) yield return new ElementValue("effect", Effect);
                    if (Incidence != null) yield return new ElementValue("incidence", Incidence);
                    if (Management != null) yield return new ElementValue("management", Management);
                }
            }

            
        }
        
        
        [FhirType("InteractantComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class InteractantComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "InteractantComponent"; } }
            
            /// <summary>
            /// The specific medication, food or laboratory test that interacts
            /// </summary>
            [FhirElement("item", InSummary=true, Order=40, Choice=ChoiceType.DatatypeChoice)]
            [CLSCompliant(false)]
			[AllowedTypes(typeof(Hl7.Fhir.Model.ResourceReference),typeof(Hl7.Fhir.Model.CodeableConcept))]
            [Cardinality(Min=1,Max=1)]
            [DataMember]
            public Hl7.Fhir.Model.Element Item
            {
                get { return _Item; }
                set { _Item = value; OnPropertyChanged("Item"); }
            }
            
            private Hl7.Fhir.Model.Element _Item;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as InteractantComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(Item != null) dest.Item = (Hl7.Fhir.Model.Element)Item.DeepCopy();
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new InteractantComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as InteractantComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(Item, otherT.Item)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as InteractantComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(Item, otherT.Item)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (Item != null) yield return Item;
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (Item != null) yield return new ElementValue("item", Item);
                }
            }

            
        }
        
        
        [FhirType("UndesirableEffectComponent", NamedBackboneElement=true)]
        [DataContract]
        public partial class UndesirableEffectComponent : Hl7.Fhir.Model.BackboneElement, System.ComponentModel.INotifyPropertyChanged
        {
            [NotMapped]
            public override string TypeName { get { return "UndesirableEffectComponent"; } }
            
            /// <summary>
            /// The symptom, condition or undesirable effect
            /// </summary>
            [FhirElement("symptomConditionEffect", InSummary=true, Order=40)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept SymptomConditionEffect
            {
                get { return _SymptomConditionEffect; }
                set { _SymptomConditionEffect = value; OnPropertyChanged("SymptomConditionEffect"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _SymptomConditionEffect;
            
            /// <summary>
            /// Classification of the effect
            /// </summary>
            [FhirElement("classification", InSummary=true, Order=50)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept Classification
            {
                get { return _Classification; }
                set { _Classification = value; OnPropertyChanged("Classification"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _Classification;
            
            /// <summary>
            /// The frequency of occurrence of the effect
            /// </summary>
            [FhirElement("frequencyOfOccurrence", InSummary=true, Order=60)]
            [DataMember]
            public Hl7.Fhir.Model.CodeableConcept FrequencyOfOccurrence
            {
                get { return _FrequencyOfOccurrence; }
                set { _FrequencyOfOccurrence = value; OnPropertyChanged("FrequencyOfOccurrence"); }
            }
            
            private Hl7.Fhir.Model.CodeableConcept _FrequencyOfOccurrence;
            
            public override IDeepCopyable CopyTo(IDeepCopyable other)
            {
                var dest = other as UndesirableEffectComponent;
                
                if (dest != null)
                {
                    base.CopyTo(dest);
                    if(SymptomConditionEffect != null) dest.SymptomConditionEffect = (Hl7.Fhir.Model.CodeableConcept)SymptomConditionEffect.DeepCopy();
                    if(Classification != null) dest.Classification = (Hl7.Fhir.Model.CodeableConcept)Classification.DeepCopy();
                    if(FrequencyOfOccurrence != null) dest.FrequencyOfOccurrence = (Hl7.Fhir.Model.CodeableConcept)FrequencyOfOccurrence.DeepCopy();
                    return dest;
                }
                else
                	throw new ArgumentException("Can only copy to an object of the same type", "other");
            }
            
            public override IDeepCopyable DeepCopy()
            {
                return CopyTo(new UndesirableEffectComponent());
            }
            
            public override bool Matches(IDeepComparable other)
            {
                var otherT = other as UndesirableEffectComponent;
                if(otherT == null) return false;
                
                if(!base.Matches(otherT)) return false;
                if( !DeepComparable.Matches(SymptomConditionEffect, otherT.SymptomConditionEffect)) return false;
                if( !DeepComparable.Matches(Classification, otherT.Classification)) return false;
                if( !DeepComparable.Matches(FrequencyOfOccurrence, otherT.FrequencyOfOccurrence)) return false;
                
                return true;
            }
            
            public override bool IsExactly(IDeepComparable other)
            {
                var otherT = other as UndesirableEffectComponent;
                if(otherT == null) return false;
                
                if(!base.IsExactly(otherT)) return false;
                if( !DeepComparable.IsExactly(SymptomConditionEffect, otherT.SymptomConditionEffect)) return false;
                if( !DeepComparable.IsExactly(Classification, otherT.Classification)) return false;
                if( !DeepComparable.IsExactly(FrequencyOfOccurrence, otherT.FrequencyOfOccurrence)) return false;
                
                return true;
            }


            [NotMapped]
            public override IEnumerable<Base> Children
            {
                get
                {
                    foreach (var item in base.Children) yield return item;
                    if (SymptomConditionEffect != null) yield return SymptomConditionEffect;
                    if (Classification != null) yield return Classification;
                    if (FrequencyOfOccurrence != null) yield return FrequencyOfOccurrence;
                }
            }

            [NotMapped]
            internal override IEnumerable<ElementValue> NamedChildren
            {
                get
                {
                    foreach (var item in base.NamedChildren) yield return item;
                    if (SymptomConditionEffect != null) yield return new ElementValue("symptomConditionEffect", SymptomConditionEffect);
                    if (Classification != null) yield return new ElementValue("classification", Classification);
                    if (FrequencyOfOccurrence != null) yield return new ElementValue("frequencyOfOccurrence", FrequencyOfOccurrence);
                }
            }

            
        }
        
        
        /// <summary>
        /// indication | contraindication | interaction | undesirable-effect | other
        /// </summary>
        [FhirElement("type", InSummary=true, Order=90)]
        [Cardinality(Min=1,Max=1)]
        [DataMember]
        public Code<Hl7.Fhir.Model.ClinicalParticulars.ClinicalParticularsType> TypeElement
        {
            get { return _TypeElement; }
            set { _TypeElement = value; OnPropertyChanged("TypeElement"); }
        }
        
        private Code<Hl7.Fhir.Model.ClinicalParticulars.ClinicalParticularsType> _TypeElement;
        
        /// <summary>
        /// indication | contraindication | interaction | undesirable-effect | other
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public Hl7.Fhir.Model.ClinicalParticulars.ClinicalParticularsType? Type
        {
            get { return TypeElement != null ? TypeElement.Value : null; }
            set
            {
                if (!value.HasValue)
                  TypeElement = null; 
                else
                  TypeElement = new Code<Hl7.Fhir.Model.ClinicalParticulars.ClinicalParticularsType>(value);
                OnPropertyChanged("Type");
            }
        }
        
        /// <summary>
        /// The medication or procedure for which this is an indication
        /// </summary>
        [FhirElement("subject", InSummary=true, Order=100)]
        [CLSCompliant(false)]
		[References("RegulatedMedicinalProduct","Medication","ActivityDefinition","PlanDefinition","Device","DeviceDefinition","Substance")]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Hl7.Fhir.Model.ResourceReference> Subject
        {
            get { if(_Subject==null) _Subject = new List<Hl7.Fhir.Model.ResourceReference>(); return _Subject; }
            set { _Subject = value; OnPropertyChanged("Subject"); }
        }
        
        private List<Hl7.Fhir.Model.ResourceReference> _Subject;
        
        /// <summary>
        /// General description
        /// </summary>
        [FhirElement("description", InSummary=true, Order=110)]
        [DataMember]
        public Hl7.Fhir.Model.FhirString DescriptionElement
        {
            get { return _DescriptionElement; }
            set { _DescriptionElement = value; OnPropertyChanged("DescriptionElement"); }
        }
        
        private Hl7.Fhir.Model.FhirString _DescriptionElement;
        
        /// <summary>
        /// General description
        /// </summary>
        /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
        [NotMapped]
        [IgnoreDataMemberAttribute]
        public string Description
        {
            get { return DescriptionElement != null ? DescriptionElement.Value : null; }
            set
            {
                if (value == null)
                  DescriptionElement = null; 
                else
                  DescriptionElement = new Hl7.Fhir.Model.FhirString(value);
                OnPropertyChanged("Description");
            }
        }
        
        /// <summary>
        /// Specifics for when this is a contraindication
        /// </summary>
        [FhirElement("contraindication", InSummary=true, Order=120)]
        [DataMember]
        public Hl7.Fhir.Model.ClinicalParticulars.ContraindicationComponent Contraindication
        {
            get { return _Contraindication; }
            set { _Contraindication = value; OnPropertyChanged("Contraindication"); }
        }
        
        private Hl7.Fhir.Model.ClinicalParticulars.ContraindicationComponent _Contraindication;
        
        /// <summary>
        /// Specifics for when this is an indication
        /// </summary>
        [FhirElement("indication", InSummary=true, Order=130)]
        [DataMember]
        public Hl7.Fhir.Model.ClinicalParticulars.IndicationComponent Indication
        {
            get { return _Indication; }
            set { _Indication = value; OnPropertyChanged("Indication"); }
        }
        
        private Hl7.Fhir.Model.ClinicalParticulars.IndicationComponent _Indication;
        
        /// <summary>
        /// Specifics for when this is an interaction
        /// </summary>
        [FhirElement("interaction", InSummary=true, Order=140)]
        [DataMember]
        public Hl7.Fhir.Model.ClinicalParticulars.InteractionComponent Interaction
        {
            get { return _Interaction; }
            set { _Interaction = value; OnPropertyChanged("Interaction"); }
        }
        
        private Hl7.Fhir.Model.ClinicalParticulars.InteractionComponent _Interaction;
        
        /// <summary>
        /// The population group to which this applies
        /// </summary>
        [FhirElement("population", InSummary=true, Order=150)]
        [Cardinality(Min=0,Max=-1)]
        [DataMember]
        public List<Population> Population
        {
            get { if(_Population==null) _Population = new List<Population>(); return _Population; }
            set { _Population = value; OnPropertyChanged("Population"); }
        }
        
        private List<Population> _Population;
        
        /// <summary>
        /// UndesirableEffect
        /// </summary>
        [FhirElement("undesirableEffect", InSummary=true, Order=160)]
        [DataMember]
        public Hl7.Fhir.Model.ClinicalParticulars.UndesirableEffectComponent UndesirableEffect
        {
            get { return _UndesirableEffect; }
            set { _UndesirableEffect = value; OnPropertyChanged("UndesirableEffect"); }
        }
        
        private Hl7.Fhir.Model.ClinicalParticulars.UndesirableEffectComponent _UndesirableEffect;
        

        public override void AddDefaultConstraints()
        {
            base.AddDefaultConstraints();

        }

        public override IDeepCopyable CopyTo(IDeepCopyable other)
        {
            var dest = other as ClinicalParticulars;
            
            if (dest != null)
            {
                base.CopyTo(dest);
                if(TypeElement != null) dest.TypeElement = (Code<Hl7.Fhir.Model.ClinicalParticulars.ClinicalParticularsType>)TypeElement.DeepCopy();
                if(Subject != null) dest.Subject = new List<Hl7.Fhir.Model.ResourceReference>(Subject.DeepCopy());
                if(DescriptionElement != null) dest.DescriptionElement = (Hl7.Fhir.Model.FhirString)DescriptionElement.DeepCopy();
                if(Contraindication != null) dest.Contraindication = (Hl7.Fhir.Model.ClinicalParticulars.ContraindicationComponent)Contraindication.DeepCopy();
                if(Indication != null) dest.Indication = (Hl7.Fhir.Model.ClinicalParticulars.IndicationComponent)Indication.DeepCopy();
                if(Interaction != null) dest.Interaction = (Hl7.Fhir.Model.ClinicalParticulars.InteractionComponent)Interaction.DeepCopy();
                if(Population != null) dest.Population = new List<Population>(Population.DeepCopy());
                if(UndesirableEffect != null) dest.UndesirableEffect = (Hl7.Fhir.Model.ClinicalParticulars.UndesirableEffectComponent)UndesirableEffect.DeepCopy();
                return dest;
            }
            else
            	throw new ArgumentException("Can only copy to an object of the same type", "other");
        }
        
        public override IDeepCopyable DeepCopy()
        {
            return CopyTo(new ClinicalParticulars());
        }
        
        public override bool Matches(IDeepComparable other)
        {
            var otherT = other as ClinicalParticulars;
            if(otherT == null) return false;
            
            if(!base.Matches(otherT)) return false;
            if( !DeepComparable.Matches(TypeElement, otherT.TypeElement)) return false;
            if( !DeepComparable.Matches(Subject, otherT.Subject)) return false;
            if( !DeepComparable.Matches(DescriptionElement, otherT.DescriptionElement)) return false;
            if( !DeepComparable.Matches(Contraindication, otherT.Contraindication)) return false;
            if( !DeepComparable.Matches(Indication, otherT.Indication)) return false;
            if( !DeepComparable.Matches(Interaction, otherT.Interaction)) return false;
            if( !DeepComparable.Matches(Population, otherT.Population)) return false;
            if( !DeepComparable.Matches(UndesirableEffect, otherT.UndesirableEffect)) return false;
            
            return true;
        }
        
        public override bool IsExactly(IDeepComparable other)
        {
            var otherT = other as ClinicalParticulars;
            if(otherT == null) return false;
            
            if(!base.IsExactly(otherT)) return false;
            if( !DeepComparable.IsExactly(TypeElement, otherT.TypeElement)) return false;
            if( !DeepComparable.IsExactly(Subject, otherT.Subject)) return false;
            if( !DeepComparable.IsExactly(DescriptionElement, otherT.DescriptionElement)) return false;
            if( !DeepComparable.IsExactly(Contraindication, otherT.Contraindication)) return false;
            if( !DeepComparable.IsExactly(Indication, otherT.Indication)) return false;
            if( !DeepComparable.IsExactly(Interaction, otherT.Interaction)) return false;
            if( !DeepComparable.IsExactly(Population, otherT.Population)) return false;
            if( !DeepComparable.IsExactly(UndesirableEffect, otherT.UndesirableEffect)) return false;
            
            return true;
        }

        [NotMapped]
        public override IEnumerable<Base> Children
        {
            get
            {
                foreach (var item in base.Children) yield return item;
				if (TypeElement != null) yield return TypeElement;
				foreach (var elem in Subject) { if (elem != null) yield return elem; }
				if (DescriptionElement != null) yield return DescriptionElement;
				if (Contraindication != null) yield return Contraindication;
				if (Indication != null) yield return Indication;
				if (Interaction != null) yield return Interaction;
				foreach (var elem in Population) { if (elem != null) yield return elem; }
				if (UndesirableEffect != null) yield return UndesirableEffect;
            }
        }

        [NotMapped]
        internal override IEnumerable<ElementValue> NamedChildren
        {
            get
            {
                foreach (var item in base.NamedChildren) yield return item;
                if (TypeElement != null) yield return new ElementValue("type", TypeElement);
                foreach (var elem in Subject) { if (elem != null) yield return new ElementValue("subject", elem); }
                if (DescriptionElement != null) yield return new ElementValue("description", DescriptionElement);
                if (Contraindication != null) yield return new ElementValue("contraindication", Contraindication);
                if (Indication != null) yield return new ElementValue("indication", Indication);
                if (Interaction != null) yield return new ElementValue("interaction", Interaction);
                foreach (var elem in Population) { if (elem != null) yield return new ElementValue("population", elem); }
                if (UndesirableEffect != null) yield return new ElementValue("undesirableEffect", UndesirableEffect);
            }
        }

    }
    
}
