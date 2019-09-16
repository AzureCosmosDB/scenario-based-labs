using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using P.Pager;

namespace FleetManagementWebApp.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync<T>(T item, string partitionKey) where T : class
        {
            await this._container.CreateItemAsync<T>(item, new PartitionKey(partitionKey));
        }

        public async Task DeleteItemAsync<T>(string id, string partitionKey) where T : class
        {
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task<T> GetItemAsync<T>(string id, string partitionKey) where T : class
        {
            try
            {
                var response = await this._container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string queryString, string partitionKey = null) where T : class
        {
            var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString),
                requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null);
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(QueryDefinition queryDefinition, string partitionKey = null) where T : class
        {
            var query = this._container.GetItemQueryIterator<T>(queryDefinition,
                requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null);
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, int? skip = null, int? take = null, string partitionKey = null) where T : class
        {
            FeedIterator<T> setIterator;
            var query = this._container.GetItemLinqQueryable<T>(requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null);

            // Implement paging:
            if (skip.HasValue && take.HasValue)
            {
                setIterator = query.Where(predicate).Skip(skip.Value).Take(take.Value).ToFeedIterator();
            }
            else
            {
                setIterator = take.HasValue ? query.Where(predicate).Take(take.Value).ToFeedIterator() : query.Where(predicate).ToFeedIterator();
            }
            
            var results = new List<T>();
            while (setIterator.HasMoreResults)
            {
                var response = await setIterator.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IPager<T>> GetItemsWithPagingAsync<T>(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, string partitionKey = null) where T : class
        {
            // Find the item index for the Skip command:
            var itemIndex = (pageIndex - 1) * pageSize;

            var query = this._container.GetItemLinqQueryable<T>(requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null);

            // Implement paging:
            var setIterator = query.Where(predicate).Skip(itemIndex).Take(pageSize).ToFeedIterator();
            
            var list = new List<T>();
            while (setIterator.HasMoreResults)
            {
                var response = await setIterator.ReadNextAsync();

                list.AddRange(response.ToList());
            }

            // Get total item count from the database:
            var count = this._container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true, requestOptions: !string.IsNullOrWhiteSpace(partitionKey) ? new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) } : null)
                .Where(predicate).Count();

            var results = list.ToPagerList();
            results.TotalItemCount = count;
            results.CurrentPageIndex = pageIndex;
            results.PageSize = pageSize;

            return results;
        }

        public async Task UpdateItemAsync<T>(T item, string partitionKey) where T : class
        {
            await this._container.UpsertItemAsync<T>(item, new PartitionKey(partitionKey));
        }
    }
}
