#!/bin/bash

docker stop $(docker ps -a -q)

docker kill $(docker ps -a -q)

docker rm $(docker ps -a -q)

# COMPLETE CLEANUP

docker system prune -af

docker network prune -f
