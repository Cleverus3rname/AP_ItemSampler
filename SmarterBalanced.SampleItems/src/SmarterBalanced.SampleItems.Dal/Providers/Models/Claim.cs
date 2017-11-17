using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public sealed class Claim
    {
        /// <summary>
        /// A claim identifier within a subject. e.g. ELA2
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// A claim identifier shared across subjects. e.g. Claim2
        /// </summary>
        public string ClaimNumber { get; }

        /// <summary>
        /// The user facing name of the claim. e.g. Problem Solving and Modeling
        /// </summary>
        public string Label { get; }

        [JsonIgnore]
        public ImmutableArray<Target> Targets { get; }

        public ImmutableArray<int> TargetCodes { get; }

        public Claim(string code, string claimNumber, string label, ImmutableArray<Target> targets)
        {
            Code = code;
            ClaimNumber = claimNumber;
            Label = label;
            Targets = targets;
            TargetCodes = targets.Select(t => t.NameHash).ToImmutableArray();
        }

        public static Claim Create(
            ImmutableArray<Target> targets,
            string code = "", 
            string claimNumber = "", 
            string label = "")
        {
            if (targets == null)
            {
                targets = ImmutableArray.Create<Target>();
            }
            return new Claim(
                 code: code,
                 claimNumber: claimNumber,
                 label: label,
                 targets: targets);
        }

        public static Claim Create(XElement element)
        {
            var claim = new Claim(
                code: (string)element.Element("Code"),
                label: (string)element.Element("Label"),
                claimNumber: (string)element.Element("ClaimNumber"),
                targets: ImmutableArray.Create<Target>());

            return claim;
        }

        public Claim WithTargets(IList<Target> targets)
        {
            return new Claim(
                code: Code,
                claimNumber: ClaimNumber,
                label: Label,
                targets: targets.ToImmutableArray());
        }
    }
}
