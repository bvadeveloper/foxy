# check also version in docker-compose.yaml file
foxy_image_ver = 0.1
rabbit_image_ver = 3.8

up_local: build_toolkit_image
	docker compose build
	docker compose up

build_toolkit_image:
	docker build -f toolkit.Dockerfile -t vsp/toolkit:$(foxy_image_ver) .

down_local:
	docker compose down

clean_local: down_local
	docker rmi vsp/telegram-bot:$(foxy_image_ver) vsp/reporter:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/collector:$(foxy_image_ver) vsp/scanner:$(foxy_image_ver)

clean_all_local: down_local clean_local
	docker rmi vsp/toolkit:$(foxy_image_ver)
	docker rmi rabbitmq:$(rabbit_image_ver)-management