<div class="MCWHeader1">
Cosmos DB scenario-based labs - Retail
</div>

<div class="MCWHeader2">
Hands-on lab step-by-step
</div>

<div class="MCWHeader3">
September 2019
</div>


Information in this document, including URL and other Internet Web site references, is subject to change without notice. Unless otherwise noted, the example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious, and no association with any real company, organization, product, domain name, e-mail address, logo, person, place or event is intended or should be inferred. Complying with all applicable copyright laws is the responsibility of the user. Without limiting the rights under copyright, no part of this document may be reproduced, stored in or introduced into a retrieval system, or transmitted in any form or by any means (electronic, mechanical, photocopying, recording, or otherwise), or for any purpose, without the express written permission of Microsoft Corporation.

Microsoft may have patents, patent applications, trademarks, copyrights, or other intellectual property rights covering subject matter in this document. Except as expressly provided in any written license agreement from Microsoft, the furnishing of this document does not give you any license to these patents, trademarks, copyrights, or other intellectual property.

The names of manufacturers, products, or URLs are provided for informational purposes only and Microsoft makes no representations and warranties, either expressed, implied, or statutory, regarding these manufacturers or the use of the products with any Microsoft technologies. The inclusion of a manufacturer or product does not imply endorsement of Microsoft of the manufacturer or product. Links may be provided to third party sites. Such sites are not under the control of Microsoft and Microsoft is not responsible for the contents of any linked site or any link contained in a linked site, or any changes or updates to such sites. Microsoft is not responsible for webcasting or any other form of transmission received from any linked site. Microsoft is providing these links to you only as a convenience, and the inclusion of any link does not imply endorsement of Microsoft of the site or the products contained therein.

© 2019 Microsoft Corporation. All rights reserved.

Microsoft and the trademarks listed at <https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/Usage/General.aspx> are trademarks of the Microsoft group of companies. All other trademarks are property of their respective owners.

**Contents** 

<!-- TOC -->

