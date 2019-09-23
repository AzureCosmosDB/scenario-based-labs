# Cosmos DB scenario-based labs - Retail hands-on lab step-by-step

<!-- TOC -->

- [Cosmos DB scenario-based labs - Retail hands-on lab step-by-step](#Cosmos-DB-scenario-based-labs---Retail-hands-on-lab-step-by-step)
  - [Abstract and learning objectives](#Abstract-and-learning-objectives)
  - [Overview](#Overview)
  - [Solution architecture (High-level)](#Solution-architecture-High-level)
  - [Requirements](#Requirements)
  - [Before the demo](#Before-the-demo)
  - [Exercise 1: Deployment and Setup](#Exercise-1-Deployment-and-Setup)
    - [Task 1: Setup Stream Analytics](#Task-1-Setup-Stream-Analytics)
    - [Task 2: Generate user events for PowerBI](#Task-2-Generate-user-events-for-PowerBI)
    - [Task 3: Setup Power BI Dashboard](#Task-3-Setup-Power-BI-Dashboard)
    - [Task 4: Generate user events for real time](#Task-4-Generate-user-events-for-real-time)
  - [Exercise 2: Explore Contoso Movie Store](#Exercise-2-Explore-Contoso-Movie-Store)
    - [Task 1: Explore the Contoso Movie Store](#Task-1-Explore-the-Contoso-Movie-Store)
  - [Exercise 4: Email alerts using Logic Apps](#Exercise-4-Email-alerts-using-Logic-Apps)
    - [Task 1: Setup Logic App](#Task-1-Setup-Logic-App)
    - [Task 2: Configure the function app settings](#Task-2-Configure-the-function-app-settings)
    - [Task 3: Test order email delivery](#Task-3-Test-order-email-delivery)
  - [After the hands-on lab](#After-the-hands-on-lab)
    - [Task 1: Delete resource group](#Task-1-Delete-resource-group)

<!-- /TOC -->

## Abstract and learning objectives

In this demo you will show your audience how to utilize Azure services to host a movie retail store with custom AI models and CosmosDb. Several other PaaS based technologies will be used to show how Azure can be used to migrate legacy applications to the cloud.

## Overview

Contoso Movies, Ltd. has redesigned its website to utilize Azure PaaS services including CosmosDb, Functions, EventHubs, Stream Analytics, Power BI and Logic Apps. As part of this redesign they have also implemented a new recommendation system based on custom AI models. These AI models are done **offline** and stored in CosmosDb for reference when users are browsing a site. User events will implicitly rank the items they are clicking on and then modify their recommendations based on these events.

## Solution architecture (High-level)

![The Cosmos DB high level solution diagram.](./media/solution-diagram-1.png 'Solution Architecture')

## Requirements

1.  Microsoft Azure subscription must be pay-as-you-go or MSDN.

    - Trial subscriptions will not work.

1.  [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)

1.  [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) - version 2.0.68 or later

1.  [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

1.  [.Net Core 2.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

## Before the demo

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

Be sure that you change the script mode to `demo` such that the solution code is deployed to the web app and function apps.

## Exercise 1: Deployment and Setup

Duration: 60 minutes

Synopsis: In this exercise you will do the necessary setup items that could not be done in the deployment scripts.

### Task 1: Setup Stream Analytics

1.  Open the Azure Portal, navigate to your Stream Analytics job that was created for you in the setup script

1.  Select **Inputs**

1.  Select **+Add stream input**, then select **Event Hub**

1.  For the alias, type **s2events**

1.  Select your subscription

1.  Select the **s2ns..** event hub

1.  For the event hub, select **store**

1.  For the policy name, select **RootManageSharedAccessKey**

1.  Select **Save**

1.  Select **Outputs**

1.  Select **+Add**, then select **Power BI**

1.  For the output alias, type **eventOrdersLastHour**

1.  For the dataset, type **eventOrdersLastHour**

1.  For the table name, type **eventOrdersLastHour**

1.  Select **Authorize**, login to your Power BI instance

1.  Select **Save**

1.  Repeat for steps 11-16, but replace **eventOrdersLastHour** with:

-   eventSummary
-   failureCount
-   eventData

1.  Select **Query**

1.  Update the query to the following:

```sql
SELECT Count(*) as FailureCount
 INTO failureCount
 FROM s2events
 WHERE Event = 'paymentFailure'
 GROUP BY TumblingWindow(second,10)

SELECT Count(distinct UserId) as UserCount, System.TimeStamp AS Time, Count(*) as EventCount
 INTO eventData
 FROM s2events
 GROUP BY TumblingWindow(second,10)

 SELECT System.TimeStamp AS Time, Event, Count(*)
 INTO eventSummary
 FROM s2events
 GROUP BY Event, TumblingWindow(second,10)

 select DateAdd(second,-10,System.Timestamp()) AS WinStartTime, System.Timestamp() AS WinEndTime,0 as Min, Count(*) as Count, 10 as Target
 into eventOrdersLastHour
 from s2events
 where event = 'buy'
 GROUP BY SlidingWindow(second,10)
```

1.  The Query windows should look similar to this:

![An example item from the ratings collection is displayed.](./media/xx_StreamAnalytics_05.png 'The ratings collection')

1.  Select **Save query**

1.  Select **Overview**, in the menu, select **Start** to start your stream analytics job

![Go to the overview tab, then select 'start'.](./media/xx_StreamAnalytics_05.png 'Start the analytics job')

1.  In the dialog, ensure that **Now** is selected, then select **Start**

> NOTE: If your job fails for any reason, you can use the **Activity Log** to see what the error(s) were.

### Task 2: Generate user events for PowerBI

1.  Browse to the **{un-zipped repo folder}/lab-files/Retail/Solution/Contoso Movies** folder and open the **Contoso.Apps.Movies.sln** solution

1.  Right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  Notice events will be generated based on a set of users and their preferred movie type

![The data generator window is displayed with events streaming.](./media/xx_DataGenerator_01.png 'Run the data generator')

1.  Buy events will be generated for the first 30 seconds with random payment failures also generated. After 30 seconds, you will notice the orders per hour will fall below the target of 10. This would signify that something is wrong with the front end web site or order processing.

1.  After about 1 minute, close the DataGenerator console program

### Task 5: Setup Power BI Dashabord

1.  Open a new browser window to [Power BI](https://www.powerbi.com)

1.  Click **Sign In**, sign in using the same credentials you used to authorize your outputs for Stream Analytics above.

1.  Select **My workspace**

1.  Select **+Create**, then select **Dashboard**

![The main Power BI dashboard page is displayed with the '+Create' highlighted.](./media/xx_PowerBI_01.png 'Create a dashboard')

1.  For the name, type **Contoso Movies**, select **Create**

1.  Select the **...** ellipses, then select **+Add tile**

1.  Select **Custom Streaming Data**, select **Next**

1.  Select the **eventData** data set, then select **Next**

![Select the eventCount dataset and select next.](./media/xx_PowerBI_02.png 'Add the eventCount tile')

1.  For the visualization type, select **Card**

1.  For the Fields, select **EventCount**

1.  Select **Next**

1.  For the title, type **Event Count**, then select **Apply**

1.  Select **+Add tile**, you may need to select the **...** ellipses first

1.  Select **Custom Streaming Data**, select **Next**.  Use the following table to create the needed tiles:

|    |    |    |    |
|----------|-------------|-------------|:-------------:|
| **Dataset** | **Type** | **Fields** | **Title** |
| eventData | Card | UserCount  | User Count |
| failureCount | Card |FailureCount  | Payment Failures |
| eventSummary | Line cart | Axis = UserCount, Legend = Event, Values = Count  | Count By Event |
| eventOrdersLastHour | Gauge |  Value = Count, Minimum = Min, Target = Target  | Orders Per Hour |

1.  Your dashboard should look similar to the following:

![This graphic shows the layout of the tiles in the Power BI Dashboard.](./media/xx_PowerBI_03.png 'Configure the dashboard')

### Task 6: Generate user events for real time analytics

1.  Switch back to Visual Studio, press **F5** to run the data generator project

1.  Switch to your Power BI dashboard, after a few minutes, you should see it update with the event data:

![This graphic shows the layout of the tiles in the Power BI Dashboard when the event stream is running.](./media/xx_PowerBI_04.png 'Continuously updating dashboard')

## Exercise 2: Email alerts using Logic Apps

Duration: 30 minutes

In this exercise you will configure your change feed function to call an HTTP login app endpoint that will then send an email when an order event occurs. The function will be using Polly to handle retries in the case the function app is not available.

### Task 1: Setup Logic App

1.  Open the Azure Portal to your resource group and select the Logic App in your resource group, it should be named **s2*logicapp*...**

1.  Click **Edit**

![The Logic App blade with 'edit' highlighted.](./media/xx_LogicApp_01.png 'Edit the Logic App')

1.  Click **+New step**

![The Logic App Designer is displayed with 'new step' highlighted.](./media/xx_LogicApp_02.png 'Add a new step')

1.  Search for **send an email**, then select the Office 365 outlook connector

![Action search box is displayed with the text 'send an email' typed and the corresponding action highlighted.](./media/xx_LogicApp_03.png 'Add Send an Email action')

1.  Click **Sign in**, login using your Azure AD credentials

![Sign in button is highlighted.](./media/xx_LogicApp_04.png 'Sign in to Office 365')

1.  Set the **To** as your email

1.  Set the **Subject** as **Thank you for your order**

1.  Set the **Body** as **Your order is being processed**

1.  Click **Save**

![Action properties are completed and the 'Save' button is highlighted](./media/xx_LogicApp_05.png 'Complete the action properties')

1.  Click on the **When a HTTP request is received** action, copy the **HTTP POST URL** for the logic app and save it for the next task

![The http action trigger is expanded and the url is highlighted.](./media/xx_LogicApp_06.png 'Copy the function url trigger endpoint')

### Task 2: Configure the function app settings

1.  Open the Azure Portal to your resource group and select the Function App in your resource group, it should be named **s2func...**

1.  Click **Configuration**

1.  Add or update the **LogicAppUrl** configuration variable to the Logic App http endpoint you recorded above

1.  Click **Save**

### Task 3: Explore the Databricks notebooks

1.  Switch back to the Azure Portal

1.  Select your Databricks instance, then click **Launch Workspace**

1.  Browse to each of the notebooks that were deployed as part of your deployment script and review the contents with your audience.  Note the following:

- 01 Event Generator - this notebook will generate a random set of events for each target user and their personality.  This is then used to generate the 'ratings'.  Most of the generation code is in Cmd 9 and you can focus your converstation around that cell.

- 02 Associations Rules - Review the comments in Cmd 7, this describes what is happening in the rest of the notebook

- 03 Ratings - Review Cmd 9, point out the weightings for each action and then where the implict rating is created.

- 04 Similarity - REview the comments in Cmd 7, this describes what is happening in the rest of the notebook

### Task 4: Explore the Function App Recommendation Code

1.  Switch to Visual Studio and open the **Contoso.Apps.FunctionApp** project, then open the **RecommendationHelper.cs** file

1.  Navigate to the `public static List<Item> Get(string algo, int userId, int contentId, int take)` Get method signature.  Point out that this is the entry point for where a recommedation will start based on the algorithm requested.

1.  Review the following methods and their code:

- TopRecommendation - this is the basic method for randomly selecting a set of top purchased items.
- AssociationRecommendationByUser - 
- CollaborativeBasedRecommendation -

### Task 5: Explore the Function App ChangeFeed Code

1.  Switch to Visual Studio and open the **Contoso.Apps.FunctionApp** project, then open the **FuncChangeFeed.cs** file

2.  Review the Dependency Injection for the **IHttpClientFactory** and the **CosmosClient** objects:

```csharp
// Use Dependency Injection to inject the HttpClientFactory service that was configured in Startup.cs.
public FuncChangeFeed(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
{
    _httpClientFactory = httpClientFactory;
    _cosmosClient = cosmosClient;
}
```

3.  Review the following methods and their code:

- **DoAggregateCalculations** - This method updates the item aggregations for the `buy` events to keep track of the top items purchased.  This will continually update and drive the `top` suggestions.  You will see this when you execute the Data Generator tool.

- **AddEventToEventHub** - This method will forward the changefeed item to the event hub where Stream Analytics will then process the data.

- **CallLogicApp** - This method will forward the changefeed item to the logic app's http endpoint that will generate an email

### Task 6: Test order email delivery

1.  Switch to Visual Studio, right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  For each `buy` event, you should receive an email

> NOTE: You could receive quite a `few` emails.

## Exercise 3: Explore Contoso Movie Store

Duration: 15 minutes

Synopsis: You will show your attendees the Contoso Movies store. It is an ecommerce site setup using Cosmos DB as its data store. In addition, Azure Functions are monitoring the `changefeed` of Cosmos DB to execute reporting and notification activities. A second function is in charge of providing recommendations based on the logged in user. This function calls logic and pre-calculated offline AI models based on user behavior to make movie recommendations.

### Task 1: Explore the Contoso Movie Store

1.  Open the deployed Conotos Movie web site

> NOTE: This should have opened as part of the `demo` mode setup script.

1.  Mention that you are not logged in as any user and the results that are being displayed are based on the **top** purchased items in the Cosmso database.

1.  In the top navigation, select the **Login** link

1.  Mention that there are several pre-populated _personalities_. Select the **COMEDY@CONTOSOMOVIES.COM** personality

1.  Mention that you now have targeted movies based on two different algorithms (Association and Collaborative)

1.  In the top navigation, select the **COMEDY@CONTOSOMOVIES.COM** link, then select **SWITCH**

1.  Change the user to the **DRAMA@CONTOSOMOVIES.COM** user.  Note how the recommendations are different from the comedy user.

### Task 2: Create a new personality

1.  In the top navigation, select the **DRAMA@CONTOSOMOVIES.COM** link, then select **SWITCH**

1.  Select **New User**.  This will create a session as a new user that has no implict ratings (no actions have been generated).  

1.  Point out that you have no **Association** or **Collaboration** recommendations.

1.  Click on a few movies in the portal, then select **Add to Cart** for a random set.  These actions will generate events for the new user.  

1.  Click **Home**, you should now see recommendations displayed.

>NOTE:  Some movies may not have a corresponding similarity or assocations depending on the randomness of the Databricks notebook execution.  You may need to click on a few movies before you see any recommendations.

## After the hands-on lab

Duration: 10 minutes

In this exercise, attendees will deprovision any Azure resources that were created in support of the lab.

### Task 1: Delete resource group

1.  Using the Azure portal, navigate to the Resource group you used throughout this hands-on lab by selecting **Resource groups** in the menu.

2.  Search for the name of your research group, and select it from the list.

3.  Select **Delete** in the command bar, and confirm the deletion by re-typing the Resource group name and selecting **Delete**.

You should follow all steps provided _after_ attending the Hands-on lab.