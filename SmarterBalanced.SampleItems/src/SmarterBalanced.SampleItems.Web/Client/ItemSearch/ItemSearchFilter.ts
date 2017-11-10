import { AdvancedFilterCategoryModel, ItemCardModel, gradeLevelContains } from '@osu-cass/sb-components';

export function filterItems(filter: AdvancedFilterCategoryModel[], itemCards: ItemCardModel[]) {
    const gradeCategory = filter.find(c => c.label.toLowerCase() === 'grades');
    if (gradeCategory && !gradeCategory.disabled) {
        const selectedGrades = gradeCategory.filterOptions.filter(o => o.isSelected).map(o => Number(o.key)).reduce((prev, curr) => prev | curr);
        if (selectedGrades) {
            itemCards = itemCards.filter(card => gradeLevelContains(selectedGrades, card.grade));
        }
    }

    const subjectsCategory = filter.find(c => c.label.toLowerCase() === 'subjects');
    if (subjectsCategory && !subjectsCategory.disabled) {
        const selectedSubjects = subjectsCategory.filterOptions.filter(o => o.isSelected).map(s => s.key);
        if (selectedSubjects) {
            itemCards = itemCards.filter(card => selectedSubjects.indexOf(card.subjectCode) !== -1);
        }
    }

    
    return itemCards;
}