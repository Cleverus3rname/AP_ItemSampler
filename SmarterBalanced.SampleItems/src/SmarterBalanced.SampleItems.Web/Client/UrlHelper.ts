import { AdvancedFilterOption, AdvancedFilterCategory } from "@osu-cass/react-advanced-filter";
import { parseQueryString } from './ApiModel';

export function updateUrl(filters: AdvancedFilterCategory[]) {
    let pairs: string[] = [];

    filters.forEach(f => {
        if (!f.disabled) {
            let pair = f.label.toLowerCase() + "=";
            pair += f.filterOptions.filter(o => o.isSelected).map(o => o.key).join(",");
            pairs.push(pair);
        }
    });

    let query: string;
    if (pairs.length === 0) {
        query = "/";
    } else {
        query = "?" + pairs.join("&");
    }
    history.replaceState(null, "", window.location.pathname + query);
}

export function readUrl(filters: AdvancedFilterCategory[]) {
    const queryObject = parseQueryString(window.location.href);

    const newFilters = filters.map(f => {
        if (!f.disabled) {
            let selectedKeys = queryObject[f.label.toLowerCase()] || [];

            if (f.isMultiSelect) {
                selectedKeys = selectedKeys[0] ? [selectedKeys[0]] : [];
            }

            const newFilterOptions = f.filterOptions.map(o => {
                return {
                    ...o,
                    isSelected: (selectedKeys.indexOf(o.key) !== -1)
                }
            });

            return {
                ...f,
                filterOptions: newFilterOptions
            };
        } else {
            return f;
        }
    });
    return newFilters;
}