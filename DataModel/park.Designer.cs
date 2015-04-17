// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.3.29394
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hoerstemeier.com/tambon/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://hoerstemeier.com/tambon/", IsNullable=true)]
    [System.Runtime.Serialization.DataContractAttribute(Name="AreaPark", Namespace="http://hoerstemeier.com/tambon/", IsReference=true)]
    public partial class AreaPark {
        
        private string commentField;
        
        private double squarekilometerField;
        
        private bool squarekilometerFieldSpecified;
        
        private double raiField;
        
        private bool raiFieldSpecified;
        
        private int ngamField;
        
        private bool ngamFieldSpecified;
        
        private double tarangwaField;
        
        private bool tarangwaFieldSpecified;
        
        private string yearField;
        
        private bool obsoleteField;
        
        private List<uint> locationgeocodeField;
        
        /// <summary>
        /// Creates a new instance of AreaPark.
        /// </summary>
        public AreaPark() {
            this.locationgeocodeField = new List<uint>();
            this.obsoleteField = false;
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string comment {
            get {
                return this.commentField;
            }
            set {
                this.commentField = value;
            }
        }
        
        /// <summary>
        /// Area in square kilometer (km²).
        /// </summary>
        /// <value>
        /// The squarekilometer.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double squarekilometer {
            get {
                return this.squarekilometerField;
            }
            set {
                this.squarekilometerField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool squarekilometerSpecified {
            get {
                return this.squarekilometerFieldSpecified;
            }
            set {
                this.squarekilometerFieldSpecified = value;
            }
        }
        
        /// <summary>
        /// Area in rai (ไร่, 1600 m²). Only integer part if ngam and tarangwa are defined.
        /// </summary>
        /// <value>
        /// The rai.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double rai {
            get {
                return this.raiField;
            }
            set {
                this.raiField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool raiSpecified {
            get {
                return this.raiFieldSpecified;
            }
            set {
                this.raiFieldSpecified = value;
            }
        }
        
        /// <summary>
        /// Area part in ngam (งาน, 400 m²). Only if value in rai is integer.
        /// </summary>
        /// <value>
        /// The ngam.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ngam {
            get {
                return this.ngamField;
            }
            set {
                this.ngamField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ngamSpecified {
            get {
                return this.ngamFieldSpecified;
            }
            set {
                this.ngamFieldSpecified = value;
            }
        }
        
        /// <summary>
        /// Area part in tarangwa (ตารางวา, 4 m²). Only if value in rai is integer.
        /// </summary>
        /// <value>
        /// The tarangwa.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double tarangwa {
            get {
                return this.tarangwaField;
            }
            set {
                this.tarangwaField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool tarangwaSpecified {
            get {
                return this.tarangwaFieldSpecified;
            }
            set {
                this.tarangwaFieldSpecified = value;
            }
        }
        
        /// <summary>
        /// Year in which the area value was valid.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="gYear")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string year {
            get {
                return this.yearField;
            }
            set {
                this.yearField = value;
            }
        }
        
        /// <summary>
        /// Marking area values which are no longer valid.
        /// </summary>
        /// <value>
        /// The obsolete.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool obsolete {
            get {
                return this.obsoleteField;
            }
            set {
                this.obsoleteField = value;
            }
        }
        
        /// <summary>
        /// Geocodes of the entites covered by the protected area.
        /// </summary>
        /// <value>
        /// The locationgeocode.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<uint> locationgeocode {
            get {
                return this.locationgeocodeField;
            }
            set {
                this.locationgeocodeField = value;
            }
        }
    }
    
    /// <summary>
    /// Protected area.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://hoerstemeier.com/tambon/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://hoerstemeier.com/tambon/", IsNullable=true)]
    [System.Runtime.Serialization.DataContractAttribute(Name="Park", Namespace="http://hoerstemeier.com/tambon/", IsReference=true)]
    public partial class Park {
        
        private WikiLocation wikiField;
        
        private Point pointField;
        
        private List<AreaPark> areaField;
        
        private HistoryList historyField;
        
        private ParkType typeField;
        
        private string idField;
        
        private string nameField;
        
        private string englishField;
        
        private bool obsoleteField;
        
        private bool obsoleteFieldSpecified;
        
        private string commentField;
        
        /// <summary>
        /// Creates a new instance of Park.
        /// </summary>
        public Park() {
            this.historyField = new HistoryList();
            this.areaField = new List<AreaPark>();
        }
        
        /// <summary>
        /// Symbols, slogan and vision statements of the office.
        /// </summary>
        /// <value>
        /// The wiki.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WikiLocation wiki {
            get {
                return this.wikiField;
            }
            set {
                this.wikiField = value;
            }
        }
        
        /// <summary>
        /// Location of park, pointing approximately at the main entrance.
        /// </summary>
        /// <value>
        /// The Point.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.w3.org/2003/01/geo/wgs84_pos#", Order=1)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Point Point {
            get {
                return this.pointField;
            }
            set {
                this.pointField = value;
            }
        }
        
        /// <summary>
        /// Area covered by the protected area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute("area", Order=2)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<AreaPark> area {
            get {
                return this.areaField;
            }
            set {
                this.areaField = value;
            }
        }
        
        /// <summary>
        /// List of events concerning the park.
        /// </summary>
        /// <value>
        /// The history.
        /// </value>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public HistoryList history {
            get {
                return this.historyField;
            }
            set {
                this.historyField = value;
            }
        }
        
        /// <summary>
        /// Type of the protected area.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ParkType type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <summary>
        /// Id of the park, unique within the type.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
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
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
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
        
        /// <summary>
        /// Protected area no longer existing/protected.
        /// </summary>
        /// <value>
        /// The obsolete.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool obsolete {
            get {
                return this.obsoleteField;
            }
            set {
                this.obsoleteField = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool obsoleteSpecified {
            get {
                return this.obsoleteFieldSpecified;
            }
            set {
                this.obsoleteFieldSpecified = value;
            }
        }
        
        /// <summary>
        /// Auto generated comment tag to suppress XML code documentation warning.
        /// </summary>
        /// <value>
        /// Auto generated value tag to suppress XML code documentation warning.
        /// </value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string comment {
            get {
                return this.commentField;
            }
            set {
                this.commentField = value;
            }
        }
    }
}
