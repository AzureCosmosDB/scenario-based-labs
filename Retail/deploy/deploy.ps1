#################
#
# Run to get the lasest AZ powershell commands (for stream analytics) NOTE:  Not all stream analytics components can be auto deployed
#
#################
#Install-Module -Name Az -AllowClobber -Scope CurrentUser
#################
$githubPath = "C:\github\solliancenet\cosmos-db-scenario-based-labs";
$mode = "lab"  #can be 'lab' or 'demo'
$subscriptionId = "YOUR SUBSCRIPTION ID"
$subName = "YOUR SUBSCRIPTION NAME"

$prefix = "YOUR INITIALS"
$rgName = $prefix + "_s2_retail"
$databaseId = "movies";
$region = "westus";

#register at https://api.themoviedb.org
$movieApiKey = "YOUR API KEY";

#toggles for skipping items
$skipDeployment = $false;

#databricks api token
$databrickToken = ""

#this should get set on a successful deployment...
$suffix = ""

###################################
#
#  Solliance settings
#
###################################
$githubPath = "C:\github\solliancenet\cosmos-db-scenario-based-labs";
$mode = "demo"  #can be 'lab' or 'demo'
$subscriptionId = "8c924580-ce70-48d0-a031-1b21726acc1a"
$subName = "Solliance MPN 12K"

$prefix = "cjg"
$rgName = $prefix + "_s2_retail"
$databaseId = "movies";
$region = "centralus";

#register at https://api.themoviedb.org
$movieApiKey = "6918a9db428b01e4a7a88757e7c6467c";

#databricks api token
$databrickToken = "dapie5a250a148d3dc3481fa8d525a8f1d02"

#this should get set on a successful deployment...
$suffix = ""

###################################
#
#  Functions
#
###################################

function DeployTemplate($filename, $skipDeployment, $parameters)
{
    write-host "Deploying $filename - Please wait";

    if (!$skipDeployment)
    {
        #deploy the template
        $deployId = "Microsoft.Template"

        if (!$parameters)
        {
            $result = $(az group deployment create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\$fileName") --output json)
        }
        else
        {
            $result = $(az group deployment create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\$fileName") --output json --parameters "$parameters")
        }
        

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

        write-host "Deploying [$fileName] finished with status $($deployment.properties.provisioningState)";
    }

    return $deployment;
}

function UpdateConfig($path)
{
    [xml]$xml = get-content $filepath;

    #set the function url
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "funcAPIUrl"}

    if($data)
    {
        $data.value = $funcApiUrl;
    }

    #set the function key
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "funcAPIKey"}

    if($data)
    {
        $data.value = $funcApiKey;
    }

    #set the database url
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "dbConnectionUrl"}

    if($data)
    {
        $data.value = $dbConnectionUrl;
    }

    #set the database key
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "dbConnectionKey"}

    if($data)
    {
        $data.value = $dbConnectionKey;
    }

    #set the movie api key
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "movieApiKey"}

    if($data)
    {
        $data.value = $movieApiKey;
    }

    #set the database id
    $data = $xml.configuration.appSettings.add | where {$_.key -eq "databaseId"}

    if($data)
    {
        $data.value = $databaseId;
    }

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
    write-host "CosmosDBFull: $CosmosDBConnection"
    write-host "AzureKeyVault: $keyvaulturl"
    write-host "Email: $userEmail"
    write-host "databricksInstance: $databricksInstance"
}

