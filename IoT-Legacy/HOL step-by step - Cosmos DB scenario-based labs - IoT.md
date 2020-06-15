# Cosmos DB scenario-based labs - IoT hands-on lab step-by-step

<details>
<summary><strong><em>Table of Contents</em></strong></summary>
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
    - [Task 4: Create system-assigned managed identities for your Function Apps and Web App to connect to Key Vault](#task-4-create-system-assigned-managed-identities-for-your-function-apps-and-web-app-to-connect-to-key-vault)
    - [Task 5: Add Function Apps and Web App to Key Vault access policy](#task-5-add-function-apps-and-web-app-to-key-vault-access-policy)
    - [Task 6: Add your user account to Key Vault access policy](#task-6-add-your-user-account-to-key-vault-access-policy)
    - [Task 7: Add Key Vault secrets](#task-7-add-key-vault-secrets)
    - [Task 8: Create Azure Databricks cluster](#task-8-create-azure-databricks-cluster)
    - [Task 9: Configure Key Vault-backed Databricks secret store](#task-9-configure-key-vault-backed-databricks-secret-store)
  - [Exercise 2: Configure windowed queries in Stream Analytics](#exercise-2-configure-windowed-queries-in-stream-analytics)
    - [Task 1: Add Stream Analytics Event Hubs input](#task-1-add-stream-analytics-event-hubs-input)
    - [Task 2: Add Stream Analytics outputs](#task-2-add-stream-analytics-outputs)
    - [Task 3: Create Stream Analytics query](#task-3-create-stream-analytics-query)
    - [Task 4: Run Stream Analytics job](#task-4-run-stream-analytics-job)
  - [Exercise 3: Deploy Azure functions and Web App](#exercise-3-deploy-azure-functions-and-web-app)
    - [Task 1: Retrieve the URI for each Key Vault secret](#task-1-retrieve-the-uri-for-each-key-vault-secret)
    - [Task 2: Configure application settings in Azure](#task-2-configure-application-settings-in-azure)
    - [Task 3: Open solution](#task-3-open-solution)
    - [Task 4: Code completion and walk-through](#task-4-code-completion-and-walk-through)
    - [Task 5: Deploy Cosmos DB Processing Function App](#task-5-deploy-cosmos-db-processing-function-app)
    - [Task 6: Deploy Stream Processing Function App](#task-6-deploy-stream-processing-function-app)
    - [Task 7: Deploy Web App](#task-7-deploy-web-app)
    - [Task 8: View Cosmos DB processing Function App in the portal and copy the Health Check URL](#task-8-view-cosmos-db-processing-function-app-in-the-portal-and-copy-the-health-check-url)
    - [Task 9: View stream processing Function App in the portal and copy the Health Check URL](#task-9-view-stream-processing-function-app-in-the-portal-and-copy-the-health-check-url)
  - [Exercise 4: Explore and execute data generator](#exercise-4-explore-and-execute-data-generator)
    - [Task 1: Open the data generator project](#task-1-open-the-data-generator-project)
    - [Task 2: Code walk-through](#task-2-code-walk-through)
    - [Task 3: Update application configuration](#task-3-update-application-configuration)
    - [Task 4: Run generator](#task-4-run-generator)
    - [Task 5: View devices in IoT Hub](#task-5-view-devices-in-iot-hub)
  - [Exercise 5: Observe Change Feed using Azure Functions and App Insights](#exercise-5-observe-change-feed-using-azure-functions-and-app-insights)
    - [Task 1: Open App Insights Live Metrics Stream](#task-1-open-app-insights-live-metrics-stream)
  - [Exercise 6: Observe data using Cosmos DB Data Explorer and Web App](#exercise-6-observe-data-using-cosmos-db-data-explorer-and-web-app)
    - [Task 1: View data in Cosmos DB Data Explorer](#task-1-view-data-in-cosmos-db-data-explorer)
    - [Task 2: Search and view data in Web App](#task-2-search-and-view-data-in-web-app)
  - [Exercise 7: Perform CRUD operations using the Web App](#exercise-7-perform-crud-operations-using-the-web-app)
    - [Task 1: Create a new vehicle](#task-1-create-a-new-vehicle)
    - [Task 2: View and edit the vehicle](#task-2-view-and-edit-the-vehicle)
    - [Task 3: Delete the vehicle](#task-3-delete-the-vehicle)
  - [Exercise 8: Create the Fleet status real-time dashboard in Power BI](#exercise-8-create-the-fleet-status-real-time-dashboard-in-power-bi)
    - [Task 1: Log in to Power BI online and create real-time dashboard](#task-1-log-in-to-power-bi-online-and-create-real-time-dashboard)
  - [Exercise 9: Create the Trip/Consignment Status reports in Power BI](#exercise-9-create-the-tripconsignment-status-reports-in-power-bi)
    - [Task 1: Import report in Power BI Desktop](#task-1-import-report-in-power-bi-desktop)
    - [Task 2: Update report data sources](#task-2-update-report-data-sources)
    - [Task 3: Explore report](#task-3-explore-report)
  - [Exercise 10: Run the predictive maintenance batch scoring](#exercise-10-run-the-predictive-maintenance-batch-scoring)
    - [Task 1: Import lab notebooks into Azure Databricks](#task-1-import-lab-notebooks-into-azure-databricks)
    - [Task 2: Run batch scoring notebook](#task-2-run-batch-scoring-notebook)
  - [Exercise 11: Deploy the predictive maintenance web service](#exercise-11-deploy-the-predictive-maintenance-web-service)
    - [Task 1: Run deployment notebook](#task-1-run-deployment-notebook)
    - [Task 2: Call the deployed scoring web service from the Web App](#task-2-call-the-deployed-scoring-web-service-from-the-web-app)
  - [Exercise 12: Refresh the Power BI report and view maintenance data](#exercise-12-refresh-the-power-bi-report-and-view-maintenance-data)
    - [Task 1: Open and refresh the report in Power BI Desktop](#task-1-open-and-refresh-the-report-in-power-bi-desktop)
  - [Environment clean-up](#environment-clean-up)
    - [Task 1: Delete the resource group](#task-1-delete-the-resource-group)

<!-- /TOC -->
</details>

## Overview

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

  **Azure Databricks** is used to train a machine learning model to predict vehicle battery failure, based on historic information. It saves a trained model locally for batch predictions, and deploys a model and scoring web service to **Azure Kubernetes Service (AKS)** or **Azure Container Instances (ACI)** for real-time predictions. Azure Databricks also uses the **Spark Cosmos DB connector** to pull down each day's trip information to make batch predictions on battery failure and store the predictions in the maintenance container.

- Fleet management web app, security, and monitoring:

  A **Web App** allows Contoso Auto to manage vehicles and view consignment, package, and trip information that is stored in Cosmos DB. The Web App is also used to make real-time battery failure predictions while viewing vehicle information. **Azure Key Vault** is used to securely store centralized application secrets, such as connection strings and access keys, and is used by the Function Apps, Web App, and Azure Databricks. Finally, **Application Insights** provides real-time monitoring, metrics, and logging information for the Function Apps and Web App.

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.
   - Trial subscriptions will not work.
   - **IMPORTANT**: To complete the OAuth 2.0 access components of this hands-on lab you must have permissions within your Azure subscription to create an App Registration and service principal within Azure Active Directory.
2. Install [Power BI Desktop](https://powerbi.microsoft.com/desktop/)
3. [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) - version 2.0.68 or later
4. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater
5. Install [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) or greater
   - If you are running Visual Studio 2017, install SDK 2.2.109

## Before the hands-on lab

Refer to the [Before the hands-on lab setup guide](./Before%20the%20HOL%20-%20Cosmos%20DB%20scenario-based%20labs%20-%20IoT.md) manual before continuing to the lab exercises.

## Exercise 1: Configure environment

**Duration**: 45 minutes

You must provision a few resources in Azure before you start developing the solution. Ensure all resources use the same resource group for easier cleanup.

In this exercise, you will configure your lab environment so you can start sending and processing generated vehicle, consignment, package, and trip data. You will begin by creating a Cosmos DB database and containers, then you will create a new Logic App and create a workflow for sending email notifications, create an Application Insights service for real-time monitoring of your solution, then retrieve secrets used in the solution's application settings (such as connection strings) and securely store them in Azure Key Vault, and finally configure your Azure Databricks environment.

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

   e. Throughput: **50000**

   ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-metadata.png 'New metadata container')

   > **Note**: We are initially setting the throughput on this container to `50000` RU/s because the data generator will perform a bulk insert of metadata the first time it runs. After inserting the data, it will programmatically reduce the throughput to `15000`.

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

   Turning the Time to Live setting on with no default allows us to define the TTL individually for each document, giving us more flexibility in deciding which documents should expire after a set period of time. To do this, we have a `ttl` field on the document that is saved to this container that specifies the TTL in seconds.

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

In this task, we updated the indexing policy for the `telemetry` container, but left the other two containers with the default policy. The default indexing policy for newly created containers indexes every property of every item, enforcing range indexes for any string or number, and spatial indexes for any GeoJSON object of type Point. This allows you to get high query performance without having to think about indexing and index management upfront. Since the `metadata` and `maintenance` containers have more read-heavy workloads than `telemetry`, it makes sense to use the default indexing policy where query performance is optimized. Since we need faster writes for `telemetry`, we exclude unused paths. The use of indexing paths can offer improved write performance and lower index storage for scenarios in which the query patterns are known beforehand, as indexing costs are directly correlated to the number of unique paths indexed.

The indexing mode for all three containers is set to **Consistent**. This means the index is updated synchronously as items are added, updated, or deleted, enforcing the consistency level configured for the account for read queries. The other indexing mode one could choose is None, which disables indexing on the container. Usually this mode is used when your container acts as a pure key-value store, and you do not need indexes for any of the other properties. It is possible to dynamically change the consistency mode prior to executing bulk operations, then changing the mode back to Consistent afterwards, if the potential performance increase warrants the temporary change.

### Task 3: Create a Logic App workflow for email alerts

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

### Task 4: Create system-assigned managed identities for your Function Apps and Web App to connect to Key Vault

In order for your Function Apps and Web App to be able to access Key Vault to read the secrets, you must [create a system-assigned managed identity](https://docs.microsoft.com/azure/app-service/overview-managed-identity#adding-a-system-assigned-identity) for each, and [create an access policy in Key Vault](https://docs.microsoft.com/azure/key-vault/key-vault-secure-your-key-vault#key-vault-access-policies) for the application identities.

1. Open the Azure Function App whose name begins with **IoT-CosmosDBProcessing** and navigate to **Platform features**.

2. Select **Identity**.

   ![Identity is highlighted in the platform features tab.](media/function-app-platform-features-identity.png 'Platform features')

3. Within the **System assigned** tab, switch **Status** to **On**. Select **Save**.

   ![The Function App Identity value is set to On.](media/function-app-identity.png 'Identity')

4. Open the Azure Function App whose name begins with **IoT-StreamProcessing** and navigate to **Platform features**.

5. Select **Identity**.

   ![Identity is highlighted in the platform features tab.](media/function-app-platform-features-identity.png 'Platform features')

6. Within the **System assigned** tab, switch **Status** to **On**. Select **Save**.

   ![The Function App Identity value is set to On.](media/function-app-identity.png 'Identity')

7. Open the Web App (App Service) whose name begins with **IoTWebApp**.

8. Select **Identity** in the left-hand menu.

9. Within the **System assigned** tab, switch **Status** to **On**. Select **Save**.

   ![The Web App Identity value is set to On.](media/web-app-identity.png 'Identity')

### Task 5: Add Function Apps and Web App to Key Vault access policy

> We recommend that you open two browser tabs for this and the following Key Vault tasks. One tab will be used to copy secrets from each Azure service, and the other to add the secrets to Key Vault.

Perform these steps to create an access policy that enables the "Get" secret permission:

1. Using a new tab or instance of your browser, navigate to the Azure portal, <https://portal.azure.com>.

2. Select **Resource groups** from the left-hand menu, then search for your resource group by typing in `cosmos-db-iot`. Select your resource group that you are using for this lab.

3. Open the your **Key Vault**. The name should begin with `iot-vault`.

   ![The Key Vault is highlighted in the resource group.](media/resource-group-keyvault.png 'Resource group')

4. Select **Access policies** in the left-hand menu.

5. Select **+ Add Access Policy**.

   ![The Add Access Policy link is highlighted.](media/key-vault-add-access-policy.png 'Access policies')

6. Select the **Select principal** section on the Add access policy form.

   ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

7. In the Principal blade, search for your `IoT-CosmosDBProcessing` Function App's service principal, select it, then select the **Select** button.

   ![The Function App's principal is selected.](media/key-vault-principal-function1.png 'Principal')

   > **Note**: It may take a while before your managed identities appear after adding them in the previous step. If you cannot find this or the other identities, try refreshing the page or wait a minute or two.

8. Expand the **Secret permissions** and check **Get** under Secret Management Operations.

   ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

9. Select **Add** to add the new access policy.

10. When you are done, you should have an access policy for the Function App's managed identity. Select **+ Add Access Policy** to add another access policy.

    ![Key Vault access policies.](media/key-vault-access-policies-function1.png 'Access policies')

11. Select the **Select principal** section on the Add access policy form.

    ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

12. In the Principal blade, search for your `IoT-StreamProcessing` Function App's service principal, select it, then select the **Select** button.

    ![The Function App's principal is selected.](media/key-vault-principal-function2.png 'Principal')

13. Expand the **Secret permissions** and check **Get** under Secret Management Operations.

    ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

14. Select **Add** to add the new access policy.

15. When you are done, you should have an access policy for the Function App's managed identity. Select **+ Add Access Policy** to add another access policy.

    ![Key Vault access policies.](media/key-vault-access-policies-function2.png 'Access policies')

16. Select the **Select principal** section on the Add access policy form.

    ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

17. In the Principal blade, search for your `IoTWebApp` Web App's service principal, select it, then select the **Select** button.

    ![The Web App's principal is selected.](media/key-vault-principal-webapp.png 'Principal')

18. Expand the **Secret permissions** and check **Get** under Secret Management Operations.

    ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

19. Select **Add** to add the new access policy.

20. When you are done, you should have an access policy for the Web App's managed identity. Select **Save** to save your new access policies.

    ![Key Vault access policies.](media/key-vault-access-policies-webapp.png 'Access policies')

### Task 6: Add your user account to Key Vault access policy

Perform these steps to create an access policy for your user account so you can manage secrets. Since we created Key Vault with a template, your account was not automatically added to the access policies.

1. Within Key Vault, select **Access policies** in the left-hand menu.

2. Select **+ Add Access Policy**.

   ![The Add Access Policy link is highlighted.](media/key-vault-add-access-policy.png 'Access policies')

3. Select the **Select principal** section on the Add access policy form.

   ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

4. In the Principal blade, search for your Azure account you are using for this lab, select it, then select the **Select** button.

   ![The user principal is selected.](media/key-vault-principal-user.png 'Principal')

5. Expand the **Secret permissions** and check **Select all** under Secret Management Operations. All 8 should be selected.

   ![The Select all checkbox is checked under the Secret permissions dropdown.](media/key-vault-all-secret-policy.png 'Add access policy')

6. Select **Add** to add the new access policy.. When you are done, you should have an access policy for your user account. Select **Save** to save your new access policy.

    ![Key Vault access policies.](media/key-vault-access-policies-user.png 'Access policies')

### Task 7: Add Key Vault secrets

Azure Key Vault is used to Securely store and tightly control access to tokens, passwords, certificates, API keys, and other secrets. In addition, secrets that are stored in Azure Key Vault are centralized, giving the added benefits of only needing to update secrets in one place, such as an application key value after recycling the key for security purposes. In this task, we will store application secrets in Azure Key Vault, then configure the Function Apps and Web App to securely connect to Azure Key Vault by performing the following steps:

- Add secrets to the provisioned Key Vault.
- Create a system-assigned managed identity for each Azure Function App and the Web App to read from the vault.
- Create an access policy in Key Vault with the "Get" secret permission, assigned to each of these application identities.

1. Within Key Vault, select **Secrets** in the left-hand menu, then select **+ Generate/Import** to create a new secret.

   ![The Secrets menu item is highlighted, and the Generate/Import button is selected.](media/key-vault-secrets-generate.png 'Key Vault Secrets')

2. Use the table below for the Name / Value pairs to use when creating the secrets. You only need to populate the **Name** and **Value** fields for each secret, and can leave the other fields at their default values.

   | **Name**            |                                                                          **Value**                                                                          |
   | ------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
   | CosmosDBConnection  |                            Your Cosmos DB connection string found here: **Cosmos DB account > Keys > Primary Connection String**                            |
   | CosmosDBEndpoint    |                                           Your Cosmos DB endpoint found here: **Cosmos DB account > Keys > URI**                                            |
   | CosmosDBPrimaryKey  |                                      Your Cosmos DB primary key found here: **Cosmos DB account > Keys > Primary Key**                                      |
   | IoTHubConnection    |                         Your IoT Hub connection string found here: **IoT Hub > Built-in endpoints > Event Hub-compatible endpoint**                         |
   | ColdStorageAccount  |  Connection string to the Azure Storage account whose name starts with `iotstore`, found here: **Storage account > Access keys > key1 Connection string**   |
   | EventHubsConnection | Your Event Hubs connection string found here: **Event Hubs namespace > Shared access policies > RootManageSharedAccessKey > Connection string-primary key** |
   | LogicAppUrl         |                         Your Logic App's HTTP Post URL found here: **Logic App Designer > Select the HTTP trigger > HTTP POST URL**                         |

3. You can locate most of your secrets by viewing the outputs of your deployment. To do this, open your resource group then select **Deployments** in the left-hand menu. Select the **Microsoft.Template** deployment.

    ![The resource group deployments blade is shown.](media/resource-group-deployments.png "Deployments")

4. Select **Outputs** in the left-hand menu. You can find most of the values above and simply copy them.

    ![The outputs are displayed.](media/resource-group-deployment-outputs.png "Outputs")

5. When you are finished creating the secrets, your list should look similar to the following:

   ![The list of secrets is displayed.](media/key-vault-keys.png 'Key Vault Secrets')

### Task 8: Create Azure Databricks cluster

Contoso Auto wants to use the valuable data they are collecting from their vehicles to make predictions about the health of their fleet to reduce downtime due to maintenance-related issues. One of the predictions they would like to make is whether a vehicle's battery is likely to fail within the next 30 days, based on historical data. They would like to run a nightly batch process to identify vehicles that should be serviced, based on these predictions. They also want to have a way to make a prediction in real time when viewing a vehicle on their fleet management website.

To support this requirement, you will use Apache Spark on Azure Databricks, a fully managed Apache Spark platform optimized to run on Azure. Spark is a unified big data and advanced analytics platform that enables data scientists and data engineers to explore and prepare large amounts of structured and unstructured data, then use that data to train, use, and deploy machine learning models at scale. We will read and write to Cosmos DB, using the `azure-cosmosdb-spark` connector (<https://github.com/Azure/azure-cosmosdb-spark>).

In this task, you will create a new cluster on which data exploration and model deployment tasks will be executed in later exercises.

1. In the [Azure portal](https://portal.azure.com), open your lab resource group, then open your **Azure Databricks Service**. The name should start with `iot-databricks`.

   ![The Azure Databricks Service is highlighted in the resource group.](media/resource-group-databricks.png 'Resource Group')

2. Select **Launch Workspace**. Azure Databricks will automatically sign you in through its Azure Active Directory integration.

   ![Launch Workspace](media/databricks-launch-workspace.png 'Launch Workspace')

3. Once in the workspace, select **Clusters** in the left-hand menu, then select **+ Create Cluster**.

   ![Create Cluster is highlighted.](media/databricks-clusters.png 'Clusters')

4. In the **New Cluster** form, specify the following configuration options:

   1. **Cluster Name**: Enter **lab**.
   2. **Cluster Mode**: Select **Standard**.
   3. **Pool**: Select **None**.
   4. **Databricks Runtime Version**: Select **Runtime 6.1 (Scala 2.11, Spark 2.4.4)**.
   5. **Autopilot Options**: Uncheck **Enable autoscaling** and **Terminate after...**, with a value of **120** minutes.
   6. **Worker Type**: Select **Standard_DS3_v2**.
   7. **Driver Type**: Select **Same as worker**.
   8. **Workers**: Enter **1**.

   ![The New Cluster form is displayed with the previously described values.](media/databricks-new-cluster.png 'New Cluster')
   **Note** If you failed to create a cluster, try to change the worker type from Standard_DS3_v2 to other VM sizes.

5. Select **Create Cluster**.

6. Before continuing to the next step, verify that your new cluster is running. Wait for the state to change from **Pending** to **Running**

7. Select the **lab** cluster, then select **Libraries**.

8. Select **Install New**.

    ![Navigate to the libraries tab and select `Install New`.](media/databricks-new-library.png 'Adding a new library')

9. In the Install Library dialog, select **Maven** for the Library Source.

10. In the Coordinates field type:

    ```text
    com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1
    ```

11. Select **Install**

    ![Populated library dialog for Maven.](media/databricks-install-library-cosmos.png 'Add the Maven library')

12. **Wait** until the library's status shows as **Installed** before continuing.

### Task 9: Configure Key Vault-backed Databricks secret store

In an earlier task, you added application secrets to Key Vault, such as the Cosmos DB connection string. In this task, you will configure the Key Vault-backed Databricks secret store to securely access these secrets.

Azure Databricks has two types of secret scopes: Key Vault-backed and Databricks-backed. These secret scopes allow you to store secrets, such as database connection strings, securely. If someone tries to output a secret to a notebook, it is replaced by `[REDACTED]`. This helps prevent someone from viewing the secret or accidentally leaking it when displaying or sharing the notebook.

1. Return to the [Azure portal](https://portal.azure.com), which should still be open in another browser tab, then navigate to your Key Vault account and select **Properties** on the left-hand menu.

2. Copy the **DNS Name** and **Resource ID** property values and paste them to Notepad or some other text application that you can reference in a moment.

   ![Properties is selected on the left-hand menu, and DNS Name and Resource ID are highlighted to show where to copy the values from.](media/key-vault-properties.png 'Key Vault properties')

3. Navigate back to the Azure Databricks workspace.

4. In your browser's URL bar, append **#secrets/createScope** to your Azure Databricks base URL (for example, <https://eastus.azuredatabricks.net#secrets/createScope>).

5. Enter `key-vault-secrets` for the name of the secret scope.

6. Select **Creator** within the Manage Principal drop-down to specify only the creator (which is you) of the secret scope has the MANAGE permission.

   > MANAGE permission allows users to read and write to this secret scope, and, in the case of accounts on the Azure Databricks Premium Plan, to change permissions for the scope.

   > Your account must have the Azure Databricks Premium Plan for you to be able to select Creator. This is the recommended approach: grant MANAGE permission to the Creator when you create the secret scope, and then assign more granular access permissions after you have tested the scope.

7. Enter the **DNS Name** (for example, <https://iot-vault.vault.azure.net/>) and **Resource ID** you copied earlier during the Key Vault creation step, for example: `/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/cosmos-db-iot/providers/Microsoft.KeyVault/vaults/iot-vault`.

   ![Create Secret Scope form](media/create-secret-scope.png 'Create Secret Scope')

8. Select **Create**.

After a moment, you will see a dialog verifying that the secret scope has been created.

## Exercise 2: Configure windowed queries in Stream Analytics

**Duration**: 15 minutes

If you examine the right-hand side of the solution architecture diagram, you will see a flow of event data that feeds into Event Hubs from a Cosmos DB change feed-triggered function. Stream Analytics uses the event hub as an input source for a set of time window queries that create aggregates for individual vehicle telemetry, and overall vehicle telemetry that flows through the architecture from the vehicle IoT devices. Stream Analytics has two output data sinks:

1. Cosmos DB: Individual vehicle telemetry (grouped by VIN) is aggregated over a 30-second `TumblingWindow` and saved to the `metadata` container. This information is used in a Power BI report you will create in Power BI Desktop in a later task to display individual vehicle and multiple vehicle statistics.
2. Power BI: All vehicle telemetry is aggregated over a 10-second `TumblingWindow` and output to a Power BI data set. This near real-time data is displayed in a live Power BI dashboard to show in 10 second snapshots how many events were processed, whether there are engine temperature, oil, or refrigeration unit warnings, whether aggressive driving was detected during the period, and the average speed, engine temperature, and refrigeration unit readings.

![The stream processing components of the solution architecture are displayed.](media/solution-architecture-stream-processing.png 'Solution Architecture - Stream Processing')

In this exercise, you will configure Stream Analytics for stream processing as described above.

### Task 1: Add Stream Analytics Event Hubs input

1. In the [Azure portal](https://portal.azure.com), open your lab resource group, then open your **Stream Analytics job**.

   ![The Stream Analytics job is highlighted in the resource group.](media/resource-group-stream-analytics.png 'Resource Group')

2. Select **Inputs** in the left-hand menu. In the Inputs blade, select **+ Add stream input**, then select **Event Hub** from the list.

   ![The Event Hub input is selected in the Add Stream Input menu.](media/stream-analytics-inputs-add-event-hub.png 'Inputs')

3. In the **New input** form, specify the following configuration options:

   1. **Input alias**: Enter **events**.
   2. Select the **Select Event Hub from your subscriptions** option beneath.
   3. **Subscription**: Choose your Azure subscription for this lab.
   4. **Event Hub namespace**: Find and select your Event Hub namespace (eg. `iot-namespace`).
   5. **Event Hub name**: Select **Use existing**, then **reporting**.
   6. **Event Hub policy name**: Choose the default `RootManageSharedAccessKey` policy.

   ![The New Input form is displayed with the previously described values.](media/stream-analytics-new-input.png 'New input')

4. Select **Save**.

You should now see your Event Hubs input listed.

![The Event Hubs input is listed.](media/stream-analytics-inputs.png 'Inputs')

### Task 2: Add Stream Analytics outputs

1. Select **Outputs** in the left-hand menu. In the Outputs blade, select **+ Add**, then select **Cosmos DB** from the list.

   ![The Cosmos DB output is selected in the Add menu.](media/stream-analytics-outputs-add-cosmos-db.png 'Outputs')

2. In the **New output** form, specify the following configuration options:

   1. **Output alias**: Enter **cosmosdb**.
   2. Select the **Select Cosmos DB from your subscriptions** option beneath.
   3. **Subscription**: Choose your Azure subscription for this lab.
   4. **Account id**: Find and select your Cosmos DB account (eg. `cosmos-db-iot`).
   5. **Database**: Select **Use existing**, then **ContosoAuto**.
   6. **Container name**: Enter **metadata**.

   ![The New Output form is displayed with the previously described values.](media/stream-analytics-new-output-cosmos.png 'New output')

3. Select **Save**.

4. **If you have never signed in to Power BI with this account**, open a new browser tab and navigate to <https://powerbi.com> and sign in. Confirm any messages if they appear and continue to the next step after the home page appears. This will help the connection authorization step from Stream Analytics succeed and find the group workspace.

5. While remaining in the Outputs blade, select **+ Add** once again, then select **Power BI** from the list.

   ![The Power BI output is selected in the Add menu.](media/stream-analytics-outputs-add-power-bi.png 'Outputs')

6. In the **New output** form, look toward the bottom to find the **Authorize connection** section, then select **Authorize** to sign in to your Power BI account. If you do not have a Power BI account, select the _Sign up_ option first.

   ![The Authorize connection section is displayed.](media/stream-analytics-authorize-power-bi.png 'Authorize connection')

7. After authorizing the connection to Power BI, specify the following configuration options in the form:

   1. **Output alias**: Enter **powerbi**.
   2. **Group workspace**: Select **My workspace**.
   3. **Dataset name**: Enter **Contoso Auto IoT Events**.
   4. **Table name**: Enter **FleetEvents**.

   ![The New Output form is displayed with the previously described values.](media/stream-analytics-new-output-power-bi.png 'New output')

8. Select **Save**.

You should now have two outputs listed.

![The two added outputs are listed.](media/stream-analytics-outputs.png 'Outputs')

### Task 3: Create Stream Analytics query

The Query is Stream Analytics' work horse. This is where we process streaming inputs and write data to our outputs. The Stream Analytics query language is SQL-like, allowing you to use familiar syntax to explore and transform the streaming data, create aggregates, and create materialized views that can be used to help shape your data structure before writing to the output sinks. Stream Analytics jobs can only have one Query, but you can write to multiple outputs in a single Query, as you will do in the steps that follow.

Please take a moment to analyze the query below. Notice how we are using the `events` input name for the Event Hubs input you created, and the `powerbi` and `cosmosDB` outputs, respectively. Also see where we use the `TumblingWindow` in durations of 30 seconds for `VehicleData`, and 10 seconds for `VehicleDataAll`. The `TumblingWindow` helps us evaluate events that occurred during the past X seconds and, in our case, create averages over those time periods for reporting.

1. Select **Query** in the left-hand menu. Replace the contents of the query window with the script below:

    ```sql
    WITH
    VehicleData AS (
        select
            vin,
            AVG(engineTemperature) AS engineTemperature,
            AVG(speed) AS speed,
            AVG(refrigerationUnitKw) AS refrigerationUnitKw,
            AVG(refrigerationUnitTemp) AS refrigerationUnitTemp,
            (case when AVG(engineTemperature) >= 400 OR AVG(engineTemperature) <= 15 then 1 else 0 end) as engineTempAnomaly,
            (case when AVG(engineoil) <= 18 then 1 else 0 end) as oilAnomaly,
            (case when AVG(transmission_gear_position) <= 3.5 AND
                AVG(accelerator_pedal_position) >= 50 AND
                AVG(speed) >= 55 then 1 else 0 end) as aggressiveDriving,
            (case when AVG(refrigerationUnitTemp) >= 30 then 1 else 0 end) as refrigerationTempAnomaly,
            System.TimeStamp() as snapshot
        from events TIMESTAMP BY [timestamp]
        GROUP BY
            vin,
            TumblingWindow(Duration(second, 30))
    ),
    VehicleDataAll AS (
        select
            AVG(engineTemperature) AS engineTemperature,
            AVG(speed) AS speed,
            AVG(refrigerationUnitKw) AS refrigerationUnitKw,
            AVG(refrigerationUnitTemp) AS refrigerationUnitTemp,
            COUNT(*) AS eventCount,
            (case when AVG(engineTemperature) >= 318 OR AVG(engineTemperature) <= 15 then 1 else 0 end) as engineTempAnomaly,
            (case when AVG(engineoil) <= 20 then 1 else 0 end) as oilAnomaly,
            (case when AVG(transmission_gear_position) <= 4 AND
                AVG(accelerator_pedal_position) >= 50 AND
                AVG(speed) >= 55 then 1 else 0 end) as aggressiveDriving,
            (case when AVG(refrigerationUnitTemp) >= 22.5 then 1 else 0 end) as refrigerationTempAnomaly,
            System.TimeStamp() as snapshot
        from events t TIMESTAMP BY [timestamp]
        GROUP BY
            TumblingWindow(Duration(second, 10))
    )
    -- INSERT INTO POWER BI
    SELECT
        *
    INTO
        powerbi
    FROM
        VehicleDataAll
    -- INSERT INTO COSMOS DB
    SELECT
        *,
        entityType = 'VehicleAverage',
        partitionKey = vin
    INTO
        cosmosdb
    FROM
        VehicleData
    ```

    ![The Stream Analytics job Query is displayed.](media/stream-analytics-query.png 'Query')

2. Select **Save query**.

### Task 4: Run Stream Analytics job

Next, we will start the Stream Analytics job so we can begin processing event data once it starts to flow through the services.

1. Select **Overview**.

2. In the Overview blade, select **Start** and select **Now** for the job output start time.

3. Select **Start** to beginning running the Stream Analytics job.

   ![The steps to start the job as described are displayed.](media/stream-analytics-start-job.png 'Start job')

## Exercise 3: Deploy Azure functions and Web App

**Duration**: 30 minutes

In the architecture for this scenario, Azure functions play a major role in event processing. These functions execute within an Azure Function App, Microsoft's serverless solution for easily running small pieces of code, or "functions," in the cloud. You can write just the code you need for the problem at hand, without worrying about a whole application or the infrastructure to run it. Functions can make development even more productive, and you can use your development language of choice, such as C#, F#, Node.js, Java, or PHP.

Before we dive into this exercise, let's go over how the functions and Web App fit into our architecture.

There are two Function Apps and one Web App in the solution. The Function Apps handle event processing within two stages of the data pipeline, and the Web App is used to perform CRUD operations against data stored in Cosmos DB.

![The two Function Apps and Web App are highlighted.](media/solution-diagram-function-apps-web-app.png 'Solution diagram')

You may wonder, if a Function App contains several functions within, _why do we need two Function Apps instead of one_? The primary reason for using two Function Apps is due to how functions scale to meet demand. When you use the Azure Functions consumption plan, you only pay for the time your code runs. More importantly, Azure automatically handles scaling your functions to meet demand. It scales using an internal scale controller that evaluates the type of trigger the functions are using, and applies heuristics to determine when to scale out to multiple instances. The important thing to know is that functions scale at the Function App level. Meaning, if you have one very busy function and the rest are mostly idle, that one busy function causes the entire Function App to scale. Think about this when designing your solution. It is a good idea to **divide extremely high-load functions into separate Function Apps**.

Now let's introduce the Function Apps and Web App and how they contribute to the architecture.

- **IoT-StreamProcessing Function App**: This is the Stream Processing Function App, and it contains a two functions:

  - **IoTHubTrigger**: This function is automatically triggered by the IoT Hub's Event Hub endpoint as vehicle telemetry is sent by the data generator. The function performs some light processing to the data by defining the partition key value, the document's TTL, adds a timestamp value, then saves the information to Cosmos DB.
  - **HealthCheck**: This function has an Http trigger that enables users to verify that the Function App is up and running, and that each configuration setting exists and has a value. More thorough checks would validate each value against an expected format or by connecting to each service as required. The function will return an HTTP status of `200` (OK) if all values contain non-zero strings. If any are null or empty, the function will return an error (`400`), indicating which values are missing. The data generator calls this function before running.

  ![The Event Processing function is shown.](media/solution-architecture-function1.png 'Solution architecture')

- **IoT-CosmosDBProcessing Function App**: This is the Trip Processing Function App. It contains three functions that are triggered by the Cosmos DB Change Feed on the `telemetry` container. Because the Cosmos DB Change Feed supports multiple consumers, these three functions can run in parallel, processing the same information simultaneously without conflicting with one another. When we define the `CosmosDBTrigger` for each of these functions, we configure the trigger settings to connect to a Cosmos DB collection named `leases` to keep track of which change feed events they have processed. We also set the `LeaseCollectionPrefix` value for each function with a unique prefix so one function does not attempt to retrieve or update the lease information for another. The following functions are in this Function App:

  - **TripProcessor**: This function groups vehicle telemetry data by VIN, retrieves the associated Trip record from the `metadata` container, updates the Trip record with a trip start timestamp, an end timestamp if completed, and a status showing whether the trip has started, is delayed, or has completed. It also updates the associated Consignment record with the status, and triggers the Logic App with the trip information if an alert needs to be emailed to the recipient defined in the Function App's app settings (`RecipientEmail`).
  - **ColdStorage**: This function connects to the Azure Storage account (`ColdStorageAccount`) and writes the raw vehicle telemetry data for cold storage in the following time-sliced path format: `telemetry/custom/scenario1/yyyy/MM/dd/HH/mm/ss-fffffff.json`.
  - **SendToEventHubsForReporting**: This function simply sends the vehicle telemetry data straight to Event Hubs, allowing Stream Analytics to apply windowed aggregates and save those aggregates in batches to Power BI and to the Cosmos DB `metadata` container.
  - **HealthCheck**: As with the function of the same name within the Stream Processing Function App, this function has an Http trigger that enables users to verify that the Function App is up and running, and that each configuration setting exists and has a value. The data generator calls this function before running.

  ![The Trip Processing function is shown.](media/solution-architecture-function2.png 'Solution architecture')

- **IoTWebApp**: The Web App provides a Fleet Management portal, allowing users to perform CRUD operations on vehicle data, make real-time battery failure predictions for a vehicle against the deployed machine learning model, and view consignments, packages, and trips. It connects to the Cosmos DB `metadata` container, using the [.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/).

  ![The Web App is shown.](media/solution-architecture-webapp.png 'Solution architecture')

### Task 1: Retrieve the URI for each Key Vault secret

When you set the App Settings for the Function Apps and Web App in the next task, you will need to reference the URI of a secret in Key Vault, including the version number. To do this, perform the following steps for each secret and **copy the values** to Notepad or similar text application.

1. Open your Key Vault instance in the portal.

2. Select **Secrets** under Settings in the left-hand menu.

3. Select the secret whose URI value you wish to obtain.

4. Select the **Current Version** of the secret.

   ![The secret's current version is displayed.](media/key-vault-secret-current-version.png 'Current Version')

5. Copy the **Secret Identifier**.

   ![The Secret Identifier is highlighted.](media/key-vault-secret-identifier.png 'Secret Identifier')

   When you add the Key Vault reference to this secret within a Function App's App Settings, you will use the following format: `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is replaced by the Secret Identifier (URI) value above. **Make sure you remove the curly braces (`{}`)**.

   For example, a complete reference would look like the following:

   `@Microsoft.KeyVault(SecretUri=https://iot-vault-501993860.vault.azure.net/secrets/CosmosDBConnection/794f93084861483d823d37233569561d)`

### Task 2: Configure application settings in Azure

> We recommend that you open two browser tabs for these steps. One to copy secrets from each Azure service, and the other to add the secrets to Key Vault.

1. Using a new tab or instance of your browser, navigate to the Azure portal, <https://portal.azure.com>.

2. Select **Resource groups** from the left-hand menu, then search for your resource group by typing in `cosmos-db-iot`. Select your resource group that you are using for this lab.

3. Open the your **Key Vault**. The name should begin with `iot-vault`.

   ![The Key Vault is highlighted in the resource group.](media/resource-group-keyvault.png 'Resource group')

4. In another browser tab, open the Azure Function App whose name begins with **IoT-CosmosDBProcessing**.

5. Select **Configuration** on the Overview pane.

    ![The Configuration link is highlighted in the Overview blade.](media/cosmosdb-function-overview.png "Overview")

6. Scroll to the **Application settings** section. Use the **+ New application setting** link to create the following additional Key/Value pairs (the key names must exactly match those found in the table below):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | ColdStorageAccount     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **ColdStorageAccount** Key Vault secret                                                                   |
    | EventHubsConnection   | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **EventHubsConnection** Key Vault secret |
    | LogicAppUrl        | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **LogicAppUrl** Key Vault secret |
    | RecipientEmail      | Enter a **valid email address** you want to receive notification emails from the Logic App. |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-cosmosdb-function.png 'Application Settings section')

7. Select **Save** to apply your changes.

8. Open the Azure Function App whose name begins with **IoT-StreamProcessing**.

9. Select **Configuration** on the Overview pane.

10. Scroll to the **Application settings** section. Use the **+ New application setting** link to create the following additional Key/Value pairs (the key names must exactly match those found in the table below):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | IoTHubConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **IoTHubConnection** Key Vault secret |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-stream-function.png 'Application Settings section')

11. Select **Save** to apply your changes.

12. Open the Web App (App Service) whose name begins with **IoTWebApp**.

13. Select **Configuration** in the left-hand menu.

14. Scroll to the **Application settings** section. Use the **+ New application setting** link to create the following additional Key/Value pairs (the key names must exactly match those found in the table below):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | DatabaseName     | Enter `ContosoAuto` |
    | ContainerName     | Enter `metadata` |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-web-app.png 'Application Settings section')

15. Select **Save** to apply your changes.

> Verify that the system-managed identities for both Function Apps and Web App are working properly and able to access Key Vault. To do this, within each Function App and the Web App, open the **CosmosDBConnection** setting and look at the **Key Vault Reference Details** underneath the setting. You should see an output similar to the following, which displays the secret details and indicates that it is using the _System assigned managed identity_:

![The application setting shows the Key Vault reference details underneath.](media/webapp-app-setting-key-vault-reference.png "Key Vault reference details")

> If you see an error in the Key Vault Reference Details, go to Key Vault and delete the access policy for the related system identity. Then go back to the Function App or web app, turn off the System Identity, turn it back on (which creates a new one), then re-add it to Key Vault's access policies.

### Task 3: Open solution

In this task, you will open the Visual Studio solution for this lab. It contains projects for both Function Apps, the Web App, and the data generator.

1. Open Windows Explorer and navigate to the location you extracted the solution ZIP file in the _Before the HOL_ guide. If you extracted the ZIP file directly to `C:\`, you need to open the following folder: `C:\scenario-based-labs-master\IoT\Starter`. Open the Visual Studio solution file: **CosmosDbIoTScenario.sln**.

    > If Visual Studio prompts you to sign in when it first launches, use the account provided to you for this lab (if applicable), or an existing Microsoft account.

2. After opening the solution, observe the included projects in the **Solution Explorer**:

    1. **Functions.CosmosDB**: Project for the **IoT-CosmosDBProcessing** Function App.
    2. **Functions.StreamProcessing**: Project for the **IoT-StreamProcessing** Function App.
    3. **CosmosDbIoTScenario.Common**: Contains entity models, extensions, and helpers used by the other projects.
    4. **FleetDataGenerator**: The data generator seeds the Cosmos DB `metadata` container with data and simulates vehicles, connects them to IoT Hub, then sends generated telemetry data.
    5. **FleetManagementWebApp**: Project for the **IoTWebApp** Web App.

    ![The Visual Studio Solution Explorer is displayed.](media/vs-solution-explorer.png "Solution Explorer")

3. Right-click on the `CosmosDbIoTScenario` solution in Solution Explorer, then select **Restore NuGet Packages**. The packages may have already been restored upon opening the solution.

### Task 4: Code completion and walk-through

The Function App and Web App projects contain blocks of code that need to be completed before you can deploy them. The reason for this is to help guide you through the solution, and to better understand the code by completing small fragments.

1. In Visual Studio, select **View**, then select **Task List**. This will display the list of **TODO** items, helping you navigate to each one.

    ![The View menu in Visual Studio is displayed, and the Task List item is highlighted.](media/vs-view-tasklist.png "View Task List")

    The Task List appears at the bottom of the window:

    ![The Task List is displayed.](media/vs-tasklist.png "Task List")

2. Open **Startup.cs** within the **Functions.CosmosDB** project and complete the code beneath **TODO 1** by pasting the following:

    ```csharp
    builder.Services.AddSingleton((s) => {
        var connectionString = configuration["CosmosDBConnection"];
        var cosmosDbConnectionString = new CosmosDbConnectionString(connectionString);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("Please specify a value for CosmosDBConnection in the local.settings.json file or your Azure Functions Settings.");
        }

        CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(cosmosDbConnectionString.ServiceEndpoint.OriginalString, cosmosDbConnectionString.AuthKey);
        return configurationBuilder
            .Build();
    });
    ```

    Your completed code should look as follows:

    ![The TODO 1 code is completed.](media/vs-todo1.png "TODO 1")

    Since we are using the [.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/), and dependency injection is supported starting with Function Apps v2, we are using a [singleton Azure Cosmos DB client for the lifetime of the application](https://docs.microsoft.com/azure/cosmos-db/performance-tips#sdk-usage). This is injected into the `Functions` class through its constructor, as you will see in the next TODO block.

3. **Save** the **Startup.cs** file.

4. Open **Functions.cs** within the **Functions.CosmosDB** project and complete the code beneath **TODO 2** by pasting the following:

    ```csharp
    public Functions(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
    {
        _httpClientFactory = httpClientFactory;
        _cosmosClient = cosmosClient;
    }
    ```

    Adding the code above allows the `HttpClientFactory` and the `CosmosClient` to be injected into the function code, which allows these services to manage their own connections and lifecycle to improve performance and prevent thread starvation and other issues caused by incorrectly creating too many instances of expensive objects. The `HttpClientFactory` was already configured in `Startup.cs` where you made your previous code change. It is used to send alerts to the Logic App endpoint, and uses [Polly](https://github.com/App-vNext/Polly) to employ a gradual back-off wait and retry policy in case the Logic App is overloaded or has other issues causing calls to the HTTP endpoint to fail.

5. Look at the first function code below the constructor code you just completed:

    ```csharp
    [FunctionName("TripProcessor")]
    public async Task TripProcessor([CosmosDBTrigger(
        databaseName: "ContosoAuto",
        collectionName: "telemetry",
        ConnectionStringSetting = "CosmosDBConnection",
        LeaseCollectionName = "leases",
        LeaseCollectionPrefix = "trips",
        CreateLeaseCollectionIfNotExists = true,
        StartFromBeginning = true)]IReadOnlyList<Document> vehicleEvents,
        ILogger log)
    {
    ```

    The `FunctionName` attribute defines how the function name appears within the Function App, and can be different from the C# method name. This `TripProcessor` function uses the `CosmosDBTrigger` to fire on every Cosmos DB change feed event. The events arrive in batches, whose size depends on factors such as how many Insert, Update, or Delete events there are for the container. The `databaseName` and `collectionName` properties define which container's change feed triggers the function. The `ConnectionStringSetting` indicates the name of the Function App's application setting from which to pull the Cosmos DB connection string. In our case, the connection string value will draw from the Key Vault secret you created. The `LeaseCollection` properties define the name of the lease container and the prefix applied to lease data for this function, and whether to create the lease container if it does not exist. `StartFromBeginning` is set to `true`, ensuring that all events since the function last run are processed. The function outputs the change feed documents into an `IReadOnlyList` collection.

6. Scroll down a little further in the function and complete the code beneath **TODO 3** by pasting the following:

    ```csharp
    var vin = group.Key;
    var odometerHigh = group.Max(item => item.GetPropertyValue<double>("odometer"));
    var averageRefrigerationUnitTemp =
        group.Average(item => item.GetPropertyValue<double>("refrigerationUnitTemp"));
    ```

    We have grouped the events by vehicle VIN, so we assign a local `vin` variable to hold the group key (VIN). Next, we use the `group.Max` aggregate function to calculate the max odometer value, and use the `group.Average` function to calculate the average refrigeration unit temperature. We will use the `odometerHigh` value to calculate the trip distance and determine whether the trip is completed, based on the planned trip distance from the `Trip` record in the Cosmos DB `metadata` container. The `averageRefrigerationUnitTemp` is added in the alert that gets sent to the Logic App, if needed.

7. Review the code that is just below the new code you added:

    ```csharp
    // First, retrieve the metadata Cosmos DB container reference:
    var container = _cosmosClient.GetContainer(database, metadataContainer);

    // Create a query, defining the partition key so we don't execute a fan-out query (saving RUs), where the entity type is a Trip and the status is not Completed, Canceled, or Inactive.
    var query = container.GetItemLinqQueryable<Trip>(requestOptions: new QueryRequestOptions { PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey(vin) })
        .Where(p => p.status != WellKnown.Status.Completed
                    && p.status != WellKnown.Status.Canceled
                    && p.status != WellKnown.Status.Inactive
                    && p.entityType == WellKnown.EntityTypes.Trip)
        .ToFeedIterator();

    if (query.HasMoreResults)
    {
        // Only retrieve the first result.
        var trip = (await query.ReadNextAsync()).FirstOrDefault();
        
        if (trip != null)
        {
            // Retrieve the Consignment record.
            var document = await container.ReadItemAsync<Consignment>(trip.consignmentId,
                new Microsoft.Azure.Cosmos.PartitionKey(trip.consignmentId));
            var consignment = document.Resource;
    ```

    Here we are using the [.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/) by retrieving a Cosmos DB container reference with the CosmosClient (`_cosmosClient`) that was injected into the class. We use the container's `GetItemLinqQueryable` with the `Trip` class type to query the container using LINQ syntax and binding the results to a new collection of type `Trip`. Note how we are passing the **partition key**, in this case the VIN, to prevent executing a cross-partion, fan-out query, saving RU/s. We also define the type of document we want to retrieve by setting the `entityType` document property in the query to Trip, since other entity types can also have the same partition key, such as the Vehicle type.

    Since we have the Consignment ID, we can use the `ReadItemAsync` method to retrieve a single Consignment record. Here we also pass the partition key to minimize fan-out. Within a Cosmos DB container, a document's unique ID is a combination of the `id` field and the partition key value.

8. Scroll down a little further in the function and complete the code beneath **TODO 4** by pasting the following:

    ```csharp
    if (updateTrip)
    {
        await container.ReplaceItemAsync(trip, trip.id, new Microsoft.Azure.Cosmos.PartitionKey(trip.partitionKey));
    }

    if (updateConsignment)
    {
        await container.ReplaceItemAsync(consignment, consignment.id, new Microsoft.Azure.Cosmos.PartitionKey(consignment.partitionKey));
    }
    ```

    The `ReplaceItemAsync` method updates the Cosmos DB document with the passed in object with the associated `id` and partition key value.

9. Scroll down and complete the code beneath **TODO 5** by pasting the following:

    ```csharp
    await httpClient.PostAsync(Environment.GetEnvironmentVariable("LogicAppUrl"), new StringContent(postBody, Encoding.UTF8, "application/json"));
    ```

    Here we are using the `HttpClient` created by the injected `HttpClientFactory` to post the serialized `LogicAppAlert` object to the Logic App. The `Environment.GetEnvironmentVariable("LogicAppUrl")` method extracts the Logic App URL from the Function App's application settings and, using the special Key Vault access string you added to the app setting, extracts the encrypted value from the Key Vault secret.

10. Scroll to the bottom of the file to find and complete **TODO 6** with the following code:

    ```csharp
    // Convert to a VehicleEvent class.
    var vehicleEventOut = await vehicleEvent.ReadAsAsync<VehicleEvent>();
    // Add to the Event Hub output collection.
    await vehicleEventsOut.AddAsync(new EventData(
        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(vehicleEventOut))));
    ```

    The `ReadAsAsync` method is an extension method located in `CosmosDbIoTScenario.Common.ExtensionMethods` that converts a Cosmos DB Document to a class; in this case, a `VehicleEvent` class. Currently, the `CosmosDBTrigger` on a function only supports returning an `IReadOnlyList` of `Documents`, requiring a conversion to another class if you want to work with your customer classes instead of a Document for now. This extension method automates the process.

    The `AddAsync` method asynchronously adds to the `IAsyncCollector<EventData>` collection defined in the function attributes, which takes care of sending the collection items to the defined Event Hub endpoint.

11. **Save** the **Functions.cs** file.

12. Open **Functions.cs** within the **Functions.StreamProcessing** project. Let us first review the function parameters:

    ```csharp
    [FunctionName("IoTHubTrigger")]
    public static async Task IoTHubTrigger([IoTHubTrigger("messages/events", Connection = "IoTHubConnection")] EventData[] vehicleEventData,
        [CosmosDB(
            databaseName: "ContosoAuto",
            collectionName: "telemetry",
            ConnectionStringSetting = "CosmosDBConnection")]
        IAsyncCollector<VehicleEvent> vehicleTelemetryOut,
        ILogger log)
    {
    ```

    This function is defined with the `IoTHubTrigger`. Each time the IoT devices send data to IoT Hub, this function gets triggered and sent the event data in batches (`EventData[] vehicleEventData`). The `CosmosDB` attribute is an output attribute, simplifying writing Cosmos DB documents to the defined database and container; in our case, the `ContosoAuto` database and `telemetry` container, respectively.

13. Scroll down in the function code to find and complete **TODO 7** with the following code:

    ```csharp
    vehicleEvent.partitionKey = $"{vehicleEvent.vin}-{DateTime.UtcNow:yyyy-MM}";
    // Set the TTL to expire the document after 60 days.
    vehicleEvent.ttl = 60 * 60 * 24 * 60;
    vehicleEvent.timestamp = DateTime.UtcNow;

    await vehicleTelemetryOut.AddAsync(vehicleEvent);
    ```

    The `partitionKey` property represents a synthetic composite partition key for the Cosmos DB container, consisting of the VIN + current year/month. Using a composite key instead of simply the VIN provides us with the following benefits:
    
    1. Distributing the write workload at any given point in time over a high cardinality of partition keys.
    2. Ensuring efficient routing on queries on a given VIN - you can spread these across time, e.g. `SELECT * FROM c WHERE c.partitionKey IN ("VIN123-2019-01", "VIN123-2019-02", …)`
    3. Scale beyond the 10GB quota for a single partition key value.

    The `ttl` property sets the time-to-live for the document to 60 days, after which Cosmos DB will delete the document, since the `telemetry` container is our hot path storage.

    When we asynchronously add the class to the `vehicleTelemetryOut` collection, the Cosmos DB output binding on the function automatically handles writing the data to the defined Cosmos DB database and container, managing the implementation details for us.

14. **Save** the **Functions.cs** file.

15. Open **Startup.cs** within the **FleetManagementWebApp** project. Scroll down to the bottom of the file to find and complete **TODO 8** with the following code:

    ```csharp
    CosmosClientBuilder clientBuilder = new CosmosClientBuilder(cosmosDbConnectionString.ServiceEndpoint.OriginalString, cosmosDbConnectionString.AuthKey);
    CosmosClient client = clientBuilder
        .WithConnectionModeDirect()
        .Build();
    CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
    ```

    This code uses the [.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/) to initialize the `CosmosClient` instance that is added to the `IServiceCollection` as a singleton for dependency injection and object lifetime management.

16. **Save** the **Startup.cs** file.

17. Open **CosmosDBService.cs** under the **Services** folder of the **FleetManagementWebApp** project to find and complete **TODO 9** with the following code:

    ```csharp
    var setIterator = query.Where(predicate).Skip(itemIndex).Take(pageSize).ToFeedIterator();
    ```

    Here we are using the newly added `Skip` and `Take` methods on the `IOrderedQueryable` object (`query`) to retrieve just the records for the requested page. The `predicate` represents the LINQ expression passed in to the `GetItemsWithPagingAsync` method to apply filtering.

18. Scroll down a little further to find and complete **TODO 10** with the following code:

    ```csharp
    var count = this._container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true, requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null)
        .Where(predicate).Count();
    ```

    In order to know how many pages we need to navigate, we must know the total item count with the current filter applied. To do this, we retrieve a new `IOrderedQueryable` results from the `Container`, pass the filter predicate to the `Where` method, and return the `Count` to the `count` variable. For this to work, you must set `allowSynchronousQueryExecution` to true, which we do with our first parameter to the `GetItemLinqQueryable` method.

19. **Save** the **CosmosDBService.cs** file.

20. Open **VehiclesController.cs** under the **Controllers** folder of the **FleetManagementWebApp** project to review the following code:

    ```csharp
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly Random _random = new Random();

    public VehiclesController(ICosmosDbService cosmosDbService, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _cosmosDbService = cosmosDbService;
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        if (search == null) search = "";
        //var query = new QueryDefinition("SELECT TOP @limit * FROM c WHERE c.entityType = @entityType")
        //    .WithParameter("@limit", 100)
        //    .WithParameter("@entityType", WellKnown.EntityTypes.Vehicle);
        // await _cosmosDbService.GetItemsAsync<Vehicle>(query);

        var vm = new VehicleIndexViewModel
        {
            Search = search,
            Vehicles = await _cosmosDbService.GetItemsWithPagingAsync<Vehicle>(
                x => x.entityType == WellKnown.EntityTypes.Vehicle &&
                      (string.IsNullOrWhiteSpace(search) ||
                      (x.vin.ToLower().Contains(search.ToLower()) || x.stateVehicleRegistered.ToLower() == search.ToLower())), page, 10)
        };

        return View(vm);
    }
    ```

    We are using dependency injection with this controller, just as we did with one of our Function Apps earlier. The `ICosmosDbService`, `IHttpClientFactory`, and `IConfiguration` services are injected into the controller through the controller's constructor. The `CosmosDbService` is the class whose code you updated in the previous step. The `CosmosClient` is injected into it through its constructor.

    The `Index` controller action method uses paging, which it implements by calling the `ICosmosDbService.GetItemsWithPagingAsync` method you updated in the previous step. Using this service in the controller helps abstract the Cosmos DB query implementation details and business rules from the code in the action methods, keeping the controller lightweight and the code in the service reusable across all the controllers.

    Notice that the paging query does not include a partition key, making the Cosmos DB query cross-partition, which is needed to be able to query across all the documents. If this query ends up being used a lot with the passed in `search` value, causing a higher than desired RU usage on the container per execution, then you might want to consider alternate strategies for the partition key, such as a combination of `vin` and `stateVehicleRegistered`. However, since most of our access patterns for vehicles in this container use the VIN, we are using it as the partition key right now. You will see code further down in the method that explicitly pass the partition key value.

21. Scroll down in the `VehiclesController.cs` file to find and complete **TODO 11** with the following code:

    ```csharp
    await _cosmosDbService.DeleteItemAsync<Vehicle>(item.id, item.partitionKey);
    ```

    Here we are doing a hard delete by completely removing the item. In a real-world scenario, we would most likely perform a soft delete, which means updating the document with a `deleted` property and ensuring all filters exclude items with this property. Plus, we'd soft delete related records, such as trips. Soft deletions make it much easier to recover a deleted item if needed in the future.

22. **Save** the **VehiclesController.cs** file.

### Task 5: Deploy Cosmos DB Processing Function App

1. In the Visual Studio Solution Explorer, right-click on the **Functions.CosmosDB** project, then select **Publish...**.

    ![The context menu is displayed and the Publish menu item is highlighted.](media/vs-publish.png "Publish")

2. In the publish dialog, select the **Azure Functions Consumption Plan** publish target. Next, select the **Select Existing** radio and make sure **Run from package file (recommended)** is checked. Select **Publish** on the bottom of the form.

    ![The publish dialog is displayed.](media/vs-publish-target-functions.png "Pick a publish target")

3. In the App Service pane, select your Azure Subscription you are using for this lab, and make sure View is set to **Resource group**. Find and expand your Resource Group in the results below. The name should start with **cosmos-db-iot**. Select the Function App whose name starts with **IoT-CosmosDBProcessing**, then select **OK**.

    ![The App Service blade of the publish dialog is displayed.](media/vs-publish-app-service-function-cosmos.png "App Service")

4. Click **Publish** to begin.

    After the publish completes, you should see the following in the Output window: `========== Publish: 1 succeeded, 0 failed, 0 skipped ==========` to indicate a successful publish.

    > If you do not see the Output window, select **View** in Visual Studio, then **Output**.

### Task 6: Deploy Stream Processing Function App

1. In the Visual Studio Solution Explorer, right-click on the **Functions.StreamProcessing** project, then select **Publish...**.

    ![The context menu is displayed and the Publish menu item is highlighted.](media/vs-publish.png "Publish")

2. In the publish dialog, select the **Azure Functions Consumption Plan** publish target. Next, select the **Select Existing** radio and make sure **Run from package file (recommended)** is checked. Select **Publish** on the bottom of the form.

    ![The publish dialog is displayed.](media/vs-publish-target-functions.png "Pick a publish target")

3. In the App Service pane, select your Azure Subscription you are using for this lab, and make sure View is set to **Resource group**. Find and expand your Resource Group in the results below. The name should start with **cosmos-db-iot**. Select the Function App whose name starts with **IoT-StreamProcessing**, then select **OK**.

    ![The App Service blade of the publish dialog is displayed.](media/vs-publish-app-service-function-stream.png "App Service")

4. Click **Publish** to begin.

    After the publish completes, you should see the following in the Output window: `========== Publish: 1 succeeded, 0 failed, 0 skipped ==========` to indicate a successful publish.

### Task 7: Deploy Web App

1. In the Visual Studio Solution Explorer, right-click on the **FleetManagementWebApp** project, then select **Publish...**.

    ![The context menu is displayed and the Publish menu item is highlighted.](media/vs-publish.png "Publish")

2. In the publish dialog, select the **App Service** publish target. Next, select the **Select Existing** radio, then select **Publish** on the bottom of the form.

    ![The publish dialog is displayed.](media/vs-publish-target-webapp.png "Pick a publish target")

3. In the App Service pane, select your Azure Subscription you are using for this lab, and make sure View is set to **Resource group**. Find and expand your Resource Group in the results below. The name should start with **cosmos-db-iot**. Select the Web App whose name starts with **IoTWebApp**, then select **OK**.

    ![The App Service blade of the publish dialog is displayed.](media/vs-publish-app-service-webapp.png "App Service")

4. Click **Publish** to begin.

    After the publish completes, you should see the following in the Output window: `========== Publish: 1 succeeded, 0 failed, 0 skipped ==========` to indicate a successful publish. Also, the web app should open in a new browser window. If you try to navigate through the site, you will notice there is no data. We will seed the Cosmos DB `metadata` container with data in the next exercise.

    ![The Fleet Management web app home page is displayed.](media/webapp-home-page.png "Fleet Management home page")

    If the web app does not automatically open, you can copy its URL on the publish dialog:

    ![The site URL value is highlighted on the publish dialog.](media/vs-publish-site-url.png "Publish dialog")

> **NOTE:** If the web application displays an error, then go into the Azure Portal for the **IoTWebApp** and click **Restart**. When the Azure Web App is created from the ARM Template and configured for .NET Core, it may need to be restarted for the .NET Core configuration to be fully installed and ready for the application to run. Once restarted, the web application will run as expected.

> ![App Service blade with Restart button highlighted](media/IoTWebApp-App-Service-Restart-Button.png "App Service blade with Restart button highlighted")

> **Further troubleshooting:** If, after restarting the web application more than once, you still encounter a _500_ error, there may be a problem with the system identity for the web app. To check if this is the issue, open the web application's Configuration and view its Application Settings. Open the **CosmosDBConnection** setting and look at the **Key Vault Reference Details** underneath the setting. You should see an output similar to the following, which displays the secret details and indicates that it is using the _System assigned managed identity_:

![The application setting shows the Key Vault reference details underneath.](media/webapp-app-setting-key-vault-reference.png "Key Vault reference details")

> If you see an error in the Key Vault Reference Details, go to Key Vault and delete the access policy for the web app's system identity. Then go back to the web app, turn off the System Identity, turn it back on (which creates a new one), then re-add it to Key Vault's access policies.

### Task 8: View Cosmos DB processing Function App in the portal and copy the Health Check URL

1. In the Azure portal (<https://portal.azure.com>), open the Azure Function App whose name begins with **IoT-CosmosDBProcessing**.

2. Expand the **Functions** list in the left-hand menu, then select **TripProcessor**.

    ![The TripProcessor function is displayed.](media/portal-tripprocessor-function.png "TripProcessor")

3. View the **function.json** file to the right. This file was generated when you published the Function App in Visual Studio. The bindings are the same as you saw in the project code for the function. When new instances of the Function App are created, the generated `function.json` file and a ZIP file containing the compiled application are copied to these instances, and these instances run in parallel to share the load as data flows through the architecture. The `function.json` file instructs each instance how to bind attributes to the functions, where to find application settings, and information about the compiled application (`scriptFile` and `entryPoint`).

4. Select the **HealthCheck** function. This function has an Http trigger that enables users to verify that the Function App is up and running, and that each configuration setting exists and has a value. The data generator calls this function before running.

5. Select **Get function URL**.

    ![The HealthCheck function is selected and the Get function URL link is highlighted.](media/portal-cosmos-function-healthcheck.png "HealthCheck function")

6. **Copy the URL** and save it to Notepad or similar text editor for the exercise that follows.

    ![The HealthCheck URL is highlighted.](media/portal-cosmos-function-healthcheck-url.png "Get function URL")

### Task 9: View stream processing Function App in the portal and copy the Health Check URL

1. In the Azure portal (<https://portal.azure.com>), open the Azure Function App whose name begins with **IoT-StreamProcessing**.

2. Expand the **Functions** list in the left-hand menu, then select the **HealthCheck** function. Next, select **Get function URL**.

    ![The HealthCheck function is selected and the Get function URL link is highlighted.](media/portal-stream-function-healthcheck.png "HealthCheck")

3. **Copy the URL** and save it to Notepad or similar text editor for the exercise that follows.

    ![The HealthCheck URL is highlighted.](media/portal-stream-function-healthcheck-url.png "Get function URL")

> **Hint**: You can paste the Health Check URLs into a web browser to check the status at any time. The data generator programmatically accesses these URLs each time it runs, then reports whether the Function Apps are in a failed state or missing important application settings.

## Exercise 4: Explore and execute data generator

**Duration**: 10 minutes

In this exercise, we will explore the data generator project, **FleetDataGenerator**, update the application configuration, and run it in order to seed the metadata database with data and simulate a single vehicle.

There are several tasks that the data generator performs, depending on the state of your environment. The first task is that the generator will create the Cosmos DB database and containers with the optimal configuration for this lab if these elements do not exist in your Cosmos DB account. When you run the generator in a few moments, this step will be skipped because you already created them at the beginning of the lab. The second task the generator performs is to seed your Cosmos DB `metadata` container with data if no data exists. This includes vehicle, consignment, package, and trip data. Before seeding the container with data, the generator temporarily increases the requested RU/s for the container to 50,000 for optimal data ingestion speed. After the seeding process completes, the RU/s are scaled back down to 15,000.

After the generator ensures the metadata exists, it begins simulating the specified number of vehicles. You are prompted to enter a number between 1 and 5, simulating 1, 10, 50, 100, or the number of vehicles specified in your configuration settings, respectively. For each simulated vehicle, the following tasks take place:

1. An IoT device is registered for the vehicle, using the IoT Hub connection string and setting the device ID to the vehicle's VIN. This returns a generated device key.
2. A new simulated vehicle instance (`SimulatedVehicle`) is added to a collection of simulated vehicles, each acting as an AMQP device and assigned a Trip record to simulate the delivery of packages for a consignment. These vehicles are randomly selected to have their refrigeration units fail and, out of those, some will randomly fail immediately while the others fail gradually.
3. The simulated vehicle creates its own AMQP device instance, connecting to IoT Hub with its unique device ID (VIN) and generated device key.
4. The simulated vehicle asynchronously sends vehicle telemetry information through its connection to IoT Hub continuously until it either completes the trip by reaching the distance in miles established by the Trip record, or receiving a cancellation token.

### Task 1: Open the data generator project

1. If the Visual Studio solution is not already open, navigate to `C:\scenario-based-labs-master\IoT\Starter` and open the Visual Studio solution file: **CosmosDbIoTScenario.sln**.

2. Expand the **FleetDataGenerator** project and open **Program.cs** in the Solution Explorer.

    ![The Program.cs file is highlighted in the Solution Explorer.](media/vs-data-generator-program.png "Solution Explorer")

### Task 2: Code walk-through

There is a lot of code within the data generator project, so we'll just touch on the highlights. The code we do not cover is commented and should be easy to follow if you so desire.

1. Within the **Main** method of **Program.cs**, the core workflow of the data generator is executed by the following code block:

    ```csharp
    // Instantiate Cosmos DB client and start sending messages:
    using (_cosmosDbClient = new CosmosClient(cosmosDbConnectionString.ServiceEndpoint.OriginalString,
        cosmosDbConnectionString.AuthKey, connectionPolicy))
    {
        await InitializeCosmosDb();

        // Find and output the container details, including # of RU/s.
        var container = _database.GetContainer(MetadataContainerName);

        var offer = await container.ReadThroughputAsync(cancellationToken);

        if (offer != null)
        {
            var currentCollectionThroughput = offer ?? 0;
            WriteLineInColor(
                $"Found collection `{MetadataContainerName}` with {currentCollectionThroughput} RU/s.",
                ConsoleColor.Green);
        }

        // Initially seed the Cosmos DB database with metadata if empty.
        await SeedDatabase(cosmosDbConnectionString, cancellationToken);
        trips = await GetTripsFromDatabase(numberSimulatedTrucks, container);
    }

    try
    {
        // Start sending telemetry from simulated vehicles to Event Hubs:
        _runningVehicleTasks = await SetupVehicleTelemetryRunTasks(numberSimulatedTrucks,
            trips, arguments.IoTHubConnectionString);
        var tasks = _runningVehicleTasks.Select(t => t.Value).ToList();
        while (tasks.Count > 0)
        {
            try
            {
                Task.WhenAll(tasks).Wait(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                //expected
            }

            tasks = _runningVehicleTasks.Where(t => !t.Value.IsCompleted).Select(t => t.Value).ToList();

        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("The vehicle telemetry operation was canceled.");
        // No need to throw, as this was expected.
    }
    ```

    The top section of the code instantiates a new `CosmosClient`, using the connection string defined in either `appsettings.json` or the environment variables. The first call within the block is to `InitializeCosmosDb()`. We'll dig into this method in a moment, but it is responsible for creating the Cosmos DB database and containers if they do not exist in the Cosmos DB account. Next, we create a new `Container` instance, which the v3 version of the .NET Cosmos DB SDK uses for operations against a container, such as CRUD and maintenance information. For example, we call `ReadThroughputAsync` on the container to retrieve the current throughput (RU/s), and we pass it to `GetTripsFromDatabase` to read Trip documents from the container, based on the number of vehicles we are simulating. In this method, we also call the `SeedDatabase` method, which checks whether data currently exists and, if not, calls methods in the `DataGenerator` class (`DataGenerator.cs` file) to generate vehicles, consignments, packages, and trips, then writes the data in bulk using the `BulkImporter` class (`BulkImporter.cs` file). This `SeedDatabase` method executes the following on the `Container` instance to adjust the throughput (RU/s) to 50,000 before the bulk import, and back to 15,000 after the data seeding is complete: `await container.ReplaceThroughputAsync(desiredThroughput);`.

    The `try/catch` block calls `SetupVehicleTelemetryRunTasks` to register IoT device instances for each simulated vehicle and load up the tasks from each `SimulatedVehicle` instance it creates. It uses `Task.WhenAll` to ensure all pending tasks (simulated vehicle trips) are complete, removing completed tasks from the `_runningvehicleTasks` list as they finish. The cancellation token is used to cancel all running tasks if you issue the cancel command (`Ctrl+C` or `Ctrl+Break`) in the console.

2. Scroll down the `Program.cs` file until you find the `InitializeCosmosDb()` method. Here is the code for your reference:

    ```csharp
    private static async Task InitializeCosmosDb()
    {
        _database = await _cosmosDbClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

        #region Telemetry container
        // Define a new container.
        var telemetryContainerDefinition =
            new ContainerProperties(id: TelemetryContainerName, partitionKeyPath: $"/{PartitionKey}")
            {
                IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
            };

        // Tune the indexing policy for write-heavy workloads by only including regularly queried paths.
        // Be careful when using an opt-in policy as we are below. Excluding all and only including certain paths removes
        // Cosmos DB's ability to proactively add new properties to the index.
        telemetryContainerDefinition.IndexingPolicy.ExcludedPaths.Clear();
        telemetryContainerDefinition.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/*" }); // Exclude all paths.
        telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Clear();
        telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/vin/?" });
        telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/state/?" });
        telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/partitionKey/?" });

        // Create the container with a throughput of 15000 RU/s.
        await _database.CreateContainerIfNotExistsAsync(telemetryContainerDefinition, throughput: 15000);
        #endregion

        #region Metadata container
        // Define a new container (collection).
        var metadataContainerDefinition =
            new ContainerProperties(id: MetadataContainerName, partitionKeyPath: $"/{PartitionKey}")
            {
                // Set the indexing policy to consistent and use the default settings because we expect read-heavy workloads in this container (includes all paths (/*) with all range indexes).
                // Indexing all paths when you have write-heavy workloads may impact performance and cost more RU/s than desired.
                IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
            };

        // Set initial performance to 50,000 RU/s for bulk import performance.
        await _database.CreateContainerIfNotExistsAsync(metadataContainerDefinition, throughput: 50000);
        #endregion

        #region Maintenance container
        // Define a new container (collection).
        var maintenanceContainerDefinition =
            new ContainerProperties(id: MaintenanceContainerName, partitionKeyPath: $"/vin")
            {
                IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
            };

        // Set initial performance to 400 RU/s due to light workloads.
        await _database.CreateContainerIfNotExistsAsync(maintenanceContainerDefinition, throughput: 400);
        #endregion
    }
    ```

    This method creates a Cosmos DB database if it does not already exist, otherwise it retrieves a reference to it (`await _cosmosDbClient.CreateDatabaseIfNotExistsAsync(DatabaseName);`). Then it creates `ContainerProperties` for the `telemetry`, `metadata`, and `maintenance` containers. The `ContainerProperties` object lets us specify the container's indexing policy. We use the default indexing policy for `metadata` and `maintenance` since they are read-heavy and benefit from a greater number of paths, but we exclude all paths in the `telemetry` index policy, and add paths only to those properties we need to query, due to the container's write-heavy workload. The `telemetry` container is assigned a throughput of 15,000 RU/s, 50,000 for `metadata` for the initial bulk import, then it is scaled down to 15,000, and 400 for `maintenance`.

### Task 3: Update application configuration

The data generator needs two connection strings before it can successfully run; the IoT Hub connection string, and the Cosmos DB connection string. The IoT Hub connection string can be found by selecting **Shared access policies** in IoT Hub, selecting the **iothubowner** policy, then copying the **Connection string--primary key** value. This is different from the Event Hub-compatible endpoint connection string you copied earlier.

![The iothubowner shared access policy is displayed.](media/iot-hub-connection-string.png "IoT Hub shared access policy")

1. Open **appsettings.json** within the **FleetDataGenerator** project.

2. Paste the IoT Hub connection string value in quotes next to the **IOT_HUB_CONNECTION_STRING** key. Paste the Cosmos DB connection string value in quotes next to the **COSMOS_DB_CONNECTION_STRING** key.

3. The data generator also requires the Health Check URLs you copied in the previous exercise for the `HealthCheck` functions located in both Function Apps. Paste the Cosmos DB Processing Function App's `HealthCheck` function's URL in quotes next to the **COSMOS_PROCESSING_FUNCTION_HEALTHCHECK_URL** key. Paste the Stream Processing Function App's `HealthCheck` function's URL in quotes next to the **STREAM_PROCESSING_FUNCTION_HEALTHCHECK_URL** key.

    ![The appsettings.json file is highlighted in the Solution Explorer, and the connection strings and health check URLs are highlighted within the file.](media/vs-appsettings.png "appsettings.json")

    The NUMBER_SIMULATED_TRUCKS value is used when you select option 5 when you run the generator. This gives you the flexibility to simulate between 1 and 1,000 trucks at a time. SECONDS_TO_LEAD specifies how many seconds to wait until the generator starts generating simulated data. The default value is 0. SECONDS_TO_RUN forces the simulated trucks to stop sending generated data to IoT Hub. The default value is 14400. Otherwise, the generator stops sending tasks when all the trips complete or you cancel by entering `Ctrl+C` or `Ctrl+Break` in the console window.

3. **Save** the `appsettings.json` file.

> As an alternative, you may save these settings as environment variables on your machine, or through the FleetDataGenerator properties. Doing this will remove the risk of accidentally saving your secrets to source control.

### Task 4: Run generator

In this task, you will run the generator and have it generate events for 50 trucks. The reason we are generating events for so many vehicles is two-fold:

   - In the next exercise, we will observe the function triggers and event activities with Application Insights.
   - We need to have completed trips prior to performing batch predictions in a later exercise.

> **Warning**: You will receive a lot of emails when the generator starts sending vehicle telemetry. If you do not wish to receive emails, simply disable the Logic App you created.

1. Within Visual Studio, right-click on the **FleetDataGenerator** project in the Solution Explorer and select **Set as Startup Project**. This will automatically run the data generator each time you debug.

    ![Set as Startup Project is highlighted in the Solution Explorer.](media/vs-set-startup-project.png "Solution Explorer")

2. Select the Debug button at the top of the Visual Studio window or hit **F5** to run the data generator.

    ![The debug button is highlighted.](media/vs-debug.png "Debug")

3. When the console window appears, enter **3** to simulate 50 vehicles. The generator will perform the Function App health checks, resize the requested throughput for the `metadata` container, use the bulk importer to seed the container, and resize the throughput back to 15,000 RU/s.

    ![3 has been entered in the console window.](media/cmd-run.png "Generator")

4. After the seeding is completed the generator will retrieve 50 trips from the database, sorted by shortest trip distance first so we can have completed trip data appear faster. You will see a message output for every 50 events sent, per vehicle with their VIN, the message count, and the number of miles remaining for the trip. For example: `Vehicle 19: C1OVHZ8ILU8TGGPD8 Message count: 3650 -- 3.22 miles remaining`. **Let the generator run in the background and continue to the next exercise**.

    ![Vehicle simulation begins.](media/cmd-simulated-vehicles.png "Generator")

5. As the vehicles complete their trips, you will see a message such as `Vehicle 37 has completed its trip`.

    ![A completed messages is displayed in the generator console.](media/cmd-vehicle-completed.png "Generator")

6. When the generator completes, you will see a message to this effect.

    ![A generation complete message is displayed in the generator console.](media/cmd-generator-completed.png "Generator")

If the health checks fail for the Function Apps, the data generator will display a warning, oftentimes telling you which application settings are missing. The data generator will not run until the health checks pass. Refer to Exercise 3, Task 2 above for tips on troubleshooting the application settings.

![The failed health checks are highlighted.](media/cmd-healthchecks-failed.png "Generator")

### Task 5: View devices in IoT Hub

The data generator registered and activated each simulated vehicle in IoT Hub as a device. In this task, you will open IoT Hub and view these registered devices.

1. In the Azure portal (<https://portal.azure.com>), open the IoT Hub instance within your **cosmos-db-iot** resource group.

    ![The IoT Hub resource is displayed in the resource group.](media/portal-resource-group-iot-hub.png "IoT Hub")

2. Select **IoT devices** in the left-hand menu. You will see all 50 IoT devices listed in the IoT devices pane to the right, with the VIN specified as the device ID. When we simulate more vehicles, we will see additional IoT devices registered here.

    ![The IoT devices pane is displayed.](media/iot-hub-iot-devices.png "IoT devices")

## Exercise 5: Observe Change Feed using Azure Functions and App Insights

**Duration**: 10 minutes

In this exercise, we use the Live Metrics Stream feature of Application Insights to view the incoming requests, outgoing requests, overall health, allocated server information, and sample telemetry in near-real time. This will help you observe how your functions scale under load and allow you to spot any potential bottlenecks or problematic components, through a single interactive interface.

### Task 1: Open App Insights Live Metrics Stream

1. In the Azure portal (<https://portal.azure.com>), open the Application Insights instance within your **cosmos-db-iot** resource group.

    ![The App Insights resource is displayed in the resource group.](media/portal-resource-group-app-insights.png "Application Insights")

2. Select **Live Metrics Stream** in the left-hand menu.

    ![The Live Metrics Stream link is displayed in the left-hand menu.](media/app-insights-live-metrics-stream-link.png "Application Insights")

3. Observe the metrics within the Live Metrics Stream as data flows through the system.

    ![The Live Metrics Stream page is displayed.](media/app-insights-live-metrics-stream.png "Live Metrics Stream")

    At the top of the page, you will see a server count. This shows how many instances of the Function Apps there are, and one server is allocated to the Web App. As the Function App server instances exceed computational, memory, or request duration thresholds, and as the IoT Hub and Change Feed queues grow and age, new instances are automatically allocated to scale out the Function Apps. You can view the server list at the bottom of the page. On the right-hand side you will see sample telemetry, including messages sent to the logger within the functions. Here we highlighted a message stating that the Cosmos DB Processing function is sending 100 Cosmos DB records to Event Hubs.

    You will notice many dependency call failures (404). These can be safely ignored. They are caused by the Azure Storage binding for the **ColdStorage** function within the Cosmos DB Processing Function App. This binding checks if the file exists before writing to the specified container. Since we are writing new files, you will see a `404` message for every file that is written since it does not exist. Currently, the binding engine does not know the difference between "good" 404 messages such as these, and "bad" ones.

## Exercise 6: Observe data using Cosmos DB Data Explorer and Web App

**Duration**: 10 minutes

### Task 1: View data in Cosmos DB Data Explorer

1. In the Azure portal (<https://portal.azure.com>), open the Cosmos DB instance within your **cosmos-db-iot** resource group.

2. Select **Data Explorer** in the left-hand menu.

3. Expand the **ContosoAuto** database, then expand the **metadata** container. Select **Items** to view a list of documents stored in the container. Select one of the items to view the data.

    ![The data explorer is displayed with a selected item in the metadata container's items list.](media/cosmos-data-explorer-metadata-items.png "Data Explorer")

4. Select the ellipses (...) to the right of the **metadata** container name, then select **New SQL Query**.

    ![The New SQL Query menu item is highlighted.](media/cosmos-data-explorer-metadata-new-sql-query.png "New SQL Query")

5. Replace the query with the following:

    ```sql
    SELECT * FROM c WHERE c.entityType = 'Vehicle'
    ```

6. Execute the query to view the first 100 vehicle records.

    ![The query editor is displayed with the vehicle results.](media/cosmos-vehicle-query.png "Vehicle query")

7. Update the query to find trip records where the trip is completed.

    ```sql
    SELECT * FROM c WHERE c.entityType = 'Trip' AND c.status = 'Completed'
    ```

    ![The query editor is displayed with the trip results.](media/cosmos-trip-completed-query.png "Trip query")

    Please note, you may not have any trips that have completed yet. Try querying where the `status` = **Active** instead. Active trips are those that are currently running.

    Here is an example completed trip record (several packages removed for brevity):

    ```json
    {
        "partitionKey": "DK6JW0RNF0G9PO2FJ",
        "id": "eb96c44e-4c1d-4f54-bdea-e7d2f927009c",
        "entityType": "Trip",
        "vin": "DK6JW0RNF0G9PO2FJ",
        "consignmentId": "e1da2e74-bf37-4773-a5bf-483fc08533ac",
        "plannedTripDistance": 18.33,
        "location": "AR",
        "odometerBegin": 106841,
        "odometerEnd": 106859.36,
        "temperatureSetting": 19,
        "tripStarted": "2019-09-20T14:39:24.1855725Z",
        "tripEnded": "2019-09-20T14:54:53.7558481Z",
        "status": "Completed",
        "timestamp": "0001-01-01T00:00:00",
        "packages": [
            {
                "packageId": "a5651f48-67d5-4c1b-b7d9-80d678aabe9b",
                "storageTemperature": 30,
                "highValue": false
            },
            {
                "packageId": "b2185628-eb0e-49b9-8b7d-685fcdcb5a36",
                "storageTemperature": 22,
                "highValue": false
            },
            {
                "packageId": "25ac4bd1-5aad-4030-91f7-9539cc15b441",
                "storageTemperature": 31,
                "highValue": true
            }
        ],
        "consignment": {
            "consignmentId": "e1da2e74-bf37-4773-a5bf-483fc08533ac",
            "customer": "Fabrikam, Inc.",
            "deliveryDueDate": "2019-09-20T17:50:40.3291024Z"
        },
        "_rid": "hM5HAOavCggb5QAAAAAAAA==",
        "_self": "dbs/hM5HAA==/colls/hM5HAOavCgg=/docs/hM5HAOavCggb5QAAAAAAAA==/",
        "_etag": "\"2d0364cc-0000-0700-0000-5d84e83d0000\"",
        "_attachments": "attachments/",
        "_ts": 1568991293
    }
    ```

    Portions of the package and consignment records are included since they are often used in trip queries and reports.

### Task 2: Search and view data in Web App

1. Navigate to your deployed Fleet Management web app. If you closed it earlier, you can find the deployment URL in the Overview blade of your Web App (**IoTWebApp**) in the portal.

    ![The web app's URL is highlighted.](media/webapp-url.png "Web App overview")

2. Select **Vehicles**. Here you will see the paging capabilities at work.

    ![The vehicles page is displayed.](media/webapp-vehicles.png "Vehicles")

3. Select one of the vehicles to view the details. On the right-hand side of the details page are the trips assigned to the vehicle. This view provides the customer name from the associated consignment record, aggregate information for the packages, and the trip details.

    ![The vehicle details are displayed.](media/webapp-vehicle-details.png "Vehicle details")

4. Go back to the vehicles list and enter a search term, such as **MT**. This will search both the state registered, and the VIN, including partial matches. Feel free to search for both states and VINs. In the screenshot below, we searched for `MT` and received results for Montana state registrations, and had a record where `MT` was included in the VIN.

    ![The search results are displayed.](media/webapp-vehicle-search.png "Vehicle search")

5. Select **Consignments** in the left-hand menu, then enter **alpine ski** in the search box and execute. You should see several consignments for the `Alpine Ski House` customer. You can also search by Consignment ID. In our results, one of the consignments has a status of Completed.

    ![The search results are displayed.](media/webapp-consignments-search.png "Consignments")

6. Select a consignment to view the details. The record shows the customer, delivery due date, status, and package details. The package statistics contains aggregates to calculate the total number of packages, the required storage temperature, based on the package with the lowest storage temperature setting, the total cubic feet and combined gross weight of the packages, and whether any of the packages are considered high value.

    ![The consignment details page is displayed.](media/webapp-consignment-details.png "Consignment details")

7. Select **Trips** in the left-hand menu. Use the filter at the top of the page to filter trips by status, such as Pending, Active, Delayed, and Completed. Trips are delayed if the status is not Completed prior to the delivery due date. You may not see any delayed at this point, but you may have some that become delayed when you re-run the data generator later. You can view the Vehicle or related Consignment record from this page.

    ![The search results are displayed.](media/webapp-trips-search.png "Trips")

## Exercise 7: Perform CRUD operations using the Web App

**Duration**: 10 minutes

In this exercise, you will insert, update, and delete a vehicle record.

### Task 1: Create a new vehicle

1. In the web app, navigate to the **Vehicles** page, then select **Create New Vehicle**.

    ![The Create New Vehicle button is highlighted on the vehicles page.](media/webapp-vehicles-new-button.png "Vehicles")

2. Complete the Create Vehicle form with the following VIN: **ISO4MF7SLBXYY9OZ3**. When finished filling out the form, select **Create**.

    ![The Create Vehicle form is displayed.](media/webapp-create-vehicle.png "Create Vehicle")

### Task 2: View and edit the vehicle

1. Search for your new vehicle in the Vehicles page by pasting the VIN in the search box: **ISO4MF7SLBXYY9OZ3**.

    ![The VIN is pasted in the search box and the vehicle result is displayed.](media/webapp-vehicles-search-vin.png "Vehicles")

2. Select the vehicle in the search results. Select **Edit Vehicle** in the vehicle details page.

    ![Details for the new vehicle are displayed and the edit vehicle button is highlighted.](media/webapp-vehicles-details-new.png "Vehicle details")

3. Update the record by changing the state registered and any other field, then select **Update**.

    ![The Edit Vehicle form is displayed.](media/webapp-vehicles-edit.png "Edit Vehicle")

### Task 3: Delete the vehicle

1. Search for your new vehicle in the Vehicles page by pasting the VIN in the search box: **ISO4MF7SLBXYY9OZ3**. You should see the registered state any any other fields you updated have changed.

    ![The VIN is pasted in the search box and the vehicle result is displayed.](media/webapp-vehicles-search-vin-updated.png "Vehicles")

2. Select the vehicle in the search results. Select **Delete** in the vehicle details page.

    ![Details for the new vehicle are displayed and the delete button is highlighted.](media/webapp-vehiclde-details-updated.png "Vehicle details")

3. In the Delete Vehicle confirmation page, select **Delete** to confirm.

    ![The Delete Vehicle confirmation page is displayed.](media/webapp-vehicles-delete-confirmation.png "Delete Vehicle")

4. Search for your new vehicle in the Vehicles page by pasting the VIN in the search box: **ISO4MF7SLBXYY9OZ3**. You should see that no vehicles are found.

    ![The vehicle was not found.](media/webapp-vehicles-search-deleted.png "Vehicles")

## Exercise 8: Create the Fleet status real-time dashboard in Power BI

**Duration**: 15 minutes

### Task 1: Log in to Power BI online and create real-time dashboard

> **Important**: If the data generator is no longer running or sending new telemetry, be sure to start it before continuing. Simulating 50 vehicles should suffice for this exercise.

1. Browse to <https://powerbi.microsoft.com> and sign in with the same account you used when you created the Power BI output in Stream Analytics.

2. Select **My workspace**, then select the **Datasets** tab. You should see the **Contoso Auto IoT Events** dataset. This is the dataset you defined in the Stream Analytics Power BI output.

   ![The Contoso Auto IoT dataset is displayed.](media/powerbi-datasets.png 'Power BI Datasets')

3. Select **+ Create** at the top of the page, then select **Dashboard**.

   ![The Create button is highlighted at the top of the page, and the Dashboard menu item is highlighted underneath.](media/powerbi-create-dashboard.png 'Create Dashboard')

4. Provide a name for the dashboard, such as `Contoso Auto IoT Live Dashboard`, then select **Create**.

   ![The create dashboard dialog is displayed.](media/powerbi-create-dashboard-dialog.png 'Create dashboard dialog')

5. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

   ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png 'Add tile')

6. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

   ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png 'Your datasets')

   > **Important**: If the **Contoso Auto IoT Events** data set does not appear, it is because there is a lag time of several minutes between when you first configure the Stream Analytics Power BI output and when data first appears in the streaming data set. Please ensure the data generator is running and that you have started the Stream Analytics query. Also, you may try restarting the Function App as well.

7. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **oilAnomaly** from the dropdown. Select **Next**.

   ![The oilAnomaly field is added.](media/power-bi-dashboard-add-tile-oilanomaly.png 'Add a custom streaming data tile')

8. Leave the values at their defaults for the tile details form, then select **Apply**.

   ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png 'Tile details')

9. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

10. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

11. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **engineTempAnomaly** from the dropdown. Select **Next**.

12. Leave the values at their defaults for the tile details form, then select **Apply**.

13. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

14. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

15. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **aggressiveDriving** from the dropdown. Select **Next**.

16. Leave the values at their defaults for the tile details form, then select **Apply**.

17. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

18. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

19. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **refrigerationTempAnomaly** from the dropdown. Select **Next**.

20. Leave the values at their defaults for the tile details form, then select **Apply**.

21. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

22. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

23. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **eventCount** from the dropdown. Select **Next**.

24. Leave the values at their defaults for the tile details form, then select **Apply**.

25. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

26. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

27. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **engineTemperature**. Leave the time window to display at 1 minute. Select **Next**.

    ![The engineTemperature field is added.](media/power-bi-dashboard-add-tile-enginetemperature.png 'Add a custom streaming data tile')

28. Leave the values at their defaults for the tile details form, then select **Apply**.

29. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

30. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

31. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **refrigerationUnitTemp**. Leave the time window to display at 1 minute. Select **Next**.

32. Leave the values at their defaults for the tile details form, then select **Apply**.

33. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

34. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

35. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **speed**. Leave the time window to display at 1 minute. Select **Next**.

36. Leave the values at their defaults for the tile details form, then select **Apply**.

37. When you are done, rearrange the tiles as shown:

    ![The tiles have been rearranged.](media/power-bi-dashboard-rearranged.png 'Power BI dashboard')

38. If the data generator is finished sending events, you may notice that tiles on the dashboard are empty. If so, start the data generator again, this time selecting **option 1** for one vehicle. If you do this, the refrigeration temperature anomaly is guaranteed, and you will see the refrigeration unit temperature gradually climb above the 22.5 degree Fahrenheit alert threshold. Alternatively, you may opt to simulate more vehicles and observe the high event count numbers.

    ![The live dashboard is shown with events.](media/power-bi-dashboard-live-results.png "Power BI dashboard")

    After the generator starts sending vehicle telemetry, the dashboard should start working after a few seconds. In this screenshot, we are simulating 50 vehicles with 2,486 events in the last 10 seconds. You may see a higher or lower value for the `eventCount`, depending on the speed of your computer on which you are running the generator, your network speed and latency, and other factors.

## Exercise 9: Create the Trip/Consignment Status reports in Power BI

**Duration**: 15 minutes

In this exercise, you will import a Power BI report that has been created for you. After opening it, you will update the data source to point to your Power BI instance.

### Task 1: Import report in Power BI Desktop

1. Open **Power BI Desktop**, then select **Open other reports**.

    ![The Open other reports link is highlighted.](media/pbi-splash-screen.png "Power BI Desktop")

2. In the Open report dialog, browse to `C:\scenario-based-labs-master\IoT\Reports`, then select **FleetReport.pbix**. Click **Open**.

    ![The FleetReport.pbix file is selected in the dialog.](media/pbi-open-report.png "Open report dialog")

### Task 2: Update report data sources

1. After the report opens, click on **Edit Queries** in the ribbon bar within the Home tab.

    ![The Edit Queries button is highlighted.](media/pbi-edit-queries-button.png "Edit Queries")

2. Select **Trips** in the Queries list on the left, then select **Source** under Applied Steps. Click the gear icon next to Source.

    ![The Trip query is selected and the source configuration icon is highlighted.](media/pbi-queries-trips-source.png "Edit Queries")

3. In the source dialog, update the Cosmos DB **URL** value with your Cosmos DB URI you copied earlier in the lab, then click **OK**. If you need to find this value, navigate to your Cosmos DB account in the portal, select Keys in the left-hand menu, then copy the URI value.

    ![The Trips source dialog is displayed.](media/pbi-queries-trips-source-dialog.png "Trips source dialog")

    The Trips data source has a SQL statement defined that returns only the fields we need, and applies some aggregates:

    ```sql
    SELECT c.id, c.vin, c.consignmentId, c.plannedTripDistance,
    c.location, c.odometerBegin, c.odometerEnd, c.temperatureSetting,
    c.tripStarted, c.tripEnded, c.status,
    (
        SELECT VALUE Count(1) 
        FROM n IN c.packages
    ) AS numPackages,
    (
        SELECT VALUE MIN(n.storageTemperature) 
        FROM n IN c.packages
    ) AS packagesStorageTemp,
    (
        SELECT VALUE Count(1)
        FROM n IN c.packages
        WHERE n.highValue = true
    ) AS highValuePackages,
    c.consignment.customer,
    c.consignment.deliveryDueDate
    FROM c where c.entityType = 'Trip'
    and c.status in ('Active', 'Delayed', 'Completed')
    ```

4. When prompted, enter the Cosmos DB **Account key** value, then click **Connect**. If you need to find this value, navigate to your Cosmos DB account in the portal, select Keys in the left-hand menu, then copy the Primary Key value.

    ![The Cosmos DB account key dialog is displayed.](media/pbi-queries-trips-source-dialog-account-key.png "Cosmos DB account key dialog")

5. In a moment, you will see a table named **Document** that has several rows whose value is Record. This is because Power BI doesn't know how to display the JSON document. The document has to be expanded. After expanding the document, you want to change the data type of the numeric and date fields from the default string types, so you can perform aggregate functions in the report. These steps have already been applied for you. Select the **Changed Type** step under Applied Steps to see the columns and changed data types.

    ![The Trips table shows Record in each row.](media/pbi-queries-trips-updated.png "Queries")

    The screenshot below shows the Trips document columns with the data types applied:

    ![The Trips document columns are displayed with the changed data types.](media/pbi-queries-trips-changed-type.png "Trips with changed types")

6. Select **VehicleAverages** in the Queries list on the left, then select **Source** under Applied Steps. Click the gear icon next to Source.

    ![The VehicleAverages query is selected and the source configuration icon is highlighted.](media/pbi-queries-vehicleaverages-source.png "Edit Queries")

7. In the source dialog, update the Cosmos DB **URL** value with your Cosmos DB URI, then click **OK**.

    ![The VehicleAverages source dialog is displayed.](media/pbi-queries-vehicleaverages-source-dialog.png "Trips source dialog")

    The VehicleAverages data source has the following SQL statement defined:

    ```sql
    SELECT c.vin, c.engineTemperature, c.speed,
    c.refrigerationUnitKw, c.refrigerationUnitTemp,
    c.engineTempAnomaly, c.oilAnomaly, c.aggressiveDriving,
    c.refrigerationTempAnomaly, c.snapshot
    FROM c WHERE c.entityType = 'VehicleAverage'
    ```

8. If prompted, enter the Cosmos DB **Account key** value, then click **Connect**. You may not be prompted since you entered the key in an earlier step.

    ![The Cosmos DB account key dialog is displayed.](media/pbi-queries-trips-source-dialog-account-key.png "Cosmos DB account key dialog")

9. Select **VehicleMaintenance** in the Queries list on the left, then select **Source** under Applied Steps. Click the gear icon next to Source.

    ![The VehicleMaintenance query is selected and the source configuration icon is highlighted.](media/pbi-queries-vehiclemaintenance-source.png "Edit Queries")

10. In the source dialog, update the Cosmos DB **URL** value with your Cosmos DB URI, then click **OK**.

    ![The VehicleMaintenance source dialog is displayed.](media/pbi-queries-vehiclemaintenance-source-dialog.png "Trips source dialog")

    The VehicleMaintenance data source has the following SQL statement defined, which is simpler than the other two since there are no other entity types in the `maintenance` container, and no aggregates are needed:

    ```sql
    SELECT c.vin, c.serviceRequired FROM c
    ```

11. If prompted, enter the Cosmos DB **Account key** value, then click **Connect**. You may not be prompted since you entered the key in an earlier step.

    ![The Cosmos DB account key dialog is displayed.](media/pbi-queries-trips-source-dialog-account-key.png "Cosmos DB account key dialog")

12. If you are prompted, click **Close & Apply**.

    ![The Close & Apply button is highlighted.](media/pbi-close-apply.png "Close & Apply")

### Task 3: Explore report

1. The report will apply changes to the data sources and the cached data set will be updated in a few moments. Explore the report, using the slicers (status filter, customer filter, and VIN list) to filter the data for the visualizations. Also be sure to select the different tabs at the bottom of the report, such as Maintenance for more report pages.

    ![The report is displayed.](media/pbi-updated-report.png "Updated report")

2. Select a customer from the Customer Filter, which acts as a slicer. This means when you select an item, it applies a filter to the other items on the page and linked pages. After selecting a customer, you should see the map and graphs change. You will also see a filtered list of VINs and Status. Select the **Details** tab.

    ![A customer record is selected, and an arrow is pointed at the Details tab.](media/pbi-customer-slicer.png "Customer selected")

3. The Details page shows related records, filtered on the selected customer and/or VIN. Now select the **Trips** tab.

    ![The details page is displayed.](media/pbi-details-tab.png "Details")

4. The Trips page shows related trip information. Select **Maintenance**.

    ![The trips page is displayed.](media/pbi-trips-tab.png "Trips")

5. The Maintenance page should contain no data at this time. This page shows information from a batch scoring Machine Learning notebook that you can optionally execute in Databricks in the next exercise.

6. If at any time you have a number of filters set and you cannot see records, **Ctrl+Click** the **Clear Filters** button on the main report page (Trip/Consignments).

    ![The Clear Filters button is highlighted.](media/pbi-clear-filters.png "Clear Filters")

7. If your data generator is running while viewing the report, you can update the report with new data by clicking the **Refresh** button at any time.

    ![The refresh button is highlighted.](media/pbi-refresh.png "Refresh")

-------------------------------------------------------------------------------

> The following exercises walk you through setting up and running the Machine Learning (ML) components of the accelerator, and are optional. If you wish to skip these steps, please jump ahead to the [environment clean-up](#environment-clean-up) section.

## Exercise 10: Run the predictive maintenance batch scoring

**Duration**: 20 minutes

In this exercise, you will import Databricks notebooks into your Azure Databricks workspace. A notebook is interactive and runs in any web browser, mixing markup (formatted text with instructions), executable code, and outputs from running the code.

Next, you will run the Batch Scoring notebook to make battery failure predictions on vehicles, using vehicle and trip data stored in Cosmos DB.

### Task 1: Import lab notebooks into Azure Databricks

In this task, you will import the Databricks notebooks into your workspace.

1. In the [Azure portal](https://portal.azure.com), open your lab resource group, then open your **Azure Databricks Service**. The name should start with `iot-databricks`.

   ![The Azure Databricks Service is highlighted in the resource group.](media/resource-group-databricks.png 'Resource Group')

2. Select **Launch Workspace**. Azure Databricks will automatically sign you in through its Azure Active Directory integration.

   ![Launch Workspace](media/databricks-launch-workspace.png 'Launch Workspace')

3. Select **Workspace**, select **Users**, select the dropdown to the right of your username, then select **Import**.

   ![The Import link is highlighted in the Workspace.](media/databricks-import-link.png 'Workspace')

4. Select **URL** next to **Import from**, paste the following into the text box: `https://github.com/AzureCosmosDB/scenario-based-labs/blob/master/IoT/Notebooks/01%20IoT.dbc`, then select **Import**.

   ![The URL has been entered in the import form.](media/databricks-import.png 'Import Notebooks')

5. After importing, select your username. You will see a new folder named `01 IoT`, which contains two notebooks, and a sub-folder named `Includes`, which contains one notebook.

    ![The imported notebooks are displayed.](media/databricks-notebooks.png 'Imported notebooks')

6. Open the **Shared-Configuration** notebook located in the `Includes` sub-folder and provide values for your Machine Learning service workspace. You can find these values within the Overview blade of your Machine Learning service workspace that is located in your lab resource group.

    The values highlighted in the screenshot below are for the following variables in the notebooks:

    1. `subscription_id`
    2. `resource_group`
    3. `workspace_name`
    4. `workspace_region`

    ![The required values are highlighted.](media/machine-learning-workspace-values.png "Machine Learning service workspace values")

### Task 2: Run batch scoring notebook

In this task, you will run the `Batch Scoring` notebook, using a pre-trained machine learning (ML) model to determine if the battery needs to be replaced on several vehicles within the next 30 days. The notebook performs the following actions:

1. Installs required Python libraries.
2. Connects to Azure Machine Learning service (Azure ML).
3. Downloads a pre-trained ML model, saves it to Azure ML, then uses that model for batch scoring.
4. Uses the Cosmos DB Spark connector to retrieve completed Trips and Vehicle metadata from the `metadata` Cosmos DB container, prepares the data using SQL queries, then surfaces the data as temporary views.
5. Applies predictions against the data, using the pre-trained model.
6. Saves the prediction results in the Cosmos DB `maintenance` container for reporting purposes.

To run this notebook, perform the following steps:

1. In Azure Databricks, select **Workspace**, select **Users**, then select your username.

2. Select the `01 IoT` folder, then select the **Batch Scoring** notebook to open it.

   ![The Batch Scoring notebook is highlighted.](media/databricks-batch-scoring-notebook.png 'Workspace')

3. Before you can execute the cells in this or the other notebooks for this lab, you must first attach your Databricks cluster. Expand the dropdown at the top of the notebook where you see **Detached**. Select your lab cluster to attach it to the notebook. If it is not currently running, you will see an option to start the cluster.

   ![The screenshot displays the lab cluster selected for attaching to the notebook.](media/databricks-notebook-attach-cluster.png 'Attach cluster')

4. You may use keyboard shortcuts to execute the cells, such as **Ctrl+Enter** to execute a single cell, or **Shift+Enter** to execute a cell and move to the next one below.

In both notebooks, you will be required to provide values for your Machine Learning service workspace. You can find these values within the Overview blade of your Machine Learning service workspace that is located in your lab resource group.

The values highlighted in the screenshot below are for the following variables in the notebooks:

1. `subscription_id`
2. `resource_group`
3. `workspace_name`
4. `workspace_region`

![The required values are highlighted.](media/machine-learning-workspace-values.png "Machine Learning service workspace values")

> If you wish to execute this notebook on a scheduled basis, such as every evening, you can use the Jobs feature in Azure Databricks to accomplish this.

## Exercise 11: Deploy the predictive maintenance web service

**Duration**: 20 minutes

In addition to batch scoring, Contoso Auto would like to predict battery failures on-demand in real time for any given vehicle. They want to be able to call the model from their Fleet Management website when looking at a vehicle to predict whether that vehicle's battery may fail in the next 30 days.

In the previous task, you executed a notebook that used a pre-trained ML model to predict battery failures for all vehicles with trip data in a batch process. But how do you take that same model and deploy it (in data science terms, this is called "operationalization") to a web service for this purpose?

In this task, you will run the `Model Deployment` notebook to deploy the pre-trained model to a web service hosted by Azure Container Instances (ACI), using your Azure ML workspace. While it is possible to deploy the model to a web service running in Azure Kubernetes Service (AKS), we are deploying to ACI instead since doing so saves 10-20 minutes. However, once deployed, the process used to call the web service is the same, as are most of the steps to do the deployment.

### Task 1: Run deployment notebook

To run this notebook, perform the following steps:

1. In Azure Databricks, select **Workspace**, select **Users**, then select your username.

2. Select the `01 IoT` folder, then select the **Model Deployment** notebook to open it.

   ![The Model Deployment notebook is highlighted.](media/databricks-model-deployment-notebook.png 'Workspace')

3. As with the Batch Scoring notebook, be sure to attach your lab cluster before executing cells.

4. **After you are finished running the notebook**, open the Azure Machine Learning service workspace in the portal, then select **Models** in the left-hand menu to view the pre-trained model.

   ![The models blade is displayed in the AML service workspace.](media/aml-models.png 'Models')

5. Select **Deployments** in the left-hand menu, then select the Azure Container Instances deployment that was created when you ran the notebook.

    ![The deployments blade is displayed in the AML service workspace.](media/aml-deployments.png "Deployments")

6. Copy the **Scoring URI** value. This will be used by the deployed web app to request predictions in real time.

    ![The deployment's scoring URI is highlighted.](media/aml-deployment-scoring-uri.png "Scoring URI")

### Task 2: Call the deployed scoring web service from the Web App

Now that the web service is deployed to ACI, we can call it to make predictions from the Fleet Management Web App. To enable this capability, we first need to update the Web App's application configuration settings with the scoring URI.

1. Make sure you have copied the Scoring URI of your deployed service, as instructed in the previous task.

2. Open the Web App (App Service) whose name begins with **IoTWebApp**.

3. Select **Configuration** in the left-hand menu.

4. Scroll to the **Application settings** section then select **+ New application setting**.

5. In the Add/Edit application setting form, enter `ScoringUrl` for the **Name**, and paste the web service URI you copied and paste it in the **Value** field. Select **OK** to add the setting.

    ![The form is filled in with the previously described values.](media/app-setting-scoringurl.png "Add/Edit application setting")

6. Select **Save** to save your new application setting.

7. Go back to the **Overview** blade for the Web App, then select **Restart**.

8. Navigate to the deployed Fleet Management web app and open a random Vehicle record. Select **Predict battery failure**, which calls your deployed scoring web service and makes a prediction for the vehicle.

    ![The prediction results show that the battery is not predicted to fail in the next 30 days.](media/web-prediction-no.png "Vehicle details with prediction")

    This vehicle has a low number of **Lifetime cycles used**, compared to the battery's rated 200 cycle lifespan. The model predicted that the battery will not fail within the next 30 days.

9. Look through the list of vehicles to find one whose **Lifetime cycles used** value is closer to 200, then make the prediction for the vehicle.

    ![The prediction results show that the battery is is predicted to fail in the next 30 days.](media/web-prediction-yes.png "Vehicle details with prediction")

    This vehicle has a high number of **Lifetime cycles used**, which is closer to the battery's rated 200 cycle lifespan. The model predicted that the battery will fail within the next 30 days.

## Exercise 12: Refresh the Power BI report and view maintenance data

**Duration**: 15 minutes

### Task 1: Open and refresh the report in Power BI Desktop

1. Open **Power BI Desktop**, then re-open the report.

2. Select the tab at the bottom of the report to view the Maintenance page. This page shows results from the batch scoring notebook you executed in Databricks. If you do not see records here, then you need to run the entire batch scoring notebook after some trips have completed.

    ![The maintenance page is displayed.](media/pbi-maintenance-tab.png "Maintenance")

3. Update the report with new data by clicking the **Refresh** button. This will show the results of the batch scoring notebook if you had one or more completed trips prior to running the notebook.

    ![The refresh button is highlighted.](media/pbi-refresh.png "Refresh")

## Environment clean-up

**Duration**: 10 mins

In this exercise, you will delete any Azure resources that were created in support of the lab. You should follow all steps provided after attending the Hands-on lab to ensure your account does not continue to be charged for lab resources.

### Task 1: Delete the resource group

1. Using the [Azure portal](https://portal.azure.com), navigate to the Resource group you used throughout this hands-on lab by selecting Resource groups in the left menu.

2. Search for the name of your resource group, and select it from the list.

3. Select Delete in the command bar, and confirm the deletion by re-typing the Resource group name, and selecting Delete.

You should follow all steps provided _after_ attending the Hands-on lab.
