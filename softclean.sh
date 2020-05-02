#!/bin/bash

# NOTE: This script only works after running run.sh
# Useful for doing a clean run
# Removes only project related stuff and intermediate containers

docker kill fqube snrqube

docker rm fqube snrqube

docker rmi farsqube

docker rmi $(docker images -f "dangling=true" -q)

rm -rf output/