function SetupDatabricks()
{
    if ($mode -eq "demo")
    {
        #create the custer node
        $json = "{`"cluster_name`": `"small`",`"spark_version`": `"5.5.x-scala2.11`",`"node_type_id`": `"Standard_DS3_v2`",`"num_workers`" : 1}"
        $res = curl -Method Post "$databricksurl/api/2.0/clusters/create" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'} -Body "$json";
        $json = ConvertFrom-json $res.Content

        $clusterId = $json.cluster_id
        
        #install the library
        $json = "{`"cluster_id`": `"$clusterid`",`"libraries`": [{`"maven`": {`"coordinates`": `"com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1`",`"exclusions`": [`"slf4j:slf4j`"]}}]}";
        $res = curl -Method Post "$databricksurl/api/2.0/libraries/install" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'} -Body $json;
        $json = ConvertFrom-json $res.Content

        #extract the file
        $filePath = "$githubPath/lab-files/retail/notebooks/02 retail.zip";
        Expand-Archive -LiteralPath $filePath -DestinationPath "$githubPath/lab-files/retail/notebooks/export"

        #update the variables
        $sharedConfigPath = "$githubPath/lab-files/retail/notebooks/export/02 retail/includes/Shared-Configuration.ipynb"
        $content = get-content $sharedConfigPath
        $content = $content.replace("cosmos_db_endpoint = \`"\`"", "cosmos_db_endpoint = \`"$dbConnectionUrl\`"");
        $content = $content.replace("cosmos_db_master_key = \`"\`"", "cosmos_db_master_key = \`"$dbConnectionKey\`"");
        $content = $content.replace("cosmos_db_database = \`"\`"", "cosmos_db_database = \`"$databaseId\`"");

        remove-item $sharedConfigPath
        add-content $sharedConfigPath $content;

        #create the includes folder
        $data = @{"path"="/Users/$userEmail/Includes"} | ConvertTo-Json
        $res = curl -Method Post "$databricksurl/api/2.0/workspace/mkdirs" -H @{'Authorization' = "Bearer $databricktoken"} -Body $data;

        #upload all the files
        $di = new-object System.IO.DirectoryInfo ("$githubPath/lab-files/retail/notebooks/export/02 retail")

        $files = $di.GetFiles("*.*", [System.IO.SearchOption]::AllDirectories);

        foreach($file in $files)
        {            
            #get the import file..        
            $base64String = [System.Convert]::ToBase64String([system.io.file]::ReadAllBytes($file.FullName))

            $path = $file.DirectoryName.Replace($di.FullName, "");

            if ($path)
            {
                $path = "/" + $path.Replace("\","") 
            }

            $path += "/" + $file.Name.replace(".ipynb","");

            #import the notebooks
            $data = @{
             "path"="/Users/$userEmail$path"
             "format"="JUPYTER"
             "overwrite"=$true
             "language"="PYTHON"
             "content"=$base64string
            } | ConvertTo-Json

            $data

            $res = curl -Method Post "$databricksurl/api/2.0/workspace/import" -H @{'Authorization' = "Bearer $databricktoken"} -Body $data;
            $json = ConvertFrom-json $res.Content
        }

        #execute the event generation
        ExecuteDatabrickNotebook "/Users/$userEmail/01 Event Generator" "01 Event Generator" $true
        
        ExecuteDatabrickNotebook "/Users/$userEmail/02 Association Rules" "02 Association Rules" $true
        
        ExecuteDatabrickNotebook "/Users/$userEmail/03 Ratings" "03 Ratings" $true
        
        ExecuteDatabrickNotebook "/Users/$userEmail/04 Similarity" "04 Similarity" $true
    }   
}

function ExecuteDatabrickNotebook($notebookPath, $jobName, $waitToComplete)
{
    #create a job
    $json = @{
        "name"=1
        "existing_cluster_id"=$clusterId
        "notebook_task"= @{ "notebook_path"="$notebookPath"}
    } | ConvertTo-Json

    $res = curl -Method Post "$databricksurl/api/2.0/jobs/create" -H @{'Authorization' = "Bearer $databricktoken"} -Body $json;
    $json = ConvertFrom-json $res.Content

    $jobId = $json.job_id;

    #execute the notebook job
    $json = @{
        "job_id"=$jobId
        "run_name"="$jobName"
        "timeout_secods"=3600
    } | ConvertTo-Json

    $res = curl -Method Post "$databricksurl/api/2.0/jobs/run-now" -H @{'Authorization' = "Bearer $databricktoken"} -Body $json;
    $json = ConvertFrom-json $res.Content

    $runid = $json.run_id;

    if ($waitToComplete)
    {
        $res = curl -Method Get "$databricksurl/api/2.0/jobs/runs/get?run_id=$runId" -H @{'Authorization' = "Bearer $databricktoken"};
        $json = ConvertFrom-json $res.Content

        if ($json.state.life_cycle_state -eq "RUNNING")
        {
            Write-Host "Waiting for Job[$jobId] : Run[$runId] to complete..."
            start-sleep 30;
        }
    }
}

function SetupStreamAnalytics($suffix)
{
    #deploy the template
    $deployId = "Microsoft.Template"
    $result = $(az group deployment create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\labdeploy2.json") --output json )

    #wait for the job to complete...
    $res = $(az group deployment list --resource-group $rgname --output json)
    $json = ConvertObjectToJson $res;

    $deployment = $json | where {$_.name -eq $deployId};

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

###################################
#
#  Main
#
###################################

cd $githubpath

#login - do this always as AAD will error if you change location/ip
$res = az login;
$json = ConvertObjectToJson $res

#help out with the email address...
$userEmail = $json[0].user.name;

#select the subscription if you set it
if ($subName)
{
    az account set --subscription $subName;
}

$res = $(az account show)
$json = ConvertObjectToJson $res

$tenantId = $json.tenantId;

#create the resource group
$result = az group create --name $rgName --location $region;

<#
$parameters = @{
             "region"=@{"value"="$region"}
             "prefix"=@{"value"="$prefix"}
             "tenantId"=@{"value"="$tenantId"}
            } | ConvertTo-Json
            #>

$deployment = DeployTemplate "labdeploy.json" $skipDeployment $parameters;

#need the suffix...
if ($deployment.properties.provisioningState -eq "Succeeded")
{
    $suffix = $deployment.properties.outputs.hash.value
}

if (!$suffix)
{
    $suffix = read-host "Deployment failed: Please enter the suffix that was created for the resource group";

    if (!$suffix)
    {
        write-host "No suffix, stopping";
        return;
    }
}

#deploy containers - this is ok to fail
$deployment = DeployTemplate "labdeploy4.json" $skipDeployment;

#get all the resources in the RG
$res = $(az resource list --resource-group $rgName)
$json = ConvertObjectToJson $res;

#stream analytics will overwrite settings if deployed more than once!
$saJob = $json | where {$_.type -eq "Microsoft.StreamAnalytics/streamingjobs"};

if (!$saJob)
{
    #deploy stream analytics
    $deployment = DeployTemplate "labdeploy2.json" $skipDeployment;
}

#LOGIC APPS will overwrite settings if deployed more than once!
$logicApp = $json | where {$_.type -eq "Microsoft.Logic/workflows"};

if (!$logicApp)
{
    #deploy logic app
    $deployment = DeployTemplate "labdeploy3.json" $skipDeployment;
}

#used later (keyvault)
$databricksName = "s2_databricks_" + $suffix;
$databricks = $json | where {$_.type -eq "Microsoft.Databricks/workspaces" -and $_.name -eq $databricksName};

#used later (keyvault)
$keyvaultName = "s2keyvault-" + $suffix;
$keyvault = $json | where {$_.type -eq "Microsoft.KeyVault/vaults" -and $_.name -eq $keyvaultName};

#used later (function app)
$funcAppName = "s2func" + $suffix;
$funcApp = $json | where {$_.type -eq "Microsoft.Web/sites" -and $_.name -eq $funcAppName};

#used later (web app)
$webAppName = "s2web" + $suffix;
$webApp = $json | where {$_.type -eq "Microsoft.Web/sites" -and $_.name -eq $webAppName};

#get all the settings
$azurequeueConnString = "";
$paymentsApiUrl = "";
$funcApiUrl = "";
$funcApiKey = "";
$dbConnectionUrl = "";
$dbConnectionKey = "";
$databaseId = "movies"
$eventHubConnection = "";
$CosmosDBConnection = "";
$databricksInstance = "";

########################
#
#get databricks url...
#
########################

$databricksInstance = "https://$($databricks.location).azuredatabricks.net";

########################
#
#get key vault url
#
########################

$keyVaulturl = "https://$($keyvault.Name).vault.azure.net";

########################
#
#get the event hub connection
#
########################
write-host "Getting event hub connection"

$res = $(az eventhubs namespace list --output json --resource-group $rgName)
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
write-host "Getting storage account key"

$res = $(az storage account list --output json --resource-group $rgName)
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
write-host "Getting cosmos db url and key"

$res = $(az cosmosdb list --output json --resource-group $rgName)
$json = ConvertObjectToJson $res;

$db = $json | where {$_.name -eq "s2cosmosdb" + $suffix};

$dbConnectionUrl = $db.documentEndpoint;

$res = $(az cosmosdb keys list --name $db.name --resource-group $rgName)
$json = ConvertObjectToJson $res;

$dbConnectionKey = $json.primaryMasterKey;

$CosmosDBConnection = "AccountEndpoint=$dbConnectionUrl;AccountKey=$dbConnectionKey";

########################
#
#deploy the web app
#
#########################
$webAppName = "s2web" + $suffix;

if ($mode -eq "demo")
{ 
    write-host "Deploying the web application"

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
if ($mode -eq "demo")
{
    write-host "Deploying the function app"

    $res = $(az functionapp deployment source config-zip --resource-group $rgName --name $funcAppName --src "$githubpath/retail/deploy/functionapp.zip")
    $json = ConvertObjectToJson $res;
}

########################
#
#get the function url and key
#
#########################
write-host "Getting the function app url and key"

$res = $(az functionapp list --output json --resource-group $rgName)
$json = ConvertObjectToJson $res;

$func = $json | where {$_.name -eq $funcAppName};

$funcApiUrl = "https://" + $func.defaultHostName;

#open the function app endpoint to create the host.json file:
$url = "https://portal.azure.com/#blade/WebsitesExtension/FunctionsIFrameBlade/id/$($func.id)"
write-host "Action Requires: Opening url: $url";
Start-Process $url;

$res = read-host "Did you click to the function application's settings page yet?";

#key is stored in the storage account after the last url loads.
$res = $(az storage blob list --connection-string $azurequeueConnString --container-name azure-webjobs-secrets)
$json = ConvertObjectToJson $res;

$blob = $json | where {$_.name -eq "$funcAppName/host.json"};

if (!$blob)
{
    write-host "The function app did not load the url, the host.json file is not available";
    return;
}

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
write-host "Saving app settings to web application"

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
# save key vault values
#
#########################

write-host "Setting key vault values..."
<#
az keyvault secret set --vault-name $keyvault.Name --name "AzureQueueConnectionString" --value $azurequeueConnString;
az keyvault secret set --vault-name $keyvault.Name --name "paymentsAPIUrl" --value $paymentsApiUrlah;
az keyvault secret set --vault-name $keyvault.Name --name "funcAPIUrl" --value $funcApiUrl;
az keyvault secret set --vault-name $keyvault.Name --name "funcAPIApi" --value $funcApiKey;
az keyvault secret set --vault-name $keyvault.Name --name "databaseId" --value $databaseId;
az keyvault secret set --vault-name $keyvault.Name --name "CosmosDBConnection" --value $CosmosDBConnection;
az keyvault secret set --vault-name $keyvault.Name --name "dbConnectionUrl" --value $dbConnectionUrl;
az keyvault secret set --vault-name $keyvault.Name --name "dbConnectionKey" --value $dbConnectionKey;
az keyvault secret set --vault-name $keyvault.Name --name "eventHubConnection" --value $eventHubConnection;
az keyvault secret set --vault-name $keyvault.Name --name "eventHub" --value "store";
az keyvault secret set --vault-name $keyvault.Name --name "movieApiKey" --value $movieApiKey;
az keyvault secret set --vault-name $keyvault.Name --name "LogicAppUrl" --value "";
az keyvault secret set --vault-name $keyvault.Name --name "RecipientEmail" --value $userEmail;
#>

########################
#
#set the func properties
#
#########################
write-host "Saving app settings to func app..."

$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=$azurequeueConnString)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=bl$paymentsApiUrlah)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=$funcApiUrl)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIKey=$funcApiKey)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=$databaseId)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings CosmosDBConnection=$CosmosDBConnection)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=$dbConnectionUrl)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=$dbConnectionKey)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHubConnection=$eventHubConnection)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHub=store)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=$movieApiKey)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings LogicAppUrl=empty)
$res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings RecipientEmail=$userEmail)

########################
#
#Update project configs to be nice ;)
#
########################
write-host "Saving app settings to Visual Studio solutions (starter and solution)"

$folders = ("starter", "solution")

foreach($folder in $folders)
{
    $filePath = "$githubpath\lab-files\Retail\$folder\Data Import\app.config"
    UpdateConfig $filePath;

    $filePath = "$githubpath\lab-files\Retail\$folder\DataGenerator\app.config"
    UpdateConfig $filePath;

    $filePath = "$githubpath\lab-files\Retail\$folder\Contoso Movies\Contoso.Apps.Movies.Web\web.config"
    UpdateConfig $filePath;

    #update the app.config file with the new values
    $filePath = "$githubpath\lab-files\Retail\$folder\Data Import\bin\Debug\MovieDataImport.exe.config"
    UpdateConfig $filePath;
}

########################
#
#run the data bricks notebook
#
########################
if ($mode -eq "demo")
{ 
    write-host "Action Required - Opening url: $databricksInstance";
    start-process $databricksInstance;

    $databrickToken = read-host "Enter your databricks api token";

    SetupDatabricks
}

########################
#
#setup the cosmosdb (run the import tool to create collections and import initial object data)
#
########################
if ($mode -eq "demo")
{ 
    write-host "Importing all the movie data"

    #run the import tool
    . "$githubpath\lab-files\Retail\Starter\Data Import\bin\Debug\MovieDataImport.exe"
}

########################
#
# Open the web site
#
########################
if ($mode -eq "demo")
{
    $url = "https://$($webapp.name).azurewebsites.net";
    write-host "Opening url: $url";
    Start-Process $url;
}

########################
#
#deploy stream analytics - Not production ready - does not support Power BI Outputs
#
#########################
#SetupStreamAnalytics $suffix;

########################
#
# Output variables
#
########################
Output