Application Settings and Configurations
=======================================

Within the ``SmarterBalanced.SampleItems.Web`` project, configurations
are set in the file ``appsettings.json``. This file contains all of the
configurations and settings that are used throughout the application.

Within ``appsettings.json``, the ``SettingsConfig`` object contains
configurations necessary to start the application, as well as runtime
configuration settings.

Note: Following is a description of important configurations within this object:

``ContentItemDirectory``: Location of the **Items** directory within the
content package. Dependent on deployment environment setting
(development, staging, production).

``ContentRootDirectory``: Location of the content package. Dependent on
deployment environment setting (development, staging, production).

``AwsS3Bucket``: AWS S3 bucket that contains the content package.
Required for the diagnostic status feature.

``AwsRegion``: AWS region. Required for the diagnostic status feature.

``ItemViewerServiceURL``: Base URL for itemviewerservice, which renders
the test question items. URL is used to display an iframe of each item.

``AwsClusterName``: Name of AWS cluster. Required for the diagnostic
status feature.

``StatusUrl``: Diagnostic status URL for local diagnostic status.
Required for the diagnostic status feature.

``AccommodationsXMLPath``: Location of the accessibility configurations
XML document.

``InteractionTypesXMLPath``: Location of the interaction types
configuration XML document.

``ClaimsXMLPath``: Location of the claims configuration XML document.

Additionally, the ``RubricPlaceHolderText`` object contains strings that
are filtered out of item