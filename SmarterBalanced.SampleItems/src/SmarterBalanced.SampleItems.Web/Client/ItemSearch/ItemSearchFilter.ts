import { AdvancedFilterCategoryModel, ItemCardModel, gradeLevelContains } from '@osu-cass/sb-components';

export function filterItems(filter: AdvancedFilterCategoryModel[], itemCards: ItemCardModel[]) {

    const gradeCategory = filter.find(c => c.label.toLowerCase() === 'grades');
    if (gradeCategory && !gradeCategory.disabled) {
        const selectedGrades = gradeCategory.filterOptions.filter(o => o.isSelected).map(o => Number(o.key));
        if (selectedGrades && selectedGrades.length !== 0) { // accounting for all btn being pressed
            const combinedGrades = selectedGrades.reduce((prev, curr) => prev | curr);
            itemCards = itemCards.filter(card => gradeLevelContains(combinedGrades, card.grade));
        }
    }

    

    const subjectsCategory = filter.find(c => c.label.toLowerCase() === 'subjects');
    if (subjectsCategory && !subjectsCategory.disabled) {
        const selectedSubjects = subjectsCategory.filterOptions.filter(o => o.isSelected).map(s => s.key);
        if (selectedSubjects && selectedSubjects.length !== 0) {
            itemCards = itemCards.filter(card => selectedSubjects.indexOf(card.subjectCode) !== -1);
        }
    }

    ////claim category not completely implemented yet
    //const claimCategory = filter.find(c => c.label.toLowerCase() === 'claim');
    //if (claimCategory && !claimCategory.disabled) {
    //    const selectedClaims = claimCategory.filterOptions.filter(o => o.isSelected).map(s => s.key);
    //    if (selectedClaims && selectedClaims.length !== 0) {
    //        itemCards = itemCards.filter(card => selectedClaims.indexOf(card.claimCode) !== -1);
    //    }
    //}

    ////target category not completely implemented yet
    ////card filtering is based of targetHash
    //const targetCategory = filter.find(c => c.label.toLowerCase() === 'target');
    //if (targetCategory && !targetCategory.disabled) {
    //    const selectedTargets = targetCategory.filterOptions.filter(o => o.isSelected).map(s => s.key);
    //    if (selectedTargets && targetCategory.length !== 0) {
    //        itemCards = itemCards.filter(card => selectedTargets.indexOf(card.targetHash) !== -1);
    //    }
    //}

    //calculator category doesnt have options in card right now.

    
    return itemCards;
}