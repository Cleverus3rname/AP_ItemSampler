# Rubric and Rationale

## Content Package
Applies to item-{bankKey}-itemKey.xml files within the content directory that are items. There are two types of "scoring" information. Rubric and Rationale list. Rationale list seems to be multiple choice explanations of the correct/incorrect answer. Rubric list has two parts, samples and rubrics. Rationale list also has rationale and option lists. There can be many sets based on the language.

### Other Scoring Attributes
* `MachineRubric`: Specifies the machine rubric qrx file for automated scoring.
* `itm_att_Answer Key`

### Rubric List
Rubric contains rubric entries and sample entries. Rubric entries are listed for each possible point value.
There is usually many sample lists within a rubric list even though most sample lists contains one entry. Sample are example responses to get a certain point value. Rubrics are the requirements to achieve a specific point value.

* `content` > `rubriclist` > `rubric`
   * Score Point - entries point value
   * Name - specifies rubric name with point
   * Val - html of response
* `content` > `rubriclist` > `samplelist` > `sample`
   * Purpose
   * Name - specifies sample name with point value
   * Annotation * not mapped
   * Sample Content - html of explanation
   
### Rationale List
Multiple choice and rationale for each possible answer. Note:  Not mapped Explanation of Correct Answer and rationaloptlist

* Not Mapped - `content` > `rationaleoptlist` > `rationale` 
   * Name - each option value (a,b, ...)
   * Val - html of choice option (usually blank)
* `content` > `optionlist` > `option`
   * Name - each option value (a,b, ...)
   * Val - html of choice option
   * Feedback - html explanation of chosen answer

## Item Sampler Translation
`ItemDigest` is made up of two files, `ItemContents` and `ItemMetadata`. Item Digest is the merged result of the two files in the content package. `ItemContents` has `ItemXmlFieldRepresentation` of the Item. This has the list of `Content` which holds all Rubric and Rationale List. `SampleItem` is the final version of the translated content package.

### `ItemDigest` - `ItemContents` and `ItemMetadata`
* `rubriclist` from content is mapped to `RubricList`
   * `rubric` from content is mapped to `RubricEntry`
   * `samplelist` is mapped to `RubricSample`
* `optionlist` > `option` is mapped to `SmarterAppOption`

### `SampleItem` from `ItemDigest`
`ItemDigest` is translated to `SampleItemScore`. Please see code documentation for more information.
`SampleItemScore` combines rubric and rationale for easier consumption in the API. It also filters out placeholder text and identifies correct/incorrect options.
