using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class ItemDigest
    {
        public int BankKey { get; set; }
        public int ItemKey { get; set; }
        public string ItemType { get; set; }
        public string TargetAssessmentType { get; set; }
        public string SufficentEvidenceOfClaim { get; set; }
        public int? AssociatedStimulus { get; set; }
        public int? AssociatedWordlist { get; set; }
        public int? AssociatedTutorial { get; set; }
        public bool? AslSupported { get; set; }
        public bool AllowCalculator { get; set; }
        public string DepthOfKnowledge { get; set; }
        public string SubjectCode { get; set; }
        public string InteractionTypeCode { get; set; }
        public string GradeCode { get; set; }
        public int?  MaximumNumberOfPoints { get; set; }
        public List<StandardPublication> StandardPublications { get; set; }
        public List<Content> Contents { get; set; }
        public List<ItemMetadataAttribute> ItemMetadataAttributes { get; set; }
        public StimulusDigest StimulusDigest { get; set; }
        public string SmarterAppItemDescriptor { get; set; }
        public string EducationalDifficulty { get; set; }
        public string EvidenceStatement { get; set; }
        public bool HasMachineRubric { get; set; }
        public override string ToString()
        {
            return $"{BankKey}-{ItemKey}";
        }

    }
}
