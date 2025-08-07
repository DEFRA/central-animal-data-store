declare -a arr=(
            "localstack-emulator"
            "casapilocaldevelopmentenvironment-redis-1"
            "casapilocaldevelopmentenvironment-mongodb-1"
    )

for contName in "${arr[@]}"
do
   docker stop $contName
   docker rm $contName
done