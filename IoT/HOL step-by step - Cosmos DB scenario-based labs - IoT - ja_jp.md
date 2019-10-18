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
  - [Exercise 9: Run the predictive maintenance batch scoring](#exercise-9-run-the-predictive-maintenance-batch-scoring)
    - [Task 1: Import lab notebooks into Azure Databricks](#task-1-import-lab-notebooks-into-azure-databricks)
    - [Task 2: Run batch scoring notebook](#task-2-run-batch-scoring-notebook)
  - [Exercise 10: Deploy the predictive maintenance web service](#exercise-10-deploy-the-predictive-maintenance-web-service)
    - [Task 1: Run deployment notebook](#task-1-run-deployment-notebook)
    - [Task 2: Call the deployed scoring web service from the Web App](#task-2-call-the-deployed-scoring-web-service-from-the-web-app)
  - [Exercise 11: Create the Predictive Maintenance & Trip/Consignment Status reports in Power BI](#exercise-11-create-the-predictive-maintenance--tripconsignment-status-reports-in-power-bi)
    - [Task 1: Import report in Power BI Desktop](#task-1-import-report-in-power-bi-desktop)
    - [Task 2: Update report data sources](#task-2-update-report-data-sources)
    - [Task 3: Explore report](#task-3-explore-report)
  - [After the hands-on lab](#after-the-hands-on-lab)
    - [Task 1: Delete the resource group](#task-1-delete-the-resource-group)

<!-- /TOC -->
</details>

## Overview

Contoso Auto は、車両とパッケージのテレメトリ データを収集し、Azure Cosmos DB を使用してこのデータを迅速に取り込んで未加工の形式で保存し、ほぼリアルタイムで処理を行い、サポートする洞察を生成する価値の高い貨物ロジスティクス組織です。いくつかのビジネス目標を達成し、組織内で最も適切なユーザー コミュニティに表示します。急成長を遂げている組織であり、選択したテクノロジーの関連コストを拡張および管理し、その爆発的な成長と物流ビジネスの固有の季節性に対応できるようにしたいと考えています。このシナリオには、トラック輸送と貨物センシング データの包含に重点を置いて、車両テレメトリとロジスティクスのユース ケースの両方に適用可能が含まれます。これにより、多くの代表的な顧客分析シナリオが可能になります。

技術の観点から、Contoso は、ホット データ パスのコア リポジトリとして Azure Cosmos DB を活用し、Azure Cosmos DB 変更フィードを活用して、Contoso を可能にする堅牢で堅牢なイベント ソーシング アーキテクチャを推進したいと考えています。開発者は、ソリューションを迅速に強化します。これは、アプリケーション (データベース) 内の状態の変更を反映する変更フィードによって公開されたイベントを活用することで、堅牢でアジャイルなサーバーレスアプローチを使用して実現しました。

最終的に Contoso は、3 つのロールのいずれかで、生のインサイト データと派生したインサイト データをユーザーに表示します。:

- **物流業務担当者** 車両や貨物ロジスティクスの現状に興味があり、Webアプリを使用して単一の車両や貨物の状態を迅速に把握する人は、アラートの通知だけでなく、車両や貨物のメタデータをシステムにロードします。ダッシュボードで見たいのは、エンジンの過熱、異常な油圧、アグレッシブな運転など、検出された異常のさまざまな視覚化です。

- **管理および顧客報告担当者** 処理後に流れ込む新しいデータで自動的に更新される Power BI レポートに表示される車両の車両の現在の状態と顧客の委託レベル情報を確認する立場に配置する必要があるユーザー。彼らが見たいのは、ドライバーによる悪い運転行動に関するレポートと、都市や地域に関連する異常を表示するマップなどの視覚コンポーネントを使用するほか、総艦隊と委託情報を明確に示すさまざまなチャートやグラフです。

このエクスペリエンスでは、Cosmos DB、Azure Functions、Event Hub、Azure Data Bricks、Azure Storage、Azure Stream Analytics、Power BI、Azure Web Apps、Logic Apps上に構築されたほぼリアルタイムな分析パイプラインへのエントリーポイントとなる、車両のストリーミングテレメトリーデータを取り込むためにAzure Cosmos DBを利用します。

## Solution architecture

この実習ラボで構築するソリューション アーキテクチャの図を次に示します。さまざまなコンポーネントに取り組んでいる場合は、ソリューション全体を理解できるように、慎重に検討してください。

![A diagram showing the components of the solution is displayed.](media/solution-architecture.png 'Solution Architecture')

- データの取り込み、イベント処理、およびストレージ::

  IoT シナリオのソリューションは、イベント データ、フリート、委託、パッケージ、およびトリップ メタデータ、およびレポート用の集計データをストリーミングするための、グローバルに利用可能でスケーラブルなデータ ストレージとして機能する **Cosmos DB** を中心に展開します。車両テレメトリ データは、**IoT Hub** に登録された IoT デバイスを介してデータ ジェネレータから流れ込み、**Azure Functions** がイベント データを処理し、Cosmos DB のテレメトリ コンテナーに挿入します。

- Azure Functionsを使用したトリップ処理:

  Cosmos DB の change feedは、3 つの別々の Azure Functionsをトリガーし、それぞれが独自のチェックポイントを管理し、互いに競合することなく同じ着信データを処理できるようにします。1 つのFunctionは、イベント データをシリアル化し、**Azure Storage** のタイム スライスされたフォルダーに格納して、生データを長期保存します。別のFunctionは、車両テレメトリを処理し、バッチ データを集約し、走行距離計の読み取り値とトリップがスケジュール通りに実行されているかどうかに基づいて、メタデータ コンテナー内のトリップおよび委託ステータスを更新します。この機能は、旅行のマイルストーンに達したときに電子メールアラートを送信する **Logic App** をトリガします。3 番目のFunctionはイベント データを **Event Hubs** に送信し、**Stream Analytics** をトリガーしてタイム ウィンドウの集約クエリを実行します。

- ストリーム処理、ダッシュボード、およびレポート:

  Stream Analytics は、Cosmos DB メタデータ コンテナーに車両固有の集計を出力し、車両全体の集約を **Power BI** に出力して、車両のステータス情報のリアルタイム ダッシュボードに入力します。Power BI Desktop レポートは、Cosmos DB メタデータ コンテナーから直接プルされた詳細な車両、旅行、および委託情報を表示するために使用されます。また、メンテナンスコンテナから引き出されたバッチバッテリ故障予測も表示されます。

- 高度な分析と ML モデルのトレーニング:

  **Azure Databricks** は、過去の情報に基づいて、車両のバッテリの故障を予測する機械学習モデルをトレーニングするために使用されます。バッチ予測用にトレーニングされたモデルをローカルに保存し、リアルタイム予測のために **Azure Kubernetes Service (AKS)** または **Azure Container Instances (ACI)** にモデルとスコアリング Web サービスをデプロイします。また、Azure Databricks は **Spark Cosmos DB connector** を使用して、毎日の出張情報をプルダウンして、バッテリ障害のバッチ予測を行い、予測をメンテナンス コンテナーに格納します。

- フリート管理 Web App、セキュリティ、および監視:

  **Web App** を使用すると、Contoso Auto は車両を管理し、Cosmos DB に保存されている委託、パッケージ、およびトリップ情報を表示できます。Web Appは、車両情報を表示しながら、リアルタイムのバッテリ故障予測を行うためにも使用されます。**Azure Key Vault** は、接続文字列やアクセス キーなどの一元化されたアプリケーション シークレットを安全に格納するために使用され、Function Apps、Web App、Azure Databricksで使用されます。最後に、**Application Insights** は、Function Appsと Web Appのリアルタイムの監視、メトリック、およびログ情報を提供します。

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

ソリューションの開発を開始する前に、Azure でいくつかのリソースをプロビジョニングする必要があります。クリーンアップを容易にするために、すべてのリソースが同じリソース グループを使用していることを確認します。

この実習では、生成された車両、委託、パッケージ、および出張データの送信と処理を開始できるように、ラボ環境を構成します。まず、Cosmos DB データベースとコンテナーを作成し、新しいLogic Appを作成し、電子メール通知を送信するワークフローを作成し、ソリューションをリアルタイム監視するための Application Insights サービスを作成してから、ソリューションのアプリケーション設定 (接続文字列など)で使用されるシークレットを取得し、それらを Azure Key Vault に安全に保存し、最終的に Azure Databricks 環境を構成します。

### Task 1: Create Cosmos DB database and container

このタスクでは、Cosmos DB データベースと 3 つの SQL-ベースのコンテナーを作成します:

- **telemetry**: 90 日間の寿命 (TTL) を持つホット 車両テレメトリ データの取り入れに使用されます。
- **metadata**: 車両、委託、パッケージ、出張、および集計イベント データを格納します。
- **maintenance**: バッチ バッテリ障害予測は、レポートの目的でここに格納されます。

1. ブラウザの新しいタブまたはインスタンスを使用して、Azure ポータル <https://portal.azure.com>に移動します。

2. 左側のメニューから**リソースグループ**を選択し、`cosmos-db-iot`と入力してリソースグループを検索します。この演習で使用しているリソース グループを選択します。

   ![Resource groups is selected and the cosmos-db-iot resource group is displayed in the search results.](media/resource-group.png 'cosmos-db-iot resource group')

3. Azure Cosmos DB アカウントを選択します。名前は `cosmos-db-iot` で始まります。

   ![The Cosmos DB account is highlighted in the resource group.](media/resource-group-cosmos-db.png 'Cosmos DB in the Resource Group')

4. 左側のメニューで **データ エクスプローラ** を選択し、**新しいコンテナー** を選択します。

   ![The Cosmos DB Data Explorer is shown with the New Container button highlighted.](media/cosmos-new-container.png 'Data Explorer - New Container')

5. **コンテナーの追加** ブレードで、次の構成オプションを指定します:

   a. **Database id** に **ContosoAuto** を入力します。

   b. **Provision database throughput** はチェックしないままにしておきます。

   c. **Container id** に **metadata** を入力します。

   d. Partition key: **/partitionKey**

   e. Throughput: **50000**

   ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-metadata.png 'New metadata container')

   > **注**: データ ジェネレータは最初に実行時にメタデータの一括挿入を実行するため、最初にこのコンテナーのスループットを `50000 RU/s` に設定します。データを挿入した後、プログラム的にスループットを `15000` に減らします。

6. **OK** を選択してコンテナーを作成します。

7. **New Container** をデータエクスプローラーで再度選択します。

8. **コンテナーの追加** ブレードで、次の構成オプションを指定します:

   a. **Database id**: **Use existing** を選択しリストから **ContosoAuto** を選択します。

   c. **Container id** に **telemetry** を入力します。

   d. Partition key: **/partitionKey**

   e. Throughput: **15000**

   ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-telemetry.png 'New telemetry container')

9. **OK** を選択してコンテナーを作成します。

10. **New Container** をデータエクスプローラーで再度選択します。

11. **コンテナーの追加** ブレードで、次の構成オプションを指定します:

    a. **Database id**: **Use existing** を選択しリストから **ContosoAuto** を選択します。

    c. **Container id** に **maintenance** を入力します。

    d. Partition key: **/vin**

    e. Throughput: **400**

    ![The New Container form is displayed with the previously described values.](media/cosmos-new-container-maintenance.png 'New maintenance container')

12. **OK** を選択してコンテナーを作成します。

13. これでデータエクスプローラーで3つのコンテナがリストされているはずです。

    ![The three new containers are shown in Data Explorer.](media/cosmos-three-containers.png 'Data Explorer')

#### About Cosmos DB throughput

予想されるイベント処理およびレポートワークロードに基づいて、各コンテナーの RU/s に **throughput** を意図的に設定していることに気付くでしょう。Azure Cosmos DB では、プロビジョニングされたスループットは要求単位/秒 (RUs) として表されます。RUs は、Cosmos DB コンテナーに対する読み取り操作と書き込み操作の両方のコストを測定します。Cosmos DB は透過的な水平方向のスケーリング (スケール アウトなど) とマルチマスター レプリケーションを使用して設計されているため、単一の API 呼び出しを使用して取得できる、1 秒あたり数千から数億の要求を処理するRUsの数を非常に迅速かつ簡単に増減できます。

Cosmos DB を使用すると、データベース レベルまたはコンテナー レベルで 100 の小さな単位で RUs を増分/減衰できます。スループットは、SLA に裏付けされたコンテナーのパフォーマンスを常に保証するために、コンテナーの粒度で構成することをお勧めします。Cosmos DB が提供するその他の保証は、世界中で 99.999% の読み取りと書き込みの可用性であり、読み取りと書き込みは 99 パーセンタイルで 10 ミリ秒未満で提供されます。

コンテナーに多数の RUs を設定すると、Cosmos DB は、それらの RUs が Cosmos DB アカウントに関連付けられているすべてのリージョンで使用できることを確認します。新しいリージョンを追加してリージョンの数をスケールアウトすると、Cosmosは新しく追加されたリージョンに同じ量の RUs を自動的にプロビジョニングします。特定のリージョンに異なる RUs を選択的に割り当てることはできません。これらの RUs は、関連するすべてのリージョンのコンテナー (またはデータベース) 用にプロビジョニングされます。

#### About Cosmos DB partitioning

各コンテナーを作成するときは、**パーティションキー** を定義する必要がありました。ソリューション ソース コードを確認するときに、後で説明するように、コレクション内に格納されている各ドキュメントには `partitionKey` プロパティが含まれています。新しいコンテナーを作成する際に行う必要がある最も重要な決定の 1 つは、データに適したパーティション キーを選択することです。パーティション キーは、ストレージとパフォーマンスのボトルネックを回避するために、任意の時点でストレージとスループットの均等な分散 (1 秒あたりの要求で測定) を提供する必要があります。たとえば、車両メタデータは、各車両の一意の値である VIN を `partitionKey` フィールドに格納します。トリップ メタデータは、ほとんどの場合 VIN によって照会され、トリップ ドキュメントは車両メタデータと同じ論理区画に格納されるため、トリップ メタデータは一緒にクエリされ、ファンアウトならびにパーティション間クエリを防ぐため、`partitionKey` フィールドにも VIN を使用します。一方、パッケージ メタデータは、同じ目的で `partitionKey` フィールドにConsignment ID 値を使用します。パーティション キーは、多数のパーティション間で過剰なファンアウトを避けるために、読み取りが多いシナリオのクエリの大部分に存在する必要があります。これは、特定のパーティション キー値を持つ各ドキュメントが同じ論理区画に属し、同じ物理パーティションに格納され、同じ物理パーティションからも処理されるためです。各物理パーティションは地理的リージョン間でレプリケートされるため、グローバルな分散が実現します。

Cosmos DB に適切なパーティション キーを選択することは、バランスの取れた読み取りと書き込み、スケーリング、およびこのソリューションの場合は、パーティションごとの順序によるchange feed処理を確実に行うための重要な手順です。論理区画の数に制限はありませんが、1 つの論理区画に対して 10 GB のストレージの上限が許可されます。論理区画を物理パーティション間で分割することはできません。同じ理由で、選択したパーティション キーのカーディナリティ（基数）が悪い場合は、ストレージの分散が歪んでいる可能性があります。たとえば、1 つの論理区画が他のパーティションよりも高速になり、最大 10 GB の上限に達した場合、他の論理パーティションがほぼ空になる場合、最大論理区画を収容する物理パーティションは分割できず、アプリケーションのダウンタイムが発生する可能性があります。

### Task 2: Configure Cosmos DB container indexing and TTL

このタスクでは、新しいコンテナーの既定のインデックス セットを確認し、書き込み負荷の高いワークロードに最適化するように `telemetry` コンテナーのインデックス作成を構成します。次に、`telemetry` コンテナーで存続時間 (TTL) を有効にし、コンテナーに格納されている個々のドキュメントに TTL 値を秒単位で設定できるようにします。この値は、ドキュメントの有効期限が切れるか、削除するか、または自動的に削除するタイミングを Cosmos DB に指示します。この設定は、不要になったものを削除することで、ストレージコストを節約するのに役立ちます。通常、これはホット データ、または規制要件により一定期間後に期限切れにする必要があるデータで使用されます。

1. Cosmos DBのデータエクスプローラーで **telemetry** コンテナを開き、**Scale & Settings** を選択します。

2. Scale & Settings ブレードで、Settings を開き、**Time to Live** にある **On (no default)** を選択します。

   ![The Time to Live settings are set to On with no default.](media/cosmos-ttl-on.png 'Scale & Settings')

   存続時間設定を既定値以外でオンにすると、ドキュメントごとに個別に TTL を定義できるため、一定期間後に期限切れになるドキュメントをより柔軟に決定できます。これを行うには、このコンテナーに保存される `ttl` フィールドがあり、TTL を秒単位で指定します。

3. Scale & Settings ブレードを下にスクロールして、**インデックス作成ポリシー** を表示します。既定のポリシーでは、コレクションに格納されている各ドキュメントのすべてのフィールドに自動的にインデックスを作成します。これは、すべてのパスが含まれている (JSON ドキュメントを格納しているため、ドキュメント内の子コレクション内に存在できるため、パスを使用してプロパティを識別します)、つまり `includedPaths` の値が `"path": "/*"` に設定されており、除外される唯一のパスはドキュメントのバージョン管理に使用される内部的な `_etag` プロパティであるからです。既定のインデックス ポリシーは次のようになります:

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

4. **Indexing Policy** を次のように置き換えると、すべてのパスを除外し、コンテナーを照会するときに使用されるパスのみが含まれます (`vin`, `state`, および `partitionKey`):

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

5. **Save** を選択し変更を適用します。

#### About the Cosmos DB indexing policies

このタスクでは、`telemetry` コンテナーのインデックス作成ポリシーを更新しましたが、他の 2 つのコンテナーは既定のポリシーのままにしておきました。新しく作成されたコンテナーの既定のインデックス 作成ポリシーは、すべての項目のすべてのプロパティにインデックスを付け、任意の文字列または数値の範囲インデックスを適用し、Point 型の GeoJSON オブジェクトに空間インデックスを適用します。これにより、インデックス作成とインデックス管理を事前に考えることなく、高いクエリ パフォーマンスを得ることができます。`metadata` コンテナーと `maintenance` コンテナーは `telemetry` よりも読み取り負荷の高いワークロードを持つため、クエリのパフォーマンスが最適化される既定のインデックス 作成ポリシーを使用するのが理にかなっています。`telemetry` の高速な書き込みが必要なため、未使用のパスは除外します。インデックス作成パスを使用すると、インデックス作成コストがインデックス付けされた一意のパスの数と直接相関するため、クエリ パターンが事前にわかっているシナリオでは、書き込みパフォーマンスが向上し、インデックスストレージが低下する可能性があります。

3 つのコンテナーすべてに対するインデックス作成モードは **Consistent** に設定されます。つまり、アイテムの追加、更新、または削除に伴ってインデックスが同期的に更新され、読み取りクエリ用にアカウントに構成された整合性レベルが適用されます。もう 1 つのインデックス作成モードは None で、コンテナーのインデックス作成を無効にします。通常、このモードは、コンテナーが純粋なキー値ストアとして機能し、他のプロパティのインデックスを必要としない場合に使用されます。一括操作を実行する前に整合性モードを動的に変更し、その後モードを Consistent に戻すことができます。

### Task 3: Create a Logic App workflow for email alerts

このタスクでは、新しいLogic App ワークフローを作成し、HTTP トリガーを介して電子メール アラートを送信するように構成します。このトリガーは、Cosmos DB change feed によってトリガーされる Azure Functions の 1 つ (トリップの完了などの通知イベントが発生するたびに呼び出されます) によって呼び出されます。電子メールを送信するには、Office 365 またはOutlook.com アカウントが必要です。

1. [Azure portal](https://portal.azure.com)で, **+ Create a resource** を選択し、上部にある検索ボックスに **logic app** と入力します。結果から **Logic App** を選択します。

   ![The Create a resource button and search box are highlighted in the Azure portal.](media/portal-new-logic-app.png 'Azure portal')

2. **Logic App overview** ブレード上で、**Create** ボタンを選択します。

3. **Create Logic App** ブレードで以下の設定オプションを指定します:

   1. **Name**: Unique value for the name, such as `Cosmos-IoT-Logic` (ensure the green check mark appears).
   2. **Subscription**: Select the Azure subscription you are using for this lab.
   3. **Resource group**: Select your lab resource group. The name should start with `cosmos-db-iot`.
   4. **Location**: Select the same location as your resource group.
   5. **Log Analytics**: Select **Off**.

   ![The form is displayed with the previously described values.](media/portal-new-logic-app-form.png 'New Logic App')

4. **Create** を選択します。

5. Logic App が作成されたら、リソースグループを開いて新しいLogic Appを選択して Logic Appに行きます。

6. Logic App デザイナーで、Start with a common triggerセクションまでページをスクロールし、 **When a HTTP request is received** トリガーを選択します。

   ![The HTTP common trigger option is highlighted.](media/logic-app-http-trigger.png 'Logic App Designer')

7. **Request Body JSON Schema** フィールドに以下のJSONをペーストします。アラートが送信される必要がある時に、Azure Functionが送信するHTTPリクエストのボディのデータの形式を定義します:

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

8. HTTPトリガーの付近にある **+ New step** を選択します。

   ![The new step button is highlighted.](media/logic-app-new-step.png 'New step')

9. 新しいアクションの中の検索ボックスに、`send email` をタイプして、下にあるアクションのリストの中から **Send an email - Office 365 Outlook** を選択します。**注**: Office 365 Outlook アカウントが無い場合、他のメールサービスのオプションを試してください。

   ![Send email is typed in the search box and Send an email - Office 365 Outlook is highlighted below.](media/logic-app-send-email.png 'Choose an action')

10. **Sign in** ボタンを選択します。表示されたウインドウで、アカウントにサインインします。

    ![The Sign in button is highlighted.](media/logic-app-sign-in-button.png 'Office 365 Outlook')

11. サインインしたら、アクションボックスが **Send an email** アクションフォームとして表示されます。**To** フィールドを選択します。Toを選択すると、**Dynamic content** ボックスが表示されます。HTTPリクエストトリガーからの動的な値の完全なリストを見るには、"When a HTTP request is received"の次の **See more** を選択します。

    ![The To field is selected, and the See more link is highlighted in the Dynamic content window.](media/logic-app-dynamic-content-see-more.png 'Dynamic content')

12. 動的なコンテンツのリストから **recipientEmail** を選択します。**To** フィールドに動的な値が追加されるでしょう。

    ![The recipientEmail dynamic value is added to the To field.](media/logic-app-recipientemail.png 'Dynamic content - recipientEmail')

13. **Subject** フィールドに以下を入力します: `Contoso Auto trip status update:`。最後にスペースが入力されていることを確認してください。 **status** 動的コンテンツが件名の最後にトリップのステータスを追加することを確認してください。

    ![The Subject field is filled in with the status dynamic content appended to the end.](media/logic-app-status.png 'Dynamic content - status')

14. **Body** フィールドに以下をペーストします。動的なコンテンツが自動的に追加されます:

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

15. ここまででLogic Appのワークフローは以下のようになっているはずです:

    ![The Logic App workflow is complete.](media/logic-app-completed-workflow.png 'Logic App')

16. デザイナーの上部にある **Save** を選択してワークフローを保存します。

17. 保存後、HTTPトリガーのURLが生成されます。ワークフローのHTTPトリガーを展開し、**HTTP POST URL** の値をコピーしNotepadか同様のテキストアプリケーションに、あとの手順で使うためにペーストしておきます。

    ![The http post URL is highlighted.](media/logic-app-url.png 'Logic App')

### Task 4: Create system-assigned managed identities for your Function Apps and Web App to connect to Key Vault

Function Appと Web Appsが Key Vault にアクセスしてシークレットを読み取ることができるようにするには、[システムで割り当てられたマネージドの認証を作成](https://docs.microsoft.com/azure/app-service/overview-managed-identity#adding-a-system-assigned-identity)する必要があり、および [Key Vault でアクセス ポリシーを作成](https://docs.microsoft.com/azure/key-vault/key-vault/key-vault/key-vault-key-vault)する必要があります。

1. 名前が **IoT-CosmosDBProcessing** で始まるAzure Function Appを開いて、 **Platform features** を表示します。

2. **Identity** を選択します。

   ![Identity is highlighted in the platform features tab.](media/function-app-platform-features-identity.png 'Platform features')

3. **System assigned** タブで、**Status** を **On** に変更し、 **Save** を選択します。

   ![The Function App Identity value is set to On.](media/function-app-identity.png 'Identity')

4. 名前が **IoT-StreamProcessing** で始まるAzure Function Appを開いて、 **Platform features** を表示します。

5. **Identity** を選択します。

   ![Identity is highlighted in the platform features tab.](media/function-app-platform-features-identity.png 'Platform features')

6. **System assigned** タブで、**Status** を **On** に変更し、 **Save** を選択します。

   ![The Function App Identity value is set to On.](media/function-app-identity.png 'Identity')

7. **IoTWebApp** という名前のWeb App (App Service) を開きます。

8. 左のメニューの **Identity** を選択します。

9. **System assigned** タブで、**Status** を **On** に変更し、 **Save** を選択します。

   ![The Web App Identity value is set to On.](media/web-app-identity.png 'Identity')

### Task 5: Add Function Apps and Web App to Key Vault access policy

> この手順と次の Key Vault タスク用に、ブラウザのタブを2つ開いたままにしておくことをお勧めします。1 つのタブを使用して各 Azure サービスのシークレットをコピーし、もう 1 つで Key Vault にシークレットを追加します。

次の手順を実行して、"Get" シークレットアクセス許可を有効にするアクセス ポリシーを作成します:

1. ブラウザの新しいタブまたはインスタンスを使用して、Azure ポータル <https://portal.azure.com>に移動します。

2. 左側のメニューから **リソースグループ** を選択し、`cosmos-db-iot`と入力してリソースグループを検索します。この演習で使用しているリソース グループを選択します。

3. **Key Vault** を開きます。名前は`iot-vault`で始まるはずです。

   ![The Key Vault is highlighted in the resource group.](media/resource-group-keyvault.png 'Resource group')

4. 左側の **Access policies** を選択します。

5. **+ Add Access Policy** を選択します。

   ![The Add Access Policy link is highlighted.](media/key-vault-add-access-policy.png 'Access policies')

6. Addアクセスポリシーフォームの **Select principal** セクションを選択します。

   ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

7. Principalブレードで、`IoT-CosmosDBProcessing` Function App のサービスプリンシパルを検索し、それを選択後、 **Select** ボタンを押します。

   ![The Function App's principal is selected.](media/key-vault-principal-function1.png 'Principal')

   > **注**: 前の手順でマネージ ID を追加した後、マネージ ID が表示されるまでにしばらく時間がかかる場合があります。この ID やその他の ID が見つからない場合は、ページを更新するか、1 ~ 2 分待ちます。

8. **Secret permissions** を開き、Secret Management Operationsにある **Get** をチェックします。

   ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

9. **Add** を選択して、新しいアクセスポリシーを追加します。

10. 完了すると、Function App'のマネージ ID用のアクセスポリシーがあるはずです。**+ Add Access Policy** を選択肢、別のアクセスポリシーを追加します。

    ![Key Vault access policies.](media/key-vault-access-policies-function1.png 'Access policies')

11. Addアクセスポリシーフォームの **Select principal** セクションを選択します。

    ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

12. Principalブレードで、`IoT-StreamProcessing` Function App のサービスプリンシパルを検索し、それを選択後、 **Select** ボタンを押します。

    ![The Function App's principal is selected.](media/key-vault-principal-function2.png 'Principal')

13. **Secret permissions** を開き、Secret Management Operationsにある **Get** をチェックします。

    ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

14. **Add** を選択して、新しいアクセスポリシーを追加します。

15. 完了すると、Function App'のマネージ ID用のアクセスポリシーがあるはずです。**+ Add Access Policy** を選択肢、別のアクセスポリシーを追加します。

    ![Key Vault access policies.](media/key-vault-access-policies-function2.png 'Access policies')

16. Addアクセスポリシーフォームの **Select principal** セクションを選択します。

    ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

17. Principalブレードで、`IoTWebApp` Web App のサービスプリンシパルを検索し、それを選択後、 **Select** ボタンを押します。

    ![The Web App's principal is selected.](media/key-vault-principal-webapp.png 'Principal')

18. **Secret permissions** を開き、Secret Management Operationsにある **Get** をチェックします。

    ![The Get checkbox is checked under the Secret permissions dropdown.](media/key-vault-get-secret-policy.png 'Add access policy')

19. **Add** を選択して、新しいアクセスポリシーを追加します。

20. 完了すると、Web App'のマネージ ID用のアクセスポリシーがあるはずです。**Save** を選択し、新しいアクセスポリシーを保存します。

    ![Key Vault access policies.](media/key-vault-access-policies-webapp.png 'Access policies')

### Task 6: Add your user account to Key Vault access policy

次の手順を実行して、シークレットを管理できるように、ユーザー アカウントのアクセス ポリシーを作成します。テンプレートを使用して Key Vault を作成したので、アカウントがアクセス ポリシーに自動的に追加されるわけではありません。

1. Key Vaultで、左側のメニューから **Access policies** を選択します。

2. **+ Add Access Policy** を選択します。

   ![The Add Access Policy link is highlighted.](media/key-vault-add-access-policy.png 'Access policies')

3. Addアクセスポリシーフォームの **Select principal** セクションを選択します。

   ![Select principal is highlighted.](media/key-vault-add-access-policy-select-principal.png 'Add access policy')

4. Principalブレードで、この演習で利用しているAzureアカウントを検索し、それを選択後、 **Select** ボタンを押します。

   ![The user principal is selected.](media/key-vault-principal-user.png 'Principal')

5. **Secret permissions** を開き、Secret Management Operationsにある **Select all** をチェックします。全てで8つが選択されるはずです。

   ![The Select all checkbox is checked under the Secret permissions dropdown.](media/key-vault-all-secret-policy.png 'Add access policy')

6. **Add** を選択して新しいアクセスポリシーを追加します。完了後、ユーザアカウント用のアクセスポリシーがあるはずです。**Save** を選択して新しいアクセスポリシーを保存します。

    ![Key Vault access policies.](media/key-vault-access-policies-user.png 'Access policies')

### Task 7: Add Key Vault secrets

Azure Key Vault は、トークン、パスワード、証明書、API キー、およびその他のシークレットへのアクセスを安全に保存し、厳密に制御するために使用されます。さらに、Azure Key Vault に格納されているシークレットは一元化されるため、セキュリティ目的でキーをリサイクルした後のアプリケーション キー値など、シークレットを 1 か所で更新するだけで済むという利点が追加されます。このタスクでは、アプリケーション シークレットを Azure Key Vault に格納し、次の手順を実行して Azure Key Vault に安全に接続するようにFunction Appと Web Appを構成します。

- プロビジョニング済みのKey Vaultにシークレットを追加する。
- Azure Function AppとWeb Appがvaultから読み出せるようにシステム割り当て済みのマネージIDを作成する。
- これらのアプリケーションのIDに割り当てられる、"Get" シークレットパーミッション付きでKey Vaultでアクセスポリシーを作成する。

1. Key Vaultで、左側のメニューから **Secrets** を選択し、**+ Generate/Import** を選択して新しいシークレットを作成します。

   ![The Secrets menu item is highlighted, and the Generate/Import button is selected.](media/key-vault-secrets-generate.png 'Key Vault Secrets')

2. 以下の表を使って、Name / Valueのペアでシークレットを作成します。各シークレットで必要なのは **Name** と **Value** フィールのみで、他のフィールドは規定値のままにしておきます。

   | **Name**            |                                                                          **Value**                                                                          |
   | ------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
   | CosmosDBConnection  |                            Your Cosmos DB connection string found here: **Cosmos DB account > Keys > Primary Connection String**                            |
   | CosmosDBEndpoint    |                                           Your Cosmos DB endpoint found here: **Cosmos DB account > Keys > URI**                                            |
   | CosmosDBPrimaryKey  |                                      Your Cosmos DB primary key found here: **Cosmos DB account > Keys > Primary Key**                                      |
   | IoTHubConnection    |                         Your IoT Hub connection string found here: **IoT Hub > Built-in endpoints > Event Hub-compatible endpoint**                         |
   | ColdStorageAccount  |  Connection string to the Azure Storage account whose name starts with `iotstore`, found here: **Storage account > Access keys > key1 Connection string**   |
   | EventHubsConnection | Your Event Hubs connection string found here: **Event Hubs namespace > Shared access policies > RootManageSharedAccessKey > Connection string-primary key** |
   | LogicAppUrl         |                         Your Logic App's HTTP Post URL found here: **Logic App Designer > Select the HTTP trigger > HTTP POST URL**                         |

3. デプロイメントの出力を表示することで、ほとんどのシークレットを見つけることができます。これを行うには、リソース グループを開き、左側のメニューで **デプロイメント** を選択します。**Microsoft.Template** デプロイメントを選択します。

    ![The resource group deployments blade is shown.](media/resource-group-deployments.png "Deployments")

4. 左側のメニューから **Outputs** を選択します。上の値がほとんど全て見つけられるので、単にそれらをコピーします。

    ![The outputs are displayed.](media/resource-group-deployment-outputs.png "Outputs")

5. シークレットの作成が完了したら、リストは以下のようになるはずです:

   ![The list of secrets is displayed.](media/key-vault-keys.png 'Key Vault Secrets')

### Task 8: Create Azure Databricks cluster

Contoso Auto は、車両から収集した貴重なデータを使用して、メンテナンス関連の問題によるダウンタイムを短縮するために、車両の正常性を予測したいと考えています。彼らが行いたい予測の1つは、過去のデータに基づいて、車両のバッテリーが今後30日以内に故障する可能性があるかどうかです。彼らは、これらの予測に基づいて、サービスを提供する必要がある車両を識別するために、毎晩バッチプロセスを実行したいと考えています。また、車両をフリート管理 Web サイトで表示する際に、リアルタイムで予測を行う方法も望んでいます。

この要件をサポートするには、Azure 上で実行するように最適化された完全に管理された Apache Spark プラットフォームである Azure Databricks で Apache Spark を使用します。Spark は、データ サイエンティストとデータ エンジニアが大量の構造化データと非構造化データを探索して準備し、そのデータを使用して機械学習モデルを大規模にトレーニング、使用、および展開できるようにする、統合されたビッグ データと高度な分析プラットフォームです。`azure-cosmosdb-spark` コネクタ (<https://github.com/Azure/azure-cosmosdb-spark>)を使用して、Cosmos DB に読み書きをします。

このタスクでは、後の演習でデータ探索タスクとモデル展開タスクを実行する新しいクラスターを作成します。

1. [Azure portal](https://portal.azure.com)で、この演習のリソースグループを開き、**Azure Databricks Service** を開きます。名前は `iot-databricks` で始まるはずです。

   ![The Azure Databricks Service is highlighted in the resource group.](media/resource-group-databricks.png 'Resource Group')

2. **Launch Workspace** を選択します。Azure Databricks はAzure Active Directoryが統合されているので、自動的にサインインできます。

   ![Launch Workspace](media/databricks-launch-workspace.png 'Launch Workspace')

3. ワークスペースで、左側のメニューから **Clusters** を選択し、**+ Create Cluster** を選択します。

   ![Create Cluster is highlighted.](media/databricks-clusters.png 'Clusters')

4. **New Cluster** フォームで以下の設定オプションを指定します:

   1. **Cluster Name**: Enter **lab**.
   2. **Cluster Mode**: Select **Standard**.
   3. **Pool**: Select **None**.
   4. **Databricks Runtime Version**: Select **Runtime 5.5 LTS (Scala 2.11, Spark 2.4.3)**.
   5. **Python Version**: Enter **3**.
   6. **Autopilot Options**: Uncheck **Enable autoscaling** and check **Terminate after...**, with a value of **120** minutes.
   7. **Worker Type**: Select **Standard_DS3_v2**.
   8. **Driver Type**: Select **Same as worker**.
   9. **Workers**: Enter **1**.

   ![The New Cluster form is displayed with the previously described values.](media/databricks-new-cluster.png 'New Cluster')

5. **Create Cluster** を選択します。

6. 次の手順に進む前に、クラスターが動作していることを確認します。ステータスが **Pending** から **Running** に変わるのを待ちます。

7. **lab** クラスターを選択し、**Libraries** を選択します。

8. **Install New** を選択します。

    ![Navigate to the libraries tab and select `Install New`.](media/databricks-new-library.png 'Adding a new library')

9. Install LibraryダイアログでLibrary Sourceとして **Maven** を選択します。

10. Coordinates フィールドで以下を入力します:

    ```text
    com.microsoft.azure:azure-cosmosdb-spark_2.4.0_2.11:1.4.1
    ```

11. **Install** を選択します。

    ![Populated library dialog for Maven.](media/databricks-install-library-cosmos.png 'Add the Maven library')

12. ライブラリのステータスが **Installed** になるまで **待ってから** 次の手順に進みます。

### Task 9: Configure Key Vault-backed Databricks secret store

以前のタスクでは、Cosmos DB 接続文字列など、Key Vault にアプリケーション シークレットを追加しました。このタスクでは、これらのシークレットに安全にアクセスするように、Key Vault がバックアップした Databricks シークレット ストアを構成します。

Azure Databricks には、Key Vault バックアップとDatabricks バックアップの 2 種類のシークレット スコープがあります。これらのシークレット スコープを使用すると、データベース接続文字列などのシークレットを安全に格納できます。誰かがノートブックにシークレットを出力しようとすると、それは `[REDACTED]` に置き換えられます。これにより、ノートブックの表示や共有時に、シークレットを表示したり、誤って漏洩したりするのを防ぐことができます。

1. 別のブラウザタブに表示されたままになっているはずの、[Azure portal](https://portal.azure.com)に戻り、Key Vaultのアカウントに移動し、左側のメニューから **Properties** を選択します。

2. **DNS Name** と **Resource ID** プロパティの値をコピーし、Notepadや他のテキストアプリケーションに、すぐあとで参照するためにペーストしておきます。

   ![Properties is selected on the left-hand menu, and DNS Name and Resource ID are highlighted to show where to copy the values from.](media/key-vault-properties.png 'Key Vault properties')

3. Azure Databricksのワークスペースに戻ります。

4. ブラウザのURLバーで、Azure Databricksのベース URL (例、<https://eastus.azuredatabricks.net#secrets/createScope>)に **#secrets/createScope** を追加します。

5. シークレットスコープの名前に `key-vault-secrets` を入力します。

6. 6. Manage Principal ドロップダウンにある **Creator** を選択し、MANAGEパーミッションを持つシークレットスコープの作成者（あなた）のみを指定します。

   > MANAGE パーミッションがあると、このシークレットスコープを読み書きでき、Azure Databricks Premium Planの場合、スコープのパーミッションを変更することが出来ます。

   > 作成者を選択できるようにするには、アカウントが Azure Databricks Premium Plan に紐付けされている必要があります。
   これは推奨される方法です: シークレットスコープの作成時に作成者に MANAGE パーミッションを付与し、スコープをテストした後、より細かなアクセスパーミッションを割り当てます。

7. Key Vaultの作成手順で以前にコピーした **DNS Name** (例、<https://iot-vault.vault.azure.net/>) と **Resource ID** 例:`/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/cosmos-db-iot/providers/Microsoft.KeyVault/vaults/iot-vault` を入力します。

   ![Create Secret Scope form](media/create-secret-scope.png 'Create Secret Scope')

8. **Create** を選択します。

しばらくすると、シークレットスコープが変更されたことを確認するダイアログが表示されます。

## Exercise 2: Configure windowed queries in Stream Analytics

**Duration**: 15 minutes

ソリューション アーキテクチャ図の右側を調べると、Cosmos DB のフィードトリガー関数から Event Hubs にフィードされるイベント データのフローが表示されます。Stream Analytics は、個々の車両テレメトリの集計を作成する一連のタイム ウィンドウ クエリの入力ソースとして Event Hubs を使用し、車両 IoT デバイスからアーキテクチャを通過する車両テレメトリ全体を作成します。Stream Analytics には、次の 2 つの出力データ シンクがあります:

1. Cosmos DB: 個々の車両テレメトリ (VIN でグループ化) は、30 秒間の `TumblingWindow` で集計され、`metadata` コンテナに保存されます。この情報は、後のタスクで Power BI Desktop で作成する Power BI レポートで使用され、個々の車両と複数の車両統計情報が表示されます。
2. Power BI: すべての車両テレメトリは、10 秒間の `TumblingWindow` で集計され、Power BI データ・セットに出力されます。このほぼリアルタイムなデータは、ライブPower BIダッシュボードに表示され、10秒間に処理されたイベントの数、エンジン温度、オイル、または冷凍ユニットの警告があるかどうか、期間中に乱暴な運転が検出されたかどうか、平均速度、エンジン温度、冷凍ユニットの測定値、が表示されます。。

![The stream processing components of the solution architecture are displayed.](media/solution-architecture-stream-processing.png 'Solution Architecture - Stream Processing')

この演習では、Stream Analyticsを設定して、上述したようなストリーム処理を行います。

### Task 1: Add Stream Analytics Event Hubs input

1. [Azure portal](https://portal.azure.com)で演習のリソースグループを開き、**Stream Analytics job** を開きます。

   ![The Stream Analytics job is highlighted in the resource group.](media/resource-group-stream-analytics.png 'Resource Group')

2. 左側のメニューから **Inputs** を選択します。Inputs ブレードで、**+ Add stream input** を選択し、リストから **Event Hub** を選択します。

   ![The Event Hub input is selected in the Add Stream Input menu.](media/stream-analytics-inputs-add-event-hub.png 'Inputs')

3. **New input** フォームで、以下の設定オプションを指定します:

   1. **Input alias**: Enter **events**.
   2. Select the **Select Event Hub from your subscriptions** option beneath.
   3. **Subscription**: Choose your Azure subscription for this lab.
   4. **Event Hub namespace**: Find and select your Event Hub namespace (eg. `iot-namespace`).
   5. **Event Hub name**: Select **Use existing**, then **reporting**.
   6. **Event Hub policy name**: Choose the default `RootManageSharedAccessKey` policy.

   ![The New Input form is displayed with the previously described values.](media/stream-analytics-new-input.png 'New input')

4. **Save** を選択します。

Event Hubsの入力がリストされているのが見えるはずです。

![The Event Hubs input is listed.](media/stream-analytics-inputs.png 'Inputs')

### Task 2: Add Stream Analytics outputs

1. 左側のメニューから **Outputs** を選択します。Outputs ブレードで、 **+ Add** を選択し、リストから **Cosmos DB** を選択します。

   ![The Cosmos DB output is selected in the Add menu.](media/stream-analytics-outputs-add-cosmos-db.png 'Outputs')

2. **New output** フォームで、以下の設定オプションを指定します:

   1. **Output alias**: Enter **cosmosdb**.
   2. Select the **Select Cosmos DB from your subscriptions** option beneath.
   3. **Subscription**: Choose your Azure subscription for this lab.
   4. **Account id**: Find and select your Cosmos DB account (eg. `cosmos-db-iot`).
   5. **Database**: Select **Use existing**, then **ContosoAuto**.
   6. **Container name**: Enter **metadata**.

   ![The New Output form is displayed with the previously described values.](media/stream-analytics-new-output-cosmos.png 'New output')

3. **Save** を選択します。

4. **このアカウントでまだPower BIにサインインしたことが無い場合**、新しいブラウザのタブを開き、<https://powerbi.com> に移動しサインインします。表示されるメッセージを確認し、ホームページが表示された後、次の手順に進みます。Stream Analyticsからの接続認証手順が成功するのに役立ち、グループワークスペースを探します。

5. Outputs ブレードがまだ表示されているので、**+ Add** を再度選択し、リストから **Power BI** を選択します。

   ![The Power BI output is selected in the Add menu.](media/stream-analytics-outputs-add-power-bi.png 'Outputs')

6. **New output** フォームで下の方を探して、**Authorize connection** セクションを見つけ、**Authorize** を選択してPower BIアカウントにサインインします。もしPower BI アカウントが無い場合、まず _Sign up_ オプションを選択します。

   ![The Authorize connection section is displayed.](media/stream-analytics-authorize-power-bi.png 'Authorize connection')

7. Power BIへの接続が認証されたら、以下の設定オプションを指定します:

   1. **Output alias**: Enter **powerbi**.
   2. **Group workspace**: Select **My workspace**.
   3. **Dataset name**: Enter **Contoso Auto IoT Events**.
   4. **Table name**: Enter **FleetEvents**.

   ![The New Output form is displayed with the previously described values.](media/stream-analytics-new-output-power-bi.png 'New output')

8. **Save** を選択します。

これで2つのoutputがリストされているはずです。

![The two added outputs are listed.](media/stream-analytics-outputs.png 'Outputs')

### Task 3: Create Stream Analytics query

クエリはStream Analyticsの馬車馬です。ここでストリーミング入力を処理し、出力にデータを書き込みます。Stream Analytics クエリ言語は SQL に似ており、使い慣れた構文を使用して、ストリーミング データの探索と変換、集計の作成、および出力シンクに書き込む前にデータ構造を形成するために使用できる具体化されたビューを作成できます。Stream Analytics ジョブは 1 つのクエリしか持ち込めませんが、次の手順で行うように、1 つのクエリで複数の出力に書き込むことができます。

以下のクエリを分析してください。作成した Event Hubs 入力に対して `events` 入力名と `powerbi` と `cosmosDB` 出力をそれぞれ使用していることに注目してください。また、`VehicleData` では30秒、`VehicleDataAll` では10秒の持続時間で `TumblingWindow` を使用する場所も確認できます。`TumblingWindow` は、過去X秒中に発生したイベントを評価し、この場合、レポートの期間にわたって平均を作成するのに役立ちます。

1. 左側のメニューから **Query** を選択します。以下のスクリプトでクエリーウインドウの中身を置き換えます:

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

2. **Save query** を選択します。

### Task 4: Run Stream Analytics job

次に、サービスのフローを開始したイベント データの処理を開始できるように、Stream Analytics ジョブを開始します。

1. **Overview** を選択します。

2. Overview ブレードで、**Start** を選択し、ジョブの出力開始時刻として **Now** を選択します。

3. **Start** を選択し、Stream Analytics ジョブの実行を開始します。

   ![The steps to start the job as described are displayed.](media/stream-analytics-start-job.png 'Start job')

## Exercise 3: Deploy Azure functions and Web App

**Duration**: 30 minutes

このシナリオのアーキテクチャでは、Azure Functions がイベント処理で大きな役割を果たします。これらのFunctionは、クラウド内の小さなコード(関数)を簡単に実行するためのMicrosoftのサーバーレス ソリューションである Azure Function App 内で実行されます。アプリケーション全体やインフラストラクチャを実行する必要なく、問題に必要なコードだけを記述できます。Functions は開発の生産性をさらに高め、C#、F#、Node.js、Java、PHP などの開発言語を使用できます。

この演習に進む前に、Functions と Web App がアーキテクチャにどのように適合するかを見ていきましょう。

ソリューションには 3 つの Function App と 1 つの Web App があります。Function Apps は、データ パイプラインの 2 段階以内でイベント処理を処理し、Web App を使用して Cosmos DB に格納されているデータに対して CRUD 操作を実行します。

![The two Function Apps and Web App are highlighted.](media/solution-diagram-function-apps-web-app.png 'Solution diagram')

Function App に複数の関数が含まれている場合、_なぜ1つではなく2つの Function App が必要なのか?_ と疑問に思うかもしれません。2 つのFunction App を使用する主な理由は、関数が需要を満たすためにどのように拡張されるかによるものです。Azure Functions の消費プランを使用する場合は、コードの実行時間に対してのみ支払います。さらに重要なのは、Azure は需要に応える機能のスケーリングを自動的に処理することです。関数が使用しているトリガーの種類を評価する内部スケール コントローラを使用してスケールし、ヒューリスティックを適用して複数のインスタンスにスケール アウトするタイミングを決定します。知っておくべき重要なことは、Function App レベルで関数がスケーリングすることです。つまり、1 つの非常にビジーな関数があり、残りがほとんどアイドル状態の場合、1 つのビジー関数によって Function App 全体がスケーリングされます。ソリューションを設計する際には、このことを考えてください。**非常に高負荷の関数を別々の Function App** に分割することは良い考えです。

次に、Function App と Web App とそのアーキテクチャへの貢献方法を紹介します。

- **IoT-StreamProcessing Function App**: これはストリーム処理の Function App であり、2つの関数が含まれています。

- **IoTHubTrigger**: この関数は、車両テレメトリがデータ ジェネレータによって送信されるにつれて、IoT Hub の Event Hub エンドポイントによって自動的にトリガーされます。この関数は、パーティションキー値、ドキュメントの TTL、を定義してデータに対して軽い処理を実行し、タイムスタンプ値を追加し、情報を Cosmos DB に保存します。
  - **HealthCheck**: この関数には HTTP トリガーがあり、ユーザーは Function App が起動して実行中であり、各構成設定が存在し、値を持っていることを確認できます。より徹底的なチェックでは、各値が予期される形式に対して、または必要に応じて各サービスに接続することによって検証されます。すべての値にゼロ以外の文字列が含まれている場合、関数は HTTP ステータス `200` (OK) を返します。null または空の値がある場合、関数はエラー (`200`) を返し、どの値が欠落していることを示します。データ ジェネレータは、実行する前にこの関数を呼び出します。

  ![The Event Processing function is shown.](media/solution-architecture-function1.png 'Solution architecture')

- **IoT-CosmosDBProcessing Function App** :これはトリップ処理のFunction App です。これは、`telemetry` コンテナ上のCosmos DB変更フィードによってトリガされる3つの関数が含まれています。Cosmos DB Change Feed は複数のコンシューマーをサポートしているため、これら 3 つの機能を並行して実行し、互いに競合することなく同じ情報を同時に処理できます。これらの各関数に対して `CosmosDBTrigger` を定義する際に、処理した変更フィードイベントを追跡するために、 `leases` という名前のCosmos DBコレクションに接続するトリガ設定を設定します。また、一つの関数が別の関数のリース情報を取得または更新しようとしないように、一意のプレフィックスを持つ各関数の `LeaseCollectionPrefix` 値も設定します。このFunction App には、次の関数があります。

- **TripProcessor**: この関数は、VINによって車両テレメトリデータをグループ化し、 `metadata` コンテナから関連するトリップレコードを取得し、トリップ開始タイムスタンプ、完了した場合は終了タイムスタンプ、およびトリップの有無を示すステータス、トリップが開始された、遅れている、または完了したのいずれか、を更新します。また、関連する委託レコードをステータスで更新し、Function App のアプリ設定で定義された受信者にアラートを電子メールで送信する必要がある場合は、出張情報を含む Logic App をトリガーします (`RecipientEmail`)。
  - **ColdStorage**: この関数は Azure Storage アカウント (`ColdStorageAccount`) に接続し、次のタイム スライスパス形式でコールド ストレージ用の生の車両テレメトリ データを書き込みます: `telemetry/custom/scenario1/yyyy/MM/dd/HH/mm/ss-fffffff.json`。
  - ** SendToEventHubsForReporting** : この関数は、車両テレメトリ データを Event Hubs に直接送信するだけで、Stream Analytics はウィンドウ化された集計を適用し、それらの集計を Power BI および Cosmos DB のメタデータ コンテナーにバッチで保存できます。
  - **HealthCheck**: ストリーム処理 Function App 内の同じ名前の関数と同様に、この関数には HTTP トリガーがあり、Function Appが稼働中であり、各構成設定が存在し、値を持っていることをユーザーが確認できます。データ ジェネレータは、実行する前にこの関数を呼び出します。

  ![The Trip Processing function is shown.](media/solution-architecture-function2.png 'Solution architecture')

- **IoTWebApp**: Web App はフリート管理ポータルを提供し、ユーザーが車両データに対して CRUD 操作を実行し、展開された機械学習モデルに対して車両のリアルタイムバッテリ故障予測を行い、委託、パッケージ、および出張を表示できるようにします。[.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/)を使用して、Cosmos DBの `metadata` コンテナに接続します。

  ![The Web App is shown.](media/solution-architecture-webapp.png 'Solution architecture')

### Task 1: Retrieve the URI for each Key Vault secret

次のタスクで Function App と Web App のアプリ設定を設定する場合は、バージョン番号を含む Key Vault のシークレットの URI を参照する必要があります。これを行うには、シークレットごとに次の手順を実行し、**値** をNotepadまたは同様のテキスト アプリケーションにコピーします。

1. ポータルでKey Vaultのインスタンスを開きます。

2. 左側のメニューの Settings 以下にある **Secrets** を選択します。

3. 取得したいURI値のシークレットを選択します。

4. **Current Version** のシークレットを選択します。

   ![The secret's current version is displayed.](media/key-vault-secret-current-version.png 'Current Version')

5. **Secret Identifier** をコピーします。

   ![The Secret Identifier is highlighted.](media/key-vault-secret-identifier.png 'Secret Identifier')

   Function Appのアプリ設定でこのシークレットへの Key Vault 参照を追加すると、`@Microsoft.KeyVault(SecretUri={referenceString})` という形式を利用することになり、`{referenceString}` は上記のシークレット識別子(URI)の値によって置き換えられます。**中かっこ(`{}`)** を削除してください。

   例えば、完全な参照は以下のようになります:

   `@Microsoft.KeyVault(SecretUri=https://iot-vault-501993860.vault.azure.net/secrets/CosmosDBConnection/794f93084861483d823d37233569561d)`

### Task 2: Configure application settings in Azure

> これらの手順では、2 つのブラウザ タブを開いたままにすることをお勧めします。1 つは各 Azure サービスからシークレットをコピーし、もう 1 つは Key Vault にシークレットを追加します。

1. ブラウザの新しいタブか画面で、Azure portal, <https://portal.azure.com> に移動します。

2. 左側のメニューから **Resource groups** を選択し、`cosmos-db-iot` を入力してリソースグループを探します。この演習で利用しているリソースグループを選択します。

3. **Key Vault** を開きます。名前は `iot-vault` で始まるはずです。

   ![The Key Vault is highlighted in the resource group.](media/resource-group-keyvault.png 'Resource group')

4. 別のブラウザのタブで、名前が **IoT-CosmosDBProcessing** で始まる Azure Function App を選択します。

5. Overviewペインで **Configuration** を選択します。

    ![The Configuration link is highlighted in the Overview blade.](media/cosmosdb-function-overview.png "Overview")

6. **Application settings** セクションへスクロールします。**+ New application setting** リンクを使って、以下の追加の Key/Value ペアを作成します (キーの名前は以下の表にあるものと完全に一致していなければいけません):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | ColdStorageAccount     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **ColdStorageAccount** Key Vault secret                                                                   |
    | EventHubsConnection   | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **EventHubsConnection** Key Vault secret |
    | LogicAppUrl        | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **LogicAppUrl** Key Vault secret |
    | RecipientEmail      | Enter a **valid email address** you want to receive notification emails from the Logic App. |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-cosmosdb-function.png 'Application Settings section')

7. **Save** を選択し、変更を適用します。

8. 名前が **IoT-StreamProcessing** で始まる Azure Function App を開きます。

9. Overviewペインで **Configuration** を選択します。

10. **Application settings** セクションへスクロールします。**+ New application setting** リンクを使って、以下の追加の Key/Value ペアを作成します (キーの名前は以下の表にあるものと完全に一致していなければいけません):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | IoTHubConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **IoTHubConnection** Key Vault secret |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-stream-function.png 'Application Settings section')

11. **Save** を選択し、変更を適用します。

12. 名前が **IoTWebApp** で始まる Web App (App Service) を開きます。

13. 左側のメニューから **Configuration** を選択します。

14. **Application settings** セクションへスクロールします。**+ New application setting** リンクを使って、以下の追加の Key/Value ペアを作成します (キーの名前は以下の表にあるものと完全に一致していなければいけません):

    | **Application Key**      |                                                                          **Value**                                                                          |
    | ------------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------: |
    | CosmosDBConnection     | Enter `@Microsoft.KeyVault(SecretUri={referenceString})`, where `{referenceString}` is the URI for the **CosmosDBConnection** Key Vault secret |
    | DatabaseName     | Enter `ContosoAuto` |
    | ContainerName     | Enter `metadata` |

    ![In the Application Settings section, the previously mentioned key / value pairs are displayed.](media/application-settings-web-app.png 'Application Settings section')

15. **Save** を選択し、変更を適用します。

> Function Appと Web App の両方のシステム管理 ID が正しく動作し、Key Vault にアクセスできることを確認します。これを行うには、各 Function App と Web App 内で**CosmosDBConnection**設定を開き、設定の下にある **Key Vault Reference Details** を見てください。次のような出力が表示され、シークレットの詳細が表示され、_システムで割り当てられたマネージ ID_ を使用していることを示します:

![The application setting shows the Key Vault reference details underneath.](media/webapp-app-setting-key-vault-reference.png "Key Vault reference details")

> Key Vault Reference Detailsにエラーが表示された場合は、Key Vault に移動し、関連するシステム ID のアクセス ポリシーを削除します。次に、Function App または  Web App に戻り、システム ID をオフにし、再びオンにして (新しいアプリを作成する)、Key Vault のアクセス ポリシーに再度追加します。

### Task 3: Open solution

このタスクでは、この演習の Visual Studio ソリューションを開きます。これには、Function App、Web App、およびデータ ジェネレータの両方のプロジェクトが含まれています。

1. Windowsエクスプローラーを開いて _Before the HOL_ガイド内でソリューションのZIPファイルが展開された場所に移動します。もし`C:\`に直接ZIPファイルを展開した場合、以下のフォルダを開く必要があります:`C:\cosmos-db-scenario-based-labs-master\IoT\Starter` Visual Studioのソリューションファイルを開きます:**CosmosDbIoTScenario.sln**

    > Visual Studio が最初の起動時にサインインを求めるメッセージが表示された場合は、この演習 (該当する場合) または既存の Microsoft アカウントに提供されたアカウントを使用します。

2. ソリューションを開いた後、**Solution Explorer** で含まれるプロジェクトを見てみます:

    1. **Functions.CosmosDB**: Project for the **IoT-CosmosDBProcessing** Function App.
    2. **Functions.StreamProcessing**: Project for the **IoT-StreamProcessing** Function App.
    3. **CosmosDbIoTScenario.Common**: Contains entity models, extensions, and helpers used by the other projects.
    4. **FleetDataGenerator**: The data generator seeds the Cosmos DB `metadata` container with data and simulates vehicles, connects them to IoT Hub, then sends generated telemetry data.
    5. **FleetManagementWebApp**: Project for the **IoTWebApp** Web App.

    ![The Visual Studio Solution Explorer is displayed.](media/vs-solution-explorer.png "Solution Explorer")

3. Solution Explorerで `CosmosDbIoTScenario` ソリューションを右クリックし、**Restore NuGet Packages** を選択します。パッケージはソリューションを開いた時に既にリストアされているかもしれません。

### Task 4: Code completion and walk-through

Function App と Web App プロジェクトには、デプロイする前に完了する必要があるコード ブロックが含まれています。この理由は、ソリューションをガイドし、小さなフラグメントを完了してコードをより深く理解するのに役立ちます。

1. Visual Studioで **View** を選択し、**Task List** を選択します。**TODO** 項目のリストを表示し、それらのいずれかに移動しやすいようになっています。

    ![The View menu in Visual Studio is displayed, and the Task List item is highlighted.](media/vs-view-tasklist.png "View Task List")

    タスクリストはウインドウの一番下に表示されます:

    ![The Task List is displayed.](media/vs-tasklist.png "Task List")

2. **Functions.CosmosDB** プロジェクトの中の **Startup.cs** を開き、**TODO 1** 内に以下をペーストしてコードを完了します:

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

    完全なコードは以下のようになります:

    ![The TODO 1 code is completed.](media/vs-todo1.png "TODO 1")

    [.NET SDK for Cosmos DB v3](https://github.com/Azure/azure-cosmos-dotnet-v3/)を使用しており、Function Apps v2 以降で依存関係の注入がサポートされているので、[singleton Azure Cosmos DB client for the lifetime of the application](https://docs.microsoft.com/azure/cosmos-db/performance-tips#sdk-usage) を利用します。これは、次の TODO ブロックで見るように、コンストラクタを通じて `Functions` クラスに挿入されます。

3. **Save** で **Startup.cs** ファイルを保存します。

4. **Functions.CosmosDB** プロジェクトの中の **Functions.cs** を開き、**TODO 2** 内に以下をペーストしてコードを完了します:

    ```csharp
    public Functions(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
    {
        _httpClientFactory = httpClientFactory;
        _cosmosClient = cosmosClient;
    }
    ```

    上記のコードを追加すると、関数コードに `HttpClientFactory` と `CosmosClient` を挿入できるため、これらのサービスは独自の接続とライフサイクルを管理してパフォーマンスを向上させ、スレッドの枯渇やその他の問題、高価なオブジェクトのインスタンスが誤って作成され過ぎて起こされる、を防ぐことができます。`HttpClientFactory` は、以前のコード変更を行った `Startup.cs` で既に構成されています。Logic App エンドポイントにアラートを送信するために使用され、[Polly](https://github.com/App-vNext/Polly) を使用して、Logic Appが過負荷になっている場合や、HTTP エンドポイントへの呼び出しが失敗する原因となるその他の問題がある場合に備えて、段階的なバックオフ待機ポリシーと再試行ポリシーを使用します。

5. 今完了したばかりのコンストラクタコードの下にある最初の関数コードを見てみます:

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

    `FunctionName` 属性は、Function App 内での関数名の表示方法を定義し、C# メソッド名とは異なる場合があります。この `TripProcessor` 関数は、`CosmosDBTrigger` を使用して、すべてのCosmos DB change feed イベントに起動します。イベントはバッチで到着し、そのサイズは、コンテナーの Insert、Update、または Delete イベントの数などの要因によって異なります。`databaseName` と `collectionName` プロパティは、どのコンテナーのchange feedが関数をトリガーするかを定義します。`ConnectionStringSetting` は、Cosmos DB 接続文字列をプルするFunction App のアプリケーション設定の名前を示します。この例では、作成した Key Vault シークレットから接続文字列の値が描画されます。`LeaseCollection` プロパティは、リース コンテナーの名前と、この関数のリース データに適用されるプレフィックス、およびリース コンテナーが存在しない場合にリース コンテナーを作成するかどうかを定義します。`StartFromBeginning` は `true` に設定され、関数の最後の実行以降のすべてのイベントが処理されるようにします。この関数は、change feedドキュメントを `IReadOnlyList` コレクションに出力します。

6. 関数内を少しだけ下にスクロールして、**TODO 3** 内に以下をペーストしてコードを完了します:

    ```csharp
    var vin = group.Key;
    var odometerHigh = group.Max(item => item.GetPropertyValue<double>("odometer"));
    var averageRefrigerationUnitTemp =
        group.Average(item => item.GetPropertyValue<double>("refrigerationUnitTemp"));
    ```

    車両 VIN でイベントをグループ化したので、グループ キー (VIN) を保持するローカル `vin` 変数を割り当てます。次に、`group.Max` 集計関数を使用して最大走行距離計の値を計算し、平均冷凍ユニット温度を計算する関数である `group.Average` を使用します。`odometerHigh` の値を使用して、旅行距離を計算し、Cosmos DB `metadata` コンテナの `Trip` レコードからの計画された移動距離に基づいて、トリップが完了したかどうかを判断します。必要に応じて、ロジックアプリに送信されるアラートに `averageRefrigerationUnitTemp` が追加されます。

7. 新たに追加したコードで以下のようになっていることを確認します:

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

1. If the Visual Studio solution is not already open, navigate to `C:\cosmos-db-scenario-based-labs-master\IoT\Starter` and open the Visual Studio solution file: **CosmosDbIoTScenario.sln**.

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

    ![The qwuery editor is displayed with the trip results.](media/cosmos-trip-completed-query.png "Trip query")

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

1. Browse to <https://powerbi.microsoft.com> and sign in with the same account you used when you created the Power BI output in Stream Analytics.

2. Select **My workspace**, then select the **Datasets** tab. You should see the **Contoso Auto IoT Events** dataset. This is the dataset you defined in the Stream Analytics Power BI output.

    ![The Contoso Auto IoT dataset is displayed.](media/powerbi-datasets.png "Power BI Datasets")

3. Select **+ Create** at the top of the page, then select **Dashboard**.

    ![The Create button is highlighted at the top of the page, and the Dashboard menu item is highlighted underneath.](media/powerbi-create-dashboard.png "Create Dashboard")

4. Provide a name for the dashboard, such as `Contoso Auto IoT Live Dashboard`, then select **Create**.

    ![The create dashboard dialog is displayed.](media/powerbi-create-dashboard-dialog.png "Create dashboard dialog")

5. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

6. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

7. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **oilAnomaly** from the dropdown. Select **Next**.

    ![The oilAnomaly field is added.](media/power-bi-dashboard-add-tile-oilanomaly.png "Add a custom streaming data tile")

8. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

9. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

10. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

11. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **engineTempAnomaly** from the dropdown. Select **Next**.

    ![The engineTempAnomaly field is added.](media/power-bi-dashboard-add-tile-enginetempanomaly.png "Add a custom streaming data tile")

12. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

13. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

14. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

15. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **aggressiveDriving** from the dropdown. Select **Next**.

    ![The aggressiveDriving field is added.](media/power-bi-dashboard-add-tile-aggressivedriving.png "Add a custom streaming data tile")

16. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

17. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

18. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

19. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **refrigerationTempAnomaly** from the dropdown. Select **Next**.

    ![The refrigerationTempAnomaly field is added.](media/power-bi-dashboard-add-tile-refrigerationtempanomaly.png "Add a custom streaming data tile")

20. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

21. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

22. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

23. Select the **Card** Visualization Type. Under fields, select **+ Add value**, then select **eventCount** from the dropdown. Select **Next**.

    ![The eventCount field is added.](media/power-bi-dashboard-add-tile-eventcount.png "Add a custom streaming data tile")

24. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

25. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

26. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

27. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **engineTemperature**. Leave the time window to display at 1 minute. Select **Next**.

    ![The engineTemperature field is added.](media/power-bi-dashboard-add-tile-enginetemperature.png "Add a custom streaming data tile")

28. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

29. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

30. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

31. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **refrigerationUnitTemp**. Leave the time window to display at 1 minute. Select **Next**.

    ![The refrigerationUnitTemp field is added.](media/power-bi-dashboard-add-tile-refrigerationunittemp.png "Add a custom streaming data tile")

32. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

33. Above the new dashboard, select **+ Add tile**, then select **Custom Streaming Data** in the dialog, then select **Next**.

    ![The add tile dialog is displayed.](media/power-bi-dashboard-add-tile.png "Add tile")

34. Select your **Contoso Auto IoT Events** dataset, then select **Next**.

    ![The Contoso Auto IoT Events dataset is selected.](media/power-bi-dashboard-add-tile-dataset.png "Your datasets")

35. Select the **Line chart** Visualization Type. Under Axis, select **+ Add value**, then select **snapshot** from the dropdown. Under Values, select **+Add value**, then select **speed**. Leave the time window to display at 1 minute. Select **Next**.

    ![The speed field is added.](media/power-bi-dashboard-add-tile-speed.png "Add a custom streaming data tile")

36. Leave the values at their defaults for the tile details form, then select **Apply**.

    ![The apply button is highlighted on the tile details form.](media/power-bi-dashboard-tile-details.png "Tile details")

37. When you are done, rearrange the tiles as shown:

    ![The tiles have been rearranged.](media/power-bi-dashboard-rearranged.png "Power BI dashboard")

38. If the data generator is finished sending events, you may notice that tiles on the dashboard are empty. If so, start the data generator again, this time selecting **option 1** for one vehicle. If you do this, the refrigeration temperature anomaly is guaranteed, and you will see the refrigeration unit temperature gradually climb above the 22.5 degree Fahrenheit alert threshold. Alternatively, you may opt to simulate more vehicles and observe the high event count numbers.

    ![The live dashboard is shown with events.](media/power-bi-dashboard-live-results.png "Power BI dashboard")

    After the generator starts sending vehicle telemetry, the dashboard should start working after a few seconds. In this screenshot, we are simulating 50 vehicles with 2,486 events in the last 10 seconds. You may see a higher or lower value for the `eventCount`, depending on the speed of your computer on which you are running the generator, your network speed and latency, and other factors.

## Exercise 9: Run the predictive maintenance batch scoring

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

5. After importing, select your username. You will see a new folder named `01 IoT`, which contains two notebooks.

   ![The imported notebooks are displayed.](media/databricks-notebooks.png 'Imported notebooks')

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

## Exercise 10: Deploy the predictive maintenance web service

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

## Exercise 11: Create the Predictive Maintenance & Trip/Consignment Status reports in Power BI

**Duration**: 15 minutes

In this lab, you will import a Power BI report that has been created for you. After opening it, you will update the data source to point to your Power BI instance.

### Task 1: Import report in Power BI Desktop

1. Open **Power BI Desktop**, then select **Open other reports**.

    ![The Open other reports link is highlighted.](media/pbi-splash-screen.png "Power BI Desktop")

2. In the Open report dialog, browse to `C:\cosmos-db-scenario-based-labs-master\IoT\Reports`, then select **FleetReport.pbix**. Click **Open**.

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

5. The Maintenance page shows results from the batch scoring notebook you executed in Databricks. If you do not see records here, then you need to run the entire batch scoring notebook after some trips have completed.

    ![The maintenance page is displayed.](media/pbi-maintenance-tab.png "Maintenance")

6. If at any time you have a number of filters set and you cannot see records, **Ctrl+Click** the **Clear Filters** button on the main report page (Trip/Consignments).

    ![The Clear Filters button is highlighted.](media/pbi-clear-filters.png "Clear Filters")

7. If your data generator is running while viewing the report, you can update the report with new data by clicking the **Refresh** button at any time.

    ![The refresh button is highlighted.](media/pbi-refresh.png "Refresh")

## After the hands-on lab

**Duration**: 10 mins

In this exercise, you will delete any Azure resources that were created in support of the lab. You should follow all steps provided after attending the Hands-on lab to ensure your account does not continue to be charged for lab resources.

### Task 1: Delete the resource group

1. Using the [Azure portal](https://portal.azure.com), navigate to the Resource group you used throughout this hands-on lab by selecting Resource groups in the left menu.

2. Search for the name of your resource group, and select it from the list.

3. Select Delete in the command bar, and confirm the deletion by re-typing the Resource group name, and selecting Delete.

You should follow all steps provided _after_ attending the Hands-on lab.
