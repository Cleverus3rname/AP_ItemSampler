Docker
=======================================

Publish Docker to AWS
~~~~~~~~~~~~~~~~~~~~~

1. Go to Amazon ECS
2. Select Repositories
3. Select a repository or create
4. Select push Commands
5. Follow the push Commands or follow:

   1. Go to the root directory containing Dockerfile
   2. Run ``aws ecr get-login --region us-west-2``
   3. Run the docker login command
   4. Run ``docker build -t {repo-name}:{latest/dev/stage/prod} .``
   5. Run
      ``docker tag {repo-name}:{latest/dev/stage/prod} {amazon-repo}:{latest/dev/stage/prod}``
   6. Run ``docker push {amazon-repo}:{latest/dev/stage/prod}``

Publish Docker to DockerHub
~~~~~~~~~~~~~~~~~~~~~~~~~~~

1. Go to the root directory containing Dockerfile
2. Run ``docker login``
3. Run ``docker build -t {repo-name}:{latest/dev/stage/prod} .``
4. Run ``docker push {repo-name}:{latest/dev/stage/prod}``

Docker Commands
~~~~~~~~~~~~~~~

To update a docker image, please follow publish to Aws or DockerHub
