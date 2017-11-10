import { AdvancedFilterCategory } from '@osu-cass/react-advanced-filter';
import { ItemCardViewModel } from '../ItemCard/ItemCardModels';
import * as GradeLevels from '../GradeLevels/GradeLevels';

export function filterItems(filter: AdvancedFilterCategory[], itemCards: ItemCardViewModel[]) {
    const gradeCategory = filter.find(c => c.label.toLowerCase() === 'grades');
    if (gradeCategory && !gradeCategory.disabled) {
        const selectedGrades = gradeCategory.filterOptions.filter(o => o.isSelected).map(o => Number(o.key)).reduce((prev, curr) => prev | curr);
        if (selectedGrades) {
            itemCards = itemCards.filter(card => GradeLevels.contains(selectedGrades, card.grade));
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