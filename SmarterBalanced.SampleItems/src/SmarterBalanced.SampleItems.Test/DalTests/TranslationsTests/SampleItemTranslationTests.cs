using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Translations;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.DalTests.TranslationsTests
{
    public class SampleItemTranslationTests
    {
        Claim Claim1, Claim2;
        Subject Math, Ela;
        InteractionType ItEla, ItMath;
        List<AccessibilityResource> Resources;
        SbContentSettings settings;
        ItemMetadata metadata;
        AppSettings appSettings;
        ItemContents contents;
        AccessibilityType accessibilityType;
        ItemDigest digest;
        List<RubricEntry> rubricEntries;
        List<RubricSample> rubricSamples;
        RubricList rubricList;

        public SampleItemTranslationTests()
        {
            rubricEntries = new List<RubricEntry>()
            {
                new RubricEntry
                {
                        Scorepoint = "0",
                        Name = "TestName",
                        Value = "TestValue"
                },
                new RubricEntry
                {
                        Scorepoint = "1",
                        Name = "TestName1",
                        Value = "TestValue1"
                }
            };

            var sampleResponces = new List<SampleResponse>()
            {
                new SampleResponse()
                {
                    Purpose = "TestPurpose",
                    ScorePoint = "0",
                    Name = "TestName",
                    SampleContent = "TestSampleContent"
                },
                new SampleResponse()
                {
                    Purpose = "TestPurpose1",
                    ScorePoint = "1",
                    Name = "TestName1",
                    SampleContent = "TestSampleContent1"
                }
            };

            rubricSamples = new List<RubricSample>()
            {
                 new RubricSample
                 {
                        MaxValue = "MaxVal",
                        MinValue = "MinVal",
                        SampleResponses = sampleResponces
                 },
                 new RubricSample
                 {
                        MaxValue = "MaxVal1",
                        MinValue = "MinVal1",
                        SampleResponses = new List<SampleResponse>()
                 }
            };

            rubricList = new RubricList()
            {
                Rubrics = rubricEntries,
                RubricSamples = rubricSamples
            };

            Resources = new List<AccessibilityResource>
            {
                AccessibilityResource.Create(
                    resourceCode: "ACC1",
                    order: 1,
                    disabled: false,
                    defaultSelection: "ACC1_SEL1",
                    currentSelectionCode:  "ACC1_SEL1",
                    label: "Accessibility 1",
                    description: "Accessibility Selection One",
                    resourceType: "Acc1Type",
                    selections: ImmutableArray.Create(
                        new AccessibilitySelection(
                            code: "ACC1_SEL1",
                            order: 1,
                            disabled: false,
                            label: "Selection 1",
                            hidden: false))),
                AccessibilityResource.Create(
                    resourceCode: "ACC2",
                    order: 2,
                    disabled: false,
                    defaultSelection: "ACC2_SEL2",
                    currentSelectionCode:  "ACC2_SEL2",
                    label: "Accessibility 2",
                    description: "Accessibility Selection Two",
                    resourceType: "Acc2Type",
                    selections: ImmutableArray.Create(
                        new AccessibilitySelection(
                            code: "ACC2_SEL1",
                            order: 1,
                            disabled: false,
                            label: "Selection 1",
                            hidden: false),
                        new AccessibilitySelection(
                            code: "ACC2_SEL2",
                            order: 2,
                            disabled: false,
                            label: "Selection 2",
                            hidden: false)))
            };

            accessibilityType = new AccessibilityType();
            accessibilityType.Id = "Acc1Type";
            accessibilityType.Label = "Accessibility 1";
            accessibilityType.Order = 1;

            int testItemKey = 1;
            int testItemBank = 2;
            string testGrade = "5";

            metadata = new ItemMetadata();
            contents = new ItemContents();
            metadata.Metadata = new SmarterAppMetadataXmlRepresentation();
            contents.Item = new ItemXmlFieldRepresentation();
            metadata.Metadata.ItemKey = testItemKey;
            metadata.Metadata.GradeCode = testGrade;
            metadata.Metadata.TargetAssessmentType = "Test target string";
            metadata.Metadata.SufficientEvidenceOfClaim = "Test claim string";
            metadata.Metadata.InteractionType = "2";
            metadata.Metadata.SubjectCode = "MATH";
            metadata.Metadata.MaximumNumberOfPoints = 2;
            metadata.Metadata.StandardPublications = new List<StandardPublication>();
            metadata.Metadata.StandardPublications.Add(
                new StandardPublication
                {
                    PrimaryStandard = "SBAC-ELA-v1:3-L|4-6|6.SL.2",
                    Publication = "SupportedPubs"
                });

            contents.Item.ItemKey = testItemKey;
            contents.Item.ItemBank = testItemBank;
            contents.Item.Contents = new List<Content>();
            var placeholderText = new RubricPlaceHolderText
            {
                RubricPlaceHolderContains = new string[] { "RubricSampleText", "RubricSampleText1" },
                RubricPlaceHolderEquals = new string[0]
            };

            settings = new SbContentSettings
            {
                SupportedPublications = new string[] { "SupportedPubs" },
                AccessibilityTypes = new List<AccessibilityType>() { accessibilityType },
                InteractionTypesToItem = new Dictionary<string, string>(),
                DictionarySupportedItemTypes = new List<string>(),
                LanguageToLabel = new Dictionary<string, string>(),
                RubricPlaceHolderText = placeholderText
            };

            appSettings = new AppSettings
            {
                SbContent = settings
            };

            digest = ItemDigestTranslation.ToItemDigest(metadata, contents);
        }

        [Fact]
        public void TestToSampleItems()
        {
            var digests = ImmutableArray.Create(digest);
            ImmutableArray<AccessibilityFamilyResource> noPartialResources = ImmutableArray.Create<AccessibilityFamilyResource>();
            var subjectMath = ImmutableArray.Create("MATH");
            AccessibilityFamily noPartialResourcesFamily = new AccessibilityFamily(
            subjects: subjectMath,
            grades: GradeLevels.Grade5,
            resources: noPartialResources);

            MergedAccessibilityFamily resourceFamily = AccessibilityResourceTranslation.MergeGlobalResources(noPartialResourcesFamily, Resources);
            var resourceFamilies = ImmutableArray.Create(resourceFamily);

            Claim1 = Claim.Create(code: "claim1", claimNumber: "1");
            Claim2 = Claim.Create(code: "claim2", claimNumber: "1");

            ItMath = new InteractionType("2", "EQ", "", 2);
            ItEla = new InteractionType("1", "WER", "", 1);
            Math = Subject.Create("MATH", "Mathematics", "Math", ImmutableArray.Create(Claim2), ImmutableArray.Create(ItMath.Code));
            Ela = Subject.Create("ELA", "English", "ELA", ImmutableArray.Create(Claim1), ImmutableArray.Create(ItEla.Code));
            var interactionTypes = ImmutableArray.Create(ItEla, ItMath);
            var subjects = ImmutableArray.Create(Ela, Math);

            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier("SBAC-ELA-v1:3-L|4-6|6.SL.2");
            var coreStandardsRowCcss = ImmutableArray.Create(
            CoreStandardsRow.Create(
                subjectCode: "ELA",
                key: "3-l|4-6|6.SL.2",
                name: "",
                description: "CCSS Desc",
                levelType: "CCSS",
                identifier: identifier));

            var coreStandardsRowTarget = ImmutableArray.Create(
               CoreStandardsRow.Create(
                   subjectCode: "ELA",
                   key: "4-6|3-6",
                   name: "",
                   description: "Target Desc",
                   levelType: "Target",
                   identifier: identifier));

            CoreStandardsXml coreStandardsXml = new CoreStandardsXml(coreStandardsRowTarget, coreStandardsRowCcss);
            var itemPatch = ImmutableArray.Create(new ItemPatch());
            var brailleFileInfo = new List<BrailleFileInfo>();
            var items = SampleItemTranslation.ToSampleItems(digests, resourceFamilies, interactionTypes, subjects, coreStandardsXml, itemPatch, brailleFileInfo, appSettings);

            Assert.NotNull(items);
            Assert.Equal(items[0].AccessibilityResourceGroups[0].AccessibilityResources[0].Label, Resources[0].Label);
            Assert.Equal(items[0].AccessibilityResourceGroups[0].AccessibilityResources[0].Order, Resources[0].Order);
            Assert.Equal(items[0].AccessibilityResourceGroups[0].AccessibilityResources[0].ResourceCode, Resources[0].ResourceCode);
            Assert.Equal(items[0].AccessibilityResourceGroups[0].AccessibilityResources[0].ResourceTypeId, Resources[0].ResourceTypeId);
            Assert.Equal(items[0].SufficentEvidenceOfClaim, metadata.Metadata.SufficientEvidenceOfClaim);
            Assert.Equal(items[0].TargetAssessmentType, metadata.Metadata.TargetAssessmentType);
            Assert.Equal(items[0].Grade, GradeLevelsUtils.FromString(digest.GradeCode));
            Assert.Equal(items[0].ItemKey, digest.ItemKey);
            Assert.Equal(items[0].BankKey, digest.BankKey);
            Assert.Equal(items[0].CoreStandards.CommonCoreStandardsDescription, coreStandardsRowCcss[0].Description);
            Assert.Equal(items[0].CoreStandards.Target.Descripton, coreStandardsRowTarget[0].Description);
            Assert.Equal(items[0].InteractionType.Code, ItMath.Code);
            Assert.Equal(items[0].InteractionType.Label, ItMath.Label);
            Assert.Equal(items[0].Subject.Code, digest.SubjectCode);
        }

        [Fact]
        public void TestGroupItemResources()
        {
            var accessibilityResources = ImmutableArray.Create(Resources[0], Resources[1]);
            var resourceGroup = SampleItemTranslation.GroupItemResources(accessibilityType, accessibilityResources);

            Assert.NotNull(resourceGroup);
            Assert.Equal(accessibilityType.Label, resourceGroup.Label);
            Assert.Equal(accessibilityType.Id, resourceGroup.AccessibilityResources[0].ResourceTypeId);
            Assert.Equal(accessibilityType.Order, resourceGroup.Order);
            Assert.Equal(accessibilityResources[0].Description, resourceGroup.AccessibilityResources[0].Description);
            Assert.Equal(accessibilityResources[0].CurrentSelectionCode, resourceGroup.AccessibilityResources[0].CurrentSelectionCode);
            Assert.Equal(accessibilityResources[0].Disabled, resourceGroup.AccessibilityResources[0].Disabled);
            Assert.Equal(accessibilityResources[0].DefaultSelection, resourceGroup.AccessibilityResources[0].DefaultSelection);
            Assert.Equal(accessibilityResources[0].ResourceCode, resourceGroup.AccessibilityResources[0].ResourceCode);
        }

        [Fact]
        public void TestGetRubrics()
        {
            settings.LanguageToLabel.Add("ENU", "English");
            digest.Contents = new List<Content>()
            {
                new Content()
                {
                    Language = "ENU",
                    RubricList = rubricList
                }
            };
            var rubrics = SampleItemsScoringTranslation.GetRubrics(digest, appSettings);

            Assert.NotNull(rubrics);
            Assert.Equal(rubrics[0].Language, "English");
            Assert.Equal(rubrics[0].RubricEntries.Length, 2);
            Assert.Equal(rubrics[0].Samples.Length, 1);
            Assert.Equal(rubrics[0].RubricEntries[0], rubricEntries[0]);
            Assert.Equal(rubrics[0].RubricEntries[1], rubricEntries[1]);
            Assert.Equal(rubrics[0].Samples[0], rubricSamples[0]);
            Assert.Equal(rubrics[0].Samples[0].SampleResponses, rubricSamples[0].SampleResponses);
        }
    }
}
