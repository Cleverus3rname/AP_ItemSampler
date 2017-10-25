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
    /// <summary>
    /// Tests the translation of a standards string into a StandardIdentifier object
    /// Different publications use different standard ids
    /// This tests the translation of standards listed in the SmarterApp "Interpreting Smarter Balances Standard Ids" document.
    /// http://www.smarterapp.org/documents/InterpretingSmarterBalancedStandardIDs.html
    /// </summary>
    public class StandardIdentifierTranslationTests
    {
        string elaStandardString;
        string mathv1StandardString;
        string mathV4StandardString;
        string mathV5StandardString;
        string mathV6StandardString;
        string invalidElaStandardString;
        string invalidMathV1StandardString;
        string invalidMathV4StandardString;
        string invalidMathV5StandardString;
        string invalidMathV6StandardString;
        string noClaimString;
        string noTargetString;
        ItemMetadata metadata;
        ItemContents contents;
        AppSettings appSettings;

        public StandardIdentifierTranslationTests()
        {
            elaStandardString = "SBAC-ELA-v1:3-L|4-6|6.SL.2";
            mathv1StandardString = "SBAC-MA-v1:1|NBT|E-3|a/s|3.NBT.2";
            mathV4StandardString = "SBAC-MA-v4:1|NS|D-6|m|6.NS.6c";
            mathV5StandardString = "SBAC-MA-v5:2|NS|D-6|m|6.NS.6c";
            mathV6StandardString = "SBAC-MA-v6:3|P|TS04|D-6";
            invalidElaStandardString = "SBAC-ELA-v1:3-L";
            invalidMathV1StandardString = "SBAC-MA-v1:1|NBT||a/s|";
            invalidMathV4StandardString = "SBAC-MA-v4:|NS|D-6||";
            invalidMathV5StandardString = "SBAC-MA-v5:2||||6.NS.6c";
            invalidMathV6StandardString = "SBAC-MA-v6:3|||";
            noClaimString = "SBAC-ELA-v1|4-6|6.SL.2";
            noTargetString = "SBAC-ELA-v1:3-L||6.SL.2";
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
            metadata.Metadata.InteractionType = "EQ";
            metadata.Metadata.SubjectCode = "MATH";
            metadata.Metadata.StandardPublications = new List<StandardPublication>();
            metadata.Metadata.StandardPublications.Add(
                new StandardPublication
                {
                    PrimaryStandard = "SBAC-MA-v1:1|NBT|E-3|a/s|3.NBT.2",
                    Publication = "SBAC-MA-v1"
                });

            contents.Item.ItemKey = testItemKey;
            contents.Item.ItemBank = testItemBank;
            contents.Item.Contents = new List<Content>();

            var interactionTypes = new List<InteractionType>
            {
                new InteractionType(code: "EQ", label: "", description: "", order: 0)
            };

            var subjects = new List<Subject>
            {
                new Subject(
                    code: "MATH",
                    label: string.Empty,
                    shortLabel: string.Empty,
                    claims: ImmutableArray.Create<Claim>(),
                    interactionTypeCodes: ImmutableArray.Create<string>())
            };

            var placeholderText = new RubricPlaceHolderText
            {
                RubricPlaceHolderContains = new string[0],
                RubricPlaceHolderEquals = new string[0]
            };

            var settings = new SbContentSettings
            {
                SupportedPublications = new string[] { "" },
                RubricPlaceHolderText = placeholderText
            };

            appSettings = new AppSettings
            {
                SbContent = settings
            };
        }

        [Fact]
        public void TestElaV1Translation()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
            Assert.Equal("3-L", identifier.Claim);
            Assert.Equal("4-6", identifier.Target);
            Assert.Equal("6.SL.2", identifier.CommonCoreStandard);
        }

        [Fact]
        public void TestMathV1Translation()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(mathv1StandardString);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("NBT", identifier.ContentDomain);
            Assert.Equal("E-3", identifier.Target);
            Assert.Equal("3.NBT.2", identifier.CommonCoreStandard);
        }

        [Fact]
        public void TestMathV4Translation()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(mathV4StandardString);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("D-6", identifier.Target);
            Assert.Equal("6.NS.6c", identifier.CommonCoreStandard);
            Assert.Equal("NS", identifier.ContentDomain);
            Assert.Equal("m", identifier.Emphasis);
        }

        [Fact]
        public void TestMathV5Translation()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(mathV5StandardString);
            Assert.Equal("2", identifier.Claim);
            Assert.Equal("D-6", identifier.Target);
            Assert.Equal("6.NS.6c", identifier.CommonCoreStandard);
            Assert.Equal("NS", identifier.ContentDomain);
            Assert.Equal("m", identifier.Emphasis);
        }

        [Fact]
        public void TestMathV6Translation()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(mathV6StandardString);
            Assert.Equal("3", identifier.Claim);
            Assert.Equal("D-6", identifier.Target);
            Assert.Equal("P", identifier.ContentCategory);
            Assert.Equal("TS04", identifier.TargetSet);
        }

        [Fact]
        public void TestInvalidElaString()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(invalidElaStandardString);
            Assert.Equal("3-L", identifier.Claim);
            Assert.Equal(string.Empty, identifier.Target);
            Assert.Equal(string.Empty, identifier.CommonCoreStandard);
        }

        [Fact]
        public void TestNoClaimString()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(noClaimString);
            Assert.Null(identifier);
        }

        [Fact]
        public void TestNullInput()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(null);
            Assert.Null(identifier);
        }

        [Fact]
        public void TestEmptyInput()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(string.Empty);
            Assert.Null(identifier);
        }

        [Fact]
        public void TestInvalidMathV1String()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(invalidMathV1StandardString);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal(string.Empty, identifier.Target);
            Assert.Equal(string.Empty, identifier.CommonCoreStandard);
        }

        [Fact]
        public void TestInvalidMathV4String()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(invalidMathV4StandardString);
            Assert.Equal(string.Empty, identifier.Claim);
            Assert.Equal("D-6", identifier.Target);
            Assert.Equal("NS", identifier.ContentDomain);
            Assert.Equal(string.Empty, identifier.CommonCoreStandard);
            Assert.Equal(string.Empty, identifier.Emphasis);
        }

        [Fact]
        public void TestInvalidMathV5String()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(invalidMathV5StandardString);
            Assert.Equal("2", identifier.Claim);
            Assert.Equal("6.NS.6c", identifier.CommonCoreStandard);
            Assert.Equal(string.Empty, identifier.Target);
            Assert.Equal(string.Empty, identifier.ContentDomain);
            Assert.Equal(string.Empty, identifier.Emphasis);
        }

        [Fact]
        public void TestInvalidMathV6String()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(invalidMathV6StandardString);
            Assert.Equal("3", identifier.Claim);
            Assert.Equal(string.Empty, identifier.ContentCategory);
            Assert.Equal(string.Empty, identifier.TargetSet);
            Assert.Equal(string.Empty, identifier.TargetSet);
        }

        [Fact]
        public void TestElaCoreStandardToTarget()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.ElaCoreStandardToTarget(new string[] { "4-6", "3-6" });
            Assert.Equal("4-6", identifier.Claim);
            Assert.Equal("3-6", identifier.Target);
            Assert.Equal("ELA", identifier.SubjectCode);
        }

        [Fact]
        public void TestInvalidElaCoreStandardToTarget()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.ElaCoreStandardToTarget(new string[] { });
            Assert.Null(identifier);
        }

        [Fact]
        public void TestElaCoreStandardToCcss()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.ElaCoreStandardToCcss(new string[] { "4-6", "3-6", "6.SL.2" });
            Assert.Equal("4-6", identifier.Claim);
            Assert.Equal("3-6", identifier.Target);
            Assert.Equal("6.SL.2", identifier.CommonCoreStandard);
            Assert.Equal("ELA", identifier.SubjectCode);
        }

        [Fact]
        public void TestInvalidElaCoreStandardToCcss()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.ElaCoreStandardToCcss(new string[] { });
            Assert.Null(identifier);
        }

        [Fact]
        public void TestMathCoreStandardToTarget()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.MathCoreStandardToTarget(new string[] { "1", "NBT", "E-3" });
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("NBT", identifier.ContentDomain);
            Assert.Equal("E-3", identifier.Target);
            Assert.Equal("MATH", identifier.SubjectCode);
        }

        [Fact]
        public void TestInvalidMathCoreStandardToTarget()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.MathCoreStandardToTarget(new string[] { });
            Assert.Null(identifier);
        }

        [Fact]
        public void TestMathCoreStandardToCcss()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.MathCoreStandardToCcss(new string[] { "4-6", "NS", "3-6", "m", "6.NS.6c" });
            Assert.Equal("4-6", identifier.Claim);
            Assert.Equal("3-6", identifier.Target);
            Assert.Equal("6.NS.6c", identifier.CommonCoreStandard);
            Assert.Equal("NS", identifier.ContentDomain);
            Assert.Equal("MATH", identifier.SubjectCode);
        }

        [Fact]
        public void TestInvalidMathCoreStandardToCcss()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.MathCoreStandardToCcss(new string[] { });
            Assert.Null(identifier);
        }

        [Fact]
        public void TestInvalidCoreStandardToIdentifier()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("", null, null, null, null, null);
            Exception exception = Assert.Throws<ArgumentException>(() => StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow));
            Assert.Equal("The standards string must not be null or empty", exception.Message);
            exception = Assert.Throws<ArgumentException>(() => StandardIdentifierTranslation.CoreStandardToIdentifier(null));
            Assert.Equal("The standards string must not be null or empty", exception.Message);
        }

        [Fact]
        public void TestNullCoreStandardToIdentifier()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("", "3-l", null, null, null, null);
            StandardIdentifier identifier = StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow);
            Assert.Null(identifier);
        }

        [Fact]
        public void TestCoreStandardToIdentifierELACcss()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("ELA", "3-l|4-6|6.SL.2", "", "", "CCSS", null);
            StandardIdentifier identifier = StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow);
            Assert.Equal("3-l", identifier.Claim);
            Assert.Equal("4-6", identifier.Target);
            Assert.Equal("6.SL.2", identifier.CommonCoreStandard);
            Assert.Equal("ELA", identifier.SubjectCode);
        }

        [Fact]
        public void TestCoreStandardToIdentifierELATarget()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("ELA", "4-6|3-6", "", "", "Target", null);
            StandardIdentifier identifier = StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow);
            Assert.Equal("4-6", identifier.Claim);
            Assert.Equal("3-6", identifier.Target);
            Assert.Equal("ELA", identifier.SubjectCode);
        }

        [Fact]
        public void TestCoreStandardToIdentifierMathCcss()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("MATH", "1|NBT|E-3|a/s|3.NBT.2", "", "", "CCSS", null);
            StandardIdentifier identifier = StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("NBT", identifier.ContentDomain);
            Assert.Equal("E-3", identifier.Target);
            Assert.Equal("3.NBT.2", identifier.CommonCoreStandard);
        }

        [Fact]
        public void TestCoreStandardToIdentifierMathTarget()
        {
            CoreStandardsRow coreStandardsRow = new CoreStandardsRow("MATH", "1|NBT|E-3", "", "", "Target", null);
            StandardIdentifier identifier = StandardIdentifierTranslation.CoreStandardToIdentifier(coreStandardsRow);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("NBT", identifier.ContentDomain);
            Assert.Equal("E-3", identifier.Target);
            Assert.Equal("MATH", identifier.SubjectCode);
        }

        [Fact]
        public void TestToClaimId()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
            var claimId = StandardIdentifierTranslation.ToClaimId(identifier);
            identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(noClaimString);
            var noClaimId = StandardIdentifierTranslation.ToClaimId(identifier);

            Assert.Equal("3", claimId);
            Assert.Equal(string.Empty, noClaimId);
        }

        [Fact]
        public void TestToTargetId()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
            var targetId = StandardIdentifierTranslation.ToTargetId(identifier);
            identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(noTargetString);
            var noTargetId = StandardIdentifierTranslation.ToTargetId(identifier);

            Assert.Equal("4", targetId);
            Assert.Equal(string.Empty, noTargetId);
        }

        [Fact]
        public void TestToStandardIdentifierGoodDigest()
        {
            
            ItemDigest digest = ItemDigestTranslation.ToItemDigest(metadata, contents);
            StandardIdentifier identifier = StandardIdentifierTranslation.ToStandardIdentifier(digest, new string[] { "SBAC-MA-v1" });

            Assert.NotNull(identifier);
            Assert.Equal(digest.SubjectCode, identifier.SubjectCode);
            Assert.Equal("1", identifier.Claim);
            Assert.Equal("3.NBT.2", identifier.CommonCoreStandard);
            Assert.Equal("NBT", identifier.ContentDomain);
            Assert.Equal("E-3", identifier.Target);
        }

        [Fact]
        public void TestCoreStandardFromIdentifier()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
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
            CoreStandards core = StandardIdentifierTranslation.CoreStandardFromIdentifier(coreStandardsXml, identifier);

            Assert.NotNull(core);
            Assert.Equal(core.CommonCoreStandardsId, "6.SL.2");
            Assert.Equal(core.Target.Id, "4-6");
            Assert.Equal(core.Target.IdLabel, "4");
            Assert.Equal(core.Target.Descripton, "Target Desc");
            Assert.Equal(core.CommonCoreStandardsDescription, "CCSS Desc");
        }

        [Fact]
        public void TestCoreStandardsTarget()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
            var coreStandardsRowTarget = ImmutableArray.Create(
                CoreStandardsRow.Create(
                    subjectCode: "ELA",
                    key: "4-6|3-6",
                    description: "SHORT NAME: Target description",
                    levelType: "Target",
                    identifier: identifier));
            CoreStandardsXml coreStandardsXml = new CoreStandardsXml(coreStandardsRowTarget, ImmutableArray.Create<CoreStandardsRow>());
            var coreStandard = StandardIdentifierTranslation.CoreStandardFromIdentifier(coreStandardsXml, identifier);

            Assert.NotNull(coreStandard);
            Assert.Equal(coreStandard.Target.Descripton, "Target description");
            Assert.Equal(coreStandard.Target.Name, "Short Name");
            Assert.Equal(coreStandard.Target.Id, "4-6");
            Assert.Equal(coreStandard.Target.IdLabel, "4");
            Assert.Equal(coreStandard.Target.NameHash, coreStandard.Target.GetHashCode());
            Assert.Equal(coreStandard.Target.Subject, "ELA");
            Assert.Equal(coreStandard.Target.ClaimId, "3");
        }

        [Fact]
        public void TestTargetWithDescription()
        {
            StandardIdentifier identifier = StandardIdentifierTranslation.StandardStringToStandardIdentifier(elaStandardString);
            var coreStandardsRowTarget = ImmutableArray.Create(
                CoreStandardsRow.Create(
                    subjectCode: "ELA",
                    key: "4-6|3-6",
                    description: "SHORT NAME: Target description",
                    levelType: "Target",
                    identifier: identifier));
            CoreStandardsXml coreStandardsXml = new CoreStandardsXml(coreStandardsRowTarget, ImmutableArray.Create<CoreStandardsRow>());
            var coreStandard = StandardIdentifierTranslation.CoreStandardFromIdentifier(coreStandardsXml, identifier);
            var newCoreStandard = coreStandard.WithTargetCCSSDescriptions(targetDescription: "NEW NAME: New description");

            Assert.Equal(newCoreStandard.Target.Descripton, "New description");
            Assert.Equal(newCoreStandard.Target.Name, "New Name");
            Assert.Equal(newCoreStandard.Target.Id, "4-6");
            Assert.Equal(newCoreStandard.Target.IdLabel, "4");
            Assert.Equal(newCoreStandard.Target.NameHash, newCoreStandard.Target.GetHashCode());
            Assert.Equal(newCoreStandard.Target.Subject, "ELA");
            Assert.Equal(newCoreStandard.Target.ClaimId, "3");
            Assert.NotEqual(newCoreStandard.Target.NameHash, coreStandard.Target.NameHash);
        }
    }
}
