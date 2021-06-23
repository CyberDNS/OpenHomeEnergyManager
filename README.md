# OpenHomeEnergyManager

!!! This is not working yet, because the IP op the backend container is "hardcoded" in the frontend container image. !!!

Prerequisites:
docker and docker-compose up and running

Create files:

docker-compose.yaml:

    version: '3.4'

    services:
      openhomeenergymanager.blazor:
        image: cyberdns/openhomeenergymanagerblazor
        environment:
          - ASPNETCORE_ENVIRONMENT=Development
        ports:
          - "5000:80"

      openhomeenergymanager.api:
        image: cyberdns/openhomeenergymanagerapi
        network_mode: "host"
        environment:
          - ASPNETCORE_ENVIRONMENT=Development
          - ASPNETCORE_URLS=http://+:5123
        volumes:
          - data:/app/data

    volumes:
      data:
      
update.sh

    #!/bin/bash

    docker-compose down
    docker-compose pull
    
start.sh

    #!/bin/bash

    docker-compose up -d
    docker logs openhomeenergymanager_openhomeenergymanager.api_1 -f
   
Run following:

    chmod +x start.sh
    chmod +x update.sh

    ./update.sh
    ./start.sh
    
Start a browser and go to: http://[your server]:5000
    

