using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmarterBalanced.SampleItems.Dal.Xml.Models
{
    public class Content
    {
        [XmlAttribute("language")]
        public string Language { get; set; }

        [XmlElement("rubriclist")]
        public RubricList RubricList { get; set; }

        [XmlArray("attachmentlist")]
        [XmlArrayItem("attachment")]
        public List<Attachment> Attachments { get; set; }

        [XmlArray("optionlist")]
        [XmlArrayItem("option")]
        public List<SmarterAppOption> ScoringOptions { get; set; }
    }
}
