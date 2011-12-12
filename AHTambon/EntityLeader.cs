using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class EntityLeader : ICloneable
    {
        #region properties
        private String _name = String.Empty;
        public String Name { get { return _name; } set { SetName(value); } }
        public String English { get; set; }
        private PersonTitle _title = PersonTitle.Unknown;
        public PersonTitle Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public String Telephone { get; set; }
        public String CellPhone { get; set; }
        public String Comment { get; set; }
        private EntityLeaderType _position = EntityLeaderType.Unknown;
        public EntityLeaderType Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Int32 Index { get; set; }
        public DateTime BeginOfTerm { get; set; }
        public Int32 BeginOfTermYear { get; set; }
        public DateTime EndOfTerm { get; set; }
        public Int32 EndOfTermYear { get; set; }
        #endregion

        #region constructor
        public EntityLeader()
        {
        }
        public EntityLeader(EntityLeader value)
        {
            Name = value.Name;
            English = value.English;
            Title = value.Title;
            Telephone = value.Telephone;
            CellPhone = value.CellPhone;
            Position = value.Position;
            Index = value.Index;
            BeginOfTerm = value.BeginOfTerm;
            BeginOfTermYear = value.BeginOfTermYear;
            EndOfTerm = value.EndOfTerm;
            EndOfTermYear = value.EndOfTermYear;
        }
        #endregion

        #region methods
        private void SetName(String name)
        {
            _name = name;
            foreach ( KeyValuePair<String, PersonTitle> entry in TambonHelper.PersonTitleStrings )
            {
                String search = entry.Key;
                if ( name.StartsWith(search) )
                {
                    Title = entry.Value;
                    _name = name.Remove(0, search.Length).Trim();
                }
            }
            // TODO Strip persontitle and store it separately in Title property
        }
        public void ExportToXML(XmlElement node)
        {
            XmlDocument xmlDocument = TambonHelper.XmlDocumentFromNode(node);
            var newElement = (XmlElement)xmlDocument.CreateNode("element", "official", "");
            newElement.SetAttribute("title", Position.ToString());
            if ( Index > 0 )
            {
                newElement.SetAttribute("index", Index.ToString());
            }
            newElement.SetAttribute("name", Name);
            if ( Title != PersonTitle.Unknown )
            {
                newElement.SetAttribute("nametitle", Title.ToString());
            }
            if ( !String.IsNullOrEmpty(English) )
            {
                newElement.SetAttribute("english", English);
            }
            if ( !String.IsNullOrEmpty(Telephone) )
            {
                newElement.SetAttribute("telephone", Telephone);
            }
            if ( !String.IsNullOrEmpty(CellPhone) )
            {
                newElement.SetAttribute("cellphone", CellPhone);
            }
            if ( (BeginOfTerm != null) && (BeginOfTerm.Year > 1) )
            {
                newElement.SetAttribute("begin", BeginOfTerm.ToString("yyyy-MM-dd", TambonHelper.CultureInfoUS));
            }
            if ( BeginOfTermYear > 0 )
            {
                newElement.SetAttribute("beginyear", BeginOfTermYear.ToString());
            }
            if ( (EndOfTerm != null) && (EndOfTerm.Year > 1) )
            {
                newElement.SetAttribute("begin", EndOfTerm.ToString("yyyy-MM-dd", TambonHelper.CultureInfoUS));
            }
            if ( EndOfTermYear > 0 )
            {
                newElement.SetAttribute("endyear", EndOfTermYear.ToString());
            }
            if ( !String.IsNullOrEmpty(Comment) )
            {
                newElement.SetAttribute("comment", Comment);
            }
            node.AppendChild(newElement);
        }
        internal static EntityLeader Load(XmlNode node)
        {
            EntityLeader result = null;

            if ( node != null && node.Name.Equals("official") )
            {
                result = new EntityLeader();
                result.Name = TambonHelper.GetAttribute(node, "name");
                result.English = TambonHelper.GetAttributeOptionalString(node, "english");
                result.Telephone = TambonHelper.GetAttributeOptionalString(node, "telephone");
                result.CellPhone = TambonHelper.GetAttributeOptionalString(node, "cellphone");
                result.Comment = TambonHelper.GetAttributeOptionalString(node, "comment");

                result.BeginOfTermYear = TambonHelper.GetAttributeOptionalInt(node, "beginyear", 0);
                result.EndOfTermYear = TambonHelper.GetAttributeOptionalInt(node, "endyear", 0);
                result.Index = TambonHelper.GetAttributeOptionalInt(node, "index", 0);
                result.BeginOfTerm = TambonHelper.GetAttributeOptionalDateTime(node, "begin");
                result.EndOfTerm = TambonHelper.GetAttributeOptionalDateTime(node, "end");

                String position = TambonHelper.GetAttribute(node, "title");
                result.Position = (EntityLeaderType)Enum.Parse(typeof(EntityLeaderType), position);

                String personTitle = TambonHelper.GetAttributeOptionalString(node, "nametitle");
                if ( !String.IsNullOrEmpty(personTitle) )
                {
                    result.Title = (PersonTitle)Enum.Parse(typeof(PersonTitle), position);
                }
            }
            return result;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new EntityLeader(this);
        }

        #endregion
    }
}
