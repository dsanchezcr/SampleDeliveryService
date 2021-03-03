using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleDeliveryService.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace SampleDeliveryService.Services
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

        public async Task AddItemAsync(Order order)
        {
            await this._container.CreateItemAsync<Order>(order, new PartitionKey(order.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Order>(id, new PartitionKey(id));
        }

        public async Task<Order> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Order> response = await this._container.ReadItemAsync<Order>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Order>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Order>(new QueryDefinition(queryString));
            List<Order> results = new List<Order>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(Order order)
        {
            await this._container.UpsertItemAsync<Order>(order, new PartitionKey(order.Id));
        }
    }
}