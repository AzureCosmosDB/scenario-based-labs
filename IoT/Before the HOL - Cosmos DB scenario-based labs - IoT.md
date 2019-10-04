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
3. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater
4. Install [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) or greater

## Before the hands-on lab

Duration: 10 minutes

In this exercise, you will set up your environment for use in the rest of the hands-on lab. You should follow all steps provided before attending the Hands-on lab.

> **IMPORTANT**: Many Azure resources require unique names. Throughout these steps you will see the word "SUFFIX" as part of resource names. You should replace this with your Microsoft alias, initials, or another value to ensure resources are uniquely named.

### Task 1: Install .NET Core SDK 2.2 (or greater)

The starter solution requires .NET Core SDK 2.2 or greater to be installed on your machine.

1. Navigate to the [.NET Core 2.2 download page](https://dotnet.microsoft.com/download/dotnet-core/2.2), then download the SDK for your environment, such as Windows .NET Core Installer x64.

   ![The webpage is displayed with the SDK download section highlighted.](media/dotnet-sdk-2-2.png 'Download .NET Core 2.2')

### Task 2: Download the starter files

Download a starter project that includes a vehicle simulator, Azure Function App projects, a Web App project, Azure Databricks notebooks, and data files used in the lab.

1. From your lab computer, download the starter files by downloading a .zip copy of the Cosmos DB scenario-based labs GitHub repo.

2. In a web browser, navigate to the [Cosmos DB scenario-based labs repo](https://github.com/solliancenet/cosmos-db-scenario-based-labs).

3. On the repo page, select **Clone or download**, then select **Download ZIP**.

   ![Download .zip containing the repository](media/github-download-repo.png 'Download ZIP')

4. Unzip the contents to your root hard drive (i.e. `C:\`). This will create a folder on your root drive named `cosmos-db-scenario-based-labs-master`.

### Task 3: Run deployment scripts

In this task, you will deploy the infrastructure for this demo using an ARM Template deployment.

1. In the [Azure portal](https://portal.azure.com), navigate to **Azure Active Directory** in the left-hand menu, then select **Users** under Manage.

   ![Azure Active Directory blade with Users highlighted](media/deploy-azure-ad-users-link.png 'Azure Active Directory blade with Users highlighted')

2. Select your user from the list with which you logged in to the portal.

   ![All users list with user highlighted](media/deploy-azure-ad-user-list.png 'All users list with user highlighted')

3. On the **User** blade, copy the **Object ID** for this user.

   ![User blade is shown with Object ID highlighted](media/deploy-azure-ad-user-object-id.png 'User blade is shown with Object ID highlighted')

4. In the Azure portal, select **+ Create a resource**. Enter **template** in the search box, then select **Template deployment (deploy using custom templates)**.

   ![Create a resource is highlighted, and template is entered in the search box.](media/portal-search-template.png 'New Template Resource')

5. Select **Create** on the Template deployment overview blade.

6. On the **Custom deployment** pane, select **Build your own template in the editor**.

   ![The Build your own template in the editor option is highlighted.](media/portal-custom-deployment-build-own.png 'Custom deployment')

7. On the **Edit template** blade, select the **Load file** button and upload the **labDeploy.json** ARM Template located at `\cosmos-db-scenario-based-labs\IoT\deploy\labDeploy.json`.

   ![The load file button is highlighted and the labDeploy.json file is selected](media/portal-custom-deployment-load.png 'Load file')

8. Select **Save**.

9. Enter the following values:

   - Subscription: select the Azure subscription you are using for this lab.
   - Resource group: _if you are using a hosted environment, select the existing `iot` resource group provided for you_; Otherwise, create a new resource group like `cosmos-db-iot`.
   - Location: _it doesn't matter which region is selected, the template will use West US to ensure everything works_
   - Key Vault Access Policy User Object Id: paste your user account's `Object ID` you copied earlier.

10. Check the **I agree to the terms and conditions stated above** box.

    ![The fields are completed as shown.](media/portal-template-params.png 'Custom template form')

11. Select **Purchase**

    After the deployment completes, you can select **Outputs** from the left-hand menu to find the names of your new Azure resources, as well as useful information like connection strings and endpoints.

    ![The outputs are displayed after the template deployment is completed.](media/portal-deployment-outputs.png 'Microsoft Template - Outputs')

    > The template deployment will take a few minutes to complete. Continue with the hands-on lab guide once it completes.

You should follow all steps provided _before_ performing the Hands-on lab.
