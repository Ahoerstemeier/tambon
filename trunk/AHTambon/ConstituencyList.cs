using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace De.AHoerstemeier.Tambon
{
    public class ConstituencyList:List<ConstituencyEntry>
    {
        public Int32 Population()
        {
            Int32 lResult = 0;
            foreach (ConstituencyEntry lEntry in this)
            {
                lResult += lEntry.Population();
            }
            return lResult;
        }
        public Int32 NumberOfSeats()
        {
            Int32 lResult = 0;
            foreach (ConstituencyEntry lEntry in this)
            {
                lResult += lEntry.NumberOfSeats;
            }
            return lResult;
        }

        internal void ReadFromXml(System.Xml.XmlNode iNode)
        {
            if ( iNode != null && iNode.Name.Equals("constituencies") )
            {
                if ( iNode.HasChildNodes )
                {
                    foreach ( XmlNode lChildNode in iNode.ChildNodes )
                    {
                        if ( lChildNode.Name == "constituency" )
                        {
                            ConstituencyEntry lConstituency = ConstituencyEntry.Load(lChildNode);
                            Add(lConstituency);
                        }
                    }
                }
            }
        }
    }
}
