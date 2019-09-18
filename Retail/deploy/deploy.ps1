function SetupStreamAnalytics($suffix)
{
    #https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-quick-create-powershell

    Connect-AzAccount -Subscription $subName

    $jobName = "s2_analytics_$suffix";

    #set the stream analytics inputs - TODO needs sharedaccesspolicykey...
    $jobInputName = "s2event"
    $jobInputDefinitionFile = "streamanaltyics_input_1.json"

    New-AzStreamAnalyticsInput -ResourceGroupName $rgName -JobName $jobName -File $jobInputDefinitionFile -Name $jobInputName;

    #set the stream analytics outputs (#1)
    $jobOutputName = "eventCount"
    $jobOutputDefinitionFile = "streamanaltyics_output_1.json"

    New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

    #set the stream analytics outputs (#2)
    $jobOutputName = "eventOrdersLastHour"
    $jobOutputDefinitionFile = "streamanaltyics_output_2.json"

    New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

    #set the stream analytics outputs (#3)
    $jobOutputName = "eventSummary"
    $jobOutputDefinitionFile = "streamanaltyics_output_3.json"

    New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

    #set the stream analytics outputs (#4)
    $jobOutputName = "failureCount"
    $jobOutputDefinitionFile = "streamanaltyics_output_4.json"

    New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

    #set the stream analytics outputs (#5)
    $jobOutputName = "userCount"
    $jobOutputDefinitionFile = "streamanaltyics_output_5.json"

    New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

    #set the stream analytics query
    $jobTransformationName = "s2_retail_job"
    $jobTransformationDefinitionFile = "streamanaltyics_query.json"

    New-AzStreamAnalyticsTransformation -ResourceGroupName $rgName -JobName $jobName -File $jobTransformationDefinitionFile -Name $jobTransformationName -Force

    #start the job
    Start-AzStreamAnalyticsJob -ResourceGroupName $rgName -Name $jobName -OutputStartMode 'JobStartTime'
}

function ConvertObject($data)
{
    $str = "";
    foreach($c in $data)
    {
        $str += $c;
    }

    return $str;
}

function ConvertObjectToJson($data)
{
    $json = ConvertObject $data;

    return ConvertFrom-json $json;
}

#################
#
# Run to get the lasest AZ powershell commands (for stream analytics)
#
#################
#Install-Module -Name Az -AllowClobber -Scope CurrentUser
#################
$mode = "lab"  #can be 'lab' or 'demo'
$subscriptionId = "8c924580-ce70-48d0-a031-1b21726acc1a"
$subName = "Solliance MPN 12K"
$suffix = "mi4tatni3b2y4"

$prefix = "s2_retail"
$rgName = $prefix;
$databaseId = "movie";

#register at https://api.themoviedb.org
$movieApiKey = "";

$res = az account list;

if ($res.contains("cloudName"))
{
    #login
    $subs = az login;    
}

#select the subscription if you set it
if ($subName)
{
    az account set --subscription $subName;
}

SetupStreamAnalytics $suffix;

return;

#create the resource group
$result = az group create --name $rgName --location "Central US"

#deploy the tempalte
$deployId = [System.Guid]::newguid().ToString().replace("-","");
#$result = $(az group deployment create --name $deployId --resource-group $rgName --mode Incremental --template-file labdeploy.json --output json #--parameters storageAccountType=Standard_GRS)

#wait for the job to complete...
$res = $(az group deployment list --resource-group $rgname --output json)

$json = ConvertObjectToJson $res;

$deployment = $json | where {$_.name -eq $deployId};

#check the status
$deployment.properties.provisioningState;

#get all the settings
$azurequeueConnString = "";
$paymentsApiUrl = "";
$funcApiUrl = "";
$funcApiKey = "";
$dbConnectionUrl = "";
$dbConnectionKey = "";

#get the storage connection string
$res = $(az storage account list --output json)
$json = ConvertObjectToJson $res;

$sa = $json | where {$_.name -eq "s2data3" + $suffix};

$res = $(az storage account keys list --account-name $sa.name)
$json = ConvertObjectToJson $res;

$key = $json[0].value;

$azurequeueConnString = "DefaultEndpointsProtocol=https;AccountName=$($sa.name);AccountKey=$($key);EndpointSuffix=core.windows.net";

########################
#
#get the cosmos db url and key
#
#########################
$res = $(az cosmosdb list --output json)
$json = ConvertObjectToJson $res;

$db = $json | where {$_.name -eq "s2cosmosdb" + $suffix};

$dbConnectionUrl = $db.documentEndpoint;

$res = $(az cosmosdb keys list --name $db.name --resource-group $rgName)
$json = ConvertObjectToJson $res;

$dbConnectionKey = $json.primaryMasterKey;

########################
#
#deploy the web app
#
#########################

$webAppName = "s2web" + $suffix;

az webapp deployment source config-zip --resource-group $rgName --name $webAppName --src "webapp.zip"

########################
#
#deploy the function
#
#########################

$funcAppName = "s2cosmosdb" + $suffix;

az functionapp deployment source config-zip --resource-group $rgName --name $funcAppName --src "functionapp.zip"

########################
#
#get the function url and key
#
#########################
$res = $(az functionapp list --output json)
$json = ConvertObjectToJson $res;

$func = $json | where {$_.name -eq "s2cosmosdb" + $suffix};

$funcApiUrl = "https://" + $func.defaultHostName;

#key is stored in the storage account...
$res = $(az storage blob list --connection-string $azurequeueConnString --container-name azure-webjobs-secrets)
$json = ConvertObjectToJson $res;

$blob = $json | where {$_.name -eq "s2cosmosdb$($suffix)/host.json"};

#download it..
az storage blob download --connection-string $azurequeueConnString --container-name azure-webjobs-secrets --name $blob.name --file host.json;

$data = Get-content "host.json" -raw
$json = ConvertFrom-json $data;

$funcApiKey = $json.masterkey.value;

########################
#
#set the web app properties
#
#########################

az webapp config appsettings set -g $rgName -n $webAppName --settings AzureQueueConnectionString=$azurequeueConnString
az webapp config appsettings set -g $rgName -n $webAppName --settings paymentsAPIUrl=$paymentsApiUrl
az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIUrl=$funcApiUrl
az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIKey=$funcApiKey
az webapp config appsettings set -g $rgName -n $webAppName --settings databaseId=$databaseId
az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionUrl=$dbConnectionUrl
az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionKey=$dbConnectionKey
az webapp config appsettings set -g $rgName -n $webAppName --settings movieApiKey=$movieApiKey


########################
#
#set the func properties
#
#########################

az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=$azurequeueConnString
az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=bl$paymentsApiUrlah
az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=$funcApiUrl
az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIKey=$funcApiKey
az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=$databaseId
az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=$dbConnectionUrl
az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=$dbConnectionKey
az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=$movieApiKey

########################
#
#deploy stream analytics
#
#########################
SetupStreamAnalytics $suffix;

########################
#
#setup the cosmosdb (run the import tool to create collections and import initial event data)
#
########################

#DataImport.exe

########################
#
#run the data bricks notebook
#
########################
#Done in the labs

