#!/bin/bash

# ====================
# Variables

subscription_id=""
location="eastus"
resource_group_name=""

ml_workspace_name=""

template_file="deploy.ml.json"
deployment_name="deploy_ml"

# ====================

az group deployment create --subscription "$subscription_id" \
	-g "$resource_group_name" -n "$deployment_name" --template-file "$template_file" \
	--parameters location="$location" --verbose

#echo -e "List workspaces"
#az ml workspace list -g "$resource_group_name"

#echo -e "List environments"
#az ml environment list -g $resource_group_name --workspace-name $ml_workspace_name

#echo -e "List services"
#az ml service list -g "$resource_group_name" -w "$ml_workspace_name"

#echo -e "List models"
#az ml model list -g "$resource_group_name" -w "$ml_workspace_name"

#echo -e "List endpoints"
#az ml endpoint realtime list -g $resource_group_name --workspace-name $ml_workspace_name

