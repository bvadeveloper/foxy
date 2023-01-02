terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}
provider "aws" {
  region     = "us-east-2"
  access_key = "<your_aws_access_key>"
  secret_key = "<your_aws_secret_key>"

  default_tags {
    tags = {
      Name = "foxy-aws"
    }
  }
}