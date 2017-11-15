Application Overview
=======================================

The Sample Items Website is a Microsoft ASP.NET Core MVC web application
that displays sample test question items.

License
-------

Mozilla Public License Version 2.0

Installation
------------

-  Clone the project

   -  Don't forget to initialize the project submodules
      (`instructions`_)

-  Install the latest version of Microsoft .NET Core for your operating
   system (`link`_)

-  Install the latest TypeScript compiler
   (`link <https://www.typescriptlang.org/index.html#download-links>`__)

-  Download the latest sample item content package (TODO: Link?)

   -  Place this in the location specified in src/Web/appsettings.json
      (``ContentRootDirectory``)

Project Architecture
--------------------

The Sample Items Website project is composed of three layers: Web, Core,
and Dal.

**Web** is the startup project and contains controllers, views, style,
and other web dependencies. See the **Dependencies** section for more
information.

**Core** provides business logic to Web via repositories. It also houses
the business logic for the diagnosticAPI and model translations.

**Dal** provides data to the Core layer. All data is supplied by a
"content package" which is a set of XML files that represent test
question items. This XML is parsed into the immutable ``ItemDigest`` and
``ItemCard`` models, which are used throughout the application.

Other XML data is used, such as
``AccessibilityAccommodationConfigurations``, ``ClaimConfigurations``,
and ``InteractionTypeConfigurations``. These configure information such
as order and labels for accessibility and interaction types.

**Test** is the test project for Sample Items Website. It contains unit
tests and integration tests for each of the three aforementioned layers.

.. _instructions: https://git-scm.com/book/en/v2/Git-Tools-Submodules#_cloning_submodules
.. _link: https://www.microsoft.com/net/download/core#/current