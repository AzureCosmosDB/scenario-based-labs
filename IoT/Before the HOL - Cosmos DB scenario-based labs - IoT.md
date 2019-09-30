# Cosmos DB scenario-based labs - IoT before the hands-on lab setup guide

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
<!-- TOC -->

- [Cosmos DB scenario-based labs - IoT before the hands-on lab setup guide](#cosmos-db-scenario-based-labs---iot-before-the-hands-on-lab-setup-guide)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
    - [Task 1: Create an Azure resource group using Azure Cloud Shell](#task-1-create-an-azure-resource-group-using-azure-cloud-shell)
    - [Task 2: Create Cloud Shell variables](#task-2-create-cloud-shell-variables)
    - [Task 3: Create IoT Hub](#task-3-create-iot-hub)
    - [Task 4: Create a Cosmos DB SQL API account](#task-4-create-a-cosmos-db-sql-api-account)
    - [Task 5: Create an Azure Key Vault](#task-5-create-an-azure-key-vault)
    - [Task 6: Create an event hub](#task-6-create-an-event-hub)
    - [Task 7: Create an Azure Storage accounts](#task-7-create-an-azure-storage-accounts)
    - [Task 8: Create Azure Function Apps](#task-8-create-azure-function-apps)
    - [Task 9: Create an App Service Plan and Web App](#task-9-create-an-app-service-plan-and-web-app)
    - [Task 10: Create Azure Databricks workspace](#task-10-create-azure-databricks-workspace)
    - [Task 11: Create Stream Analytics job](#task-11-create-stream-analytics-job)
    - [Task 12: Create and configure Application Insights](#task-12-create-and-configure-application-insights)
    - [Task 13: Install .NET Core SDK 2.2 (or greater)](#task-13-install-net-core-sdk-22-or-greater)
    - [Task 14: Download the starter files](#task-14-download-the-starter-files)

<!-- /TOC -->
</details>

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.
   - Trial subscriptions will not work.
   - **IMPORTANT**: To complete the OAuth 2.0 access components of this hands-on lab you must have permissions within your Azure subscription to create an App Registration and service principal within Azure Active Directory.
2. Install [Power BI Desktop](https://powerbi.microsoft.com/desktop/)
3. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater
4. Install [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) or greater

## Before the hands-on lab

Duration: 30 minutes

In this exercise, you will set up your environment for use in the rest of the hands-on lab. You should follow all steps provided before attending the Hands-on lab.

> **IMPORTANT**: Many Azure resources require unique names. Throughout these steps you will see the word "SUFFIX" as part of resource names. You should replace this with your Microsoft alias, initials, or another value to ensure resources are uniquely named.

### Task 1: Create an Azure resource group using Azure Cloud Shell

In this task, you will use the Azure Cloud Shell to create a new Azure Resource Group for this lab.

> To run the scripts below in Azure Cloud Shell:
>
> - Paste the code into the Cloud Shell session with **Ctrl+Shift+V** on Windows and Linux, or **Cmd+Shift+V** on macOS.
> - Press **Enter** to run the code.

1. In the [Azure portal](https://portal.azure.com), select the Azure Cloud Shell icon from the top menu.

   ![The Azure Cloud Shell icon is highlighted in the Azure portal's top menu.](media/cloud-shell-icon.png 'Azure Cloud Shell')

2. In the Cloud Shell window that opens at the bottom of your browser window, select **Bash**.

   ![In the Welcome to Azure Cloud Shell window, Bash is highlighted.](media/cloud-shell-select-bash.png 'Azure Cloud Shell')

3. If prompted that you have no storage mounted, select the subscription you are using for this hands-on lab and select **Create storage**.

   ![In the You have no storage mounted dialog, a subscription has been selected, and the Create Storage button is highlighted.](media/cloud-shell-create-storage.png 'Azure Cloud Shell')

   > **NOTE**: If creation fails, you may need to select **Advanced settings** and specify the subscription, region and resource group for the new storage account.

4. After a moment, you will receive a message that you have successfully requested a Cloud Shell, and be presented with bash Azure prompt.

   ![The Azure Cloud Shell is displayed with its default prompt.](media/cloud-shell-prompt.png 'Azure Cloud Shell')

5. If you have multiple subscriptions, choose the appropriate subscription in which the resource should be billed. List all your subscriptions by entering the following into the shell:

   ```bash
   az account list
   ```

6. Select the specific subscription ID under your account using `az account set` command. Copy the `id` value from the output of the previous command for the subscription you wish to use into the `subscription id` placeholder:

   ```bash
   az account set --subscription <subscription id>
   ```

7. Create a variable with random numbers that you will use to append to your Azure service names in the scripts that follow, to ensure the names are unique.

   ```bash
   let suffix=$RANDOM*$RANDOM
   ```

8. Create a variable to hold your resource group name. This will be used when creating other resources. The `suffix` variable ensures uniqueness.

   ```bash
   resourcegroup=cosmos-db-iot-$suffix
   ```

9. Create a variable to hold your resource group location name. Replace the `westus` location with a location closest to you. This same location will be used when provisioning other Azure resources.

   ```bash
   location=westus
   ```

   > For a list of valid location names, execute: `az account list-locations -o table`

10. Enter the following command to create an Azure resource group.

    ```bash
    az group create --name $resourcegroup --location $location
    ```

### Task 2: Create Cloud Shell variables

Azure Cloud Shell allows you to create variables to store values that can be referenced when executing scripts. In this task, you will create variables in addition to the two you have already created. These variables will be used in the tasks that follow.

1. Create variables to hold names of the following services: IoT Hub, Cosmos DB account, Event Hubs namespace and event hub, Azure Storage accounts, Azure Databricks, two Azure Function Apps, a Web App with an App Service Plan, and Azure Key Vault.

   ```bash
   iothubname=iot-hub-$suffix
   cosmosdbname=cosmos-db-iot-$suffix
   functionapp1=IoT-StreamProcessing-$suffix
   functionapp2=IoT-CosmosDBProcessing-$suffix
   functionapp1storage=iotfunc1$suffix
   functionapp2storage=iotfunc2$suffix
   namespace=iot-namespace-$suffix
   storagename=iotstore$suffix
   workspace=iot-databricks-$suffix
   keyvault=iot-keyvault-$suffix
   appserviceplan=IoTWebAppPlan-$suffix
   webapp=IoTWebApp-$suffix
   ```

### Task 3: Create IoT Hub

1. Create an IoT Hub to which the simulated vehicles will connect. We are using the Basic 1 (B1) SKU with four partitions and four units.

   ```bash
   az iot hub create --name $iothubname --sku B2 --partition-count 4 --unit 4 --resource-group $resourcegroup --location $location
   ```

### Task 4: Create a Cosmos DB SQL API account

1. Execute the following to create the Cosmos DB SQL API account.

   ```bash
   az cosmosdb create --name $cosmosdbname --resource-group $resourcegroup --kind GlobalDocumentDB --locations regionName=$location
   ```

### Task 5: Create an Azure Key Vault

Azure Key Vault is a cloud service that works as a secure secrets store. You can securely store keys, passwords, certificates, and other secrets. In this task, you will create an Azure Key Vault that will be used to securely store secrets, such as your PostgreSQL database and Azure Data Lake Storage Gen2 credentials. These secrets will be accessed by Azure Databricks.

1. Enter the following to create a Key Vault:

   ```bash
   az keyvault create --name $keyvault --resource-group $resourcegroup --location $location
   ```

### Task 6: Create an event hub

In this task, you will first create an Event Hubs namespace, then add an event hub to it, which will be used to ingest data used for writing aggregates to Cosmos DB and Power BI. An Event Hubs namespace provides a unique scoping container, referenced by its fully qualified domain name, in which you create one or more event hubs.

1. Enter the following to create your Event Hubs namespace:

   ```bash
   az eventhubs namespace create --name $namespace --resource-group $resourcegroup -l $location --sku Standard --enable-auto-inflate --maximum-throughput-units 4
   ```

2. Enter the following to add an event hub named "reporting" to your namespace:

   ```bash
   az eventhubs eventhub create --name reporting --resource-group $resourcegroup --namespace-name $namespace --message-retention 4 --partition-count 4
   ```

### Task 7: Create an Azure Storage accounts

In this task, you will create three Azure Storage accounts. One is used for cold storage of all telemetry events, and the other two are used for your Function Apps.

2. Enter the following to create a general-purpose storage account used for cold storage of all telemetry events:

   ```bash
   az storage account create --name $storagename --resource-group $resourcegroup --location $location --sku Standard_LRS
   ```

3. Enter the following to create a general-purpose storage account for the first Function App:

   ```bash
   az storage account create --name $functionapp1storage --resource-group $resourcegroup --location $location --sku Standard_LRS
   ```

4. Enter the following to create a general-purpose storage account for the first Function App:

   ```bash
   az storage account create --name $functionapp2storage --resource-group $resourcegroup --location $location --sku Standard_LRS
   ```

### Task 8: Create Azure Function Apps

1. Create the Function App that contains a function triggered from IoT Hub and writes to Cosmos DB. We are disabling Application Insights for both of these Function Apps temporarily so two separate Application Insights instances are not automatically created.

   ```bash
   az functionapp create --resource-group $resourcegroup --consumption-plan-location $location --name $functionapp1 --storage-account $functionapp1storage --runtime dotnet --disable-app-insights true
   ```

2. Create the Function App that contains functions triggered from the Cosmos DB change feed.

   ```bash
   az functionapp create --resource-group $resourcegroup --consumption-plan-location $location --name $functionapp2 --storage-account $functionapp2storage --runtime dotnet --disable-app-insights true
   ```

### Task 9: Create an App Service Plan and Web App

1. Create the App Service Plan used to host the Web App.

   ```bash
   az appservice plan create -g $resourcegroup -n $appserviceplan --sku S1 --location $location
   ```

2. Create the Web App that will host the Fleet Management website.

   ```bash
   az webapp create -g $resourcegroup -p $appserviceplan -n $webapp
   ```

### Task 10: Create Azure Databricks workspace

In this task, you will use the Azure Cloud Shell to create a new Azure Databricks workspace with an Azure Resource Management (ARM) template. During the lab, you will create a Spark cluster within your Azure Databricks workspace to perform real-time stream processing against website clickstream data that is sent through Event Hubs using the Kafka protocol.

1. Execute the following command to create your Azure Databricks workspace with an ARM template:

   ```bash
   az group deployment create \
       --name DatabricksWorkspaceDeployment \
       --resource-group $resourcegroup \
       --template-uri "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-databricks-workspace/azuredeploy.json" \
       --parameters workspaceName=$workspace pricingTier=premium location=$location
   ```

### Task 11: Create Stream Analytics job

In this task, you will use the Azure portal to create a new Stream Analytics job. This service is used in this lab for real-time event processing of vehicle telemetry data after it has been saved to Cosmos DB. You will use the Stream Analytics windowing query functions to perform aggregates on the data, pumping overall vehicle statistics to Power BI for display on a real-time dashboard, and saving individual vehicle aggregates back to Cosmos DB for reporting in Power BI.

1. In the Azure portal (<https://portal.azure.com>), select **+ Create a resource**, then type **stream analytics** into the search box on top. Select **Stream Analytics job** from the results.

   ![The Create a resource link is highlighted, and Stream Analytics is displayed in the search box.](media/new-stream-analytics-job.png 'Create a resource - Stream Analytics job')

2. Select the **Create** button on the **Stream Analytics job overview** blade.

3. On the New Stream Analytics job blade, specify the following configuration options:

   1. **Job name**: Unique value for the name, similar to **Cosmos-DB-IoT-Analytics** (ensure the green check mark appears).
   2. **Subscription**: Select the Azure subscription you are using for this lab.
   3. **Resource Group**: Select the resource group you created above. The name will start with **cosmos-db-iot-** and end with a random series of numbers.
   4. **Region**: Select the same region you selected for your other Azure resources above, or the closest region if that one is not available.
   5. **Hosting environment**: Select **Cloud**.
   6. **Stream units**: Select **3**.

   ![The new Stream Analytics job form is displayed with the previously described settings.](media/new-stream-analytics-job-form.png 'Create Stream Analytics job')

4. Select **Create**.

### Task 12: Create and configure Application Insights

Application Insights helps you visualize application performance, availability, and usage information. In this lab, we use it to watch data flow through our services in real time. In this task, we create a new Application Insights instance and configure our Azure Functions and Web App to send metrics and logging information by updating their configuration settings.

1. In the Azure portal (<https://portal.azure.com>), select **+ Create a resource**, then type **application insights** into the search box on top. Select **Application Insights** from the results.

   ![The Create a resource link is highlighted, and application insights is displayed in the search box.](media/new-application-insights.png 'Create a resource - Application Insights')

2. Select the **Create** button on the **Application Insights overview** blade.

3. On the Application Insights blade, specify the following configuration options:

   1. **Subscription**: Select the Azure subscription you are using for this lab.
   2. **Resource Group**: Select the resource group you created above. The name will start with **cosmos-db-iot-** and end with a random series of numbers.
   3. **Name**: Unique value for the name, similar to **Cosmos-DB-IoT-Insights** (ensure the green check mark appears).
   4. **Region**: Select the same region you selected for your other Azure resources above, or the closest region if that one is not available.

   ![The new Application Insights form is displayed with the previously described settings.](media/new-application-insights-form.png 'Create Application Insights')

4. Select **Review + create**, then select **Create** on the validation screen.

5. You will be redirected to the Deployment screen, which shows the deployment status for Application Insights. When your deployment is complete, select **Go to resource**.

   ![The deployment page is displayed, and the Go to resource button is highlighted.](media/application-insights-deployment-complete.png 'Your deployment is complete')

6. In the Application Insights Overview blade, copy the **instrumentation key** value from the Essentials section at the top of the page.

   ![The Instrumentation Key is highlighted in the Application Insights Overview blade.](media/application-insights-instrumentation-key.png 'Instrumentation Key')

7. Navigate to your resource group by selecting the resource group name to the left of the instrumentation key, or by selecting Resource Groups on the portal's left-hand menu, then searching for it by its prefix (`cosmos-db-iot`).

8. Select the Function App whose name starts with **IoT-CosmosDBProcessing**.

   ![The IoT-CosmosDBProcessing Function App is highlighted in the resource group.](media/resource-group-function-app1.png 'Resource Group')

9. Select **Configuration** on the Function App's Overview blade.

   ![The Configuration link is highlighted.](media/function1-overview.png 'Overview blade')

10. Select **+ New application setting**.

    ![Thew New application setting link is highlighted.](media/functions-new-application-setting.png 'Application settings')

11. In the Add/Edit application setting form, enter **APPINSIGHTS_INSTRUMENTATIONKEY** in the Name field, and paste your Application Insights **Instrumentation Key** you copied earlier into the Value field. Select **OK** to save the new application setting.

    ![The new Application setting form is displayed.](media/function-app-insights-app-setting.png 'Application setting')

12. Select **Save** at the top of Application settings to apply your changes.

    ![The Save button is highlighted.](media/function-app-save-app-settings.png 'Save Application settings')

13. Go back to the resource group and select the Function App whose name starts with **IoT-StreamProcessing**.

    ![The IoT-StreamProcessing Function App is highlighted in the resource group.](media/resource-group-function-app2.png 'Resource Group')

14. Select **Configuration** on the Function App's Overview blade.

    ![The Configuration link is highlighted.](media/function2-overview.png 'Overview blade')

15. Select **+ New application setting**.

    ![Thew New application setting link is highlighted.](media/functions-new-application-setting.png 'Application settings')

16. In the Add/Edit application setting form, enter **APPINSIGHTS_INSTRUMENTATIONKEY** in the Name field, and paste your Application Insights **Instrumentation Key** you copied earlier into the Value field. Select **OK** to save the new application setting.

    ![The new Application setting form is displayed.](media/function-app-insights-app-setting.png 'Application setting')

17. Select **Save** at the top of Application settings to apply your changes.

    ![The Save button is highlighted.](media/function-app-save-app-settings.png 'Save Application settings')

18. Go back to the resource group and select your Web App. Its name starts with **IoTWebApp**.

    ![The IoTWebApp Web App is highlighted in the resource group.](media/resource-group-web-app.png 'Resource Group')

19. Select **Configuration** in the left-hand menu under Settings.

    ![The Configuration link is highlighted.](media/web-app-configuration-link.png 'Configuration link')

20. Select **+ New application setting**.

    ![The New application setting link is highlighted.](media/web-app-new-application-setting.png 'Application settings')

21. In the Add/Edit application setting form, enter **APPINSIGHTS_INSTRUMENTATIONKEY** in the Name field, and paste your Application Insights **Instrumentation Key** you copied earlier into the Value field. Select **OK** to save the new application setting.

    ![The new Application setting form is displayed.](media/function-app-insights-app-setting.png 'Application setting')

22. Select **Save** at the top of Application settings to apply your changes.

    ![The Save button is highlighted.](media/web-app-save-app-settings.png 'Save Application settings')

### Task 13: Install .NET Core SDK 2.2 (or greater)

The starter solution requires .NET Core SDK 2.2 or greater to be installed on your machine.

1. Navigate to the [.NET Core 2.2 download page](https://dotnet.microsoft.com/download/dotnet-core/2.2), then download the SDK for your environment, such as Windows .NET Core Installer x64.

   ![The webpage is displayed with the SDK download section highlighted.](media/dotnet-sdk-2-2.png 'Download .NET Core 2.2')

### Task 14: Download the starter files

Download a starter project that includes a vehicle simulator, Azure Function App projects, a Web App project, Azure Databricks notebooks, and data files used in the lab.

1. From your lab computer, download the starter files by downloading a .zip copy of the Cosmos DB scenario-based labs GitHub repo.

2. In a web browser, navigate to the [Cosmos DB scenario-based labs repo](https://github.com/solliancenet/cosmos-db-scenario-based-labs).

3. On the repo page, select **Clone or download**, then select **Download ZIP**.

   ![Download .zip containing the repository](media/github-download-repo.png 'Download ZIP')

4. Unzip the contents to your root hard drive (i.e. `C:\`). This will create a folder on your root drive named `cosmos-db-scenario-based-labs-master`.

You should follow all steps provided _before_ performing the Hands-on lab.
