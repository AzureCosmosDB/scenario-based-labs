#   Frequently Asked Questions / Issues

##  What is the difference between the `demo` and `lab` modes?

These scenerios are designed for two use cases:

+   **Demo** - a field person wants to show how cosmosdb works with a retail type environment with AI built into it.  They just want to fire up an environment and go.

+   **Lab** - an attendee or Microsoft customer/partner wants to do all the work from scratch and setup the environment to learn how to work with CosmosDB at an intimate level, and to see how to integrate AI into a solution.

##  I got an error about multiple subscriptions named the same

The script expects that you do not have more than one subscription with the same name when it enumrates the results.

##  I'm getting errors in later parts of the script, what gives?

The deploy script expects the main deployment to succeed.  In most cases it will hard stop if it fails, but sometimes it may continue even though something did not occur properly.

In several cases it could be that your subscription may be throttled by Microsoft (internal or external).  You will need to refer to the deployment status in the Azure Portal for you targeted resource group to determine what might have happened.

##  Why multiple ARM templates?

Some Azure resources do not support the `incremental` mode yet.  Stream Analytics and Logic Apps are two examples of this.  Redeploying them will overwrite many of your settings.  We break these out to ensure if you have already created them, you won't need to reconfigure the services if you redeploy something.

-   labdeploy_main.json - All the major components.  This script is requried to succeed.
-   labdeploy_cosmos.json - The CosmosDB containers
-   labdeploy_streamanalytics.json - The stream analytics resource
-   labdeploy_logicapp.json - The Logic App resource

##  How to I change the region for the deployment?

You can target the region by changing the $region variable in the deploy.ps1 PowerShell script.  We have not tested all regions other than a handful so if you go outside westus or eastus, you could experience issues.

##  Why does the script do a hard stop for user input?

Unfotunately, not everything in Azure is fully automatable.  Certain resources such as Azure Functions and Azure Databricks require user intervention in order to reach the maximum automation level available.  

The deployment script pushes the limits for full automation of a demo envrionment.  Feel free to take and use in your own deployment scripts!