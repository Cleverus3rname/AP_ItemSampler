using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmarterBalanced.SampleItems.Dal.Xml;
using SmarterBalanced.SampleItems.Dal.Translations;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;

namespace SmarterBalanced.SampleItems.Dal.Providers
{
    public static class SampleItemsProvider
    {
        public static SampleItemsContext LoadContext(AppSettings appSettings, ILogger logger)
        {
            SbContentSettings contentSettings = appSettings.SbContent;

            CoreStandardsXml standardsXml = LoadCoreStandards(contentSettings.CoreStandardsXMLPath);
            var targetCategories = standardsXml.TargetRows
                .Select(tr => StandardIdentifierTranslation.CoreStandardFromIdentifier(standardsXml, tr.StandardIdentifier).Target)
                .GroupBy(t => new {t.Subject, t.ClaimId, t.Name }) //this was filtering out the same targets. I enabled grouping by subject and claim first. Now we have targets for each claim
                .Select(g => g.First())
                .OrderBy(t => {
                    int i;
                    Int32.TryParse(t.IdLabel, out i);
                    return i;
                })
                .ToImmutableArray();

            var accessibilityResourceFamilies = LoadAccessibility(contentSettings.AccommodationsXMLPath);
            var interactionGroup = LoadInteractionGroup(contentSettings.InteractionTypesXMLPath);
            ImmutableArray<Subject> subjects = LoadSubjects(contentSettings.ClaimsXMLPath, interactionGroup.InteractionFamilies);
            subjects = subjects.Select(s => s.WithClaimTargets(targetCategories)).ToImmutableArray();

            var itemDigests = LoadItemDigests(appSettings.SbContent.ContentRootDirectory).Result;

            var itemPatchPath = appSettings.SbContent.PatchXMLPath;
            var itemPatchRoot = XmlSerialization.DeserializeXml<ItemPatchRoot>(filePath: itemPatchPath);
            var brailleFileInfo = BrailleManifestReader.GetBrailleFileInfo(appSettings).Result;

            var sampleItems = SampleItemTranslation.ToSampleItems(
                digests: itemDigests,
                settings: appSettings,
                resourceFamilies: accessibilityResourceFamilies,
                interactionTypes: interactionGroup.InteractionTypes,
                subjects: subjects,
                patches: itemPatchRoot.Patches,
                standardsXml: standardsXml,
                brailleFileInfo: brailleFileInfo);

            var itemCards = sampleItems
                .Select(i => i.ToItemCardViewModel())
                .ToImmutableArray();

            var aboutItems = sampleItems
                .Select(item => AboutThisItemViewModelTranslations.FromSampleItem(
                    sampleItem: item,
                    itemCards: itemCards,
                    allSampleItems: sampleItems))
                .ToImmutableArray();

            var aboutInteractionTypes = LoadAboutInteractionTypes(interactionGroup);
            var claims = GetClaims(subjects);
            var targets = GetTargets(claims);

            var subjectInteractionTypes = LoadSubjectInteractionTypes(interactionGroup, subjects);
            var filterSearch = SearchFilterTranslation.ToSearchFilter(appSettings.SbContent.FilterCategories,
                subjects: subjects, claims: claims, interactionTypes: subjectInteractionTypes, targets: targets);

            SampleItemsContext context = new SampleItemsContext(
                sampleItems: sampleItems,
                itemCards: itemCards,
                interactionTypes: subjectInteractionTypes,
                subjects: subjects,
                appSettings: appSettings,
                aboutAllItems: aboutItems,
                aboutInteractionTypes: aboutInteractionTypes,
                mergedAccessibilityFamilies: accessibilityResourceFamilies,
                targets: targets,
                claims: claims,
                filterSearch: filterSearch);

            logger.LogInformation($"Loaded {sampleItems.Length} sample items");
            logger.LogInformation($"Context loaded successfully");

            return context;
        }

        private static ImmutableArray<Target> GetTargets(ImmutableArray<Claim> claims)
        {
            var targets = claims.SelectMany(i => i.Targets).ToImmutableArray();
            return targets;
        }

        private static ImmutableArray<Claim> GetClaims(ImmutableArray<Subject> subjects)
        {
            var claims = subjects.SelectMany(i => i.Claims).ToImmutableArray();
            return claims;
        }

