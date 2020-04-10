using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Amazon.Lambda.Core;
using DetektivKollektiv.DataLayer.Abstraction;
using DetektivKollektiv.LambdaFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace DetektivKollektiv.DataLayer
{
    public class ItemRepository : IItemRepository
    {
        private ILambdaLogger _logger;
        public ItemRepository(ILambdaLogger logger) {
             _logger = logger;
        }
        /// <summary>
        /// Returns all items from the database
        /// </summary>
        /// <returns>All items in an IEnumerable object</returns>
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            _logger.LogLine("INFO: GetAllItems Method initiated");
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                var conditions = new List<ScanCondition>();
                var allItems = await dbContext.ScanAsync<Item>(conditions).GetRemainingAsync();
                return(allItems);
            }
        }
        /// <summary>
        /// Returns a random <see cref="Item"/>
        /// </summary>
        /// <returns>A random <see cref="Item"/> from the database</returns>
        public async Task<Item> GetRandomItem()
        {
            _logger.LogLine("INFO: Retrieving random item from database.");
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            {
                var lastKeyEvaluated = new Dictionary<string, AttributeValue>()
                    {
                        { "ItemId", new AttributeValue(Guid.NewGuid().ToString()) }
                    };
                var request = new ScanRequest()
                {
                    TableName = "items",
                    ExclusiveStartKey = lastKeyEvaluated,
                    Limit = 1
                };
                var response = await client.ScanAsync(request);
                if (response.Items.Count == 0)
                {
                    return null;
                }
                else
                {
                    var responseItem = response.Items[0];
                    Item randomItem = Converter.ConvertAttributesToItem(responseItem);

                    return randomItem;
                }
            }
        }
        /// <summary>
        /// Queries the database for the item with the specified id
        /// </summary>
        /// <param name="id">The id of the desired item</param>
        /// <returns>The desired item</returns>
        /// <returns>null, if no obejct was found</returns>
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
