using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmarterBalanced.SampleItems.Dal.Xml.Models
{
    /// <summary>
    /// Represents the item element in an item-BANK-KEY.xml file
    /// </summary>
    public class ItemXmlFieldRepresentation
    {
        [XmlAttribute("bankkey")]
        public int ItemBank;

        [XmlAttribute("id")]
        public int ItemKey { get; set; }

        [XmlAttribute("type")]
        public string ItemType { get; set; }

        [XmlElement("content")]
        public List<Content> Contents { get; set; }
        
        [XmlElement("MachineRubric")]
        public MachineRubric MachineRubric { get; set; }

        [XmlArray("attriblist")]
        [XmlArrayItem("attrib")]
        public List<ItemMetadataAttribute> ItemMetadataAttributes { get; set; }
    }
}
