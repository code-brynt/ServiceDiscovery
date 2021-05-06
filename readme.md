#Service Discovery for .Net 5 using consul

## Docker
docker pull consul

 docker run  -p 8500:8500 -p 8600:8600/udp --name=dev-consul consul:v0.6.4 agent -server -bootstrap -ui -client=0.0.0.0

docker exec -t dev-consul consul members


docker run --name=agent-consul  -d consul agent --retry-join=172.17.0.2 

docker ps

docker exec -it <container ID> consul members

## Consul UI
http://localhost:8500/ui/dc1/services






