#################
#
# Run to get the lasest AZ powershell commands (for stream analytics) NOTE:  Not all stream analytics components can be auto deployed
#
#################
#Install-Module -Name Az -AllowClobber -Scope CurrentUser
#################
$githubPath = "YOUR GIT PATH";

#can be 'lab' or 'demo'
$mode = "demo"

#if you want to use a specific subscription
$subscriptionId = "YOUR SUB ID"

#create a unique resource group name
$prefix = "YOUR INIT"

#used for when you are using spektra environment
$isSpektra = $false;

if ($isSpektra)
{
    #if you are using spektra...you have to set your resource group here:
    $rgName = read-host "What is your spektra resource group name?";
}
else
{
    $rgName = $prefix + "_s2_retail"
}

#used for cosmos db
$databaseId = "movies";

#FYI - not all regions have been tested - 
#Check your region support here : https://azure.microsoft.com/en-us/global-infrastructure/services/?products=
#for a list of regions run : az account list-locations -o table
#OK - westus, eastus, northeurope
$region = "northeurope";

#register at https://api.themoviedb.org
$movieApiKey = "YOUR API KEY";

#toggles for skipping items
$skipDeployment = $false;

#this should get set on a successful deployment...
$suffix = ""

#Implicit Key Vault usage
$useKeyVault = $false

###################################
#
#  Functions
#
###################################

function SetKeyVaultValue($kvName, $name, $value)
{
    if ($value)
    {
        write-host "Setting $name to $value";

        $res = $(az keyvault secret set --vault-name $kvName --name $name --value $value);    
    }

    return $res;
}

function DownloadNuget()
{
    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $targetNugetExe = "$githubPath\nuget.exe"
    Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
    Set-Alias nuget $targetNugetExe -Scope Global -Verbose
}

function BuildVS
{
    param
    (
        [parameter(Mandatory=$true)]
        [String] $path,

        [parameter(Mandatory=$false)]
        [bool] $nuget = $true,
        
        [parameter(Mandatory=$false)]
        [bool] $clean = $true
    )
    process
    {
        #install nuget...
        DownloadNuget

        #default
        $msBuildExe = 'C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe'

        $msBuild = "msbuild"

        try
        {
            & $msBuild /version
            Write-Host "Likely on Linux/macOS."
        }
        catch
        {
            Write-Host "MSBuild doesn't exist. Use VSSetup instead."
            
            Install-Module VSSetup -Scope CurrentUser -Force
            
            $instance = Get-VSSetupInstance -All -Prerelease | Select-VSSetupInstance -Require 'Microsoft.Component.MSBuild' -Latest
            $installDir = $instance.installationPath

            Write-Host "Visual Studio is found at $installDir"
            
            $msBuildExe = $installDir + '\MSBuild\Current\Bin\MSBuild.exe' # VS2019
            
            if (![System.IO.File]::Exists($msBuildExe))
            {
                $msBuild = $installDir + '\MSBuild\15.0\Bin\MSBuild.exe' # VS2017

                if (![System.IO.File]::Exists($msBuildExe))
                {
                    Write-Host "MSBuild doesn't exist. Exit."
                    exit 1
                }

            }    Write-Host "Likely on Windows."
        }Write-Host "MSBuild found. Compile the projects."

        if ($nuget) {
            Write-Host "Restoring NuGet packages" -foregroundcolor green
            nuget restore "$($path)"
        }

        if ($clean) {
            Write-Host "Cleaning $($path)" -foregroundcolor green
            & "$($msBuildExe)" "$($path)" /t:Clean /m
        }

        Write-Host "Building $($path)" -foregroundcolor green
        & "$($msBuildExe)" "$($path)" /t:Build /m
    }
}

