function UpdateConfig($path)
{
    [xml]$xml = get-content $filepath;

    #set the database url
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "dbConnectionUrl"}
    $data.value = $dbConnectionUrl;

    #set the database key
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "dbConnectionKey"}
    $data.value = $dbConnectionKey;

    #set the movie api key
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "movieApiKey"}
    $data.value = $movieApiKey;

    #set the database id
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "databaseId"}
    $data.value = $databaseId;

    $xml.save($filePath);    
}

function Output()
{
    write-host "Output variables:"

    write-host "Azure Queue: $azurequeueConnString"
    write-host "Func Url: $funcApiUrl"
    write-host "Func Key: $funcApiKey";
    write-host "Cosmos DB Url: $dbConnectionUrl"
    write-host "Cosmos DB Key: $dbConnectionKey"
    write-host "DatabaseId: $databaseId"
    write-host "EventHubConn: $eventHubConnection"
}

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
# Run to get the lasest AZ powershell commands (for stream analytics) NOTE:  Not all stream analytics components can be auto deployed
#
#################
#Install-Module -Name Az -AllowClobber -Scope CurrentUser
#################
$githubPath = "C:\github\solliancenet\cosmos-db-scenario-based-labs";
$mode = "lab"  #can be 'lab' or 'demo'
$subscriptionId = "8c924580-ce70-48d0-a031-1b21726acc1a"
$subName = "Solliance MPN 12K"

#this should get set on a successful deployment...
$suffix = ""

$rgName = "s2_retail"
$databaseId = "movies";

#register at https://api.themoviedb.org
$movieApiKey = "6918a9db428b01e4a7a88757e7c6467c";

#toggles for skipping items
$skipDeployment = $false;

cd $githubpath

#login - do this always as AAD will error if you change location/ip
$subs = az login --use-device-code;

#select the subscription if you set it
if ($subName)
{
    az account set --subscription $subName;
}

#create the resource group
$result = az group create --name $rgName --location "Central US"

if (!$skipDeployment)
{
    #deploy the tempalte
    $deployId = "Microsoft.Template"
    $result = $(az group deployment create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\labdeploy.json") --output json )#--parameters storageAccountType=Standard_GRS)

    #wait for the job to complete...
    $res = $(az group deployment list --resource-group $rgname --output json)
    $json = ConvertObjectToJson $res;

    $deployment = $json | where {$_.name -eq $deployId};

    #check the status
    while($deployment.properties.provisioningState -eq "Running")
    {
        start-sleep 10;

        $res = $(az group deployment list --resource-group $rgname --output json)
        $json = ConvertObjectToJson $res;

        $deployment = $json | where {$_.name -eq $deployId};

        write-host "Deployment status is : $($deployment.properties.provisioningState)";
    }

    if ($deployment.properties.provisioningState -eq "Failed")
    {
        write-host "Deployment failed";
        return;
    }
}

#need the suffix...
if ($deployment.properties.provisioningState -eq "Succeeded")
{
    $suffix = $deployment.properties.outputs.hash.value
}

#get all the settings
$azurequeueConnString = "";
$paymentsApiUrl = "";
$funcApiUrl = "";
$funcApiKey = "";
$dbConnectionUrl = "";
$dbConnectionKey = "";
$databaseId = "movies"
$eventHubConnection = "";

########################
#
#get the event hub connection
#
########################
$res = $(az eventhubs namespace list --output json)
$json = ConvertObjectToJson $res;

$sa = $json | where {$_.name -eq "s2ns" + $suffix};
$res = $(az eventhubs namespace authorization-rule keys list --resource-group $rgName --namespace-name $sa.name --name RootManageSharedAccessKey)
$json = ConvertObjectToJson $res;

$eventHubConnection = $json.primaryConnectionString

########################
#
#get the storage connection string
#
########################

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

if ($mode -eq "demo")
{ 
    $res = $(az webapp deployment source config-zip --resource-group $rgName --name $webAppName --src "$githubpath/retail/deploy/webapp.zip")
    $json = ConvertObjectToJson $res;
}

########################
#
#deploy the function
#
#########################

$funcAppName = "s2func" + $suffix;

#we have to deploy something in order for the host.json file to be created in the storage account...
if ($mode -eq "demo" -or $mode -eq "lab")
{
    $deployed = get-content "funcdeployed.txt";

    if ($deployed -ne "true")
    {
        $res = $(az functionapp deployment source config-zip --resource-group $rgName --name $funcAppName --src "$githubpath/retail/deploy/functionapp.zip")
        $json = ConvertObjectToJson $res;
    }

    add-content "funcdeployed.txt" "true";
}

########################
#
#get the function url and key
#
#########################
$res = $(az functionapp list --output json)
$json = ConvertObjectToJson $res;

$func = $json | where {$_.name -eq $funcAppName};

$funcApiUrl = "https://" + $func.defaultHostName;

#key is stored in the storage account...
$res = $(az storage blob list --connection-string $azurequeueConnString --container-name azure-webjobs-secrets)
$json = ConvertObjectToJson $res;

$blob = $json | where {$_.name -eq "$funcAppName/host.json"};

#download it..
az storage blob download --connection-string $azurequeueConnString --container-name azure-webjobs-secrets --name $blob.name --file host.json;

$data = Get-content "host.json" -raw
$json = ConvertFrom-json $data;

$funcApiKey = $json.masterkey.value;

########################
#
# Output variables
#
########################

Output

########################
#
#set the web app properties
#
#########################

$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings AzureQueueConnectionString=$azurequeueConnString)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings paymentsAPIUrl=$paymentsApiUrl)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIUrl=$funcApiUrl)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIKey=$funcApiKey)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings databaseId=$databaseId)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionUrl=$dbConnectionUrl)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionKey=$dbConnectionKey)
$res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings movieApiKey=$movieApiKey)


########################
#
#set the func properties
#
#########################
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=$azurequeueConnString)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=bl$paymentsApiUrlah)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=$funcApiUrl)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIKey=$funcApiKey)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=$databaseId)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=$dbConnectionUrl)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=$dbConnectionKey)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHubConnection=$eventHubConnection)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=$movieApiKey)

########################
#
#Update project configs to be nice ;)
#
########################

$filePath = "$githubpath\lab-files\Retail\Data Import\app.config"
UpdateConfig $filePath;

$filePath = "$githubpath\lab-files\Retail\DataGenerator\app.config"
UpdateConfig $filePath;

$filePath = "$githubpath\lab-files\Retail\Contoso Movies\Contoso.Apps.Movies.Web\web.config"
UpdateConfig $filePath;

########################
#
#setup the cosmosdb (run the import tool to create collections and import initial event data)
#
########################

#update the app.config file with the new values
$filePath = "$githubpath\lab-files\Retail\Data Import\bin\Debug\MovieDataImport.exe.config"

UpdateConfig $filePath;

#run the tool
. "$githubpath\lab-files\Retail\Data Import\bin\Debug\MovieDataImport.exe"


########################
#
#deploy stream analytics
#
#########################
SetupStreamAnalytics $suffix;

########################
#
#run the data bricks notebook - Future
#
########################

if ($mode -eq "demo")
{
    #create the node

    #import the notebooks

    #update the variables

    #execute the notebook
}

write-host "Output variables:"

write-host $azurequeueConnString
write-host $paymentsApiUrl;
write-host $funcApiUrl;
write-host $funcApiKey;
write-host $dbConnectionUrl
write-host $dbConnectionKey
write-host $databaseId
write-host $eventHubConnection