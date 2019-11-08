# Cosmos DB scenario-based labs - Retail before the hands-on lab setup guide

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
<!-- TOC -->

- [Cosmos DB scenario-based labs - Retail before the hands-on lab setup guide](#cosmos-db-scenario-based-labs---retail-before-the-hands-on-lab-setup-guide)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
    - [Task 1: Download GitHub resources](#task-1-download-github-resources)
    - [Task 2: Get a Movie API Key](#task-2-get-a-movie-api-key)
    - [Task 3: Deploy resources to Azure](#task-3-deploy-resources-to-azure)

<!-- /TOC -->
</details>

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.

   - Trial subscriptions will not work.

2. [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)

3. [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) - version 2.0.68 or later

4. [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

5. [.Net Core 2.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

   - If you are running Visual Studio 2017, install SDK 2.2.109

## Before the hands-on lab

Duration: 10 minutes

Synopsis: In this exercise, you will set up your environment for use in the rest of the hands-on lab. You should follow all the steps provided in the Before the Hands-on Lab section to prepare your environment _before_ attempting the labs.

### Task 1: Download GitHub resources

1. Open a browser window to the cloud workshop GitHub repository (<https://github.com/AzureCosmosDB/scenario-based-labs>).

1. Select **Clone or download**, then select **Download Zip**.

   ![Clone or download and Download ZIP are highlighted in this screenshot of the  GitHub repository.](./media/beforehol-image1.png 'Download the zip file')

1. Extract the zip file to your local machine, be sure to keep note of where you have extracted the files. You should now see a set of folders:

   ![A set of extracted folders and files are visible in File Explorer: Hands On Lab, Media, Whiteboard design session, README.md., etc.](./media/beforehol-image2.png 'Extract the zip file')

### Task 2: Get a Movie API Key

1. Open a Chrome browser window to **https://api.themoviedb.org**

1. Create an account and copy your API Key for use in the next task

   - Select **SIGN UP**
   - Fill in your account details, select **Sign up**
   - Select your profile icon in the menu, select **Settings**
   - Select **API**

   ![The movie db right navigation.](./media/xx_MovieKey_01.png 'API Link')

   - Select **click here** underneath **Request an API Key** to generate a new key.

   ![The Click Here link is highlighted.](media/movie-key-generate.png 'Request an API Key')

   - Select the **Developer** option for the API key type, accept the license agreement, then complete the form to create a new API key. Select **Website** under _Type of Use_. You may enter any URL (such as http://www.microsoft.com), and you must enter a description, such as: `Creates a movie recommendation engine`.

   ![The Create Key form is displayed.](media/movie-key-generate-form.png 'Create API')

   - Copy the `API Key (v3 auth)` key for later use

   ![Copy the API Key v3.](./media/xx_MovieKey_02.png 'Copy the API Key v3')

### Task 3: Deploy resources to Azure

1. Open a browser window to the [Azure Portal](https://portal.azure.com), be sure to login as the user that has access to your soon to be created resource group

   > NOTE: This is necessary as the script will open a window that requires you to have already logged in to the portal.

2. Open a **PowerShell ISE** window, run the following command, if prompted, click **Yes to All**:

   ```PowerShell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
   ```

3. Execute the following command to update the Azure CLI to the latest version:

   ```PowerShell
   Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'
   ```

4. **Close** the PowerShell ISE window after the Azure CLI update completes. This ensures that the latest CLI version is used when running the following scripts.

5. **Re-open** the PowerShell ISE window, then browse to the **C:\scenario-based-labs-master\Retail\deploy\deploy.ps1** PowerShell script in an PowerShell ISE window

   > **NOTE** You can reference the [Frequently Asked Questions/Issues](FAQ.md) if you run into issues with your deployment.

6. Set the following variables:

   > NOTE: If you are performing a demo of this solution, select the "demo" setting, otherwise leave as "lab"

   ```PowerShell
   $githubPath = "PATH YOU EXTRACTED REPO ZIP TO" # Such as: C:\scenario-based-labs-master
   $mode = "lab"  # Can be 'lab' or 'demo'
   $subscriptionId = "YOUR SUBSCRIPTION ID" # You can find this at the top of your Azure resource group or any Azure resource, or in the Subscriptions link under "All services" in the portal's left-hand menu
   $prefix = "YOUR INITIALS"
   $isSpektra = $true # Set to $true if you are running in a Spektra-hosted environment (https://manage.cloudlabs.ai) instead of your own Azure subscription. Otherwise, set to $false.
   $databaseId = "movies"
   $movieApiKey = "YOUR MOVIE API KEY"
   ```

   > NOTE: You should have Azure CLI 2.0.68 or higher to run this script. You can check by running `az --version`

7. Press **F5** to run the script, this will do the following:

   - Deploy the starter ARM template(s)
   - Deploy the initial web and function apps
   - Setup the web and function app configuration variables
   - Create starter objects in the 'object' collection of the Cosmos DB database
   - Update your project application configuration files with the target azure keys and settings

8. The deployment will take 15-25 minutes to complete. You will be prompted for information in both `demo` and `lab` modes. As part of the deployment, you will see the following items created:

   - Function App
   - Web App
   - App Service Plans
   - Event Hub
   - Stream Analytics Job
   - Databricks Service
   - Cosmos DB
   - Key Vault
   - Storage Accounts
   - Application Insights

9. Record the values that were output from the script for use in the lab

You should follow all steps provided _before_ attending the hands-on lab.
