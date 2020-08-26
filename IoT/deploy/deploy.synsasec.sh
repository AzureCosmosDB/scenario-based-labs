#!/bin/bash

# ====================
# Variables

subscription_id=""
location="eastus"
resource_group_name=""

template_file="deploy.synsasec.json"
deployment_name="deploy_synsasec"

# ====================

az group deployment create --subscription "$subscription_id" \
	-g "$resource_group_name" -n "$deployment_name" --template-file "$template_file" \
	--parameters location="$location" --verbose
