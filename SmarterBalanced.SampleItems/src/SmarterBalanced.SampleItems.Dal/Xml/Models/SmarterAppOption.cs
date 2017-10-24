using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmarterBalanced.SampleItems.Dal.Xml.Models
{
    public class SmarterAppOption
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("val")]
        public string Value { get; set; }
        [XmlElement("feedback")]
        public string Feedback { get; set; }
        public string Language { get; set; }
        public bool Answer { get; set; }

        public SmarterAppOption WithOptions(bool answer, string lang)
        {
            var option = new SmarterAppOption
            {
                Name = this.Name,
                Value = this.Value,
                Feedback = this.Feedback,
                Answer = answer,
                Language = lang
            };

            return option;
        }
    }
}
