<div class="MCWHeader1">
Cosmos DB scenario-based Demo - Retail
</div>

<div class="MCWHeader2">
Hands-on demo step-by-step
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

- [Cosmos DB scenario-based demo - Retail hands-on demo step-by-step](#cosmos-db-scenario-based-demo---retail-hands-on-lab-step-by-step)
  - [Abstract and learning objectives](#abstract-and-learning-objectives)
  - [Overview](#overview)
  - [Solution architecture (High-level)](#solution-architecture-high-level)
  - [Requirements](#requirements)
  - [Before the hands-on lab](#before-the-hands-on-lab)
  - [Exercise 1: Deployment and Setup](#exercise-1-deployment-and-setup)
    - [Task 1: Deploy ARM Template](#task-1-blah)
    - [Task 2: Initialize and populate the Cosmos DB instance](#task-1-blah)
    - [Task 3: Configure resources](#task-1-blah)
  - [Exercise 2: Explore Contoso Movie Store](#exercise-2-explore-contoso-movie-store)
    - [Task 1: Explore the Contoso Movie Store](#task-1-blah-1)
  - [Exercise 3: Simulate data and events using Stream Analytics and Power BI](#exercise-3-simulate-data-and-events-using-stream-analytics-and-power-bi)
    - [Task 1: Open the Power BI Dashboard](#task-1-blah-2)
    - [Task 2: Start Stream Analytics and run the Data Generator](#task-2-blah-2)
  - [Exercise 4: Email alerts using Logic Apps](#exercise-4-email-alerts-using-logic-apps)
    - [Task 1: Review the Logic App and Emails](#task-1-review-the-logic-app-and-emails)
  - [After the hands-on lab](#after-the-hands-on-lab)
    - [Task 1: Delete resource group](#task-1-delete-resource-group)

<!-- /TOC -->

# Cosmos DB scenario-based labs - Retail hands-on lab step-by-step

## Abstract and learning objectives

In this demo you will show your audience how to uiltize Azure services to host a movie retail store with custom AI models and CosmosDb.  Several other PaaS based technologies will be used to show how Azure can be used to migrate legacy applications to the cloud.

## Overview

Contoso Movies, Ltd. has redesigned its website to utilize Azure PaaS services including CosmosDb, Functions, EventHubs, Stream Analytics, Power BI and Logic Apps.  As part of this redesign they have also implemented a new recommendation system based on custom AI models.  These AI models are done **offline** and stored in CosmosDb for reference when users are browing a site.  User events will immplicitly rank the items they are clicking on and then modify their recommendations based on these events.

## Solution architecture (High-level)

![TODO.](../Media/solution-diagram-1.png "Solution Architecture")

## Requirements

1. Microsoft Azure subscription must be pay-as-you-go or MSDN.

    - Trial subscriptions will not work.
    
## Before the demo

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

## Exercise 1: Deployment and Setup

Duration: 60 minutes

Synopsis:  In this exercise you will TODO

### Task 1: Configure Stream Analytics

1.  Set all the Azure resource configurations

### Task 2: Setup Power BI

1.  Set all the Azure resource configurations

## Exercise 2: Explore Contoso Movie Store

Duration: 15 minutes

Synopsis: TODO

### Task 1: Explore the Contoso Movie Store

1.  Open the deployed web site

2.  Mention that you are not logged in as any user and the results that are being displayed are based on the **top** purchased items in the Cosmso database.

3.  In the top navigation, cLick the **Login** link

4.  Mention that there are several pre-populated *personalities*.  Select the **comedy@contosomovies.com** personality

5.  Mention that you now have targeted movies based on two different algorithms

## Exercise 3: Simulate data and events using Stream Analytics and Power BI

Duration: 30 minutes

In this exercise you will TODO

### Task 1: Open the Power BI dashboard

1.  Browse to your PowerBI dashboard and open the Movie Dashboard

### Task 2: Start Stream Analytics and run the Data Generator

1.  Browse to your Stream Analytics service, click **Start**

2.  Open the **DataGenerator** project

3.  Update the app.config settings, then run the **DataGenerator** project, after a few moments, notice that your dashboard is being updated

4.  After 30 seconds, you will notice the **buy** events has stopped.

## Exercise 4: Email alerts using Logic Apps

Duration: 30 minutes

In this exercise you will configure your change feed function to call an HTTP login app endpoint that will then send an email when an order event occurs.  The function will be using Polly to handle retries in the case the function app is not available.

### Task 1: Setup Logic App

1.  Open the Azure Portal to your resource group and select the Logic App in your resource group, it should be named **s2_logicapp_...**

1.  Click **Edit**

![The Logic App blade with 'edit' highlighted.](./images/xx_logicapp_01.png "Edit the Logic App")

1.  Click **+New step**

![The Logic App Designer is displayed with 'new step' highlighted.](./images/xx_logicapp_02.png "Add a new step")

1.  Search for **send an email**, then select the Office 365 outlook connector

![Action search box is displayed with the text 'send an email' typed and the corresponding action highlighted.](./images/xx_logicapp_03.png "Add Send an Email action")

1.  Click **Sign in**, login using your Azure AD credentials

![Sign in button is highlighted.](./images/xx_logicapp_04.png "Sign in to Office 365")

1.  Set the **To** as your email

1.  Set the **Subject** as **Thank you for your order**

1.  Set the **Body** as **Your order is being processed**

1.  Click **Save**

![Action properties are completed and the 'Save' button is highlighted](./images/xx_logicapp_05.png "Complete the action properties")

1.  Click on the **When a HTTP request is received** action, copy the **HTTP POST URL** for the logic app and save it for the next task

![The http action trigger is expanded and the url is highlighted.](./images/xx_logicapp_06.png "Copy the function url trigger endpoint")

### Task 2: Configure the function app settings

1.  Open the Azure Portal to your resource group and select the Function App in your resource group, it should be named **s2func...**

1.  Click **Configuration**

1.  Add or update the **LogicAppUrl** configuration variable to the Logic App http endpoint you recorded above

1.  Click **Save**

### Task 3: Test order email delivery

1.  Switch to Visual Studio, right-click the **DataGenerator** project, select **Set as startup project**

1.  Press **F5** to run the project

1.  For each `buy` event, you should receive an email

>NOTE:  You could receive quite a `few` emails.

## After the hands-on lab 

Duration: 10 minutes

In this exercise, attendees will deprovision any Azure resources that were created in support of the lab.

### Task 1: Delete resource group

1.  Using the Azure portal, navigate to the Resource group you used throughout this hands-on lab by selecting **Resource groups** in the menu.

2.  Search for the name of your research group, and select it from the list.

3.  Select **Delete** in the command bar, and confirm the deletion by re-typing the Resource group name and selecting **Delete**.

You should follow all steps provided *after* attending the Hands-on lab.

