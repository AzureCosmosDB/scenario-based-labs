# Cosmos DB scenario-based labs - Retail hands-on lab step-by-step

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
<!-- TOC -->

- [Cosmos DB scenario-based labs - Retail hands-on lab step-by-step](#cosmos-db-scenario-based-labs---retail-hands-on-lab-step-by-step)
  - [Abstract and learning objectives](#abstract-and-learning-objectives)
  - [Overview](#overview)
  - [Solution architecture (High-level)](#solution-architecture-high-level)
  - [Requirements](#requirements)
  - [Before the demo](#before-the-demo)
  - [Exercise 1: Deployment and Setup](#exercise-1-deployment-and-setup)
  - [Exercise 1: Configure Databricks and generate event data](#exercise-1-configure-databricks-and-generate-event-data)
    - [Task 1: Configure Azure Databricks](#task-1-configure-azure-databricks)
    - [Task 2: Populate event data](#task-2-populate-event-data)
    - [Task 3: Run the aggregation and import utility](#task-3-run-the-aggregation-and-import-utility)
    - [Task 4: Perform and deploy association rules calculation for offline algorithms](#task-4-perform-and-deploy-association-rules-calculation-for-offline-algorithms)
  - [Task 5: Perform and deploy collaborative filtering rules calculation](#task-5-perform-and-deploy-collaborative-filtering-rules-calculation)
    - [Task 6: Generate the Collaborative Rules](#task-6-generate-the-collaborative-rules)
    - [Task 7: Setup Stream Analytics](#task-7-setup-stream-analytics)
    - [Task 8: Generate user events for PowerBI](#task-8-generate-user-events-for-powerbi)
    - [Task 9: Setup Power BI Dashboard](#task-9-setup-power-bi-dashboard)
    - [Task 10: Generate user events for real time analytics](#task-10-generate-user-events-for-real-time-analytics)
  - [Exercise 2: Email alerts using Logic Apps](#exercise-2-email-alerts-using-logic-apps)
    - [Task 1: Setup Logic App](#task-1-setup-logic-app)
    - [Task 2: Configure the function app settings](#task-2-configure-the-function-app-settings)
    - [Task 3: Explore the Databricks notebooks](#task-3-explore-the-databricks-notebooks)
    - [Task 4: Explore the Function App Recommendation Code](#task-4-explore-the-function-app-recommendation-code)
    - [Task 5: Explore the Function App ChangeFeed Code](#task-5-explore-the-function-app-changefeed-code)
    - [Task 6: Test order email delivery](#task-6-test-order-email-delivery)
  - [Exercise 3: Explore Contoso Movie Store](#exercise-3-explore-contoso-movie-store)
    - [Task 1: Explore the Contoso Movie Store](#task-1-explore-the-contoso-movie-store)
    - [Task 2: Create a new personality](#task-2-create-a-new-personality)
  - [After the hands-on lab](#after-the-hands-on-lab)
    - [Task 1: Delete resource group](#task-1-delete-resource-group)

<!-- /TOC -->
</details>

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

> **NOTE** You can run the following commands to install the latest

```PowerShell
Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi;
Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'
```

1.  [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

1.  [.Net Core 2.2](https://dotnet.microsoft.com/download/visual-studio-sdks)

## Before the demo

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

Be sure that you change the script mode to `demo` such that the solution code is deployed to the web app and function apps.

## Exercise 1: Deployment and Setup

Duration: 60 minutes

Synopsis: In this exercise you will do the necessary setup items that could not be done in the deployment scripts.

## Exercise 1: Configure Databricks and generate event data

**Duration**: 30 minutes

**Synopsis**: We have pre-generated a set of events that include **buy** and **details** events. Based on this data, a **Top Items** recommendation will be made to users that are new to the site (aka a cold start recommendation). You will implement this top items code in the web application and function applications, then deploy the applications to test the functionality.

The algorithms for creating the offline calculations are written in Python and are executed via Azure Databricks.

### Task 1: Configure Azure Databricks

1. Open the Azure portal (<https://portal.azure.com>), search for your assigned lab resource group. If you were not assigned a resource group, your generated resource group will be named after the following pattern: **YOURINIT-s2-retail**.

2. Select your resource group, and then select your Azure Databricks instance, it should be named **s2_databricks...**.

3. Select **Launch Workspace**, if prompted, login as the account you used to create your environment.

4. In the side navigation, Select **Clusters**.

   ![The cluster blade with all the settings filled in.](./media/xx_Databricks_01.png 'Databricks cluster configuration')

5. Select **Create Cluster**.

6. On the create cluster form, provide the following:

   - **Cluster Name**: small

   - **Cluster Type**: Standard

   - **Databricks Runtime Version**: Runtime: 5.5 (Scala 2.11, Spark 2.4.3) (**Note**: the runtime version may have **LTS** after the version. This is also a valid selection.)

   - **Python Version**: 3

   - **Enable Autoscaling**: Uncheck this option.

   - **Auto Termination**: Check the box and enter 120

   - **Worker Type**: Standard_DS3_v2

   - **Driver Type**: Same as worker

   - **Workers**: 1

7. Select **Create Cluster**.

   ![The cluster blade with all the settings filled in.](./media/xx_Databricks_02.png 'Databricks cluster configuration')

8. Before continuing to the next step, verify that your new cluster is running. Wait for the state to change from **Pending** to **Running**.

9. Select the **small** cluster, then select **Libraries**.

10. Select **Install New**.

    ![Navigate to the libraries tab and select `Install New`.](./media/xx_Databricks_03.png 'Adding a new library')

11. In the Install Library dialog, select **Maven** for the Library Source.

12. In the Coordinates field type:

    ```text
    com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1
    ```

13. Select **Install**.

    ![Populated library dialog for Maven.](./media/xx_Databricks_04.png 'Add the Maven library')

14. **Wait** until the library's status shows as **Installed** before continuing.

### Task 2: Populate event data

1. Within Azure Databricks, select **Workspace** on the menu, then **Users**, select your user, then select the down arrow on the top of your user workspace. Select **Import**.

2. Within the Import Notebooks dialog, select Import from: file, then drag-and-drop the file or browse to upload it (`{un-zipped repo folder}/Retail/Notebooks/02 Retail.dbc`)

3. Select **Import**

   ![Workspace is highlighted with the user expanded and the Import option highlighted.](./media/xx_Databricks_07.png 'Import the Databricks notebook')

4. After importing, select the new **02 Retail** folder, then navigation to the **Includes** folder

5. Select the **Shared-Configuration** notebook

   ![The workspace menu is displayed with `includes` and `shared-configuration` highlighted.](./media/xx_Databricks_08.png 'Navigate to Shared-Configuration')

6. Update the configuration settings and set the following using the values from your lab setup script output:

   - Endpoint = Cosmos DB endpoint url
   - Masterkey = Cosmos DB master key
   - Database = Database id of the cosmos db ('movies')

   > If you do not have your setup script output values available for reference, you may find the `Endpoint` and `Masterkey` values by navigating to your Cosmos DB account in the Azure portal, then selecting **Keys** in the left-hand menu. Copy the **URI** value for `Endpoint`, and **Primary Key** for the `Masterkey` value.

   ![The Cosmos DB Keys blade is displayed.](media/cosmos-db-keys.png 'Cosmos DB Keys')

7. Attach your cluster to the notebook using the dropdown. You will need to do this for each notebook you open. In the drop down, select the **small** cluster.

   ![The Shared-Configuration notebook is displayed.](media/databricks-shared-configuration.png 'Azure Databricks Shared-Configuration notebook')

8. Next, navigate back up to **02 Retail** and select the **01 Event Generator** notebook

   > This notebook will simulate the browsing and purchasing activity for six users with different personality based preferences and save the result to the `events` container in Cosmos DB.

   > The movies have been pre-selected and sorted into the genres of comedy, drama and action. While the actual movie selection and activity taken is random, it is weighted to respect the user's preferences in each genre to hit a distribution that would mirror that user's taste.

   > For example, user 400001 has the preference of 20 for comedy, 30 for drama, 50 for action. This will result in the user logging more activity with action movies.

   > NOTE: Your results (aka the `events` generated) may be different from your fellow lab participants

9. Attach your cluster to the notebook using the dropdown. In the drop down, select the **small** cluster.

   ![Click the `detached` drop down, select the small cluster.](./media/xx_Databricks_09.png 'Set the cluster')

10. Select **Run All**.

### Task 3: Run the aggregation and import utility

1. Browse to the **{un-zipped repo folder}/Retail/Solution/Contoso Movies** folder and open the **Contoso.Apps.Movies.sln** solution.

   > If Visual Studio prompts you to sign in when it first launches, use the account provided to you for this lab (if applicable), or an existing Microsoft account.

2. Within the Solution Explorer, expand the **/Utilities/MovieDataImport** project and open the **Program.cs** file. Take a few moments to browse code. You will see that it:

   - Aggregates all the event data generated from the Databricks notebook
   - Creates the user personalities
   - Creates the movie categories/genres
   - Creates the movies

   ![The MovieDataImport is expanded in Solution Explorer and program.cs is selected.](media/vs-moviedataimport-program.png 'Solution Explorer')

3. Right-click the project, select **Set as startup project**.

4. Press **F5** to run the project.

   You may see several of the following lines output to the console window after saving the genres and before adding the movies: `Input string was not in a correct format.`. You can safely ignore these due to some movies the API retrieved are poorly formatted.

> NOTE: You must wait for the Event Generator Databricks notebook to be completed first before running this step. This is to ensure that later steps in the lab to match.

### Task 4: Perform and deploy association rules calculation for offline algorithms

**Synopsis**: Based on the pre-calculated events in the Cosmos DB for our pre-defined personality types (Comedy fan, Drama fan, etc.), you will implement and deploy an algorithm that will generate these associations and put them in Cosmos DB for offline processing by the web and function applications.

1. Switch back to your Databricks workspace and open the **02 Association Rules** notebook.

1. Attach your cluster to the notebook using the dropdown. In the drop down, select the **small** cluster.

1. Run each cell of the **02 Association Rules** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

> The goal of this algorithm is to compute two metrics that indicate the strength of a relationship between a source item and a target item based on event history, and then save that matrix to the associations container in Cosmos DB.

> The algorithm begins with grouping events with a buy action into a transaction, grouping by the sessionId. This provides the set of items bough together.

> For example, a transaction with two items would look like: `'404973': ['5512872', '4172430']` where 404973 is the sessionId that is used as the transactionId, and the the array contains the id's of the items bought ('5512872' and '4172430').

## Task 5: Perform and deploy collaborative filtering rules calculation

**Synopsis**: In this exercise you will execute the implict ratings notebook in Azure Databricks to generate the implict rating for each user that has event data. You will only execute this once during this lab, however this notebook would need to be run on a set schedule to ensure that the users rating data is up to date.

1. Within Azure Databricks, open **03 Ratings**.

1. Attach your cluster to the notebook using the dropdown. In the drop down, select the **small** cluster.

1. Run each cell of the **03 Ratings** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

   > This notebook will use the implict events captured in the events container in Cosmos DB to calculate what a user would rate a given item, based on their actions. In other words it converts a users buy, addToCart and details actions into a numeric score for the item. The resulting user to item ratings matrix will be saved to the ratings container in Cosmos DB.

1. Switch back to the Azure portal.

1. In your resource group, navigate to your Cosmos DB instance.

1. Open the **ratings** container, review the items in the container.

   ![An example item from the ratings container is displayed.](./media/xx_RatingsColl.png 'The ratings container')

   > NOTE: These ratings are generated as part of this notebook as an 'offline' operation. If you collect a significant amount of user data, you would need to re-evaluate the events using this notebook and populate the ratings container again for the online calculations to utilize.

### Task 6: Generate the Collaborative Rules

1. Within Azure Databricks, open **04 Similarity**.

1. Attach your cluster to the notebook using the dropdown. In the drop down, select the **small** cluster.

1. Run each cell of the **04 Similarity** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

   > The notebook logic uses the user to item ratings previously created to calculate a score indicating the similarity between a source item and a target item. The process begins by loading the ratings matrix and for each user to item rating, calculating a new normalized rating (to adjust for the user's bias).

   > An overlap matrix is calculated that identifies, for any pair of items, how many users rated both items. First, the normalized ratings matrix is converted to a Boolean matrix. That is, if an item for a user has a rating (regardless of the value of the rating), it has a value of 1, otherwise it is zero. Then dot product of the normalized ratings matrix against its transpose is calculated. This yields a simpler matrix where the value each cell now contains the count of the number users who rated both items. Cells that don't have any overlap, have a value of zero.

   > Separately, the cosine similarity of the normalized ratings matrix is computed. It's easiest to understand the cosine similarity calculation as being done between an item `i` and another item `j`. The cosine similarity is a ratio:

   - The numerator is computed as the sum of the product of the normalized rating of item i multiplied with the rating of j, for all users who have provided ratings.
     The denominator is computed as the square root of the sum of the squares of the normalized rating of item i multiplied by the square root of the sum of thesquares of the normalized rating of item j.
     In Python, the logic uses the cosine_similarity method from scikit-learn to compute the similarity between items by providing it our normalized user-to-items ratings matrix.

   > The result is then filtered to remove entries with a similarity score lower than configured, and having an overlap in the overlap matrix of less than a configured overlap in quantity of ratings for the pair of items. Just before saving, any resulting similarities with scores less than the configured minimum similarity are removed, so that weaker similarities are not recommended.

### Task 7: Setup Stream Analytics

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

- eventSummary
- failureCount
- eventData

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

### Task 8: Generate user events for PowerBI

1.  Browse to the **{un-zipped repo folder}/Retail/Solution/Contoso Movies** folder and open the **Contoso.Apps.Movies.sln** solution

1.  Right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  Notice events will be generated based on a set of users and their preferred movie type

![The data generator window is displayed with events streaming.](./media/xx_DataGenerator_01.png 'Run the data generator')

1.  Buy events will be generated for the first 30 seconds with random payment failures also generated. After 30 seconds, you will notice the orders per hour will fall below the target of 10. This would signify that something is wrong with the front end web site or order processing.

1.  After about 1 minute, close the DataGenerator console program

### Task 9: Setup Power BI Dashboard

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

  > **Important**: If the **eventData** data set does not appear, it is because there is a lag time of several minutes between when you first configure the Stream Analytics Power BI output and when data first appears in the streaming data set. Please ensure the data generator is running and that you have started the Stream Analytics query. Also, you may try restarting the Function App as well.

1.  For the visualization type, select **Card**

1.  For the Fields, select **EventCount**

1.  Select **Next**

1.  For the title, type **Event Count**, then select **Apply**

1.  Select **+Add tile**, you may need to select the **...** ellipses first

1.  Select **Custom Streaming Data**, select **Next**. Use the following table to create the needed tiles:

|                     |           |                                                  |                  |
| ------------------- | --------- | ------------------------------------------------ | :--------------: |
| **Dataset**         | **Type**  | **Fields**                                       |    **Title**     |
| eventData           | Card      | UserCount                                        |    User Count    |
| failureCount        | Card      | FailureCount                                     | Payment Failures |
| eventSummary        | Line cart | Axis = UserCount, Legend = Event, Values = Count |  Count By Event  |
| eventOrdersLastHour | Gauge     | Value = Count, Minimum = Min, Target = Target    | Orders Per Hour  |

1.  Your dashboard should look similar to the following:

![This graphic shows the layout of the tiles in the Power BI Dashboard.](./media/xx_PowerBI_03.png 'Configure the dashboard')

### Task 10: Generate user events for real time analytics

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

1.  Browse to each of the notebooks that were deployed as part of your deployment script and review the contents with your audience. Note the following:

- **01 Event Generator** - this notebook will generate a random set of events for each target user and their personality. This is then used to generate the 'ratings'. Most of the generation code is in Cmd 9 and you can focus your converstation around that cell.

- **02 Associations Rules** - Review the comments in Cmd 7, this describes what is happening in the rest of the notebook

- **03 Ratings** - Review Cmd 9, point out the weightings for each action and then where the implict rating is created.

- **04 Similarity** - REview the comments in Cmd 7, this describes what is happening in the rest of the notebook

### Task 4: Explore the Function App Recommendation Code

1. Switch to Visual Studio and open the **Contoso.Apps.FunctionApp** project, then open the **RecommendationHelper.cs** file

1. Navigate to the `public static List<Item> Get(string algo, int userId, int contentId, int take)` Get method signature. Point out that this is the entry point for where a recommedation will start based on the algorithm requested.

1. Review the following methods and their code:

- TopRecommendation - this is the basic method for randomly selecting a set of top purchased items.
- AssociationRecommendationByUser
- CollaborativeBasedRecommendation

### Task 5: Explore the Function App ChangeFeed Code

1. Switch to Visual Studio and open the **Contoso.Apps.FunctionApp** project, then open the **FuncChangeFeed.cs** file

1. Review the Dependency Injection for the **IHttpClientFactory** and the **CosmosClient** objects:

```csharp
// Use Dependency Injection to inject the HttpClientFactory service that was configured in Startup.cs.
public FuncChangeFeed(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
{
    _httpClientFactory = httpClientFactory;
    _cosmosClient = cosmosClient;
}
```

1. Review the following methods and their code:

- **DoAggregateCalculations** - This method updates the item aggregations for the `buy` events to keep track of the top items purchased. This will continually update and drive the `top` suggestions. You will see this when you execute the Data Generator tool.  These aggregations will be stored in the `object` table as an `ItemAggregation` object type.

- **AddEventToEventHub** - This method will forward the changefeed item to the event hub where Stream Analytics will then process the data.

- **CallLogicApp** - This method will forward the changefeed item to the logic app's http endpoint that will generate an email

### Task 6: Test order email delivery

1. Switch to Visual Studio, right-click the **DataGenerator** project, select **Set as startup project**

1. Press **F5** to run the project

1. For each `buy` event, you should receive an email

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

1.  Change the user to the **DRAMA@CONTOSOMOVIES.COM** user. Note how the recommendations are different from the comedy user.

### Task 2: Create a new personality

1.  In the top navigation, select the **DRAMA@CONTOSOMOVIES.COM** link, then select **SWITCH**

1.  Select **New User**. This will create a session as a new user that has no implict ratings (no actions have been generated).

1.  Point out that you have no **Association** or **Collaboration** recommendations.

1.  Click on a few movies in the portal, then select **Add to Cart** for a random set. These actions will generate events for the new user.

1.  Click **Home**, you should now see recommendations displayed.

> NOTE: Some movies may not have a corresponding similarity or assocations depending on the randomness of the Databricks notebook execution. You may need to click on a few movies before you see any recommendations.

## After the hands-on lab

Duration: 10 minutes

In this exercise, attendees will deprovision any Azure resources that were created in support of the lab.

### Task 1: Delete resource group

1.  Using the Azure portal, navigate to the Resource group you used throughout this hands-on lab by selecting **Resource groups** in the menu.

2.  Search for the name of your research group, and select it from the list.

3.  Select **Delete** in the command bar, and confirm the deletion by re-typing the Resource group name and selecting **Delete**.

You should follow all steps provided _after_ attending the Hands-on lab.
