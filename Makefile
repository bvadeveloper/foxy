# check also version in docker-compose.yaml file
foxy_image_ver = 0.1
rabbit_image_ver = 3.8

docker-up: toolkit-build
	docker compose build
	docker compose up

docker-down:
	docker compose down

docker-cleanup: docker-down
	docker rmi vsp/telegram-bot:$(foxy_image_ver) vsp/reporter:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/scanner:$(foxy_image_ver)
	docker rmi rabbitmq:$(rabbit_image_ver)-management
	docker rmi redis

debug-up: 
	docker compose -f docker-compose-debug.yml up -d

debug-down:
	docker compose -f docker-compose-debug.yml down

toolkit-build:
	docker build -f toolkit.Dockerfile -t vsp/toolkit:$(foxy_image_ver) .

toolkit-cleanup:
	docker rmi vsp/toolkit:$(foxy_image_ver)

