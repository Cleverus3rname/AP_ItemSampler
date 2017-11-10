Dependencies
=======================================

NPM
~~~

NPM is required to install Grunt, Grunt packages, Less, and TypeScript.
See ``package.json`` in the **Web** project for configurations.

Bower
~~~~~

Bower is required to add Bootstrap, JQuery, and React. See
``bower.json`` in the **Web** project for configurations.

Grunt
~~~~~

Grunt is used to perform tasks with Visual Studio events as well as with
build events. Grunt:

-  Compiles TypeScript files into JavaScript on save and build
-  Compiles Less CSS into CSS on save and build
-  Minifies CSS on build
-  Removes temp files

For Grunt configurations, see ``Gruntfile.js`` in the **Web** project.

External Dependencies
---------------------

Docker
~~~~~~

Docker is used to simplify and unify the build and runtime environment
for the application.

See ``Dockerfile`` in the **Web** project for Docker configurations. The
**Web** project also contains a ``.dockerignore``.

TravisCI
~~~~~~~~

TravisCI is used to verify the state of the application every time a
developer pushes to a branch in the GitHub repository. It first installs
project dependencies, pulls and builds the project, and then runs tests.

After the previous steps succeed, Travis builds a Docker image with the
application and pushes it to AWS.

On the **dev**, **stage**, and **master** branches, Travis triggers a
build process in AWS that combines the content package, hosted in an S3
bucket, with the application Docker image and performs a rolling update
to the running versions of these branches.

See the ``travis.yml`` file in the root directory project for
configurations.

Content package
~~~~~~~~~~~~~~~

The content for the site is provided by a content package supplied by
the Smarter Balanced Assessment Consortium. It consists of XML files
that describe the test question items (the "content package").

Currently, content packages can be accessed at
ftp://ftps.smarterbalanced.org/~sbacpublic/Public/PracticeAndTrainingTests/.