        public static async Task<ImmutableArray<ItemDigest>> LoadItemDigests(
            string contentDir)
        {
            var metaDataFiles = XmlSerialization.FindMetadataXmlFiles(contentDir);
            var contentFiles = XmlSerialization.FindContentXmlFiles(contentDir);

            var itemMetadata = await XmlSerialization.DeserializeXmlFilesAsync<ItemMetadata>(metaDataFiles);
            var itemContents = await XmlSerialization.DeserializeXmlFilesAsync<ItemContents>(contentFiles);

            var itemDigests = ItemDigestTranslation
                .ToItemDigests(
                    itemMetadata,
                    itemContents)
                .Where(i => i.GradeCode != "NA" && string.IsNullOrEmpty(i.ItemType))
                .ToImmutableArray();

            return itemDigests;
        }

        private static ImmutableArray<MergedAccessibilityFamily> LoadAccessibility(string accessibilityPath)
        {
            var accessibilityDoc = XmlSerialization.GetXDocument(accessibilityPath);
            var accessibilityXml = accessibilityDoc.Element("Accessibility");
            var mergedFamilies = AccessibilityResourceTranslation.CreateMergedFamilies(accessibilityXml);

            return mergedFamilies;
        }

        private static CoreStandardsXml LoadCoreStandards(string targetFile)
        {
            var coreDoc = XmlSerialization.GetXDocument(targetFile);

            ImmutableArray<CoreStandardsRow> allRows = coreDoc.Element("Levels").Elements()
                                                .Select(t => CoreStandardsRow.Create(t))
                                                .ToImmutableArray();

            var ccss = allRows.Where(r => r.LevelType == "CCSS").ToImmutableArray();
            var targets = allRows.Where(r => r.LevelType == "Target").ToImmutableArray();

            return new CoreStandardsXml(
                targetRows: targets,
                ccssRows: ccss);
        }

        private static ImmutableArray<Subject> LoadSubjects(string subjectFile, ImmutableArray<InteractionFamily> interactionFamilies)
        {
            var subjectDoc = XmlSerialization.GetXDocument(subjectFile);
            var subjects = subjectDoc.Element("Subjects")
                .Elements("Subject")
                .Select(s => Subject.Create(s, interactionFamilies)).ToImmutableArray();
            return subjects;

        }

        private static InteractionGroup LoadInteractionGroup(string interactionTypesFile)
        {
            var interactionTypeDoc = XmlSerialization.GetXDocument(interactionTypesFile);
            var elementRoot = interactionTypeDoc.Element("InteractionTypes");
            var interactionTypes = LoadInteractionTypes(elementRoot);
            var interactionFamilies = LoadInteractionFamilies(elementRoot);

            return new InteractionGroup(interactionFamilies, interactionTypes);
        }

        private static ImmutableArray<InteractionFamily> LoadInteractionFamilies(XElement elementRoot)
        {
            var interactionFamilies = elementRoot
                             .Element("Families")
                             .Elements("Family")
                             .Select(e => InteractionFamily.Create(e))
                             .ToImmutableArray();
            return interactionFamilies;
        }

        private static ImmutableArray<InteractionType> LoadInteractionTypes(XElement elementRoot)
        {
            var interactionTypes = elementRoot
                            .Element("Items")
                            .Elements("Item")
                            .Select(i => InteractionType.Create(i))
                            .OrderBy(i => i.Order)
                            .ToImmutableArray();

            return interactionTypes;
        }

        private static ImmutableArray<InteractionType> LoadAboutInteractionTypes(InteractionGroup interactionGroup)
        {
            var aboutInteractionCodes = interactionGroup.InteractionFamilies.FirstOrDefault(i => i.Type == "AboutItems").InteractionTypeCodes;
            var aboutInteractionTypes = interactionGroup.InteractionTypes
                .Where(i => aboutInteractionCodes.Contains(i.Code))
                .OrderBy(i => i.Order)
                .ToImmutableArray();

            return aboutInteractionTypes;
        }

        //TODO: refactor and condense with loadaboutinteractiontypes
        private static ImmutableArray<InteractionType> LoadSubjectInteractionTypes(InteractionGroup interactionGroup,
            ImmutableArray<Subject> subjects)
        {
            var subjectCodes = subjects.SelectMany(s => s.InteractionTypeCodes);   
            var interactionTypes = interactionGroup.InteractionTypes
                .Where(i => subjectCodes.Contains(i.Code))
                .OrderBy(i => i.Order)
                .ToImmutableArray();

            return interactionTypes;
        }

    }
}
