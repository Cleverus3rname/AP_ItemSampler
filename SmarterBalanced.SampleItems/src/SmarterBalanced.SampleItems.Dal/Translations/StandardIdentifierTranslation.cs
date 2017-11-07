using SmarterBalanced.SampleItems.Dal.Exceptions;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public static class StandardIdentifierTranslation
    {
        /* 
         * Locate and parse the standard, claim, and target from the metadata
         * 
         * Claim and target are specified in one of the following formats:
         * SBAC-ELA-v1 (there is only one alignment for ELA, this is used for delivery)
         *     Claim|Assessment Target|Common Core Standard
         * SBAC-MA-v6 (Math, based on the blueprint hierarchy, primary alignment and does not go to standard level, THIS IS USED FOR DELIVERY, should be the same as SBAC-MA-v4)
         *     Claim|Content Category|Target Set|Assessment Target
         * SBAC-MA-v5 (Math, based on the content specifications hierarchy secondary alignment to the standard level)
         *     Claim|Content Domain|Target|Emphasis|Common Core Standard
         * SBAC-MA-v4 (Math, based on the content specifications hierarchy primary alignment to the standard level)
         *     Claim|Content Domain|Target|Emphasis|Common Core Standard
         */

        public static StandardIdentifier StandardStringToStandardIdentifier(string standards)
        {

            if (string.IsNullOrEmpty(standards))
            {
                // TODO: log error
                return null;
            }

            string[] publicationAndKey = standards.Split(':');

            if (publicationAndKey.Length < 2)
            {
                return null;

                // TODO: log error instead
                //throw new ArgumentException("The standards string does not contain a publication and claim spearated by a colon.");
            }

            StandardIdentifier standardIdentifier = null;

            string publication = publicationAndKey[0];
            string[] parts = publicationAndKey[1].Split('|');

            switch (publication)
            {
                case "SBAC-ELA-v1":
                    standardIdentifier = ElaV1Standard(parts, standards);
                    break;

                case "SBAC-MA-v1":
                    standardIdentifier = MaV1Standard(parts, standards);
                    break;

                case "SBAC-MA-v4":
                    standardIdentifier = MaV4Standard(parts, standards);
                    break;

                case "SBAC-MA-v5":
                    standardIdentifier = MaV5Standard(parts, standards);
                    break;

                case "SBAC-MA-v6":
                    standardIdentifier = MaV6Standard(parts, standards);
                    break;
            }

            if (standardIdentifier == null)
            {
                standardIdentifier = StandardIdentifier.Create(
                        claim: parts[0]);
            }

            return standardIdentifier;

        }

        private static string GetStandardPartOrDefault(string[] parts, int index)
        {
            if (parts != null && index < parts.Length)
            {
                return parts[index];
            }

            return string.Empty;
        }

        public static StandardIdentifier ElaV1Standard(string[] parts, string publication)
        {

            return StandardIdentifier.Create(
                    claim: GetStandardPartOrDefault(parts, 0),
                    target: GetStandardPartOrDefault(parts, 1),
                    commonCoreStandard: GetStandardPartOrDefault(parts, 2),
                    subjectCode: "ELA",
                    publication: publication);
        }

        public static StandardIdentifier MaV1Standard(string[] parts, string publication)
        {
            return StandardIdentifier.Create(
                    claim: GetStandardPartOrDefault(parts, 0),
                    target: GetStandardPartOrDefault(parts, 2),
                    commonCoreStandard: GetStandardPartOrDefault(parts, 4),
                    contentDomain: GetStandardPartOrDefault(parts, 1),
                    subjectCode: "MATH",
                    publication: publication);
        }

        public static StandardIdentifier MaV4Standard(string[] parts, string publication)
        {
            return StandardIdentifier.Create(
                    claim: GetStandardPartOrDefault(parts, 0),
                    target: GetStandardPartOrDefault(parts, 2),
                    contentDomain: GetStandardPartOrDefault(parts, 1),
                    emphasis: GetStandardPartOrDefault(parts, 3),
                    commonCoreStandard: GetStandardPartOrDefault(parts, 4),
                    subjectCode: "MATH",
                    publication: publication);
        }

        public static StandardIdentifier MaV5Standard(string[] parts, string publication)
        {

            return StandardIdentifier.Create(
                    claim: GetStandardPartOrDefault(parts, 0),
                    target: GetStandardPartOrDefault(parts, 2),
                    contentDomain: GetStandardPartOrDefault(parts, 1),
                    emphasis: GetStandardPartOrDefault(parts, 3),
                    commonCoreStandard: GetStandardPartOrDefault(parts, 4),
                    subjectCode: "MATH",
                    publication: publication);
        }

        public static StandardIdentifier MaV6Standard(string[] parts, string publication)
        {
            return StandardIdentifier.Create(
                    claim: GetStandardPartOrDefault(parts, 0),
                    target: GetStandardPartOrDefault(parts, 3),
                    contentCategory: GetStandardPartOrDefault(parts, 1),
                    targetSet: GetStandardPartOrDefault(parts, 2),
                    subjectCode: "MATH",
                    publication: publication);
        }

        public static StandardIdentifier ElaCoreStandardToTarget(string[] parts)
        {
            if (parts == null || parts.Length != 2)
            {
                return null;
            }

            return StandardIdentifier.Create(
                    claim: parts[0],
                    target: parts[1],
                    subjectCode: "ELA");
        }

        public static StandardIdentifier ElaCoreStandardToCcss(string[] parts)
        {
            if (parts == null || parts.Length != 3)
            {
                return null;
            }

            return StandardIdentifier.Create(
                    claim: parts[0],
                    target: parts[1],
                    commonCoreStandard: parts[2],
                    subjectCode: "ELA");
        }

        public static StandardIdentifier MathCoreStandardToTarget(string[] parts)
        {
            if (parts == null || parts.Length != 3)
            {
                return null;
            }

            return StandardIdentifier.Create(
                    claim: parts[0],
                    target: parts[2],
                    contentDomain: parts[1],
                    subjectCode: "MATH");
        }

        public static StandardIdentifier MathCoreStandardToCcss(string[] parts)
        {
            if (parts == null || parts.Length != 5)
            {
                return null;
            }

            return StandardIdentifier.Create(
                    claim: parts[0],
                    target: parts[2],
                    commonCoreStandard: parts[4],
                    contentDomain: parts[1],
                    subjectCode: "MATH");
        }

        public static StandardIdentifier CoreStandardToIdentifier(CoreStandardsRow row)
        {
            if (row == null || string.IsNullOrEmpty(row.Key))
            {
                throw new ArgumentException("The standards string must not be null or empty");
            }

            string[] parts = row.Key.Split('|');

            StandardIdentifier identifier = null;

            if (row.SubjectCode == "ELA")
            {
                if (row.LevelType == "CCSS")
                {
                    identifier = ElaCoreStandardToCcss(parts);
                }
                else if (row.LevelType == "Target")
                {
                    identifier = ElaCoreStandardToTarget(parts);
                }
            }
            else if (row.SubjectCode == "MATH")
            {
                if (row.LevelType == "CCSS")
                {
                    identifier = MathCoreStandardToCcss(parts);
                }
                else if (row.LevelType == "Target")
                {
                    identifier = MathCoreStandardToTarget(parts);
                }
            }

            return identifier;
        }

        /// <summary>
        /// Translates the standard identifier to claim id 
        /// </summary>
        public static string ToClaimId(this StandardIdentifier identifier)
        {
            return (string.IsNullOrEmpty(identifier?.Claim)) ? string.Empty : identifier.Claim.Split('-').FirstOrDefault();
        }

        /// <summary>
        /// Translates the standard identifier to target id
        /// </summary>
        public static string ToTargetId(this StandardIdentifier identifier)
        {
            return (string.IsNullOrEmpty(identifier?.Target)) ? string.Empty : identifier.Target.Split('-').FirstOrDefault();
        }

        /// <summary>
        /// Gets the standard identifier from the given item metadata
        /// </summary>
        public static StandardIdentifier ToStandardIdentifier(ItemDigest digest, string[] supportedPubs)
        {
            try
            {
                var primaryStandard = digest.StandardPublications
                    .Where(s => !s.PrimaryStandard.EndsWith("Undesignated") && supportedPubs.Any(p => p.Equals(s.Publication)))
                    .FirstOrDefault()
                    ?.PrimaryStandard;

                var identifier = StandardStringToStandardIdentifier(primaryStandard);
                return identifier;
            }
            catch (InvalidOperationException ex)
            {
                throw new SampleItemsContextException(
                    $"Publication field for item {digest.BankKey}-{digest.ItemKey} is empty.", ex);
            }
        }

        public static CoreStandards CoreStandardFromIdentifier(
            CoreStandardsXml standardsXml,
            StandardIdentifier itemIdentifier)
        {
            CoreStandardsRow targetRow = null;
            CoreStandardsRow ccssRow = null;
            if (standardsXml != null && standardsXml.TargetRows.Any())
            {
                targetRow = standardsXml.TargetRows
                    .FirstOrDefault(t =>
                    StandardIdentifierTargetComparer.Instance.Equals(t.StandardIdentifier, itemIdentifier));
            }

            if (standardsXml != null && standardsXml.CcssRows.Any())
            {
                ccssRow = standardsXml.CcssRows
                    .FirstOrDefault(t =>
                    StandardIdentifierCcssComparer.Instance.Equals(t.StandardIdentifier, itemIdentifier));
            }

            var target = Target.Create(
                desc: targetRow?.Description,
                id: itemIdentifier?.Target,
                idLabel: itemIdentifier?.ToTargetId(),
                subject: targetRow?.SubjectCode ?? ccssRow?.SubjectCode,
                claim: itemIdentifier?.ToClaimId());

            return CoreStandards.Create(
                  target: target,
                  commonCoreStandardsId: itemIdentifier?.CommonCoreStandard,
                  commonCoreStandardsDescription: ccssRow?.Description,
                  claimId: itemIdentifier?.ToClaimId(),
                  publication: itemIdentifier?.Publication,
                  subject: targetRow?.SubjectCode ?? ccssRow?.SubjectCode);
        }

    }

}
