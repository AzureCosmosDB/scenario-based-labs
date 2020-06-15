#!/bin/bash

# ====================
# Variables

subscription_id="220fc532-6091-423c-8ba0-66c2397d591b"
location="eastus"
resource_group_name="iot-lab-test-39"

template_file="demo.deploy.json"
deployment_name="deploy_iot_scenario_demo"

recipient_email="paelaz@microsoft.com"

# ====================

# Operations

echo "Create Resource Group"
az group create --subscription "$subscription_id" -n "$resource_group_name" -l "$location"

echo -e "\n"

echo "Deploy demo template"
az group deployment create --subscription "$subscription_id" --verbose \
	-g "$resource_group_name" -n "$deployment_name" --template-file "$template_file" \
	--parameters location="$location" recipientEmail="$recipient_email"

# ====================
