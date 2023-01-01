ver = 0.1

build_toolkit_image:
	docker build -f toolkit.Dockerfile -t vsp/toolkit:${ver} .

up_locally:
	build_toolkit_image
	docker compose build
	docker compose up

down_locally:
	docker compose down

cleanup_locally:
	docker rmi vsp/telegram-bot:{ver} vsp/reporter:{ver} vsp/collector:{ver} vsp/collector:{ver} vsp/scanner:{ver}

cleanup_all_locally:
	down_locally
	docker rmi vsp/telegram-bot:{ver} vsp/reporter:{ver} vsp/collector:{ver} vsp/collector:{ver} vsp/scanner:{ver}
	docker rmi vsp/toolkit:{ver}
	docker rmi rabbitmq:3.8-management
	