using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DetektivKollektiv.DataLayer.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DetektivKollektiv.DataLayer
{
    public class ItemRepository : IItemRepository
    {

        public Task<IEnumerable<Item>> GetAllItems()
        {
              throw new NotImplementedException();
        }

        public async Task<Item> GetRandomItem()
        {
            var rand = new Random();

            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var context = new DynamoDBContext(client))

            {
                var response = await client.ScanAsync(new ScanRequest("items"));
                int itemLength = response.Count;
                int randomItemId = rand.Next(itemLength) + 1;
                var randomItem = await context.LoadAsync<Item>(randomItemId);
                return randomItem;
            }
        }

        public async Task<Item> GetItemById(int id)
        {
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var context = new DynamoDBContext(client))

            {
                var item = await context.LoadAsync<Item>(id);
                return item;
            }
        }

        public async Task<Item> GetItemByText(string text)
        {
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                var conditions = new List<ScanCondition> { new ScanCondition("Text", ScanOperator.Equal, text) };
                var result = await dbContext.ScanAsync<Item>(conditions).GetRemainingAsync();
                if (result.Count == 0)
                {
                    return null;
                }
                else
                {
                    return result[0];
                }
            }
        }

        public async Task<Item> CreateItem(Item item)
        {
            int itemId = item.ItemId;
                        
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                await dbContext.SaveAsync<Item>(item);
                return await GetItemById(itemId);
            }
        }
    }
}
