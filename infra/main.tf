resource "aws_ecs_cluster" "main" {
  name = "foxy-cluster"
}

# create repository for docker images
resource "aws_ecr_repository" "foxy-repo" {
  name = "foxy-images"
}

# ---------> bot <--------- #
resource "aws_ecs_task_definition" "foxy-bot" {
  family                   = "foxy-bot"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 254
  memory                   = 512

  container_definitions = <<DEFINITION
[
  {
    "image": "<path to ECR image>",
    "cpu": 254,
    "memory": 512,
    "name": "foxy-bot",
    "networkMode": "awsvpc",
    "portMappings": [
      {
        "containerPort": 80,
        "hostPort": 8080
      }
    ]
  }
]
DEFINITION
}

resource "aws_security_group" "foxy-bot" {
  name   = "foxy-bot-task-security-group"
  vpc_id = aws_vpc.default.id

  ingress {
    protocol        = "tcp"
    from_port       = 8080
    to_port         = 8080
    security_groups = [aws_security_group.lb.id]
  }

  egress {
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_ecs_service" "foxy-bot" {
  name            = "foxy-bot-service"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.foxy-bot.arn
  desired_count   = var.app_count
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [aws_security_group.foxy-bot.id]
    subnets         = aws_subnet.private.*.id
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.foxy-bot.id
    container_name   = "foxy-bot-app"
    container_port   = 8080
  }

  depends_on = [aws_lb_listener.foxy-bot]
}

# ---------> collector <--------- #
resource "aws_ecs_task_definition" "foxy-collector" {
  family                   = "foxy-collector"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 254
  memory                   = 512

  container_definitions = <<DEFINITION
[
  {
    "image": "<path to ECR image>",
    "cpu": 254,
    "memory": 512,
    "name": "foxy-collector",
    "networkMode": "awsvpc",
    "portMappings": [
      {
        "containerPort": 80,
        "hostPort": 8081
      }
    ]
  }
]
DEFINITION
}

resource "aws_security_group" "foxy-collector" {
  name   = "foxy-collector-task-security-group"
  vpc_id = aws_vpc.default.id

  ingress {
    protocol        = "tcp"
    from_port       = 8081
    to_port         = 8081
    security_groups = [aws_security_group.lb.id]
  }

  egress {
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_ecs_service" "foxy-collector" {
  name            = "foxy-collector-service"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.foxy-collector.arn
  desired_count   = var.app_count
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [aws_security_group.foxy-collector.id]
    subnets         = aws_subnet.private.*.id
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.foxy-collector.id
    container_name   = "foxy-collector-app"
    container_port   = 8081
  }

  depends_on = [aws_lb_listener.foxy-collector]
}

# ---------> scanner <--------- #
resource "aws_ecs_task_definition" "foxy-scanner" {
  family                   = "foxy-scanner"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 254
  memory                   = 512

  container_definitions = <<DEFINITION
[
  {
    "image": "<path to ECR image>",
    "cpu": 254,
    "memory": 512,
    "name": "foxy-scanner",
    "networkMode": "awsvpc",
    "portMappings": [
      {
        "containerPort": 80,
        "hostPort": 8082
      }
    ]
  }
]
DEFINITION
}

resource "aws_security_group" "foxy-scanner" {
  name   = "foxy-scanner-task-security-group"
  vpc_id = aws_vpc.default.id

  ingress {
    protocol        = "tcp"
    from_port       = 8082
    to_port         = 8082
    security_groups = [aws_security_group.lb.id]
  }

  egress {
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_ecs_service" "foxy-scanner" {
  name            = "foxy-scanner-service"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.foxy-scanner.arn
  desired_count   = var.app_count
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [aws_security_group.foxy-scanner.id]
    subnets         = aws_subnet.private.*.id
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.foxy-scanner.id
    container_name   = "foxy-scanner-app"
    container_port   = 8082
  }

  depends_on = [aws_lb_listener.foxy-scanner]
}

# ---------> reporter <--------- #
resource "aws_ecs_task_definition" "foxy-reporter" {
  family                   = "foxy-reporter"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 254
  memory                   = 512

  container_definitions = <<DEFINITION
[
  {
    "image": "<path to ECR image>",
    "cpu": 254,
    "memory": 512,
    "name": "foxy-reporter",
    "networkMode": "awsvpc",
    "portMappings": [
      {
        "containerPort": 80,
        "hostPort": 8083
      }
    ]
  }
]
DEFINITION
}

resource "aws_security_group" "foxy-reporter" {
  name   = "foxy-reporter-task-security-group"
  vpc_id = aws_vpc.default.id

  ingress {
    protocol        = "tcp"
    from_port       = 8083
    to_port         = 8083
    security_groups = [aws_security_group.lb.id]
  }

  egress {
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_ecs_service" "foxy-reporter" {
  name            = "foxy-reporter-service"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.foxy-reporter.arn
  desired_count   = var.app_count
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [aws_security_group.foxy-reporter.id]
    subnets         = aws_subnet.private.*.id
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.foxy-reporter.id
    container_name   = "foxy-reporter-app"
    container_port   = 8083
  }

  depends_on = [aws_lb_listener.foxy-reporter]
}