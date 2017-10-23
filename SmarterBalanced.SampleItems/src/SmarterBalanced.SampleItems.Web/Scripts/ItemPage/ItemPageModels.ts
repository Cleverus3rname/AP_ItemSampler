﻿import * as Accessibility from '../Accessibility/Accessibility';
import * as MoreLikeThis from '../Modals/MoreLikeThisModal';
import * as Dropdown from '../DropDown/DropDown';

export interface ItemIdentifier {
    itemName: string;
    bankKey: number;
    itemKey: number;
}

export interface ItemPageViewModel {
    itemViewerServiceUrl: string;
    itemNames: string;
    brailleItemNames: string;
    brailleItem: ItemIdentifier;
    nonBrailleItem: ItemIdentifier;
    currentItem: ItemIdentifier;
    accessibilityCookieName: string;
    isPerformanceItem: boolean;
    performanceItemDescription: string;
    subject: string;
    accResourceGroups: Accessibility.AccResourceGroup[];
    moreLikeThisVM: MoreLikeThis.Props;
    brailleItemCodes: string[];
    braillePassageCodes: string[];
}

export function toiSAAP(accResourceGroups: Accessibility.AccResourceGroup[]): string {
    let isaapCodes = "TDS_ITM1;TDS_APC_SCRUBBER;"; // always enable item tools menu
    for (let group of accResourceGroups) {
        for (let res of group.accessibilityResources) {
            if (res.currentSelectionCode && !res.disabled) {
                isaapCodes += res.currentSelectionCode + ";";
            }
        }
    }

    return encodeURIComponent(isaapCodes);
}

export function resetResource(model: Accessibility.AccessibilityResource): Accessibility.AccessibilityResource {
    const newModel = Object.assign({}, model);
    newModel.currentSelectionCode = model.defaultSelection;
    return newModel;
}

export function trimAccResource(resource: Accessibility.AccessibilityResource): { label: string, selectedCode: string } {
    return {
        label: resource.label,
        selectedCode: resource.currentSelectionCode,
    };
}

export function toCookie(accGroups: Accessibility.AccResourceGroup[]): string {
    let prefs: Accessibility.ResourceSelections = {};
    for (const group of accGroups) {
        for (const resource of group.accessibilityResources) {
            prefs[resource.resourceCode] = resource.currentSelectionCode;
        }
    }

    const json = JSON.stringify(prefs);
    const cookie = btoa(json);
    return cookie;
}

export function addDisabledPlaceholder(resource: Accessibility.AccessibilityResource): Accessibility.AccessibilityResource {
    if (resource.disabled) {
        let newSelection = { ...resource };
        let disabledOption: Dropdown.Selection = {
            label: "Disabled for item",
            selectionCode: "",
            disabled: true,
            order: 0,
            hidden: false
        };
        newSelection.selections.push(disabledOption);
        newSelection.currentSelectionCode = "";
        return newSelection;
    }
    return resource; function readCookie(name: string): string | undefined {
        var cookie = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
        return cookie ? cookie.pop() : '';
    }
}