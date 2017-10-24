using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmarterBalanced.SampleItems.Dal.Xml.Models
{
    /// <summary>
    /// Represents the smarterAppMetadata element in a metadata.xml file
    /// </summary>
    public class SmarterAppMetadataXmlRepresentation
    {
        [XmlElement("Identifier")]
        public int ItemKey { get; set; }

        [XmlElement("AssociatedStimulus")]
        public int? AssociatedStimulus { get; set; }

        [XmlElement("AssociatedTutorial")]
        public int? AssociatedTutorial { get; set; }

        [XmlElement("AssociatedWordlist")]
        public int? AssociatedWordList { get; set; }

        [XmlElement("Subject")]
        public string SubjectCode { get; set; }

        [XmlElement("IntendedGrade")]
        public string GradeCode { get; set; }

        [XmlElement("SufficientEvidenceOfClaim")]
        public string SufficientEvidenceOfClaim { get; set; }

        [XmlElement("TargetAssessmentType")]
        public string TargetAssessmentType { get; set; }

        [XmlElement("InteractionType")]
        public string InteractionType { get; set; }

        [XmlElement("StandardPublication")]
        public List<StandardPublication> StandardPublications { get; set; }

        [XmlElement("AccessibilityTagsASLLanguage")]
        public string AccessibilityTagsASLLanguage { get; set; }

        [XmlElement("AllowCalculator")]
        public string AllowCalculator { get; set; }

        [XmlElement("DepthOfKnowledge")]
        public string DepthOfKnowledge { get; set; }

        [XmlElement("MaximumNumberOfPoints")]
        public int? MaximumNumberOfPoints { get; set; }

        [XmlElement("SmarterAppItemDescriptor")]
        public string SmarterAppItemDescriptor { get; set; }

        [XmlElement("EducationalDifficulty")]
        public string EducationalDifficulty { get; set; }

        [XmlElement("EvidenceStatement")]
        public string EvidenceStatement { get; set; }
    }
}
