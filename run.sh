#!/bin/bash

copycsv () {
	echo "copying result csv to local.."
	docker cp fqube:/tmp/sonartemp/output ./
}

detach () {
	echo "restarting farsqube.."
	docker stop fqube
	docker start fqube
	docker pause fqube
}

postprocess () {
	detach
	copycsv
}

rerun () {
	detach

	docker cp repositories.yml fqube:/opt/analyser/CodeAnalyzer/CodeAnalyzer.ConsoleApplication/Data/repositories.yml
	docker cp repositories.yml fqube:/opt/analyser/CodeAnalyzer/CodeAnalyzer.ConsoleApplication/bin/Release/netcoreapp3.1/Data/repositories.yml

	echo "running farsqube.."
	docker stop fqube
	docker start -i fqube

	postprocess
}

echo "setting up networks"
docker network create farsnet

simg=$(docker images | grep "sonarqube")
srn=$(docker ps -a | grep "snrqube")
scr=$(docker ps | grep "snrqube")

if [[ $srn == "" ]]
then
	echo "configuring sonarqube.."
	docker run -d -p 9000:9000 --network="farsnet" --network-alias="sonar" --name snrqube sonarqube
fi

if [[ $scr == "" && $srn != "" ]]
then
	docker start snrqube
fi

if [[ $scr == "" || $img != "" ]]
then
	echo "ensuring Sonarqube gets running before starting dotnet container.."
	echo "WAITING 80 SECONDS.."
	sleep 80
fi

bld=$(docker images | grep farsqube)
rn=$(docker ps -a | grep fqube)

if [[ $bld == "" ]]
then
	echo "building farsqube.."
	docker build -t farsqube .
fi

if [[ $rn == "" ]]
then
	echo "running farsqube.."
	docker run --network="farsnet" --name fqube farsqube

	postprocess
else
	rerun
fi

echo "stopping farsqube.."
docker stop fqube
