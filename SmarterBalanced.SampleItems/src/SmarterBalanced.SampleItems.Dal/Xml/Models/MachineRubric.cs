using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmarterBalanced.SampleItems.Dal.Xml.Models
{
    public class MachineRubric
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }
    }
}
