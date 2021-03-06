using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;

namespace Catalog.Api.Repositories
{
     public interface IItemRepository
    {
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }
}