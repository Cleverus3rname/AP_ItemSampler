import { AdvancedFilterCategoryModel, parseQueryString } from "@osu-cass/sb-components";

export function updateUrl(filters: AdvancedFilterCategoryModel[]) {
    let pairs: string[] = [];

    filters.forEach(f => {
        if (!f.disabled) {
            const key = f.label.toLowerCase().replace(/ /g, "-");
            const value = f.filterOptions.filter(o => o.isSelected).map(o => o.key).join(",");
            if (value !== "") {
                pairs.push(key + "=" + value);
            }
        }
    });

    let query: string;
    if (pairs.length === 0) {
        query = "";
    } else {
        query = "?" + pairs.join("&");
    }
    history.replaceState(null, "", window.location.pathname + query);
}

export function readUrl(filters: AdvancedFilterCategoryModel[]) {
    const queryObject = parseQueryString(window.location.href);
    for (let a in queryObject) {
        queryObject[a.replace(/-/g, " ")] = queryObject[a];
    }

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