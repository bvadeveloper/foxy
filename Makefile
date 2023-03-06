# check also version in docker-compose.yaml file
foxy_image_ver = 0.1
rabbit_image_ver = 3.8

docker-up: toolkit-build
	docker compose up -d --build

docker-down:
	docker compose down

docker-cleanup: docker-down
	docker rmi $(docker images vsp/* -q)
	docker rmi rabbitmq:$(rabbit_image_ver)-management
	docker rmi redis

debug-up: 
	docker compose -f docker-compose-debug.yml up -d --build

debug-down:
	docker compose -f docker-compose-debug.yml down

toolkit-build:
	docker build -f toolkit.Dockerfile -t vsp/toolkit:$(foxy_image_ver) .

toolkit-cleanup:
	docker rmi vsp/toolkit:$(foxy_image_ver)