function DeployTemplate($filename, $skipDeployment, $parameters, $name)
{
    write-host "Deploying [$filename] - Please wait";

    if (!$skipDeployment)
    {
        if ($name)
        {
            $deployid = $name;
        }
        else
        {
            #deploy the template
            $deployId = [System.Guid]::NewGuid().ToString();
        }

        Remove-Item "parameters.json" -ea SilentlyContinue;
        add-content "parameters.json" $parameters;

        if (!$parameters)
        {
            $result = $(az deployment group create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\$fileName") --output json)
        }
        else
        {
            $result = $(az deployment group create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\$fileName") --output json --parameters `@$githubpath\parameters.json)
        }
        

        #wait for the job to complete...
        $res = $(az deployment group list --resource-group $rgname --output json)
        $json = ConvertObjectToJson $res;

        $deployment = $json | where {$_.name -eq $deployId};

        #check the status
        while($deployment.properties.provisioningState -eq "Running")
        {
            start-sleep 10;

            $res = $(az deployment group list --resource-group $rgname --output json)
            $json = ConvertObjectToJson $res;

            $deployment = $json | where {$_.name -eq $deployId};

            write-host "Deployment status is : $($deployment.properties.provisioningState)";
        }

        Remove-Item "parameters.json" -ea SilentlyContinue;

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
        #create the cluster node
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        $json = "{`"cluster_name`": `"small`",`"spark_version`": `"5.5.x-scala2.11`",`"node_type_id`": `"Standard_DS3_v2`",`"num_workers`" : 1}"
        $res = curl -Method Post "$databricksInstance/api/2.0/clusters/create" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'} -Body "$json";
        $json = ConvertFrom-json $res.Content

        $clusterId = $json.cluster_id

        # allow the creation of the cluster to begin before installing libraries
        start-sleep 15;
        
        #install the library
        $json = "{`"cluster_id`": `"$clusterid`",`"libraries`": [{`"maven`": {`"coordinates`": `"com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1`",`"exclusions`": [`"slf4j:slf4j`"]}}]}";
        $res = curl -Method Post "$databricksInstance/api/2.0/libraries/install" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'} -Body $json;
        $json = ConvertFrom-json $res.Content

        #wait for the cluster to start...
        $res = curl -Method Get "$databricksInstance/api/2.0/clusters/get?cluster_id=$clusterId" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'}
        $json = ConvertFrom-json $res.Content

        while($json.state -ne "RUNNING")
        {
            write-host "Waiting for cluster [$clusterId] to start";

            start-sleep 10;

            $res = curl -Method Get "$databricksInstance/api/2.0/clusters/get?cluster_id=$clusterId" -H @{'Authorization' = "Bearer $databricktoken"; 'Content-Type' = 'application/json'}
            $json = ConvertFrom-json $res.Content

            if ($json.state -eq "ERROR")
            {
                write-host "Error starting the cluster, please attempt to start manually!";
                $res = read-host "Press enter when complete";
            }
        }

        #extract the files
        #$filePath = "$githubPath\retail\notebooks\02 retail.zip";
        #Expand-Archive -LiteralPath $filePath -DestinationPath "$githubPath/retail/notebooks/export" -force

        #update the variables
        $sharedConfigPath = "$githubPath/retail/notebooks/includes/Shared-Configuration.ipynb"
        $content = get-content $sharedConfigPath
        $content = $content.replace("cosmos_db_endpoint = \`"\`"", "cosmos_db_endpoint = \`"$dbConnectionUrl\`"");
        $content = $content.replace("cosmos_db_master_key = \`"\`"", "cosmos_db_master_key = \`"$dbConnectionKey\`"");
        $content = $content.replace("cosmos_db_database = \`"\`"", "cosmos_db_database = \`"$databaseId\`"");

        remove-item $sharedConfigPath
        add-content $sharedConfigPath $content;

        #create the includes folder
        $userFolderName = $userEmail.ToLower()
        $notebookFolderName = "02 Retail"
        $data = @{"path"="/Users/$userFolderName/$notebookFolderName/Includes"} | ConvertTo-Json
        $res = curl -Method Post "$databricksInstance/api/2.0/workspace/mkdirs" -H @{'Authorization' = "Bearer $databricktoken"} -Body $data;

        #upload all the files
        $di = new-object System.IO.DirectoryInfo ("$githubPath/retail/notebooks")

        $files = $di.GetFiles("*.ipynb", [System.IO.SearchOption]::AllDirectories);

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
                "path"="/Users/$userFolderName/$notebookFolderName$path"
                "format"="JUPYTER"
                "overwrite"=$true
                "language"="PYTHON"
                "content"=$base64string
            } | ConvertTo-Json

            $res = curl -Method Post "$databricksInstance/api/2.0/workspace/import" -H @{'Authorization' = "Bearer $databricktoken"} -Body $data;
            $json = ConvertFrom-json $res.Content
        }

        #execute the event generation
        ExecuteDatabrickNotebook "/Users/$userFolderName/$notebookFolderName/01 Event Generator" "01 Event Generator" $true $clusterId
        
        ExecuteDatabrickNotebook "/Users/$userFolderName/$notebookFolderName/02 Association Rules" "02 Association Rules" $true $clusterId
        
        ExecuteDatabrickNotebook "/Users/$userFolderName/$notebookFolderName/03 Ratings" "03 Ratings" $true $clusterId
        
        ExecuteDatabrickNotebook "/Users/$userFolderName/$notebookFolderName/04 Similarity" "04 Similarity" $true $clusterId
    }   
}

function ExecuteDatabrickNotebook($notebookPath, $jobName, $waitToComplete, $clusterId)
{
    #create a job
    $json = @{
        "name"=1
        "existing_cluster_id"=$clusterId
        "notebook_task"= @{ "notebook_path"="$notebookPath"}
    } | ConvertTo-Json

    $res = curl -Method Post "$databricksInstance/api/2.0/jobs/create" -H @{'Authorization' = "Bearer $databricktoken"} -Body $json;
    $json = ConvertFrom-json $res.Content

    $jobId = $json.job_id;

    #execute the notebook job
    $json = @{
        "job_id"=$jobId
        "run_name"="$jobName"
        "timeout_secods"=3600
    } | ConvertTo-Json

    $res = curl -Method Post "$databricksInstance/api/2.0/jobs/run-now" -H @{'Authorization' = "Bearer $databricktoken"} -Body $json;
    $json = ConvertFrom-json $res.Content

    $runid = $json.run_id;

    if ($waitToComplete)
    {
        $res = curl -Method Get "$databricksInstance/api/2.0/jobs/runs/get?run_id=$runId" -H @{'Authorization' = "Bearer $databricktoken"};
        $json = ConvertFrom-json $res.Content

        while ($json.state.life_cycle_state -eq "PENDING" -or $json.state.life_cycle_state -eq "RUNNING")
        {
            Write-Host "Waiting for Job[$jobId] : Run[$runId] to complete..."
            start-sleep 10;
            $res = curl -Method Get "$databricksInstance/api/2.0/jobs/runs/get?run_id=$runId" -H @{'Authorization' = "Bearer $databricktoken"};
            $json = ConvertFrom-json $res.Content
        }
    }
}

function SetupStreamAnalytics($suffix)
{
    #deploy the template
    $deployId = "Microsoft.Template"
    $result = $(az deployment group create --name $deployId --resource-group $rgName --mode Incremental --template-file $($githubpath + "\retail\deploy\labdeploy2.json") --output json )

    #wait for the job to complete...
    $res = $(az deployment group list --resource-group $rgname --output json)
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
$json = ConvertObjectToJson $res;

#help out with the email address...
$userEmail = $json[0].user.name;

$res = az ad user show --upn-or-object-id $userEmail
$json = ConvertObjectToJson $res

#get object id for current user to assign to key vault
$userObjectId = $json.objectId;

#select the subscription if you set it
if ($subscriptionId)
{
    $res = az account set --subscription $subscriptionId;
}

$res = $(az account show)
$json = ConvertObjectToJson $res

$tenantId = $json.tenantId;

#create the resource group (if not in spektra)
if (!$isSpektra)
{
    $res = az group create --name $rgName --location $region;
}

$parametersRegion = @{
            "schema"="http://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"
            "contentVersion"="1.0.0.0"
            "parameters"=@{
                 "region"=@{"value"="$region"}
                 }
            } | ConvertTo-Json

$parameters = @{
            "schema"="http://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"
            "contentVersion"="1.0.0.0"
            "parameters"=@{
                 "region"=@{"value"="$region"}
                 "msiId"=@{"value"="TBD"}
                 "prefix"=@{"value"="$prefix"}
                 "tenantId"=@{"value"="$tenantId"}
                 "userObjectId"=@{"value"="$userObjectId"}
                 }
            } | ConvertTo-Json
            
$deployment = DeployTemplate "labdeploy_main.json" $skipDeployment $parameters "01_Main";

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

$parameters = @{
            "schema"="http://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"
            "contentVersion"="1.0.0.0"
            "parameters"=@{
                 "region"=@{"value"="$region"}
                 "msiId"=@{"value"="TBD"}
                 "prefix"=@{"value"="$prefix"}
                 "tenantId"=@{"value"="$tenantId"}
                 "userObjectId"=@{"value"="$userObjectId"}
                 }
            } | ConvertTo-Json

#deploy containers - this is ok to fail
$deployment = DeployTemplate "labdeploy_cosmos.json" $skipDeployment $parametersRegion "02_CosmosContainers";

#get all the resources in the RG
$res = $(az resource list --resource-group $rgName)
$json = ConvertObjectToJson $res;

#stream analytics will overwrite settings if deployed more than once!
$saJob = $json | where {$_.type -eq "Microsoft.StreamAnalytics/streamingjobs"};

if (!$saJob)
{
    #deploy stream analytics
    $deployment = DeployTemplate "labdeploy_streamanalytics.json" $skipDeployment $parametersRegion "03_StreamAnalytics";
}

#LOGIC APPS will overwrite settings if deployed more than once!
$logicApp = $json | where {$_.type -eq "Microsoft.Logic/workflows"};

if (!$logicApp)
{
    #deploy logic app
    $deployment = DeployTemplate "labdeploy_logicapp.json" $skipDeployment $parametersRegion "04_LogicApp";
}

#used later (databricks)
$databricksName = "s2databricks" + $suffix;
$databricks = $json | where {$_.type -eq "Microsoft.Databricks/workspaces" -and $_.name -eq $databricksName};

#used later (keyvault)
$keyvaultName = "s2keyvault" + $suffix;
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

$databricksInstanceUrl = "https://portal.azure.com/#@$($tenantid)/resource/$($databricks.id)/overview"
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
#get the function url
#
#########################
write-host "Getting the function app url and key"

$res = $(az functionapp list --output json --resource-group $rgName)
$json = ConvertObjectToJson $res;

$func = $json | where {$_.name -eq $funcAppName};

$funcApiUrl = "https://" + $func.defaultHostName;

########################
#
# save key vault values
#
#########################

write-host "Setting key vault values..."

$res = SetKeyVaultValue $keyvault.Name "paymentsAPIUrl" $paymentsApiUrl;
$res = SetKeyVaultValue $keyvault.Name "AzureQueueConnectionString" $azurequeueConnString;

$res = SetKeyVaultValue $keyvault.Name "funcApiUrl" $funcApiUrl;
$json = ConvertObjectToJson $res;
$kvFuncApiUrl = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "databaseId" $databaseId;
$json = ConvertObjectToJson $res;
$kvdatabaseId = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "CosmosDBConnection" $CosmosDBConnection;
$json = ConvertObjectToJson $res;
$kvCosmosDBConnection = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "dbConnectionUrl" $dbConnectionUrl;
$json = ConvertObjectToJson $res;
$kvdbConnectionUrl = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "dbConnectionKey" $dbConnectionKey;
$json = ConvertObjectToJson $res;
$kvdbConnectionKey = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "eventHubConnection" $eventHubConnection;
$json = ConvertObjectToJson $res;
$kveventHubConnection = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "eventHub" "store";
$json = ConvertObjectToJson $res;
$kveventHub = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "movieApiKey" $movieApiKey;
$json = ConvertObjectToJson $res;
$kvmovieApiKey = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "LogicAppUrl" "";
$json = ConvertObjectToJson $res;
$kvLogicAppUrl = "@Microsoft.KeyVault(SecretUri=$($json.id))"

$res = SetKeyVaultValue $keyvault.Name "RecipientEmail" $userEmail;
$json = ConvertObjectToJson $res;
$kvRecipientEmail = "@Microsoft.KeyVault(SecretUri=$($json.id))"

########################
#
#set the web app properties
#
#########################
write-host "Saving app settings to web application"

if($useKeyVault)
{
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings AzureQueueConnectionString=$kvazurequeueConnString)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings paymentsAPIUrl=$kvpaymentsApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIUrl=$kvfuncApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings databaseId=$kvdatabaseId)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionUrl=$kvdbConnectionUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionKey=$kvdbConnectionKey)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings movieApiKey=$kvmovieApiKey)
}
else
{
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings AzureQueueConnectionString=$azurequeueConnString)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings paymentsAPIUrl=$paymentsApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIUrl=$funcApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings databaseId=$databaseId)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionUrl=$dbConnectionUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionKey=$dbConnectionKey)
    $res = $(az webapp config appsettings set -g $rgName -n $webAppName --settings movieApiKey=$movieApiKey)
}


########################
#
#set the func properties
#
#########################
write-host "Saving app settings to func app..."

if ($useKeyVault)
{
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=$kvazurequeueConnString)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=bl$kvpaymentsApiUrlah)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=$kvfuncApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=$kvdatabaseId)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings CosmosDBConnection=$kvCosmosDBConnection)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=$kvdbConnectionUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=$kvdbConnectionKey)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHubConnection=$kveventHubConnection)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHub=store)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=$kvmovieApiKey)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings LogicAppUrl=empty)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings RecipientEmail=$kvuserEmail)
}
else
{
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=$azurequeueConnString)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=bl$paymentsApiUrlah)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=$funcApiUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=$databaseId)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings CosmosDBConnection=$CosmosDBConnection)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=$dbConnectionUrl)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=$dbConnectionKey)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHubConnection=$eventHubConnection)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings eventHub=store)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=$movieApiKey)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings LogicAppUrl=empty)
    $res = $(az webapp config appsettings set -g $rgName -n $funcAppName --settings RecipientEmail=$userEmail)
}

########################
#
#compile the project
#
########################
if ($mode -eq "demo")
{
    write-host "Compiling the projects..."

    BuildVS "$githubPath\Retail\Solution\Data Import\MovieDataImport.sln" $true $true
    BuildVS "$githubPath\Retail\Solution\DataGenerator\DataGenerator.sln" $true $true
    BuildVS "$githubPath\Retail\Solution\Contoso Movies\Contoso.Apps.Movies.sln" $true $true
}


########################
#
#Update project configs to be nice ;)
#
########################
write-host "Saving app settings to Visual Studio solutions (starter and solution)"

$folders = ("starter", "solution")

foreach($folder in $folders)
{
    $filePath = "$githubpath\Retail\$folder\Data Import\app.config"
    UpdateConfig $filePath;

    $filePath = "$githubpath\Retail\$folder\DataGenerator\app.config"
    UpdateConfig $filePath;

    $filePath = "$githubpath\Retail\$folder\Contoso Movies\Contoso.Apps.Movies.Web\web.config"
    UpdateConfig $filePath;
}

if ($mode -eq "demo")
{ 
    #update the app.config file with the new values
    $filePath = "$githubpath\Retail\Solution\Data Import\bin\Debug\MovieDataImport.exe.config"
    UpdateConfig $filePath;
}

########################
#
#run the data bricks notebook
#
########################
# if ($mode -eq "demo")
# { 
#     write-host "Action Required: Click 'Launch Workspace', then click your user icon, select 'User Settings'.  Click 'Generate new token'"

#     write-host "Opening url: $databricksInstanceUrl";

#     start-process $databricksInstanceUrl;

#     $databrickToken = read-host "Enter your databricks api token";

#     #need to wait for a few seconds for the token to kick in or you might get an error.
#     write-host "Waiting 30 seconds...";
#     start-sleep 30;

#     SetupDatabricks
# }

########################
#
#setup the cosmosdb (run the import tool to create collections and import initial object data)
#
########################
#
# Commenting out. Must be run after executing the Databricks notebooks...
# if ($mode -eq "demo")
# { 
#     write-host "Importing all the movie data"

#     #run the import tool
#     . "$githubpath\Retail\Solution\Data Import\bin\Debug\MovieDataImport.exe"
# }

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