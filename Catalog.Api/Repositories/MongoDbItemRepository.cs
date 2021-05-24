using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Api.Repositories
{
    public class MongoDbItemRepository : IItemRepository
    {
        public const string databaseName = "catalog";
        public const string collectionName = "items";
        public readonly IMongoCollection<Item> itemsCollection;
        
        public readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        public MongoDbItemRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(collectionName);
        }
        public async Task CreateItemAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(_item => _item.Id, id);
            await itemsCollection.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(_item => _item.Id, item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}