- [Cosmos DB scenario-based labs - IoT hands-on lab step-by-step](#cosmos-db-scenario-based-labs---iot-hands-on-lab-step-by-step)
  - [Abstract and learning objectives](#abstract-and-learning-objectives)
  - [Overview](#overview)
  - [Solution architecture (High-level)](#solution-architecture-high-level)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
  - [Exercise 1: Deployment and Setup](#exercise-1-deployment-and-setup)
    - [Task 1: Blah](#task-1-blah)
  - [Exercise 2: Creating and deploying rule calculations](#exercise-2-creating-and-deploying-rule-calculations)
    - [Task 1: Blah](#task-1-blah-1)
  - [Exercise 3: Simulate data and events](#exercise-3-simulate-data-and-events)
    - [Task 1: Blah](#task-1-blah-2)
  - [Exercise 4: Reporting with Stream Analytics and Power BI](#exercise-4-reporting-with-stream-analytics-and-power-bi)
    - [Task 1: Blah](#task-1-blah-3)
  - [Exercise 5: Email alerts using Logic Apps](#exercise-5-email-alerts-using-logic-apps)
    - [Task 1: Blah](#task-1-blah-4)
  - [After the hands-on lab](#after-the-hands-on-lab)
    - [Task 1: Delete resource group](#task-1-delete-resource-group)

<!-- /TOC -->

# Cosmos DB scenario-based labs - IoT hands-on lab step-by-step

## Abstract and learning objectives

In this hands-on-lab, you will complete various tasks to implment a recommendation engine using several Microsoft Azure PaaS services.

At the end of this lab you will understand how to design offline recommendation systems that store data in Cosmos DB using Data Bricks.  You will also see how to implement a ecommerce store front utilizing Cosmos DB.  Additionally, you will see how to utilize the Cosmos DB change feed to execute functions for reporting and monitoring activities with Power BI and Logic Apps.

## Overview

Contoso Movies, Ltd. has express their desire to move to a more modern and cloud-based approach to their online ecommerce presence.  The have decided to utilize Cosmos DB and Azure Databricks to implement their next generate recommendation system.

## Solution architecture (High-level)

![The proposed solution utilizing Azure Security Center for IoT and its agents to monitor and secure the IoT Devcies.  Log data is forwarded to Log Analytics where alerts and logic apps will execute to start investigation and remediation.](../Media/solution-diagram-1.png "Solution Architecture")

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.

    - Trial subscriptions will not work.
    
## Before the hands-on lab

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

## Exercise 1: Configure Databricks and generate event data

Duration: 30 minutes

Synopsis: We have pregenerated a set of events that include **buy** events.  Based on this information, a **Top Items** recommendation will be made to users that are new to the site.  You will implement this code in the web application and function applications, then deploy the applications to test the functionality.

### Task 1: Configure Azure Databricks

1.  Open the Azure Portal, navigate to your Azure DataBricks instance

1.  Click **Launch Workspace**, if prompted, login as the account you used to create your environment

1.  Click **Cluster**

1.  Click **Create Cluster**

1.  On the create cluster form, provide the following:

   - **Cluster Name**: small

   - **Cluster Type**: Standard

   - **Databricks Runtime Version**: Runtime: 5.5 (Scala 2.11, Spark 2.4.3) (**Note**: the runtime version may have **LTS** after the version. This is also a valid selection.)

   - **Python Version**: 3

   - **Enable Autoscaling**: Uncheck this option.

   - **Auto Termination**: Check the box and enter 120

   - **Worker Type**: Standard_DS3_v2

   - **Driver Type**: Same as worker

   - **Workers**: 1

1.  Select **Create Cluster**.

1.  Before continuing to the next step, verify that your new cluster is running.  Wait for the state to change from **Pending** to **Running**

1.  Click the **small** cluster, then click **Libraries**

1. If you **do not** see the **Maven** already installed on the cluster, continue to the next step. Otherwise, continue to Task 2.

1. Select **Install New**.

1. In the Install Library dialog, select **Maven** for the Library Source, then enter the following in the Coordinates field: `com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1`. Select **Install**.

1. **Wait** until the library's status shows as **Installed** before continuing.

### Task 2: Populate event data

1. Within Azure Databricks, select **Workspace** on the menu, then **Users**, select your user, then select the down arrow on the top of your user workspace. Select **Import**.

1. Within the Import Notebooks dialog, select Import from: file, then drag-and-drop the file or browse to upload it (/lab-files/Retail/Notebooks/02 Retail.dbc)

1.  Click **Import**

1. After importing, expand the new **02 Retail** folder.

1.  Select **Event Generator**

1. Before you begin, make sure you attach your cluster to the notebooks, using the dropdown. You will need to do this for each notebook you open. 

1.  Update the configuration settings for both the **readConfig** and the **writeConfig**, set the following:

- Endpoint = Cosmos DB endpoint
- Masterkey = Cosmos DB master key
- Database = Database id of the cosmos db

1.  Click **Run All**

>NOTE:  This total process will take up to 30 minutes to generate the event data.

### Task 3: Review the data generated

1.  Open your Cosmos DB instance

1.  Open the **events** collection, review the items in the collection

>NOTE:  These items are created from the data bricks solution and include a random set of generated events for each user personality type.  You should see events generated for 'details', 'buy' and 'addToCart' as well as the item associated (via the contentId field) with the event.

## Exercise 2: Complete and deploy Web App and Function Apps

Duration: 30 minutes

Synopsis: We have pregenerated a set of events that include **buy** events.  Based on this information, a **Top Items** recommendation will be made to users that are new to the site.  You will implement this code in the web application and function applications, then deploy the applications to test the functionality.

### Task 1: Implement the Top Items recommendation

1.  Browse to the **/lab-files/Retail/Contoso Movies** folder and open the **Contoso.Apps.Movies.sln** solution

1.  In the **Contoso.Apps.Movies.Web** project, open the **/Controllers/HomeController.cs** file

1.  Find the todo task #1 and complete it with the following:

```csharp
vm.RecommendProductsBought = RecommendationHelper.GetViaFunction("top", 0, 0);
```

1.  In the **Contoso.Apps.FunctionApp.Recommend** project, open the **RecommendationHelper.cs** file

1.  Find the todo task #2 and complete it with the following:

```csharp
var container = client.GetContainer(databaseId, "object");

var query = container.GetItemLinqQueryable<Item>(true)
    .Where(c => c.EntityType == "ItemAggregation")
    .OrderByDescending(c => c.BuyCount)
    .Take(take);

items = query.ToList();

foreach(Item i in items)
{
    itemIds.Add(i.ItemId.ToString());
}

topItems = GetItemsByImdbIds(itemIds);
```

1.  Review the code, notice the following:

- We are querying an "object" collection for an entity type called 'ItemAggregation' and sorting it by the 'BuyCount'.  Essentially these are the top purchased items.

- We are then querying the object collection for all the top items to get their metadata

1.  Compile the solution, fix any errors

### Task 2: Deploy the applications

1.  Right-click the **Consoto.Apps.FunctionApp.Recommed** function app project, select **Publish**

1.  Click **New**, then ensure that **Azure Functions Consumption Plan** is selected

1.  Click **Select Existing**, then click **Publish**

1.  Select your Azure Subscription, resource group and Function App to deploy too, it should be something like **s2recommend...***

1.  Click **OK**

1.  Right-click the **Contoso.Apps.Movies.Web** web app project, select **Publish**

1.  Click **New**, then ensure that **App Service** is selected

1.  Click **Select Existing**, then click **Publish**

1.  Select your Azure Subscription, resource group and Function App to deploy too, it should be something like **s2rweb...***

1.  Click **OK**, the application will publish and the site should be displayed:

### Task 3: Test the applications

1.  In the browser window that opened from your web application deployment above, check to see that you received recommendations as a non-logged in user.  You should see the following:

**TODO**

>NOTE:  These are simply suggestions based on the top purchased items from the pre-generated events.

## Exercise 3: Perform and deploy association rules calculation for offline algorithms

Duration: 30 minutes

Synopsis: Based on the pre-calculated events in the Cosmos DB for our pre-defined personality types (Comedy fan, Drama fan, etc), you will implement and deploy an algorithm that will generate these associations and put them in Cosmos DB for offline processing by the web and function applications.

### Task 1: Generate the Associations 

1.  Switch back to your Databricks workspace, select **Association Rules**

1.  Before you begin, make sure you attach your cluster to the notebooks, using the dropdown. You will need to do this for each notebook you open. 

1.  Update the configuration settings for both the **readEventsConfig** AND the **writeAssociationConfig**, set the following:

- Endpoint = Cosmos DB endpoint
- Masterkey = Cosmos DB master key
- Database = Database id of the cosmos db

1. Run each cell of the **Association Rules** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

### Task 2: Review the data generated

1.  Switch back to your Cosmos DB instance

1.  Open the **associations** collection, review the items in the collection

>NOTE:  These items are created from the data bricks solution and include the association confidence level as compared from one movie to another movie.

**TODO IMAGE**

### Task 3: Generate the Ratings

1.  Switch back to your Databricks workspace, select **Ratings**

1.  Before you begin, make sure you attach your cluster to the notebooks, using the dropdown. You will need to do this for each notebook you open. 

1.  Update the configuration settings for both the **readEventsConfig** AND the **writeAssociationConfig**, set the following:

- Endpoint = Cosmos DB endpoint
- Masterkey = Cosmos DB master key
- Database = Database id of the cosmos db

1. Run each cell of the **Ratings** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

>NOTE:  These ratings are generated as part of this notebook as an 'offline' operation.  If you collect a significant amount of user data, you would need to reevaluate the events using this notebook and populate the ratings collection again for the online calculations to utilize.

### Task 4: Review the data generated

1.  Switch back to your Cosmos DB instance

1.  Open the **ratings** collection, review the items in the collection

>NOTE:  These items are created from the data bricks solution and include the implict item ratings of a user based on their activites on the web site.

**TODO IMAGE**

## Exercise 4: Complete and deploy Web App and Function Apps (Association Rules)

Duration: 30 minutes

Synopsis: Now that we have data for our association calculations, we will add code to the web app and function app to support this new recommendation engine.

### Task 1: Implement the Associations recommendation

1.  In the **Contoso.Apps.FunctionApp.Recommend** project, open the **RecommendationHelper.cs** file

1.  In the **CollaborativeBasedRecommendation** method, find the todo task #3 and complete it with the following:

```csharp
int neighborhoodSize = 15;
double minSim = 0.0;
int maxCandidates = 100;

//inside this we do the implict rating of events for the user...
Hashtable userRatedItems = GetRatedItems(userId, 100);

if (userRatedItems.Count == 0)
    return new List<string>();

//this is the mean rating a user gave
double ratingSum = 0;

foreach(double r in userRatedItems.Values)
{
    ratingSum += r;
}

double userMean = ratingSum / userRatedItems.Count;

//get similar items
List<SimilarItem> candidateItems = GetCandidateItems(userRatedItems.Keys, minSim);

//sort by similarity desc, take only max candidates
candidateItems = candidateItems.OrderByDescending(c=>c.similarity).Take(maxCandidates).ToList();

Hashtable recs = new Hashtable();

List<PredictionModel> precRecs = new List<PredictionModel>();

foreach(SimilarItem candidate in candidateItems)
{
    int target = candidate.Target;
    double pre = 0;
    double simSum = 0;

    List<SimilarItem> ratedItems = candidateItems.Where(c=>c.Target == target).Take(neighborhoodSize).ToList();

    if (ratedItems.Count > 1)
    {
        foreach (SimilarItem simItem in ratedItems)
        {
            try
            {
                string source = userRatedItems[simItem.sourceItemId].ToString();

                //rating of the movie - userMean;
                double r = double.Parse(source) - userMean;

                pre += simItem.similarity * r;
                simSum += simItem.similarity;

                if (simSum > 0)
                {
                    PredictionModel p = new PredictionModel();
                    p.Prediction = userMean + pre / simSum;
                    p.Items = ratedItems;
                    precRecs.Add(p);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

//sort based on the prediction, only take x of them
List<PredictionModel> sortedItems = precRecs.OrderByDescending(c => c.Prediction).Take(take).ToList();

//get first model's items...
foreach(PredictionModel pm in sortedItems)
{
    foreach(SimilarItem ri in pm.Items)
    {
        if (ri.targetItemId != null)
        {
            itemIds.Add(ri.targetItemId.ToString());
            break;
        }
    }
}
```

1.  In the **Contoso.Apps.Movies.Web** project, open the **HomeController.cs** file

1.  In the **Index** method, find the todo task #4 and complete it with the following:

```csharp
vm.RecommendProductsLiked = RecommendationHelper.GetViaFunction("assoc", 0, 0);
```

### Task 2: Deploy the applications

1.  Right-click the **Consoto.Apps.FunctionApp.Recommed** function app project, select **Publish**

1.  Click **Publish**

1.  Right-click the **Contoso.Apps.Movies.Web** web app project, select **Publish**

1.  Click **Publish**, the site should load.

### Task 3: Test the applications

1.  In the browser window that opened from your web application deployment above, check to see that you received recommendations as a non-logged in user.  You should see the same results as you received previously.

2.  Click **login**, select the **comedy@contosomovies.com** account

3.  Notice the main page now has different recommendations that what you received earlier:

**TODO IMAGE**

## Exercise 4: Perform and deploy collaborative filtering rules calculation

Duration: 30 minutes

In this exercise you will TODO

### Task 1: Compute the user implict ratings

1.  Switch to your Azure DataBricks instance

1.  Open the **Ratings** notebook

1.  Set the cluster

1. Run each cell of the **Ratings** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.

### Task 2: Implement the Collaborative Rules

1.  Open the **Similarity** notebook

1.  Set the cluster

1. Run each cell of the **Similarity** notebook by selecting within the cell, then entering **Ctrl+Enter** on your keyboard. Pay close attention to the instructions within the notebook so you understand each step of the data preparation process.


### Task 2: Review the data generated

1.  Open your Cosmos DB instance

1.  Open the **similarity** collection, review the items in the collection

>NOTE:  These items are created from the data bricks solution and include the similarity of one movie, the source, to another, the target.

**TODO**

## Exercise 5: Reporting with Stream Analytics and Power BI

Duration: 30 minutes

In this exercise you will TODO

### Task 1: Setup Stream Analytics 

1.  Open the Azure Portal, navigate to your Stream Analytics job that was created for you in the setup script

1.  Click **Inputs**

1.  Click **+Add stream input**, then select **Event Hub**

1.  For the alias, type **s2event**

1.  Select your subscription

1.  Select the **s2event..** event hub

1.  For the event hub, select **store**

1.  For the policy name, select **RootManageSharedAccessKey**

1.  Click **Save**

1.  Click **Outputs**

1.  Click **+Add**, then select **Power BI**

1.  For the output alias, type **eventCount**

1.  For the dataset, type **store**

1.  For the table name, type **eventCount**

1.  Click **Authorize**, login to your Power BI instance

1.  Click **Save**

1.  Click **+Add**, then select **Power BI**

1.  For the output alias, type **eventOrdersLastHour**

1.  For the dataset, type **store**

1.  For the table name, type **eventOrdersLastHour**

1.  Click **Authorize**, login to your Power BI instance

1.  Click **+Add**, then select **Power BI**

1.  For the output alias, type **eventSummary**

1.  For the dataset, type **store**

1.  For the table name, type **eventSummary**

1.  Click **Authorize**, login to your Power BI instance

1.  Click **+Add**, then select **Power BI**

1.  For the output alias, type **failureCount**

1.  For the dataset, type **store**

1.  For the table name, type **failureCount**

1.  Click **Authorize**, login to your Power BI instance

1.  Click **+Add**, then select **Power BI**

1.  For the output alias, type **userCount**

1.  For the dataset, type **store**

1.  For the table name, type **userCount**

1.  Click **Authorize**, login to your Power BI instance

1.  Click **Save**

1.  Click **Query**

1.  Update the query to the following:

```sql
SELECT Count(*) as FailureCount
 INTO failureCount
 FROM s2event
 WHERE Event = 'paymentFailure'
 GROUP BY TumblingWindow(second,10) 

SELECT Count(distinct UserId) as UserCount
 INTO userCount
 FROM s2event  
 GROUP BY TumblingWindow(second,10) 

SELECT System.TimeStamp AS Time, Count(*)
 INTO eventCount  
 FROM s2event  
 GROUP BY TumblingWindow(second,10) 

 SELECT System.TimeStamp AS Time, Event, Count(*)
 INTO eventSummary
 FROM s2event  
 GROUP BY Event, TumblingWindow(second,10) 

 select DateAdd(second,-10,System.Timestamp()) AS WinStartTime, System.Timestamp() AS WinEndTime,0 as Min, Count(*) as Count, 10 as Target
 into eventOrdersLastHour
 from s2event
 where event = 'buy'
 GROUP BY SlidingWindow(second,10) 
```

1.  Click **Overview**, in the menu, click **Start** to start your stream analytics job

### Task 2: Configure the ChangeFeed Function

1.  In the **Contoso.Apps.FunctionApp.ChangeFeed** project, open the **FuncChangeFeed.cs** file

1.  Take a moment to review the function signature.  Notice how it is trigger based on a Cosmos DB collection

1.  Find the todo task #1 and complete it with the following:

```csharp
AddEventToEventHub(events);
```

1.  Add the following method to the function class:

```csharp
public void AddEventToEventHub(IReadOnlyList<Document> events)
{
    try
    {
        //event hub connection
        EventHubClient eventHubClient;
        string EventHubConnectionString = config["eventHubConnection"];
        string EventHubName = "store";

        var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
        {
            EntityPath = EventHubName
        };

        eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

        foreach (var e in events)
        {
            string data = JsonConvert.SerializeObject(e);
            var result = eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(data)));
        }
    }
    catch (Exception ex)
    {
        log.LogError(ex.Message);
    }
}
```

>NOTE:  This method will forward the change feed events to the event hub where stream analytics will be monitoring and then forwarding data to a Power BI dashboard

### Task 3: Deploy the ChangeFeed Function

1.  Right-click the **Consoto.Apps.FunctionApp.ChangeFeed** function app project, select **Publish**

1.  Click **New**, then ensure that **Azure Functions Consumption Plan** is selected

1.  Click **Select Existing**, then click **Publish**

1.  Select your Azure Subscription, resource group and Function App to deploy too, it should be something like **s2changefeed...***

1.  Click **OK**

### Task 4: Generate user events

1.  Right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  Notice events will be generated based on a set of users and their preferred movie type

1.  Buy events will be generated for the first 30 seconds, after that you will notice the orders per hour will fall below the target of 10.  This would signify that something is wrong with the front end web site or order processing.

### Task 5: Setup Power BI Dashabord 

1.  Open a new window to [Power BI](https://www.powerbi.com)

1.  Click on **My workspace**

1.  Click **+Create**, then select **Dashboard**

1.  For the name, type **Contoso Movies**, click **Create**

1.  Click the **...** ellipses, then select **+Add tile**

1.  Select **Custom Streaming Data**, click **Next**

1.  Select the **eventCount** data set

### Task 4: Generate more user events

1.  Switch back to Visual Studio, press **F5** to run the data generator project

1.  Switch to your Power BI dashboard, you should see it update with the event data:

TODO

## Exercise 6: Email alerts using Logic Apps

Duration: 30 minutes

In this exercise you will configure your change feed function to call an HTTP login app endpoint that will then send an email when an order event occurs.  The function will be using Polly to handle retries in the case the function app is not available.

### Task 1: Setup Logic App

1.  Open the Azure Portal to your resource group and select the Logic App in your resource group, it should be named **s2_logicapp_...**

1.  Click **Edit**

1.  Click **+New step**

1.  Search for **send an email**, then select the Office 365 outlook connector

1.  Click **Sign in**, login using your Azure AD credentials

1.  Set the **To** as your email

1.  Set the **Subject** as **Thank you for your order**

1.  Set the **Body** as **Your order is being processed**

1.  Click **Save**

1.  Click on the **When a HTTP request is received** action, copy the **HTTP POST URL** for the logic app and save it for the next task

### Task 2: Update and deploy function app

1.  In the **Contoso.Apps.FunctionApp.ChangeFeed** project, open the **FuncChangeFeed.cs** file

1.  Take a moment to review the function signature.  Notice how it is trigger based on a Cosmos DB collection

1.  Find the todo task #3 and complete it with the following:

```csharp
CallLogicApp(events);
```

1.  Add the following method to the function class:

```csharp
public async void CallLogicApp(IReadOnlyList<Document> events)
{
    try
    {
        // Have the HttpClient factory create a new client instance.
        var httpClient = _httpClientFactory.CreateClient("LogicAppClient");

        // Create the payload to send to the Logic App.
        foreach (var e in events)
        {
            var payload = new LogicAppAlert
            {
                data = JsonConvert.SerializeObject(e),
                recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail")
            };

            var postBody = JsonConvert.SerializeObject(payload);

            var httpResult = await httpClient.PostAsync(Environment.GetEnvironmentVariable("LogicAppUrl"), new StringContent(postBody, Encoding.UTF8, "application/json"));
        }
    }
    catch (Exception ex)
    {
        log.LogError(ex.Message);
    }
}
```

### Task 2: Update and deploy function app

1.  Right-click the **Consoto.Apps.FunctionApp.ChangeFeed** function app project, select **Publish**

1.  Click **Publish**

### Task 3: Test order email delivery

1.  Switch to Visual Studio, right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  For each 'buy' event, you should received an email

## After the hands-on lab 

Duration: 10 minutes

In this exercise, attendees will deprovision any Azure resources that were created in support of the lab.

### Task 1: Delete resource group

1.  Using the Azure portal, navigate to the Resource group you used throughout this hands-on lab by selecting **Resource groups** in the menu.

2.  Search for the name of your research group, and select it from the list.

3.  Select **Delete** in the command bar, and confirm the deletion by re-typing the Resource group name and selecting **Delete**.

You should follow all steps provided *after* attending the Hands-on lab.

