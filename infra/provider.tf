terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
  
  // todo: move backend to s3
  backend "local" {
    path = "foxy.tfstate"
  }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Name = "foxy-aws"
    }
  }
}