﻿using Moq;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Translations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.DalTests.TranslationsTests
{
    public class AccessibilityTranslationTests
    {
        public List<AccessibilityResource> Resources { get; set; }
        public List<AccessibilityResource> PartialResources { get; set; }
        public AccessibilityFamilyResource familyResource;
        public AccessibilityResource globalResource;
        public AccessibilityResource calculatorResource = AccessibilityResource.Create(
               resourceCode: "Calculator",
               disabled: false,
               selections: ImmutableArray.Create(
                   new AccessibilitySelection(
                          code: "ACC1_SEL1",
                          order: 1,
                          disabled: true,
                          label: "Selection 1")));

        public AccessibilityResource aslResource = AccessibilityResource.Create(
               resourceCode: "AmericanSignLanguage",
               disabled: false,
               selections: ImmutableArray.Create(
                   new AccessibilitySelection(
                          code: "ACC1_SEL1",
                          order: 1,
                          disabled: true,
                          label: "Selection 1")));

        public AccessibilityTranslationTests()
        {
            Resources = new List<AccessibilityResource>
            {
                AccessibilityResource.Create(
                    resourceCode: "ACC1",
                    order: 1,
                    disabled: false,
                    defaultSelection: "ACC1_SEL1",
                    label: "Accessibility 1",
                    description: "Accessibility Selection One",
                    selections: ImmutableArray.Create(
                        new AccessibilitySelection(
                            code: "ACC1_SEL1",
                            order: 1,
                            disabled: false,
                            label: "Selection 1"))),
                AccessibilityResource.Create(
                    resourceCode: "ACC2",
                    order: 2,
                    disabled: false,
                    defaultSelection: "ACC2_SEL2",
                    label: "Accessibility 2",
                    description: "Accessibility Selection Two",
                    selections: ImmutableArray.Create(
                        new AccessibilitySelection(
                            code: "ACC2_SEL1",
                            order: 1,
                            disabled: false,
                            label: "Selection 1"),
                        new AccessibilitySelection(
                            code: "ACC2_SEL2",
                            order: 2,
                            disabled: false,
                            label: "Selection 2")))
            };

            PartialResources = new List<AccessibilityResource>
            {
                AccessibilityResource.Create(
                    resourceCode: "ACC1",
                    selections: ImmutableArray.Create(
                        AccessibilitySelection.Create(
                            code: "ACC1_SEL1",
                            label: "Selection 1"))),
                AccessibilityResource.Create(
                    resourceCode: "ACC2",
                    selections: ImmutableArray.Create(
                        AccessibilitySelection.Create(
                            code: "ACC1_SEL1",
                            label: "Selection 1"),
                        AccessibilitySelection.Create(
                            code: "ACC1_SEL2",
                            label: "Selection 2"))),
            };

            familyResource = new AccessibilityFamilyResource(
                resourceCode: "TDS_CC",
                selections: ImmutableArray<AccessibilityFamilySelection>.Empty,
                disabled: false);

            globalResource = new AccessibilityResource(
                resourceCode: "TDS_CC",
                currentSelectionCode: "TDS_CC0",
                order: 5,
                defaultSelection: "TDS_CC0",
                selections: ImmutableArray.Create(
                    new AccessibilitySelection("TDS_CC0", "Black on White", 2, false),
                    new AccessibilitySelection("TDS_CCInvert", "Reverse Contrast", 2, false),
                    new AccessibilitySelection("TDS_CCMagenta", "Black on Rose", 2, false),
                    new AccessibilitySelection("TDS_CCMedGrayLtGray", "Medium Gray on Light Gray", 2, false)),
                label: "globalResource",
                description: "globalResource",
                disabled: false,
                resourceType: "globalResource Type");
        }
      
        #region ToAccessibilityResourceTests
        /// <summary>
        /// Tests that a global accessibility resource copy is not modified 
        /// from the original given a partial accessibility resource that 
        /// contains all of the select elements
        /// </summary>
        [Fact] public void TestToAccessibilityResourceNoChanges()
        {
            AccessibilityResource globalResource = Resources[1];
            AccessibilityFamilyResource partialResource = new AccessibilityFamilyResource(
                resourceCode: "ACC1",
                selections: ImmutableArray.Create(
                    AccessibilityFamilySelection.Create(code: "ACC2_SEL1"),
                    AccessibilityFamilySelection.Create(code: "ACC2_SEL2")),
                disabled: false);

            AccessibilityResource outputResource = AccessibilityResourceTranslation
                .MergeGlobalResource(partialResource, globalResource);

            Assert.Equal(globalResource.CurrentSelectionCode, outputResource.CurrentSelectionCode);
            Assert.Equal(globalResource.Disabled, outputResource.Disabled);
            Assert.Equal(globalResource.Label, outputResource.Label);
            Assert.Equal(globalResource.DefaultSelection, outputResource.DefaultSelection);
            Assert.Equal(globalResource.Selections.Count(), outputResource.Selections.Count());
        }

        /// <summary>
        /// Tests that a global accessibility resource copy is 
        /// only disabled given a partial accessibility resource 
        /// that contains all of the select elements
        /// </summary>
        [Fact]
        public void TestToAccessibilityDisabledResource()
        {
            AccessibilityResource globalResource = Resources[1];
            AccessibilityFamilyResource partialResource = new AccessibilityFamilyResource(
                resourceCode: "ACC2",
                disabled: true,
                selections: ImmutableArray.Create(
                    AccessibilityFamilySelection.Create(code: "ACC2_SEL1"),
                    AccessibilityFamilySelection.Create(code: "ACC2_SEL2")));

            AccessibilityResource outputResource = AccessibilityResourceTranslation.MergeGlobalResource(partialResource, globalResource);

            Assert.Equal(globalResource.CurrentSelectionCode, outputResource.CurrentSelectionCode);
            Assert.True(outputResource.Disabled);
            Assert.Equal(globalResource.Label, outputResource.Label);
            Assert.Equal(string.Empty, outputResource.DefaultSelection);
            Assert.Equal(globalResource.Selections.Length, outputResource.Selections.Length);

            foreach (var sel in outputResource.Selections)
            {
                Assert.True(sel.Disabled);
            }
        }


        /// <summary>
        /// Tests that a global accessibility resource copy's selections 
        /// are disabled given a partial accessibility resource that does not 
        /// contain any select elements
        /// </summary>
        [Fact]
        public void TestToAccessibilityDisabledAllSelections()
        {
            AccessibilityResource globalResource = Resources[1];
            AccessibilityFamilyResource partialResource = new AccessibilityFamilyResource(
            resourceCode: "ACC2",
            selections: ImmutableArray<AccessibilityFamilySelection>.Empty,
            disabled: false);

            AccessibilityResource outputResource = AccessibilityResourceTranslation
                .MergeGlobalResource(partialResource, globalResource);

            Assert.Equal(globalResource.CurrentSelectionCode, outputResource.CurrentSelectionCode);
            Assert.Equal(false, outputResource.Disabled);
            Assert.Equal(globalResource.Label, outputResource.Label);
            Assert.Equal(string.Empty, outputResource.DefaultSelection);
            Assert.Equal(globalResource.Selections.Count(), outputResource.Selections.Count());
            foreach (var sel in outputResource.Selections)
            {
                Assert.Equal(true, sel.Disabled);
            }
        }

        /// <summary>
        /// Tests that a some of a global accessibility resource copy's 
        /// selections are disabled given a partial accessibility resource 
        /// that only contains one select element. Also check that default
        /// AccessibilityResource selection is updated
        /// </summary>
        [Fact]
        public void TestToAccessibilityDisabledSomeSelections()
        {
            AccessibilityResource globalResource = Resources[1];
            AccessibilityFamilyResource partialResource = new AccessibilityFamilyResource(
            resourceCode: "TDS_CC",
            disabled: false,
            selections: ImmutableArray.Create(AccessibilityFamilySelection.Create(code: "ACC2_SEL1")));

            AccessibilityResource outputResource = AccessibilityResourceTranslation
                .MergeGlobalResource(partialResource, globalResource);

            Assert.Equal(globalResource.CurrentSelectionCode, outputResource.CurrentSelectionCode);
            Assert.Equal(false, outputResource.Disabled);
            Assert.Equal(globalResource.Label, outputResource.Label);

            // Check that default selection was also updated
            Assert.Equal("ACC2_SEL1", outputResource.DefaultSelection); 

            Assert.Equal(globalResource.Selections.Length, outputResource.Selections.Length);
            Assert.Equal(false, outputResource.Selections[0].Disabled);
            Assert.Equal(true, outputResource.Selections[1].Disabled);
        }

        #endregion

        #region ToAccessibilityResourcesTests
        /// <summary>
        /// Tests translation of AccessibilityResources given a family's 
        /// resources, where the resources are not in the family's resources.
        /// </summary>
        [Fact]
        public void TestToAccessibilityResourcesNotModified()
        {
            ImmutableArray<AccessibilityFamilyResource> noPartialResources = ImmutableArray.Create<AccessibilityFamilyResource>();
            ImmutableArray<string> subject = new ImmutableArray<string>();
            AccessibilityFamily noPartialResourcesFamily = new AccessibilityFamily(
            subjects: subject,
            grades: GradeLevels.NA,
            resources: noPartialResources);
            var resultResources = AccessibilityResourceTranslation.MergeGlobalResources(noPartialResourcesFamily, Resources);

            Assert.Equal(Resources.Count, resultResources.Resources.Length);
        }


        /// <summary>
        /// Tests translation of AccessibilityResource given a family's 
        /// resources, where the resource is not in the family's resources.
        /// </summary>
        [Fact]
        public void TestToAccessibilityResourceNotModified()
        {
            ImmutableArray<AccessibilityFamilyResource> resource = ImmutableArray.Create<AccessibilityFamilyResource>();
            AccessibilityFamilyResource familyResource = new AccessibilityFamilyResource(
             resourceCode: "ACC1",
             selections: ImmutableArray<AccessibilityFamilySelection>.Empty,
             disabled: false);
            resource.Add(familyResource);

            AccessibilityFamily noPartialResources = new AccessibilityFamily(
                subjects: new ImmutableArray<string>(),
                grades: GradeLevels.NA,
                resources: resource);
            AccessibilityResource inputResource = Resources[1];
            List<AccessibilityResource> inputResources = new List<AccessibilityResource>
            {
                inputResource
            };

            var resultResources = AccessibilityResourceTranslation.MergeGlobalResources(noPartialResources, inputResources);
            Assert.Equal(inputResources.Count, resultResources.Resources.Count());

            AccessibilityResource outputResource = resultResources.Resources[0];

            Assert.Equal(inputResource.CurrentSelectionCode, outputResource.CurrentSelectionCode);
            Assert.Equal(inputResource.Description, outputResource.Description);
            Assert.Equal(inputResource.Disabled, outputResource.Disabled);
            Assert.Equal(inputResource.DefaultSelection, outputResource.DefaultSelection);
            Assert.Equal(inputResource.Order, outputResource.Order);
            Assert.Equal(inputResource.Selections.Count(), outputResource.Selections.Length);
        }

        /// <summary>
        /// Tests that global resources are passed through an empty list
        /// of partial resources in mergeAllWith()
        /// </summary>
        [Fact]
        public void TestToAccessibilityResourcesMatchingPartialResource()
        {
            ImmutableArray<AccessibilityFamilyResource> resource = ImmutableArray.Create<AccessibilityFamilyResource>();
            AccessibilityFamily noPartialResources = new AccessibilityFamily(
                subjects: new ImmutableArray<string>(),
                grades: GradeLevels.NA,
                resources: resource);
            var resultResources = AccessibilityResourceTranslation.MergeGlobalResources(noPartialResources, Resources);
            Assert.Equal(Resources.Count, resultResources.Resources.Length);
        }

        [Fact]
        public void TestMergeWithThrowsOnNulls()
        {
            Assert.Throws<ArgumentNullException>(() => AccessibilityResourceTranslation.MergeGlobalResource(familyResource, null));
            Assert.Throws<ArgumentNullException>(() => AccessibilityResourceTranslation.MergeGlobalResource(null, globalResource));
            Assert.Throws<ArgumentNullException>(() => AccessibilityResourceTranslation.MergeGlobalResource(null, null));
        }
        #endregion

        #region ApplyFlags
        [Fact]
        public void TestApplyAslFlag()
        {
            var itemDigest = new ItemDigest()
            {
                AslSupported = false,
                AllowCalculator = false
            };

            var resModified = aslResource.ApplyFlags(itemDigest);

            Assert.NotNull(resModified);
            Assert.Equal(resModified.Disabled, true);
        }

        [Fact]
        public void TestApplyAslFlagFalse()
        {
            var itemDigest = new ItemDigest()
            {
                AslSupported = true,
                AllowCalculator = false
            };

            var resModified = aslResource.ApplyFlags(itemDigest);

            Assert.NotNull(resModified);
            Assert.Equal(resModified.Disabled, false);
        }

        [Fact]
        public void TestApplyCalculatorFlagTrue()
        {
            var itemDigest = new ItemDigest()
            {
                AslSupported = false,
                AllowCalculator = true
            };

            var resModified = calculatorResource.ApplyFlags(itemDigest);

            Assert.NotNull(resModified);
            Assert.Equal(resModified.Disabled, false);
        }

        [Fact]
        public void TestApplyCalculatorFlagFalse()
        {
            var itemDigest = new ItemDigest()
            {
                AslSupported = true,
                AllowCalculator = false
            };

            var resModified = calculatorResource.ApplyFlags(itemDigest);

            Assert.NotNull(resModified);
            Assert.Equal(resModified.Disabled, true);
        }

        #endregion
    }
}
