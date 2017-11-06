using SmarterBalanced.SampleItems.Dal.Exceptions;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System.Text.RegularExpressions;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public static class SampleItemTranslation
    {
        /// <summary>
        /// Digests a collection of ItemMetadata objects and a collection of ItemContents objects into a collection of ItemDigest objects.
        /// Matches the ItemMetadata and ItemContents objects based on their ItemKey fields.
        /// </summary>
        public static ImmutableArray<SampleItem> ToSampleItems(
            ImmutableArray<ItemDigest> digests,
            IList<MergedAccessibilityFamily> resourceFamilies,
            IList<InteractionType> interactionTypes,
            IList<Subject> subjects,
            CoreStandardsXml standardsXml,
            IList<ItemPatch> patches,
            IList<BrailleFileInfo> brailleFileInfo,
            AppSettings settings
            )
        {

            var sampleItems = digests.Select(d =>
                {
                    try
                    {
                        return ToSampleItem(
                           itemDigest: d,
                           standardsXml: standardsXml,
                           subjects: subjects,
                           interactionTypes: interactionTypes,
                           resourceFamilies: resourceFamilies,
                           patches: patches,
                           brailleFileInfo: brailleFileInfo,
                           settings: settings
                           );

                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Item {d.BankKey}-{d.ItemKey}", innerException: e);
                    }
                }).ToImmutableArray();

            return sampleItems;
        }

        /// <summary>
        /// Translates metadata, itemcontents and lookups to item digest
        /// </summary>
        public static SampleItem ToSampleItem(
            ItemDigest itemDigest,
            CoreStandardsXml standardsXml,
            IList<Subject> subjects,
            IList<InteractionType> interactionTypes,
            IList<MergedAccessibilityFamily> resourceFamilies,
            IList<ItemPatch> patches,
            IList<BrailleFileInfo> brailleFileInfo,
            AppSettings settings)
        {
            var itemTypeCode = GetUpdatedItemType(settings, itemDigest.ItemType);
            var interactionTypeCode = GetUpdatedItemType(settings, itemDigest.InteractionTypeCode);

            var supportedPubs = settings.SbContent.SupportedPublications;
            var brailleItemCodes = GetBrailleItemCodes(itemDigest.ItemKey, brailleFileInfo);
            var braillePassageCodes = GetBraillePassageCodes(itemDigest, brailleFileInfo);
            var interactionType = interactionTypes.FirstOrDefault(t => t.Code == interactionTypeCode);
            var grade = GradeLevelsUtils.FromString(itemDigest.GradeCode);
            var patch = patches.FirstOrDefault(p => p.ItemId == itemDigest.ItemKey);
            var copiedItemPatch = patches.FirstOrDefault(p => p.BrailleCopiedId == itemDigest.ItemKey.ToString());
            var subject = subjects.FirstOrDefault(s => s.Code == itemDigest.SubjectCode);
            var depthOfKnowledge = itemDigest.DepthOfKnowledge;

            var fieldTestUseAttribute = itemDigest.ItemMetadataAttributes?.FirstOrDefault(a => a.Code == "itm_FTUse");
            var fieldTestUse = FieldTestUse.Create(fieldTestUseAttribute, itemDigest.SubjectCode);

            var scoring = SampleItemsScoringTranslation.ToSampleItemsScore(itemDigest, settings, interactionTypes);

            StandardIdentifier identifier = StandardIdentifierTranslation.ToStandardIdentifier(itemDigest, supportedPubs);
            CoreStandards coreStandards = StandardIdentifierTranslation.CoreStandardFromIdentifier(standardsXml, identifier);
            int? copiedFromItem = null;


            if (patch != null)
            {
                int tmp;
                copiedFromItem = int.TryParse(patch.BrailleCopiedId, out tmp) ? (int?)tmp : null;
                depthOfKnowledge = !string.IsNullOrEmpty(patch.DepthOfKnowledge) ? patch.DepthOfKnowledge : depthOfKnowledge;
                itemTypeCode = !string.IsNullOrEmpty(patch.ItemType) ? patch.ItemType : itemTypeCode;
                coreStandards = ApplyPatchToCoreStandards(identifier, coreStandards, standardsXml, patch);
            }

            if (copiedItemPatch != null)
            {
                var copyBrailleItemCodes = GetBrailleItemCodes(copiedItemPatch.ItemId, brailleFileInfo);
                brailleItemCodes = brailleItemCodes.Concat(copyBrailleItemCodes).Distinct().ToImmutableArray();
            }

            if (patch != null && !string.IsNullOrEmpty(patch.QuestionNumber))
            {
                fieldTestUse = ApplyPatchFieldTestUse(fieldTestUse, patch);
            }

            var claim = subject?.Claims.FirstOrDefault(t => t.ClaimNumber == coreStandards.ClaimId);
            bool brailleOnly = copiedFromItem.HasValue;
            bool isPerformance = fieldTestUse != null && itemDigest.AssociatedStimulus.HasValue;

            if (itemDigest.AssociatedStimulus.HasValue)
            {
                braillePassageCodes = brailleFileInfo
                    .Where(f => f.ItemKey == itemDigest.AssociatedStimulus.Value)
                    .Select(b => b.BrailleType).ToImmutableArray();
            }
            else
            {
                braillePassageCodes = ImmutableArray.Create<string>();
            }

            bool aslSupported = AslSupported(itemDigest);

            var groups = GetAccessibilityResourceGroups(itemDigest, resourceFamilies, grade,
                isPerformance, aslSupported, claim, interactionType, brailleItemCodes, settings);

            string interactionTypeSubCat = "";
            settings.SbContent.InteractionTypesToItem.TryGetValue(itemDigest.ToString(), out interactionTypeSubCat);

            SampleItem sampleItem = new SampleItem(
                itemType: itemTypeCode,
                itemKey: itemDigest.ItemKey,
                bankKey: itemDigest.BankKey,
                targetAssessmentType: itemDigest.TargetAssessmentType,
                depthOfKnowledge: depthOfKnowledge,
                sufficentEvidenceOfClaim: itemDigest.SufficentEvidenceOfClaim,
                associatedStimulus: itemDigest.AssociatedStimulus,
                aslSupported: aslSupported,
                allowCalculator: itemDigest.AllowCalculator,
                isPerformanceItem: isPerformance,
                accessibilityResourceGroups: groups,
                interactionType: interactionType,
                subject: subject,
                claim: claim,
                grade: grade,
                coreStandards: coreStandards,
                fieldTestUse: fieldTestUse,
                interactionTypeSubCat: interactionTypeSubCat,
                brailleItemCodes: brailleItemCodes,
                braillePassageCodes: braillePassageCodes,
                brailleOnlyItem: brailleOnly,
                copiedFromItem: copiedFromItem,
                educationalDifficulty: itemDigest.EducationalDifficulty,
                evidenceStatement: itemDigest.EvidenceStatement,
                domain: identifier?.ContentDomain,
                scoring: scoring,
                associatedTutorial: itemDigest.AssociatedTutorial);

            return sampleItem;
        }

        private static string GetUpdatedItemType(AppSettings settings, string itemType)
        {
            if (string.IsNullOrEmpty(itemType)) return string.Empty;
            var oldToNewInteraction = settings?.SbContent?.OldToNewInteractionType;

            if (oldToNewInteraction != null && oldToNewInteraction.ContainsKey(itemType))
            {
                oldToNewInteraction.TryGetValue(itemType, out itemType);
            }

            return itemType;
        }

        public static AccessibilityResourceGroup GroupItemResources(
            AccessibilityType accType,
            ImmutableArray<AccessibilityResource> resources)
        {
            var matchingResources = resources
                .Where(r => r.ResourceTypeId == accType.Id)
                .ToImmutableArray();

            var group = new AccessibilityResourceGroup(
                label: accType.Label,
                order: accType.Order,
                accessibilityResources: matchingResources);

            return group;
        }

        public static ImmutableArray<string> GetBrailleItemCodes(int itemKey, IList<BrailleFileInfo> brailleFileInfo)
        {
            ImmutableArray<string> brailleItemCodes = brailleFileInfo.Where
                (f => f.ItemKey == itemKey)
                .Select(b => b.BrailleType).ToImmutableArray();

            return brailleItemCodes;
        }

        public static ImmutableArray<string> GetBraillePassageCodes(ItemDigest itemDigest, IList<BrailleFileInfo> brailleFileInfo)
        {
            ImmutableArray<string> braillePassageCodes;

            if (itemDigest.AssociatedStimulus.HasValue)
            {
                braillePassageCodes = brailleFileInfo
                    .Where(f => f.ItemKey == itemDigest.AssociatedStimulus.Value)
                    .Select(b => b.BrailleType).ToImmutableArray();
            }
            else
            {
                braillePassageCodes = ImmutableArray.Create<string>();
            }

            return braillePassageCodes;
        }

        public static ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroups(
            IList<MergedAccessibilityFamily> resourceFamilies,
            GradeLevels grade,
            string subjectCode,
            AppSettings settings,
            string interactionType
            )
        {
            var family = resourceFamilies.FirstOrDefault(f =>
               f.Grades.Contains(grade) &&
               f.Subjects.Contains(subjectCode));

            var flaggedResources = family?.Resources
             .Select(r => r.ApplyFlags( 
                 subjectCode: subjectCode,
                 interactionType: interactionType,
                 dictionarySupportedItemTypes: settings.SbContent.DictionarySupportedItemTypes))
             .ToImmutableArray() ?? ImmutableArray<AccessibilityResource>.Empty;

            var groups = settings.SbContent.AccessibilityTypes
               .Select(accType => GroupItemResources(accType, family.Resources))
               .OrderBy(g => g.Order)
               .ToImmutableArray();

            return groups;
        }

        public static ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroups(
            ItemDigest itemDigest,
            IList<MergedAccessibilityFamily> resourceFamilies,
            GradeLevels grade,
            bool isPerformance,
            bool aslSupported,
            Claim claim,
            InteractionType interactionType,
            ImmutableArray<string> brailleItemCodes,
            AppSettings settings)
        {
            var family = resourceFamilies.FirstOrDefault(f =>
               f.Grades.Contains(grade) &&
               f.Subjects.Contains(itemDigest.SubjectCode));

            var flaggedResources = family?.Resources
             .Select(r => r.ApplyFlags(
                 subjectCode: itemDigest.SubjectCode,
                 dictionarySupportedItemTypes: settings.SbContent.DictionarySupportedItemTypes,
                 supportedBraille: brailleItemCodes,
                 claim: claim,
                 isPerformanceTask: isPerformance, 
                 interactionType: interactionType?.Code,
                 aslSupported: aslSupported,
                 allowCalculator: itemDigest.AllowCalculator))
             .ToImmutableArray() ?? ImmutableArray<AccessibilityResource>.Empty;

            var groups = settings.SbContent.AccessibilityTypes
                .Select(accType => GroupItemResources(accType, flaggedResources))
                .OrderBy(g => g.Order)
                .ToImmutableArray();

            return groups;
        }


        private static bool AslSupportedContents(List<Content> content)
        {
            if (content == null)
            {
                return false;
            }

            bool foundAslAttachment = content
             .Any(c => c.Attachments != null &&
                 c.Attachments.Any(a => !string.IsNullOrEmpty(a.Type) &&
                     a.Type.ToLower().Contains("asl")));

            return foundAslAttachment;
        }

        public static bool AslSupported(ItemDigest digest)
        {
            if (!digest.Contents.Any())
            {
                return digest.AslSupported ?? false;
            }

            bool foundAslAttachment = AslSupportedContents(digest.Contents);
            bool foundStimAslAttachment = AslSupportedContents(digest.StimulusDigest?.Contents);
            bool aslAttachment = foundAslAttachment || foundStimAslAttachment;

            bool aslSupported = (digest.AslSupported.HasValue) ? (digest.AslSupported.Value && aslAttachment) : aslAttachment;

            return aslSupported;
        }

        private static CoreStandards ApplyPatchToCoreStandards(StandardIdentifier identifier,
            CoreStandards coreStandards,
            CoreStandardsXml standardsXml,
            ItemPatch patch)
        {
            string claimNumber = Regex.Match(input: patch.Claim, pattern: @"\d+").Value;
            if (identifier == null)
            {
                identifier = StandardIdentifier.Create(claim: claimNumber, target: patch.Target);
            }
            else
            {
                string target = (!string.IsNullOrEmpty(patch.Target)) ? patch.Target : identifier.Target;
                claimNumber = (!string.IsNullOrEmpty(claimNumber)) ? claimNumber : identifier.Claim;
                identifier = identifier.WithClaimAndTarget(claimNumber, target);
            }

            string targetDesc = (!string.IsNullOrEmpty(patch.TargetDescription)) ? patch.TargetDescription : coreStandards?.Target.Descripton;
            string ccssDesc = (!string.IsNullOrEmpty(patch.CCSSDescription)) ? patch.CCSSDescription : coreStandards?.CommonCoreStandardsDescription;
            coreStandards = StandardIdentifierTranslation.CoreStandardFromIdentifier(standardsXml, identifier);
            coreStandards = coreStandards.WithTargetCCSSDescriptions(targetDesc, ccssDesc);

            return coreStandards;

        }

        private static FieldTestUse ApplyPatchFieldTestUse(FieldTestUse fieldTestUse, ItemPatch patch)
        {
            int patchQuestion;
            int.TryParse(patch.QuestionNumber, out patchQuestion);

            var newFieldTestUse = new FieldTestUse
            {
                Code = fieldTestUse?.Code,
                CodeYear = fieldTestUse?.CodeYear,
                QuestionNumber = patchQuestion,
                Section = fieldTestUse?.Section
            };

            return newFieldTestUse;
        }

        private static int? GetCopiedFromItem(string desc)
        {
            int val;
            if (string.IsNullOrEmpty(desc))
            {
                return null;
            }

            var match = Regex.Match(input: desc, pattern: @"^(?=.*\bCloned\b)(?=.*\b(\d{4,6})).*$");

            return match.Success && int.TryParse(match.Groups[1]?.Value, out val) ? (int?)val : null;
        }

    }
}
