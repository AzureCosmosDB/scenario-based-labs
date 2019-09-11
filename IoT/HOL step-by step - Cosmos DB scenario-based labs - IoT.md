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
    - [Task 1: Configure IoT Hub capture](#task-1-configure-iot-hub-capture)
    - [Task 2: Create Cosmos DB database and container](#task-2-create-cosmos-db-database-and-container)
    - [Task 3: Add Key Vault secrets](#task-3-add-key-vault-secrets)
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

TBD...

## Solution architecture

TBD...

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.
   - Trial subscriptions will not work.
   - **IMPORTANT**: To complete the OAuth 2.0 access components of this hands-on lab you must have permissions within your Azure subscription to create an App Registration and service principal within Azure Active Directory.
2. Install [Power BI Desktop](https://powerbi.microsoft.com/desktop/)
3. Install [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/) or greater

## Before the hands-on lab

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

## Exercise 1: Configure environment

### Task 1: Configure IoT Hub capture

### Task 2: Create Cosmos DB database and container

### Task 3: Add Key Vault secrets

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
