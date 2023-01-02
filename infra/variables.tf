variable "app_count" {
  type    = number
  default = 1
}

variable "aws_region" {
  type        = string
  description = "The region for resources"
  default     = "us-east-1"
}