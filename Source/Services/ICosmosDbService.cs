namespace SampleDeliveryService.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SampleDeliveryService.Models;
    public interface ICosmosDbService
    {
        Task<IEnumerable<Order>> GetItemsAsync(string query);
        Task<Order> GetItemAsync(string id);
        Task AddItemAsync(Order item);
        Task UpdateItemAsync(Order item);
        Task DeleteItemAsync(string id);
    }
}
