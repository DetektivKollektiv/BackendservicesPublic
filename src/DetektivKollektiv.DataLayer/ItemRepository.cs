using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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

        public Task<Item> CreateItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
