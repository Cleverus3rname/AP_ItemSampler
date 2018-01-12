# Accessibility Resources

* See ISAAP [docs](http://www.smarterapp.org/documents/ISAAP-AccessibilityFeatureCodes.pdf)

## Terms

* Accessibility Family: Default accessibility Resources available to the item based on the item's grade and subject

* Accessibility Resource: The group of accessibility options.
   * Example: Dictionary, Calculator

* Accessibility Option: Accessibility feature code. An ISAAP code to enable/disable accessibility. 
   * Example: Calculator has Off and On options

* ISAAP: A unique code for accessibility. Each code has a `TDS` prefix.
   * See [docs](http://www.smarterapp.org/documents/ISAAP-AccessibilityFeatureCodes.pdf)


## AP_ItemViewerService

### Usage
Please see github repo [AP_ItemViewerService](https://github.com/SmarterApp/AP_ItemViewerService/blob/master/docs/Loading-Items.md)

### Dictionary and Thesaurus
This is an external api for merriam-webster and depends on a running instance of TDS_Dictionary. AP_ItemSampler does not have any overrides or connection settings for this instance.

## AP_ItemSampler usage

### Client (Web)
Item Page and About Test Items use the AP_ItemViewerService as an iframe. Item Sampler passes a list of items, need to be related, with isaap codes. These codes are generated within the accessibility modal using the local item's accessibility options. Items and Item's family accessibility can be loaded from the Item Sampler's api.

TODO: add API link

### DAL
Accessibility is loaded from xml sources from a git submodule [Github](https://github.com/osu-cass/AccessibilityAccommodationConfigurations). Item Sampler loads them as is then applies business logic to create a complete list of accessibility resources for each family. 

#### XML
Provides a global list of all accessibility resources, options, groups, and families. Families are a list of rules to apply to the global list for the subject and grade range. 

#### Accessibility Resource Groups
This is the label for the group. Labels are loaded from the AppSettings.json and matched on the xml group key. 

#### Merged Accessibility Family
This contains a list of accessibility resources for a range of grades and a subject. This contains the default options supported for a subject (ELA) and a grade range (3-5). Accessibility resource families from the xml contain rules to enable and disable options but not the options themselves. To simplify this process, merged accessibility families were created to execute the rules on the global list and contain the final result.


### Item Accessibility
Local item accessibility is calculated from the accessibility family resource, based on subject and grade, and the item metadata attributes. 

Item's that can have different accessibility than the family:

* Metadata attribute AllowCalculator
* Item types 
* Performance Task
* Thesaurus and Dictionary
* Braille (based on the ftp server resources)
* Metadata attachements (ASL)
* Claim

#### Calculator
Metadata can enable and disable calculator based on `AllowCalculator`. Calculator only shows Off and On for all math items. On refers to the item's specific calculator option based on grade.

#### Thesaurus
Can be disabled if calculator is not allowed/available and if the item type is not WER.

#### Braille
Can be enabled if the ftp server has a listing for the item

#### American Sign Language
Can be enabled if the item's contents has a ASL video attachment

#### Closed Captioning
Can be enabled if item is subject ELA and claim 3

#### Global Notes
Can be enabled if item is a performance task


## Options

### Universal Tools

#### Digital Notepad
Displays a notepad in the item's hamburger menu. This will open a dialog for a user to enter text and save. 

#### English Glossary
Provides glossary definition for an item's word list. This will show a dashed top and bottom border for words that have a glossary definition.

#### Highlighter
Allows highlighting the item and/or passage's text. To highlight, select text and right click or use the hamburger menu then select highlight text. To remove, select highlighted text and from the menu, select remove highlight.

#### English Dictionary
Provides a dialog to search the merriam-webster api for definitions. When enabled, a toolbar icon will display with text Dictionary.

#### Expandable Passages
Allows the passage to expand over the item. To enable, an icon will appear to the left of the hamburger menu for the passage, select the expand icon.

#### Global Notes
Provides a dialog to enter global notes for the user's session. When enabled, a toolbar icon will display.

#### Strikethrough
Allows strike-through text for the item and/or passage's text. To enable, select text and right click or use the hamburger menu and then select strikethrough. To remove, select the striked-through text and from the menu select remove strikethrough. (TODO: Verify name in menu.)

#### Thesaurus
Provides a dialog to search the merriam-webster api thesaurus. When enabled, a toolbar icon will display with text Dictionary. Dictionary enabled accessibility option is required for thesaurus.


#### Zoom
Increases/decreases text size for both passage and item. This should always be enabled. Two toolbar icons will appear.

### Designated Supports

#### Color Choices
Changes the background color and text color for the item and passage. 

#### Masking
Creates a overlay on top of the item and/or passage. A toolbar icon will display with text Masking. To enable, select the masking icon. To dismiss, select the x icon in the upper right of the masked overlay.

#### Translations (Glossaries)
Provides glossary definition for an item's word list. This will show a dashed top and bottom border for words that have a glossary definition for the selected language. 

#### Translations (Stacked)
Provides translated item and passage text.

### Accommodations

#### American Sign Language
Displays a dialog with an ASL video to describe item and passage. To enable, select the option from the hamburger menu.

#### Braille Type
Provides braille files for the item and/or passage to be printed. Above the item viewer, a new link will be displayed.

#### Closed Captioning
Displays a dialog on the bottom of the item viewer. TODO: add more info

#### Streamlined Interface
Changes the layout of the item and passage. When enabled, the passage will be above the item.


