<div class="MCWHeader1">
Cosmos DB scenario-based labs - IoT
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
  - [Overview](#overview)
  - [Solution architecture](#solution-architecture)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
  - [Exercise 1: Configure environment](#exercise-1-configure-environment)
    - [Task 1: Create Cosmos DB database and container](#task-1-create-cosmos-db-database-and-container)
      - [About Cosmos DB throughput](#about-cosmos-db-throughput)
      - [About Cosmos DB partitioning](#about-cosmos-db-partitioning)
    - [Task 2: Configure Cosmos DB container indexing and TTL](#task-2-configure-cosmos-db-container-indexing-and-ttl)
      - [About the Cosmos DB indexing policies](#about-the-cosmos-db-indexing-policies)
  - [Task 3: Create a Logic App workflow for email alerts](#task-3-create-a-logic-app-workflow-for-email-alerts)
    - [Task 4: Add Key Vault secrets](#task-4-add-key-vault-secrets)
    - [Task 4: Create Azure Databricks cluster](#task-4-create-azure-databricks-cluster)
    - [Task 5: Configure Key Vault-backed Databricks secret store](#task-5-configure-key-vault-backed-databricks-secret-store)
  - [Exercise 2: Deploy Azure functions and Web App](#exercise-2-deploy-azure-functions-and-web-app)
    - [Task 1: Open solution](#task-1-open-solution)
    - [Task 2: Code walk-through](#task-2-code-walk-through)
    - [Task 3: Deploy Event Hub consumer Function App](#task-3-deploy-event-hub-consumer-function-app)
    - [Task 4: Deploy Change Feed consumer Function App](#task-4-deploy-change-feed-consumer-function-app)
    - [Task 5: Deploy Web App](#task-5-deploy-web-app)
    - [Task 6: Configure application settings in Azure](#task-6-configure-application-settings-in-azure)
  - [Exercise 3: Configure Logic App for e-mail alerts](#exercise-3-configure-logic-app-for-e-mail-alerts)
    - [Task 1: Create new workflow](#task-1-create-new-workflow)
    - [Task 2: Configure email service](#task-2-configure-email-service)
  - [Exercise 4: Explore and execute data generator](#exercise-4-explore-and-execute-data-generator)
    - [Task 1: Open solution](#task-1-open-solution-1)
    - [Task 2: Update application configuration](#task-2-update-application-configuration)
    - [Task 3: Code walk-through](#task-3-code-walk-through)
    - [Task 4: Run generator](#task-4-run-generator)
  - [Exercise 5: Observe data using Cosmos DB Data Explorer and Web App](#exercise-5-observe-data-using-cosmos-db-data-explorer-and-web-app)
    - [Task 1: View data in Cosmos DB Data Explorer](#task-1-view-data-in-cosmos-db-data-explorer)
    - [Task 2: Search and view data in Web App](#task-2-search-and-view-data-in-web-app)
  - [Exercise 6: Performing CRUD operations using the Web App](#exercise-6-performing-crud-operations-using-the-web-app)
    - [Task 1: Update vehicle metadata](#task-1-update-vehicle-metadata)
    - [Task 2: View consignment, package, and trip data](#task-2-view-consignment-package-and-trip-data)
  - [Exercise 7: Observe Change Feed using Azure Functions and App Insights](#exercise-7-observe-change-feed-using-azure-functions-and-app-insights)
    - [Task 1: Open App Insights Live View](#task-1-open-app-insights-live-view)
  - [Exercise 8: Running the predictive maintenance batch scoring](#exercise-8-running-the-predictive-maintenance-batch-scoring)
    - [Task 1: Import lab notebooks into Azure Databricks](#task-1-import-lab-notebooks-into-azure-databricks)
    - [Task 2: Run batch scoring notebook](#task-2-run-batch-scoring-notebook)
    - [Task 3: Create scheduled notebook job](#task-3-create-scheduled-notebook-job)
  - [Exercise 9: Deploying the predictive maintenance web service](#exercise-9-deploying-the-predictive-maintenance-web-service)
    - [Task 1: Run deployment notebook](#task-1-run-deployment-notebook)
  - [Exercise 10: Configure windowed queries in Stream Analytics](#exercise-10-configure-windowed-queries-in-stream-analytics)
    - [Task 1: Add Stream Analytics Event Hubs input](#task-1-add-stream-analytics-event-hubs-input)
    - [Task 2: Add Stream Analytics outputs](#task-2-add-stream-analytics-outputs)
    - [Task 3: Create Stream Analytics query](#task-3-create-stream-analytics-query)
    - [Task 4: Run query](#task-4-run-query)
  - [Exercise 11: Creating the Fleet status real-time dashboard in Power BI](#exercise-11-creating-the-fleet-status-real-time-dashboard-in-power-bi)
    - [Task 1: Log in to Power BI online](#task-1-log-in-to-power-bi-online)
    - [Task 2: Create real-time dashboard](#task-2-create-real-time-dashboard)
  - [Exercise 12: Creating the Predictive Maintenance & Trip/Consignment Status reports in Power BI](#exercise-12-creating-the-predictive-maintenance--tripconsignment-status-reports-in-power-bi)
    - [Task 1: Add Cosmos DB data sources to Power BI Desktop](#task-1-add-cosmos-db-data-sources-to-power-bi-desktop)
    - [Task 2: Create new report in Power BI Desktop](#task-2-create-new-report-in-power-bi-desktop)
  - [After the hands-on lab](#after-the-hands-on-lab)
    - [Task 1: Delete the resource group](#task-1-delete-the-resource-group)

<!-- /TOC -->

# Cosmos DB scenario-based labs - IoT hands-on lab step-by-step

## Overview

![A big rig truck is displayed containing packages and telemetry sensors.](media/truck-with-sensors.png 'Truck with sensors')

Contoso Auto is a high value cargo logistics organization that is collecting vehicle and package telemetry data and wants to use Azure Cosmos DB to rapidly ingest and store this data in its raw form, do some processing in near real-time to generate insights to support several business objectives and surface these to the most appropriate user communities within the organization. It is a fast growing organization and wants to be able to scale and manage the associated cost of its chosen technology to enable it to cope with its explosive growth and the inherent seasonality of the logistics business. This scenario includes applicability to both the vehicle telemetry and logistics use cases by focusing on trucking and inclusion of cargo sensing data. This additionally allows for many representative customer analytics scenarios.

From a technology perspective Contoso would like to leverage Azure Cosmos DB as the core repository for its hot data path and leverage the Azure Cosmos DB Change Feed as a means to drive a solid and robust event sourcing architecture that would allowing Contoso developers to quickly enhance the solution. This achieved using a robust and agile serverless approach by leveraging events published by the Change Feed that reflect the state changes within the application (database).

Ultimately Contoso would surface the raw and derived insights data to its users in one of three roles:

- **Logistic Operations personnel** who are interested in the current state of the vehicles and cargo logistics and who would use a web app to quickly understand the status of any single vehicle or piece of cargo, be notified of alerts as well as load vehicle and cargo meta data into the system. What they would like to see on the dashboard are various visualizations of detected anomalies, like engines overheating, abnormal oil pressure, and aggressive driving.

- **Management and Customer Reporting personnel** who would like to be in a position to see the current state of the vehicle fleet and customer consignment level information presented in on a Power BI report that automatically updates with new data as it flows in after being processed. What they would like to see are reports on bad driving behavior by driver and using visual components such as a map to show anomalies related to cities or areas, as well as various charts and graphs depicting aggregate fleet and consignment information in a clear way.

In this experience, you will use Azure Cosmos DB to ingest streaming vehicle telemetry data as the entry point to a near real-time analytics pipeline built on Cosmos DB, Azure Functions, Event Hubs, Azure Databricks, Azure Storage, Azure Stream Analytics, Power BI, Azure Web Apps, and Logic Apps.

## Solution architecture

Below is a diagram of the solution architecture you will build in this lab. Please study this carefully, so you understand the whole of the solution as you are working on the various components.

![A diagram showing the components of the solution is displayed.](media/solution-architecture.png 'Solution Architecture')

- Data ingest, event processing, and storage:

  The solution for the IoT scenario centers around **Cosmos DB**, which acts as the globally-available, highly scalable data storage for streaming event data, fleet, consignment, package, and trip metadata, and aggregate data for reporting. Vehicle telemetry data flows in from the data generator, through registered IoT devices in **IoT Hub**, where an **Azure function** processes the event data and inserts it into a telemetry container in Cosmos DB.

- Trip processing with Azure Functions:

  The Cosmos DB change feed triggers three separate Azure functions, with each managing their own checkpoints so they can process the same incoming data without conflicting with one another. One function serializes the event data and stores it into time-sliced folders in **Azure Storage** for long-term cold storage of raw data. Another function processes the vehicle telemetry, aggregating the batch data and updating the trip and consignment status in the metadata container, based on odometer readings and whether the trip is running on schedule. This function also triggers a **Logic App** to send email alerts when trip milestones are reached. A third function sends the event data to **Event Hubs**, which in turn triggers **Stream Analytics** to execute time window aggregate queries.

- Stream processing, dashboards, and reports:

  The Stream Analytics queries output vehicle-specific aggregates to the Cosmos DB metadata container, and overall vehicle aggregates to **Power BI** to populate its real-time dashboard of vehicle status information. A Power BI Desktop report is used to display detailed vehicle, trip, and consignment information, pulled directly from the Cosmos DB metadata container. It also displays batch battery failure predictions, pulled from the maintenance container.

- Advanced analytics and ML model training:

  **Azure Databricks** is used to train a machine learning model to predict vehicle battery failure, based on historic information. It saves a trained model locally for batch predictions, and deploys a model and scoring web service to **Azure Kubernetes Service** or **Azure Container Instances** for real-time predictions. Azure Databricks also uses the **Spark Cosmos DB connector** to pull down each day's trip information to make batch predictions on battery failure and store the predictions in the maintenance container.

- Fleet management web app, security, and monitoring:

  A **Web App** allows Contoso Auto to manage vehicles and view consignment, package, and trip information that is stored in Cosmos DB. The Web App is also used to make real-time battery failure predictions while viewing vehicle information. **Azure Key Vault** is used to securely store centralized application secrets, such as connection strings and access keys, and is used by the Function Apps, Web App, and Azure Databricks. Finally, **Application Insights** provides real-time monitoring, metrics, and logging information for the Function Apps and Web App.

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.
   - Trial subscriptions will not work.
   - **IMPORTANT**: To complete the OAuth 2.0 access components of this hands-on lab you must have permissions within your Azure subscription to create an App Registration and service principal within Azure Active Directory.
2. Install [Power BI Desktop](https://powerbi.microsoft.com/desktop/)
3. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater

## Before the hands-on lab

Refer to the [Before the hands-on lab setup guide](./Before%20the%20HOL%20-%20Cosmos%20DB%20scenario-based%20labs%20-%20IoT.md) manual before continuing to the lab exercises.

## Exercise 1: Configure environment

**Duration**: 30 minutes

You must provision a few resources in Azure before you start developing the solution. Ensure all resources use the same resource group for easier cleanup.

In this exercise, you will configure your lab environment so you can start sending and processing generated vehicle, consignment, package, and trip data. You will begin by creating a Cosmos DB database and containers, then you will create a new Logic App and create a workflow for sending email notifications, then retrieve secrets used in the solution's application settings (such as connection strings) and securely store them in Azure Key Vault, and finally configure your Azure Databricks environment.

### Task 1: Create Cosmos DB database and container

In this task, you will create a Cosmos DB database and three SQL-based containers:

- **telemetry**: Used for ingesting hot vehicle telemetry data with a 90-day lifespan (TTL).
- **metadata**: Stores vehicle, consignment, package, trip, and aggregate event data.
- **maintenance**: The batch battery failure predictions are stored here for reporting purposes.

1. Using a new tab or instance of your browser, navigate to the Azure portal, <https://portal.azure.com>.

2. Select **Resource groups** from the left-hand menu, then search for your resource group by typing in `cosmos-db-iot`. Select your resource group that you are using for this lab.

   ![Resource groups is selected and the cosmos-db-iot resource group is displayed in the search results.](media/resource-group.png 'cosmos-db-iot resource group')

3. Select your Azure Cosmos DB account. The name starts with `cosmos-db-iot`.

   ![The Cosmos DB account is highlighted in the resource group.](media/resource-group-cosmos-db.png 'Cosmos DB in the Resource Group')

4. Select **Data Explorer** in the left-hand menu, then select **New Container**.

   ![The Cosmos DB Data Explorer is shown with the New Container button highlighted.](media/cosmos-new-container.png 'Data Explorer - New Container')

5. On the **Add Container** blade, specify the following configuration options:

   a. Enter **ContosoAuto** for the **Database id**.

   b. Leave **Provision database throughput** unchecked.

   c. Enter **metadata** for the **Container id**.

   d. Partition key: **/partitionKey**

   e. Throughput: **15000**

   ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-metadata.png 'New metadata container')

6. Select **OK** to create the container.

7. Select **New Container** once again in the Data Explorer.

8. On the **Add Container** blade, specify the following configuration options:

   a. **Database id**: Select **Use existing**, then select **ContosoAuto** from the list.

   c. Enter **telemetry** for the **Container id**.

   d. Partition key: **/partitionKey**

   e. Throughput: **15000**

   ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-telemetry.png 'New telemetry container')

9. Select **OK** to create the container.

10. Select **New Container** once again in the Data Explorer.

11. On the **Add Container** blade, specify the following configuration options:

    a. **Database id**: Select **Use existing**, then select **ContosoAuto** from the list.

    c. Enter **maintenance** for the **Container id**.

    d. Partition key: **/vin**

    e. Throughput: **400**

    ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-maintenance.png 'New maintenance container')

12. Select **OK** to create the container.

13. You should now have three containers listed in the Data Explorer.

    ![The three new containers are shown in Data Explorer.](media/cosmos-three-containers.png 'Data Explorer')

#### About Cosmos DB throughput

You will notice that we have intentionally set the **throughput** in RU/s for each container, based on our anticipated event processing and reporting workloads. In Azure Cosmos DB, provisioned throughput is represented as request units/second (RUs). RUs measure the cost of both read and write operations against your Cosmos DB container. Because Cosmos DB is designed with transparent horizontal scaling (e.g., scale out) and multi-master replication, you can very quickly and easily increase or decrease the number of RUs to handle thousands to hundreds of millions of requests per second around the globe with a single API call.

Cosmos DB allows you to increment/decrement the RUs in small increments of 100 at the database level, or at the container level. It is recommended that you configure throughput at the container granularity for guaranteed performance for the container all the time, backed by SLAs. Other guarantees that Cosmos DB delivers are 99.999% read and write availability all around the world, with those reads and writes being served in less than 10 milliseconds at the 99th percentile.

When you set a number of RUs for a container, Cosmos DB ensures that those RUs are available in all regions associated with your Cosmos DB account. When you scale out the number of regions by adding a new one, Cosmos will automatically provision the same quantity of RUs in the newly added region. You cannot selectively assign different RUs to a specific region. These RUs are provisioned for a container (or database) for all associated regions.

#### About Cosmos DB partitioning

When you created each container, you were required to define a **partition key**. As you will see later in the lab when you review the solution source code, each document stored within a collection contains a `partitionKey` property. One of the most important decisions one must make when creating a new container is to select an appropriate partition key for the data. A partition key should provide even distribution of storage and throughput (measured in requests per second) at any given time to avoid storage and performance bottlenecks. For instance, vehicle metadata stores the VIN, which is a unique value for each vehicle, in the `partitionKey` field. Trip metadata also uses the VIN for the `partitionKey` field, since trips are most often queried by VIN, and trip documents are stored in the same logical partition as vehicle metadata since they are likely to be queried together, preventing fan-out, or cross-partition queries. Package metadata, on the other hand, use the Consignment ID value for the `partitionKey` field for the same purposes. The partition key should be present in the bulk of queries for read-heavy scenarios to avoid excessive fan-out across numerous partitions. This is because each document with a specific partition key value belongs to the same logical partition, and is also stored in and served from the same physical partition. Each physical partition is replicated across geographical regions, resulting in global distribution.

Choosing an appropriate partition key for Cosmos DB is a critical step for ensuring balanced reads and writes, scaling, and, in the case of this solution, in-order change feed processing per partition. While there are no limits, per se, on the number of logical partitions, a single logical partition is allowed an upper limit of 10 GB of storage. Logical partitions cannot be split across physical partitions. For the same reason, if the partition key chosen is of bad cardinality, you could potentially have skewed storage distribution. For instance, if one logical partition becomes larger faster than the others and hits the maximum limit of 10 GB, while the others are nearly empty, the physical partition housing the maxed out logical partition cannot split and could cause an application downtime.

### Task 2: Configure Cosmos DB container indexing and TTL

In this task, you will review the default indexing set on your new containers, and configure the indexing for your `telemetry` container so it is optimized for write-heavy workloads. Next, you will enable time-to-live (TTL) on the `telemetry` container, allowing you to set the TTL value, in seconds, on individual documents stored in the container. This value tells Cosmos DB when to expire, or delete, the document(s) automatically. This setting can help save in storage costs by removing what you no longer need. Typically, this is used on hot data, or data that must be expired after a period of time due to regulatory requirements.

1. Expand the **telemetry** container in the Cosmos DB Data Explorer, then select **Scale & Settings**.

2. Within the Scale & Settings blade, expand the Settings section and select **On (no default)** under **Time to Live**.

   ![The Time to Live settings are set to On with no default.](media/cosmos-ttl-on.png 'Scale & Settings')

   Turning the Time to Live setting on with no default allows us to define the TTL individually for each document, giving us more flexibilty in deciding which documents should expire after a set period of time. To do this, we have a `ttl` field on the document that is saved to this container that specifies the TTL in seconds.

3. Scroll down in the Scale & Settings blade to view the **Indexing Policy**. The default policy is to automatically index all fields for each document stored in the collection. This is because all paths are included (remember, since we are storing JSON documents, we use paths to identify the property since they can exist within child collections in the document) by setting the value of `includedPaths` to `"path": "/*"`, and the only excluded path is the internal `_etag` property, which is used for versioning the documents. Here is what the default Indexing Policy looks like:

   ```json
   {
     "indexingMode": "consistent",
     "automatic": true,
     "includedPaths": [
       {
         "path": "/*"
       }
     ],
     "excludedPaths": [
       {
         "path": "/\"_etag\"/?"
       }
     ]
   }
   ```

4. Replace the **Indexing Policy** with the following, which excludes all paths, and only includes the paths used when we query the container (`vin`, `state`, and `partitionKey`):

   ```json
   {
     "indexingMode": "consistent",
     "automatic": true,
     "includedPaths": [
       {
         "path": "/vin/?"
       },
       {
         "path": "/state/?"
       },
       {
         "path": "/partitionKey/?"
       }
     ],
     "excludedPaths": [
       {
         "path": "/*"
       },
       {
         "path": "/\"_etag\"/?"
       }
     ]
   }
   ```

   ![The Indexing Policy section is highlighted, as well as the Save button.](media/cosmos-indexing-policy.png 'Scale & Settings')

5. Select **Save** to apply your changes.

#### About the Cosmos DB indexing policies

In this task, we updated the indexing policy for the `telemetry` container, but left the other two containers with the default policy. The default indexing policy for newly created containers indexes every property of every item, enforcing range indexes for any string or number, and spatial indexes for any GeoJSON object of type Point. This allows you to get high query performance without having to think about indexing and index management upfront. Since the `metadata` and `maintenance` containers have more read-heavy workloads than `telemetry`, it makes sense to use the default indexing policy where query performance is optimized. The indexing mode for all three containers is set to **Consistent**. This means the index is updated synchronously as items are added, updated, or deleted, enforcing the consistency level configured for the account for read queries. The other indexing mode one could choose is None, which disables indexing on the container. Usually this mode is used when your container acts as a pure key-value store, and you do not need indexes for any of the other properties. It is possible to dynamically change the consistency mode prior to executing bulk operations, then changing the mode back to Consistent afterwards, if the potential performance increase warrants the temporary change.

## Task 3: Create a Logic App workflow for email alerts

In this task, you will create a new Logic App workflow and configure it to send email alerts through its HTTP trigger. This trigger will be called by one of your Azure functions that gets triggered by the Cosmos DB change feed, any time a notification event occurs, such as completing a trip. You will need to have an Office 365 or Outlook.com account to send the emails.

1. In the [Azure portal](https://portal.azure.com), select **+ Create a resource**, then enter **logic app** into the search box on top. Select **Logic App** from the results.

   ![The Create a resource button and search box are highlighted in the Azure portal.](media/portal-new-logic-app.png 'Azure portal')

2. Select the **Create** button on the **Logic App overview** blade.

3. On the **Create Logic App** blade, specify the following configuration options:

   1. **Name**: Unique value for the name, such as `Cosmos-IoT-Logic` (ensure the green check mark appears).
   2. **Subscription**: Select the Azure subscription you are using for this lab.
   3. **Resource group**: Select your lab resource group. The name should start with `cosmos-db-iot`.
   4. **Location**: Select the same location as your resource group.
   5. **Log Analytics**: Select **Off**.

   ![The form is displayed with the previously described values.](media/portal-new-logic-app-form.png 'New Logic App')

4. Select **Create**.

5. After the Logic App is created, navigate to it by opening your resource group and selecting the new Logic App.

6. In the Logic App Designer, scroll through the page until you locate the Start with a common trigger section. Select the **When a HTTP request is received** trigger.

   ![The HTTP common trigger option is highlighted.](media/logic-app-http-trigger.png 'Logic App Designer')

7. Paste the following JSON into the **Request Body JSON Schema** field. This defines the shape of the data the Azure function will send in the body of the HTTP request when an alert needs to be sent:

   ```json
   {
     "properties": {
       "consignmentId": {
         "type": "string"
       },
       "customer": {
         "type": "string"
       },
       "deliveryDueDate": {
         "type": "string"
       },
       "distanceDriven": {
         "type": "number"
       },
       "hasHighValuePackages": {
         "type": "boolean"
       },
       "id": {
         "type": "string"
       },
       "lastRefrigerationUnitTemperatureReading": {
         "type": "integer"
       },
       "location": {
         "type": "string"
       },
       "lowestPackageStorageTemperature": {
         "type": "integer"
       },
       "odometerBegin": {
         "type": "integer"
       },
       "odometerEnd": {
         "type": "number"
       },
       "plannedTripDistance": {
         "type": "number"
       },
       "recipientEmail": {
         "type": "string"
       },
       "status": {
         "type": "string"
       },
       "temperatureSetting": {
         "type": "integer"
       },
       "tripEnded": {
         "type": "string"
       },
       "tripStarted": {
         "type": "string"
       },
       "vin": {
         "type": "string"
       }
     },
     "type": "object"
   }
   ```

   ![The Request Body JSON Schema is displayed.](media/logic-app-schema.png 'Request Body JSON Schema')

8. Select **+ New step** underneath the HTTP trigger.

   ![The new step button is highlighted.](media/logic-app-new-step.png 'New step')

9. Within the new action box, type `send email` in the search box, then select **Send an email - Office 365 Outlook** from the list of actions below. **Note**: If you do not have an Office 365 Outlook account, you may try one of the other email service options.

   ![Send email is typed in the search box and Send an email - Office 365 Outlook is highlighted below.](media/logic-app-send-email.png 'Choose an action')

10. Select the **Sign in** button. Sign in to your account in the window that appears.

    ![The Sign in button is highlighted.](media/logic-app-sign-in-button.png 'Office 365 Outlook')

11. After signing in, the action box will display as the **Send an email** action form. Select the **To** field. The **Dynamic content** box will display after selecting To. To see the full list of dynamic values from the HTTP request trigger, select **See more** next to "When a HTTP request is received".

    ![The To field is selected, and the See more link is highlighted in the Dynamic content window.](media/logic-app-dynamic-content-see-more.png 'Dynamic content')

12. In the list of dynamic content, select **recipientEmail**. This will add the dynamic value to the **To** field.

    ![The recipientEmail dynamic value is added to the To field.](media/logic-app-recipientemail.png 'Dynamic content - recipientEmail')

13. In the **Subject** field, enter the following: `Contoso Auto trip status update:`, making sure you add a space at the end. Select the **status** dynamic content to append the trip status to the end of the subject.

    ![The Subject field is filled in with the status dynamic content appended to the end.](media/logic-app-status.png 'Dynamic content - status')

14. Paste the following into the **Body** field. The dynamic content will automatically be added:

    ```text
    Here are the details of the trip and consignment:

    CONSIGNMENT INFORMATION:

    Customer: @{triggerBody()?['customer']}
    Delivery Due Date: @{triggerBody()?['deliveryDueDate']}
    Location: @{triggerBody()?['location']}
    Status: @{triggerBody()?['status']}

    TRIP INFORMATION:

    Trip Start Time: @{triggerBody()?['tripStarted']}
    Trip End Time: @{triggerBody()?['tripEnded']}
    Vehicle (VIN): @{triggerBody()?['vin']}
    Planned Trip Distance: @{triggerBody()?['plannedTripDistance']}
    Distance Driven: @{triggerBody()?['distanceDriven']}
    Start Odometer Reading: @{triggerBody()?['odometerBegin']}
    End Odometer Reading: @{triggerBody()?['odometerEnd']}

    PACKAGE INFORMATION:

    Has High Value Packages: @{triggerBody()?['hasHighValuePackages']}
    Lowest Package Storage Temp (F): @{triggerBody()?['lowestPackageStorageTemperature']}
    Trip Temp Setting (F): @{triggerBody()?['temperatureSetting']}
    Last Refrigeration Unit Temp Reading (F): @{triggerBody()?['lastRefrigerationUnitTemperatureReading']}

    REFERENCE INFORMATION:

    Trip ID: @{triggerBody()?['id']}
    Consignment ID: @{triggerBody()?['consignmentId']}
    Vehicle VIN: @{triggerBody()?['vin']}

    Regards,
    Contoso Auto Bot
    ```

15. Your Logic App workflow should now look like the following:

    ![The Logic App workflow is complete.](media/logic-app-completed-workflow.png 'Logic App')

16. Select **Save** at the top of the designer to save your workflow.

17. After saving, the URL for the HTTP trigger will generate. Expand the HTTP trigger in the workflow, then copy the **HTTP POST URL** value and save it to Notepad or similar text application for a later step.

    ![The http post URL is highlighted.](media/logic-app-url.png 'Logic App')

### Task 4: Add Key Vault secrets

Azure Key Vault is used to Securely store and tightly control access to tokens, passwords, certificates, API keys, and other secrets. In addition, secrets that are stored in Azure Key Vault are centralized, giving the added benefits of only needing to update secrets in one place, such as an application key value after recycling the key for security purposes. In this task, we will store application secrets in Azure Key Vault, then configure the Function Apps and Web App to securely connect to Azure Key Vault by performing the following steps:

- Add secrets to the provisioned Key Vault.
- Create a system-assigned managed identity for each Azure Function App and the Web App to read from the vault.
- Create an access policy in Key Vault with the "Get" secret permission, assigned to each of these application identities.

> We recommend that you open two browser tabs for these steps. One to copy secrets from each Azure service, and the other to add the secrets to Key Vault.

1. Using a new tab or instance of your browser, navigate to the Azure portal, <https://portal.azure.com>.

2. Select **Resource groups** from the left-hand menu, then search for your resource group by typing in `cosmos-db-iot`. Select your resource group that you are using for this lab.

3. Open the your **Key Vault**. The name should begin with `iot-keyvault`.

   ![The Key Vault is highlighted in the resource group.](media/resource-group-keyvault.png 'Resource group')

4. Select **Secrets** in the left-hand menu, then select **+ Generate/Import** to create a new secret.

   ![The Secrets menu item is highlighted, and the Generate/Import button is selected.](media/key-vault-secrets-generate.png 'Key Vault Secrets')

5. Use the table below for the Name / Value pairs to use when creating the secrets. You only need to populate the **Name** and **Value** fields for each secret, and can leave the other fields at their default values.

   | **Name**            |                                                                          **Value**                                                                          |
   | ------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
   | CosmosDBConnection  |                            Your Cosmos DB connection string found here: **Cosmos DB account > Keys > Primary Connection String**                            |
   | IoTHubConnection    |                         Your IoT Hub connection string found here: **IoT Hub > Built-in endpoints > Event Hub-compatible endpoint**                         |
   | ColdStorageAccount  |  Connection string to the Azure Storage account whose name starts with `iotstore`, found here: **Storage account > Access keys > key1 Connection string**   |
   | EventHubsConnection | Your Event Hubs connection string found here: **Event Hubs namespace > Shared access policies > RootManageSharedAccessKey > Connection string-primary key** |
   | LogicAppUrl         |                         Your Logic App's HTTP Post URL found here: **Logic App Designer > Select the HTTP trigger > HTTP POST URL**                         |

   When you are finished creating the secrets, your list should look similar to the following:

   ![The list of secrets is displayed.](media/key-vault-keys.png 'Key Vault Secrets')

### Task 4: Create Azure Databricks cluster

### Task 5: Configure Key Vault-backed Databricks secret store

## Exercise 2: Deploy Azure functions and Web App

### Task 1: Open solution

### Task 2: Code walk-through

### Task 3: Deploy Event Hub consumer Function App

### Task 4: Deploy Change Feed consumer Function App

### Task 5: Deploy Web App

### Task 6: Configure application settings in Azure

## Exercise 3: Configure Logic App for e-mail alerts

### Task 1: Create new workflow

### Task 2: Configure email service

## Exercise 4: Explore and execute data generator

### Task 1: Open solution

### Task 2: Update application configuration

### Task 3: Code walk-through

### Task 4: Run generator

## Exercise 5: Observe data using Cosmos DB Data Explorer and Web App

### Task 1: View data in Cosmos DB Data Explorer

### Task 2: Search and view data in Web App

## Exercise 6: Performing CRUD operations using the Web App

### Task 1: Update vehicle metadata

### Task 2: View consignment, package, and trip data

## Exercise 7: Observe Change Feed using Azure Functions and App Insights

### Task 1: Open App Insights Live View

## Exercise 8: Running the predictive maintenance batch scoring

### Task 1: Import lab notebooks into Azure Databricks

### Task 2: Run batch scoring notebook

### Task 3: Create scheduled notebook job

## Exercise 9: Deploying the predictive maintenance web service

### Task 1: Run deployment notebook

## Exercise 10: Configure windowed queries in Stream Analytics

### Task 1: Add Stream Analytics Event Hubs input

### Task 2: Add Stream Analytics outputs

### Task 3: Create Stream Analytics query

### Task 4: Run query

## Exercise 11: Creating the Fleet status real-time dashboard in Power BI

### Task 1: Log in to Power BI online

### Task 2: Create real-time dashboard

## Exercise 12: Creating the Predictive Maintenance & Trip/Consignment Status reports in Power BI

### Task 1: Add Cosmos DB data sources to Power BI Desktop

### Task 2: Create new report in Power BI Desktop

## After the hands-on lab

Duration: 10 mins

In this exercise, you will delete any Azure resources that were created in support of the lab. You should follow all steps provided after attending the Hands-on lab to ensure your account does not continue to be charged for lab resources.

### Task 1: Delete the resource group

1. Using the [Azure portal](https://portal.azure.com), navigate to the Resource group you used throughout this hands-on lab by selecting Resource groups in the left menu.

2. Search for the name of your resource group, and select it from the list.

3. Select Delete in the command bar, and confirm the deletion by re-typing the Resource group name, and selecting Delete.

You should follow all steps provided _after_ attending the Hands-on lab.
