using SmarterBalanced.SampleItems.Dal.Exceptions;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public static class AboutThisItemViewModelTranslations
    {
        public static AboutThisItemViewModel FromSampleItem(
            SampleItem sampleItem,
            ImmutableArray<ItemCardViewModel> itemCards,
            ImmutableArray<SampleItem> allSampleItems)
        {
            if (sampleItem == null) return null;
            if (itemCards.IsEmpty) throw new SampleItemsContextException("item cards cannot be empty");
            if (allSampleItems.IsEmpty) throw new SampleItemsContextException("sample items cannot be empty");

            var itemCardViewModel = itemCards
                .FirstOrDefault(card => card.BankKey == sampleItem.BankKey && card.ItemKey == sampleItem.ItemKey);
            var aboutThisItemViewModel = new AboutThisItemViewModel(
                rubrics: sampleItem.Rubrics,
                itemCard: itemCardViewModel,
                targetDescription: sampleItem.CoreStandards?.Target.Descripton,
                depthOfKnowledge: sampleItem.DepthOfKnowledge,
                commonCoreStandardsDescription: sampleItem.CoreStandards?.CommonCoreStandardsDescription,
                educationalDifficulty: sampleItem.EducationalDifficulty,
                evidenceStatement: sampleItem.EvidenceStatement,
                associatedItems: GetAssociatedItems(sampleItem, allSampleItems));

            return aboutThisItemViewModel;
        }

        public static string GetAssociatedItems(SampleItem item, ImmutableArray<SampleItem> allSampleItems)
        {
            if (item == null) return null;
            if (allSampleItems.IsEmpty) throw new SampleItemsContextException("sample items cannot be empty");

            var associatedItems = allSampleItems
                .Where(i => i.IsPerformanceItem &&
                    i.FieldTestUse != null &&
                    i.AssociatedStimulus == item.AssociatedStimulus &&
                    i.Grade == item.Grade && i.Subject?.Code == item.Subject?.Code)
                .OrderBy(i => i.FieldTestUse?.Section)
                .ThenBy(i => i.FieldTestUse?.QuestionNumber)
                .Select(i => i.ToString())
                .ToList();

            return (associatedItems.Count == 0) ? item.ToString() : String.Join(",", associatedItems);
        }
    }
}
