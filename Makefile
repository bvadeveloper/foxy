# check also version in docker-compose.yaml file
foxy_image_ver = 0.1
rabbit_image_ver = 3.8

up: build_toolkit
	docker compose build
	docker compose up

build_toolkit:
	docker build -f toolkit.Dockerfile -t vsp/toolkit:$(foxy_image_ver) .

down:
	docker compose down

cleanup: down
	docker rmi vsp/telegram-bot:$(foxy_image_ver) vsp/reporter:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/scanner:$(foxy_image_ver)

cleanup_all: cleanup
	docker rmi vsp/toolkit:$(foxy_image_ver)
	docker rmi rabbitmq:$(rabbit_image_ver)-management