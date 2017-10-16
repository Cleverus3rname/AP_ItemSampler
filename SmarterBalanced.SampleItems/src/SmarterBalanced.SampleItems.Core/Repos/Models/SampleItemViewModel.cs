using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.Repos.Models
{
    public class SampleItemViewModel
    {
        public int BankKey { get; }
        public int ItemKey { get; }
        public string Title { get; }
        public string GradeLabel { get; }
        public string SubjectCode { get; }
        public string SubjectLabel { get; }
        public string ClaimCode { get; }
        public string ClaimLabel { get; }
        public string Target { get; }
        public string InteractionTypeCode { get; }
        public string InteractionTypeLabel { get; }
        public bool IsPerformanceItem { get; set; }
        public bool AslSupported { get; }
        public int? StimulusKey { get; }
        public string TargetDescription { get; }
        public string CcssDescription { get; }
        public string DepthOfKnowledge { get; }
        public string Exemplar { get; }
        public string Rubric { get; }
        public string Url { get; }

        public SampleItemViewModel(
        int bankKey,
        int itemKey,
        string title,
        string gradeLabel,
        string subjectCode,
        string subjectLabel,
        string claimCode,
        string claimLabel,
        string target,
        string interactionTypeCode,
        string interactionTypeLabel,
        bool isPerformanceItem,
        bool aslSupported,
        int? stimulusKey,
        string targetDescription,
        string ccssDescription,
        string url,
        string depthOfKnowledge,
        string exemplar,
        string rubric)
        {
            BankKey = bankKey;
            ItemKey = itemKey;
            Title = title;
            GradeLabel = gradeLabel;
            SubjectCode = subjectCode;
            SubjectLabel = subjectLabel;
            ClaimCode = claimCode;
            ClaimLabel = claimLabel;
            Target = target;
            InteractionTypeCode = interactionTypeCode;
            InteractionTypeLabel = interactionTypeLabel;
            IsPerformanceItem = isPerformanceItem;
            AslSupported = aslSupported;
            StimulusKey = stimulusKey;
            TargetDescription = targetDescription;
            CcssDescription = ccssDescription;
            DepthOfKnowledge = depthOfKnowledge;
            Url = url;
            Rubric = rubric;
            Exemplar = exemplar;
        }

        /// <summary>
        /// Used for testing so that it's not necessary to specify all parameters.
        /// </summary>
        public static SampleItemViewModel Create(
           int bankKey = -1,
           int itemKey = -1,
           string title = "",
           string gradeLabel = "",
           string subjectCode = "",
           string subjectLabel = "",
           string claimCode = "",
           string claimLabel = "",
           string target = "",
           string interactionTypeCode = "",
           string interactionTypeLabel = "",
           bool isPerformanceItem = false,
           bool aslSupported = false,
           int? stimulusKey = null,
           string targetDesc = "",
           string ccssDesc = "",
           string depthOfKnowledge = "",
           string url = "",
           string exemplar = "",
           string rubric = "")
        {
            return new SampleItemViewModel(
                bankKey: bankKey,
                itemKey: itemKey,
                title: title,
                gradeLabel: gradeLabel,
                subjectCode: subjectCode,
                subjectLabel: subjectLabel,
                claimCode: claimCode,
                claimLabel: claimLabel,
                target: target,
                interactionTypeCode: interactionTypeCode,
                interactionTypeLabel: interactionTypeLabel,
                isPerformanceItem: isPerformanceItem,
                aslSupported: aslSupported,
                stimulusKey: stimulusKey,
                ccssDescription: ccssDesc,
                targetDescription: targetDesc,
                url: url,
                depthOfKnowledge: depthOfKnowledge,
                exemplar: exemplar,
                rubric: rubric);
        }

    }
}