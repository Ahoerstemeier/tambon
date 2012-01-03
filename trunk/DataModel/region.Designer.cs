// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.1.23578
//    <NameSpace>De.AHoerstemeier.Tambon</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>True</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>True</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace De.AHoerstemeier.Tambon {
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hoerstemeier.com/tambon/")]
    [System.Xml.Serialization.XmlRootAttribute("regionlist", Namespace="http://hoerstemeier.com/tambon/", IsNullable=false)]
    [System.Runtime.Serialization.DataContractAttribute(Name="RegionList", Namespace="http://hoerstemeier.com/tambon/", IsReference=true)]
    public partial class RegionList {
        
        private List<RegionListEntry> regionsField;
        
        /// <summary>
        /// Creates a new instance of RegionList.
        /// </summary>
        public RegionList() {
            this.regionsField = new List<RegionListEntry>();
        }
        
        [System.Xml.Serialization.XmlElementAttribute("regions", Order=0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<RegionListEntry> regions {
            get {
                return this.regionsField;
            }
            set {
                this.regionsField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hoerstemeier.com/tambon/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://hoerstemeier.com/tambon/", IsNullable=true)]
    [System.Runtime.Serialization.DataContractAttribute(Name="RegionListEntry", Namespace="http://hoerstemeier.com/tambon/", IsReference=true)]
    public partial class RegionListEntry {
        
        private List<RegionEntity> entityField;
        
        private string nameField;
        
        private string englishField;
        
        /// <summary>
        /// Creates a new instance of RegionListEntry.
        /// </summary>
        public RegionListEntry() {
            this.entityField = new List<RegionEntity>();
        }
        
        [System.Xml.Serialization.XmlElementAttribute("entity", Order=0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<RegionEntity> entity {
            get {
                return this.entityField;
            }
            set {
                this.entityField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string english {
            get {
                return this.englishField;
            }
            set {
                this.englishField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hoerstemeier.com/tambon/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://hoerstemeier.com/tambon/", IsNullable=true)]
    [System.Runtime.Serialization.DataContractAttribute(Name="RegionEntity", Namespace="http://hoerstemeier.com/tambon/", IsReference=true)]
    public partial class RegionEntity : EntityBase {
        
        private List<RegionEntity> entityField;
        
        private uint indexField;
        
        /// <summary>
        /// Creates a new instance of RegionEntity.
        /// </summary>
        public RegionEntity() {
            this.entityField = new List<RegionEntity>();
            this.indexField = ((uint)(0));
        }
        
        [System.Xml.Serialization.XmlElementAttribute("entity", Order=0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<RegionEntity> entity {
            get {
                return this.entityField;
            }
            set {
                this.entityField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "0")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint index {
            get {
                return this.indexField;
            }
            set {
                this.indexField = value;
            }
        }
    }
}
