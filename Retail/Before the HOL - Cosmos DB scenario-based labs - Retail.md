# Cosmos DB scenario-based labs - Retail before the hands-on lab setup guide

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
<!-- TOC -->

- [Cosmos DB scenario-based labs - Retail before the hands-on lab setup guide](#cosmos-db-scenario-based-labs---retail-before-the-hands-on-lab-setup-guide)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
    - [Task 1: Download GitHub resources](#task-1-download-github-resources)
    - [Task 2: Get a Movie Api Key](#task-2-get-a-movie-api-key)
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

1.  Open a browser window to the cloud workshop GitHub repository (<https://github.com/solliancenet/cosmos-db-scenario-based-labs>).

1.  Select **Clone or download**, then select **Download Zip**.

    ![Clone or download and Download ZIP are highlighted in this screenshot of the  GitHub repository.](./media/beforehol-image1.png 'Download the zip file')

1.  Extract the zip file to your local machine, be sure to keep note of where you have extracted the files. You should now see a set of folders:

    ![A set of extracted folders and files are visible in File Explorer: Hands On Lab, Media, Whiteboard design session, README.md., etc.](./media/beforehol-image2.png 'Extract the zip file')

### Task 2: Get a Movie Api Key

1.  Open a Chrome browser window to **https://api.themoviedb.org**

1.  Create an account and copy your API Key for use in the next task

    - Click **SIGN UP**
    - Fill in your account details, click **Sign up**
    - Click your profile icon in the menu, select **Settings**
    - Click **API**

    ![The movie db right navigation.](./media/xx_MovieKey_01.png 'API Link')

    - Copy the `API Key (v3 auth)` key for later use

    ![Copy the API Key v3.](./media/xx_MovieKey_02.png 'Copy the API Key v3')

### Task 3: Deploy resources to Azure

1.  Open a browser window to the [Azure Portal](https://portal.azure.com), be sure to login as the user that has access to your soon to be created resource group

> NOTE: This is necessary as the script will open a window that requires you to have already logged in to the portal.

1.  Open a **PowerShell ISE** window, run the following command, if prompted, click **Yes to All**:

```PowerShell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

1.  Browse to the **\$githubdir/Retail/deploy/deploy.ps1** PowerShell script in an PowerShell ISE window

1.  Set the following variables:

> NOTE: If you are performing a demo of this solution, select the "demo" setting, otherwise leave as "lab"

```PoweShell
$mode = "lab"  #can be 'lab' or 'demo'
$subscriptionId = "YOUR SUBSCRIPTION ID"
$subName = "YOUR SUBSCRIPTION NAME"
$prefix = "YOUR INIT"
$rgName = $prefix + "_s2_retail"
$databaseId = "movies"
$movieApiKey = "YOUR MOVIE API KEY"
$githubPath = "PATH YOU EXTRACTED REPO ZIP TOO"
```

> NOTE: You should have Azure CLI 2.0.68 or higher to run this script. You can check by running `az --version`

3.  Press **F5** to run the script, this will do the following:

- Deploy the starter ARM template(s)
- Deploy the initial web and function apps
- Setup the web and function app configuration variables
- Create starter objects in the 'object' collection of the Comos DB database
- Update your project application configuration files with the target azure keys and settings

4.  The deployment will take 15-25 minutes to complete. You will be prompted for information in both `demo` and `lab` modes. As part of the deployment, you will see the following items created:

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

5.  If you run the setup in `lab` or `demo` mode, you will be prompted to navigate to the generated functions app settings page before continuing. When the browser window is displayed, do the following:

- Select your function app, it will start with **s2func...**
- Select the **Function app settings** link
- Respond `yes` to the PowerShell prompt

6.  If you run the setup in `demo` mode, you will be prompted to enter your Databricks API token. A new browser window will open to allow you to login to your databricks instance.

7.  When the browser window opens, select your Databricks instance, it should start with **s2databricks...**

8.  Once logged in, click the user icon and select **User settings**

![Click your icon, then select User Settings.](./media/xx_DatabricksKey_01.png 'User Settings link')

8.  Click **Generate New Token**, copy the API key and enter into the PowerShell window. Databricks will proceed to configure itself and execute the necessary notebooks.

![Click the Generate New Token button.](./media/xx_DatabricksKey_02.png 'Generate a Token')

10. Record the values that were output from the script for use in the lab

You should follow all steps provided _before_ attending the hands-on lab.
