# security group for ALB
resource "aws_security_group" "lb" {
  name   = "foxy-alb-security-group"
  vpc_id = aws_vpc.default.id

  ingress {
    protocol    = "tcp"
    from_port   = 8080
    to_port     = 8083
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# ALB
resource "aws_lb" "default" {
  name            = "foxy-lb"
  subnets         = aws_subnet.public.*.id
  security_groups = [aws_security_group.lb.id]
}

# bot target group
resource "aws_lb_target_group" "foxy-bot" {
  name        = "foxy-bot-target-group"
  port        = 8080
  protocol    = "HTTP"
  vpc_id      = aws_vpc.default.id
  target_type = "ip"
}

# bot listener
resource "aws_lb_listener" "foxy-bot" {
  load_balancer_arn = aws_lb.default.id
  port              = "8080"
  protocol          = "HTTP"

  default_action {
    target_group_arn = aws_lb_target_group.foxy-bot.id
    type             = "forward"
  }
}

# collector target group
resource "aws_lb_target_group" "foxy-collector" {
  name        = "foxy-collector-target-group"
  port        = 8081
  protocol    = "HTTP"
  vpc_id      = aws_vpc.default.id
  target_type = "ip"
}

# collector listener
resource "aws_lb_listener" "foxy-collector" {
  load_balancer_arn = aws_lb.default.id
  port              = "8081"
  protocol          = "HTTP"

  default_action {
    target_group_arn = aws_lb_target_group.foxy-collector.id
    type             = "forward"
  }
}

# scanner target group
resource "aws_lb_target_group" "foxy-scanner" {
  name        = "foxy-scanner-target-group"
  port        = 8082
  protocol    = "HTTP"
  vpc_id      = aws_vpc.default.id
  target_type = "ip"
}

# scanner listener
resource "aws_lb_listener" "foxy-scanner" {
  load_balancer_arn = aws_lb.default.id
  port              = "8082"
  protocol          = "HTTP"

  default_action {
    target_group_arn = aws_lb_target_group.foxy-scanner.id
    type             = "forward"
  }
}

# reporter target group
resource "aws_lb_target_group" "foxy-reporter" {
  name        = "foxy-reporter-target-group"
  port        = 8083
  protocol    = "HTTP"
  vpc_id      = aws_vpc.default.id
  target_type = "ip"
}

# reporter listener
resource "aws_lb_listener" "foxy-reporter" {
  load_balancer_arn = aws_lb.default.id
  port              = "8083"
  protocol          = "HTTP"

  default_action {
    target_group_arn = aws_lb_target_group.foxy-reporter.id
    type             = "forward"
  }
}