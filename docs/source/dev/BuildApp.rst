Build
=======================================

Sampleitems Website requires content before running.

There is a two-step process using docker. The base app without content
needs to be created as a docker image called code. Then we combine the
code image with the content package using another dockerfile.

Before starting build
~~~~~~~~~~~~~~~~~~~~~

Locate the dockerfile.stage and dockerfile.prod files from github repo,
navigate to deployScripts

Using a previous docker code image
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

To build a docker image for sample items website code base (without
content) see Building from scratch

Docker
^^^^^^

1. Navigate to directory containing dockerfiles
2. Get docker code repo for stage/prod, run
   ``docker pull reponame:{tag}``

   1. Example stage: run
      ``docker pull xxx.dkr.ecr.us-west-2.amazonaws.com/sampleitemscode:stage``
   2. Example producation: run
      ``docker pull xxx.dkr.ecr.us-west-2.amazonaws.com/sampleitemscode:prod``

3. Docker tag code, run
   ``docker tag reponame:{tag} sampleitemscode:{tag}``

   1. Example stage: run
      ``docker tag xxx.dkr.ecr.us-west-2.amazonaws.com/sampleitemscode:stage sampleitemscode:stage``
   2. Example production: run
      ``docker tag xxx.dkr.ecr.us-west-2.amazonaws.com/sampleitemscode:prod sampleitemscode:prod``

4. place content within deployScripts directory

   1. Content needs to be unzipped
   2. content directory root level needs Items directory

5. Docker build, run
   ``docker build -t sampleitemsapp:{tag} -f Dockerfile.{tag} .``

   1. Example stage: run
      ``docker build -t sampleitemsapp:stage -f Dockerfile.stage .``
   2. Example production: run
      ``docker build -t sampleitemsapp:prod -f Dockerfile.prod .``

6. Docker Run app , run
   ``docker run -it -p 8012:80 sampleitemsapp:{tag}``

   1. Example stage: run
      ``docker run -it -p 8012:80 sampleitemsapp:stage``
   2. Example production: run
      ``docker run -it -p 8012:80 sampleitemsapp:prod``

7. Go to `localhost:8012`_
8. point to docker

Building from scratch
~~~~~~~~~~~~~~~~~~~~~

Dependencies
^^^^^^^^^^^^

1. See `Project Dependencies`_
2. Install Nodejs and npm

Dotnet build and Publish
^^^^^^^^^^^^^^^^^^^^^^^^

1. ``cd SampleItemsWebsite\SmarterBalanced.SampleItems``
2. ``dotnet restore``
3. ``cd src\SmarterBalanced.SampleItems.Web``
4. ``npm install``
5. ``grunt all``
6. ``dotnet publish -o ../../publish``
7. ``cd ../../publish``

Docker build
^^^^^^^^^^^^

1. Go to the publish directory containing the dockerfile
2. Build app, run ``docker build -t sampleitemscode:{tag}``

   1. Example stage: run ``docker build -t sampleitemscode:stage``
   2. Example producation: run ``docker build -t sampleitemscode:prod``

Deploy Sample Items app
^^^^^^^^^^^^^^^^^^^^^^^

1. see `Publish Docker`_

.. _`localhost:8012`: http://localhost:8012
.. _Project Dependencies: #internal-dependencies
.. _Publish Docker: #publish-docker-to-aws