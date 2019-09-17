$prefix = "s2_retail"
$subscriptionId = "8c924580-ce70-48d0-a031-1b21726acc1a"
$subName = "Solliance MPN 12K"
$rgName = $prefix;

$res = az account list;

if ($res.contains("cloudName"))
{
    #login
    $subs = az login;    
}

#select the subscription
az account set --subscription $subName;

#create the resource group
$result = az group create --name $rgName --location "Central US"

#deploy the tempalte
$deployId = [System.Guid]::newguid().ToString();
$result = az group deployment create --name "Template" --resource-group $rgName --mode Incremental --template-file labdeploy.json #--parameters storageAccountType=Standard_GRS

#set the stream analytics inputs
$jobInputName = "EventHub"
$jobInputDefinitionFile = "streamanaltyics_input_1.json"

New-AzStreamAnalyticsInput -ResourceGroupName $rgName -JobName $jobName -File $jobInputDefinitionFile -Name $jobInputName

#set the stream analytics outputs (#1)
$jobOutputName = "BlobOutput"
$jobOutputDefinitionFile = "streamanaltyics_output_1.json"

New-AzStreamAnalyticsOutput -ResourceGroupName $rgName -JobName $jobName -File $jobOutputDefinitionFile -Name $jobOutputName -Force

#set the stream analytics query
$jobTransformationName = "RetailJob"
$jobTransformationDefinitionFile = "streamanaltyics_query.json"

New-AzStreamAnalyticsTransformation -ResourceGroupName $rgName -JobName $jobName -File $jobTransformationDefinitionFile -Name $jobTransformationName -Force

#start the job
Start-AzStreamAnalyticsJob -ResourceGroupName $rgName -Name $jobName -OutputStartMode 'JobStartTime'

#get all the settings
$azurequeueConnString = "";
$paymentsApiUrl = "";
$funcApiUrl = "";
$funcApiKey = "";
$databaseId = "";
$dbConnectionUrl = "";
$dbConnectionKey = "";
$movieApiKey = "";

#deploy the web app
az webapp deployment source config-zip --resource-group $rgName --name ZipDeploy --src "webapp.zip"

#set the web app properties
az webapp config appsettings set -g $rgName -n $webAppName --settings AzureQueueConnectionString=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings paymentsAPIUrl=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIUrl=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings funcAPIKey=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings databaseId=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionUrl=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings dbConnectionKey=blah
az webapp config appsettings set -g $rgName -n $webAppName --settings movieApiKey=blah

#deploy the function
az functionapp deployment source config-zip --resource-group $rgName --name ZipDeploy --src "functionapp.zip"

#set the func properties
az webapp config appsettings set -g $rgName -n $funcAppName --settings AzureQueueConnectionString=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings paymentsAPIUrl=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIUrl=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings funcAPIKey=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings databaseId=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionUrl=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings dbConnectionKey=blah
az webapp config appsettings set -g $rgName -n $funcAppName --settings movieApiKey=blah

#setup the cosmosdb (run the import tool to create collections and import data)
#DataImport.exe

#run the data bricks notebook

