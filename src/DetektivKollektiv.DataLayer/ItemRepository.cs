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

        public async Task<IEnumerable<Item>> GetAllItems()
        {
            throw new NotImplementedException();

        }

            public async Task<Item> GetRandomItem()
        {
            //TODO: anpassen

            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client)) {
                string randomId = Guid.NewGuid().ToString();
                var conditions = new List<ScanCondition> { new ScanCondition("ItemId", ScanOperator.NotEqual, randomId)};
                var result = await dbContext.ScanAsync<Item>(conditions).GetRemainingAsync();
                int resultLength = result.Count;
                int randomEntry = new Random().Next(resultLength);
                Item randomItem = result[randomEntry];
                return randomItem;
             }
        }

        public async Task<Item> GetItemById(string id)
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

        public async Task<Item> CreateItem(string text)
        {
            Item item = new Item();
            item.Text = text;
            item.ReviewBad = 0;
            item.ReviewGood = 0;
            item.ItemId = Guid.NewGuid().ToString();
                        
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                await dbContext.SaveAsync<Item>(item);
                return await GetItemById(item.ItemId);
            }
        }

        public Task<Item> CreateItem(Item item)
        {
            throw new NotImplementedException();
        }
       
        public async Task<Item> Review(string id, bool goodReview)
        {
            Item item = await GetItemById(id);
            if (goodReview)
            {
                item.ReviewGood++;
            }
            else
            {
                item.ReviewBad++;
            }

            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                await dbContext.SaveAsync<Item>(item);
            }
                return item;

        }
    }
}
