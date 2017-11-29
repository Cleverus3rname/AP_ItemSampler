using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public sealed class Subject
    {
        public string Code { get; }
        public string Label { get; }
        public string ShortLabel { get; }

        [JsonIgnore]
        public ImmutableArray<Claim> Claims { get; }
        public ImmutableArray<string> InteractionTypeCodes { get; }
        public ImmutableArray<string> ClaimCodes { get; }

        public Subject(
            string code,
            string label,
            string shortLabel,
            ImmutableArray<Claim> claims,
            ImmutableArray<string> interactionTypeCodes,
            ImmutableArray<string> claimCodes)
        {
            Code = code;
            Label = label;
            ShortLabel = shortLabel;
            Claims = claims;
            InteractionTypeCodes = interactionTypeCodes;
            ClaimCodes = claimCodes;
        }

        public static Subject Create(
            string code,
            string label = "",
            string shortLabel = "",
            ImmutableArray<Claim> claims = default(ImmutableArray<Claim>),
            ImmutableArray<string> interactionTypeCodes = default(ImmutableArray<string>))
        {
            claims = claims.IsDefault ? ImmutableArray<Claim>.Empty : claims;
            interactionTypeCodes = interactionTypeCodes.IsDefault ? ImmutableArray<string>.Empty : interactionTypeCodes;

            var claimCodes = claims.Select(c => c.Code).ToImmutableArray();
            return new Subject(
                code,
                label,
                shortLabel,
                claims,
                interactionTypeCodes,
                claimCodes: claimCodes
            );
        }

        public static Subject Create(XElement subjectElement, ImmutableArray<InteractionFamily> interactionFamilies)
        {
            var code = (string)subjectElement.Element("Code");
            var family = interactionFamilies.Single(f => f.SubjectCode == code);
            var claims = subjectElement.Elements("Claim").Select(c => Claim.Create(c)).ToImmutableArray();

            var subject = Create(
                code: code,
                label: (string)subjectElement.Element("Label"),
                shortLabel: (string)subjectElement.Element("ShortLabel"),
                claims: claims,
                interactionTypeCodes: family.InteractionTypeCodes);

            return subject;
        }


        public Subject WithClaimTargets(IList<Target> allTargets)
        {
            var newClaims = Claims
                .Select(c => c.WithTargets(allTargets
                    .Where(t => t.ClaimId == c.ClaimNumber && t.Subject == Code)
                    .ToList()))
                .ToImmutableArray();

            return Create(
                code: Code,
                label: Label,
                shortLabel: ShortLabel,
                claims: newClaims,
                interactionTypeCodes: InteractionTypeCodes);
        }
    }
}
