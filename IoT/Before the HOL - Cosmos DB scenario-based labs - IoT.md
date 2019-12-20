# Cosmos DB scenario-based labs - IoT before the hands-on lab setup guide

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
<!-- TOC -->

- [Cosmos DB scenario-based labs - IoT before the hands-on lab setup guide](#cosmos-db-scenario-based-labs---iot-before-the-hands-on-lab-setup-guide)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
    - [Task 1: Install .NET Core SDK 2.2 (or greater)](#task-1-install-net-core-sdk-22-or-greater)
    - [Task 2: Download the starter files](#task-2-download-the-starter-files)
    - [Task 3: Run deployment scripts](#task-3-run-deployment-scripts)

<!-- /TOC -->
</details>

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.
   - Trial subscriptions will not work.
   - **IMPORTANT**: To complete the OAuth 2.0 access components of this hands-on lab you must have permissions within your Azure subscription to create an App Registration and service principal within Azure Active Directory.
2. Install [Power BI Desktop](https://powerbi.microsoft.com/desktop/)
3. [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) - version 2.0.68 or later
4. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater
5. Install [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) or greater
   1. If you are running Visual Studio 2017, install SDK 2.2.109

## Before the hands-on lab

Duration: 10 minutes

In this exercise, you will set up your environment for use in the rest of the hands-on lab. You should follow all steps provided before attending the Hands-on lab.

> **IMPORTANT**: Many Azure resources require unique names. Throughout these steps you will see the word "SUFFIX" as part of resource names. You should replace this with your Microsoft alias, initials, or another value to ensure resources are uniquely named.

### Task 1: Install .NET Core SDK 2.2 (or greater)

he starter solution requires .NET Core SDK 2.2 or greater to be installed on your machine.

1. Navigate to the [.NET Core 2.2 download page](https://dotnet.microsoft.com/download/dotnet-core/2.2), then download the SDK for your environment, such as Windows .NET Core Installer x64.

   ![The webpage is displayed with the SDK download section highlighted.](media/dotnet-sdk-2-2.png 'Download .NET Core 2.2')

### Task 2: Download the starter files

Download a starter project that includes a vehicle simulator, Azure Function App projects, a Web App project, Azure Databricks notebooks, and data files used in the lab.

1. From your lab computer, download the starter files by downloading a .zip copy of the Cosmos DB scenario-based labs GitHub repo.

2. In a web browser, navigate to the [Cosmos DB scenario-based labs repo](https://github.com/AzureCosmosDB/scenario-based-labs).

3. On the repo page, select **Clone or download**, then select **Download ZIP**.

   ![Download .zip containing the repository](media/github-download-repo.png 'Download ZIP')

4. Unzip the contents to your root hard drive (i.e. `C:\`). This will create a folder on your root drive named `scenario-based-labs-master`.

### Task 3: Run deployment scripts

In this task, you will deploy the infrastructure for this demo using an ARM Template deployment.

1. Select the **Deploy to Azure** button below to get started. When prompted, sign in to the Azure portal with your account.

   <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzureCosmosDB%2Fscenario-based-labs%2Fmaster%2FIoT%2Fdeploy%2FlabDeploy.json" target="_blank">
   <img src="http://azuredeploy.net/deploybutton.png"/>
   </a>

2. Enter the following values:

   - **Subscription**: select the Azure subscription you are using for this lab.
   - **Resource group**: _if you are using a hosted environment, select the existing `iot` resource group provided for you_; Otherwise, create a new resource group like `cosmos-db-iot`.
   - **Location**: select the location closest to you. This value sets the Resource Group location.
   - **Location**: select the location closest to you. This value sets the location for all deployed services. _The options in this list are limited to those locations commonly available to all services in this solution_.

3. Check the **I agree to the terms and conditions stated above** box.

   ![The fields are completed as shown.](media/portal-template-params.png 'Custom template form')

4. Select **Purchase**.

5. After the deployment completes, you can select **Outputs** from the left-hand menu to find the names of your new Azure resources, as well as useful information like connection strings and endpoints. **Copy these values** to Notepad or similar text editor.

   ![The outputs are displayed after the template deployment is completed.](media/portal-deployment-outputs.png 'Microsoft Template - Outputs')

   > The template deployment will take a few minutes to complete. Continue with the hands-on lab guide once it completes.

You should follow all steps provided _before_ performing the Hands-on lab.
