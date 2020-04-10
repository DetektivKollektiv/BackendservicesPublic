using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DetektivKollektiv.DataLayer;
using DetektivKollektiv.LambdaFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.LambdaJsonSerializer))]

namespace DetektivKollektiv
{
    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }
        /// <summary>
        /// Returns an <see cref="Item"/> with the specified id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If an <see cref="Item"/> was found, HTTP Code 200 and the item in JSON notation</returns>
        /// <returns>If no <see cref="Item"/> was found, HTTP Code 204</returns>
        public async Task<APIGatewayProxyResponse> GetItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: GetItem initiated");
            ItemRepository repo = new ItemRepository(context.Logger);

            try
            {
                context.Logger.LogLine("INFO: Checking if itemId is in path");
                
                string checkId = "";

                if(request.PathParameters.TryGetValue("itemId", out checkId))
                {
                    context.Logger.LogLine("INFO: itemId found in path. Checking database for item");
                    
                    Item item = await repo.GetItemById(checkId);
                    if (item == null)
                    {
                        context.Logger.LogLine("INFO: No database entry found with the specified id. Returning 404");
                        return Responses.NotFound("No item found with the specified id");
                    }
                    else
                    {
                        context.Logger.LogLine("INFO: Item found. Returning item.");
                        return Responses.Ok(JsonConvert.SerializeObject(item));
                    }
                }
                else
                {
                    context.Logger.LogLine("ERROR: No itemId in path.");
                    return Responses.InternalServerError("Could not process path correctly");
                    
                }                
            }
            catch (ArgumentNullException)
            {
                return Responses.InternalServerError("Could not parse itemId parameter");
            }           
        }

        /// <summary>
        /// Returns all <see cref="Item"/>s from the database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If the table is not empty, HTTP Code 200 and all <see cref="Item"/>s in JSON notation</returns>
        /// <returns>If the table is empty, HTTP Code 204</returns>
        public async Task<APIGatewayProxyResponse> GetAllItemsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: GetAllItems Initiated");
            ItemRepository repo = new ItemRepository(context.Logger);
            IEnumerable<Item> allItems = await repo.GetAllItems();
            if (allItems.Any())
            {
                context.Logger.LogLine("INFO: Table not empty. Returning all items");
                return Responses.Ok(JsonConvert.SerializeObject(allItems));
            }
            else
            {
                context.Logger.LogLine("INFO: No items found in database. Returning 204.");
                return Responses.NoContent("No items in database");
            }
        }

        /// <summary>
        /// A Lambda function to get a random <see cref="Item"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context">The <see cref="ILambdaContext"/> for the function call.</param>
        /// <returns>A random <see cref="Item"/> from the database.</returns>
        public async Task<APIGatewayProxyResponse> GetRandomItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: GetRandomItem initiated. Checking if random item exists");

            var itemRepo = new ItemRepository(context.Logger);

            var randomItem = await itemRepo.GetRandomItem();

            if(randomItem != null)
            {
                context.Logger.LogLine("INFO: Got random item. Returning item.");
                return Responses.Ok(JsonConvert.SerializeObject(randomItem));
            }
            else
            {
                context.Logger.LogLine("INFO: No item found. Returning 204");
                return Responses.NoContent("Could not retrieve a random item from database. The table seems to be empty.");
            }
        }

        /// <summary>
        /// A Lambda function to check 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetItemByTextAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string checkText = request.Body;

            ItemRepository repo = new ItemRepository(context.Logger);
            
            Item item = await repo.GetItemByText(checkText);

            if (item == null)
            {
                return Responses.NoContent("");
            }
            else
            {
                return Responses.Ok(JsonConvert.SerializeObject(item));
            }        
        }

        public async Task<APIGatewayProxyResponse> CreateNewItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                string text = request.Body; 
                ItemRepository repo = new ItemRepository(context.Logger);

                Item responseItem = await repo.CreateItem(text);

                if (responseItem == null)
                {
                    return Responses.NoContent("");
                }
                else
                {
                    return Responses.Ok(JsonConvert.SerializeObject(responseItem));
                }
            }
            catch(Exception)
            {
                return Responses.NoContent("");
            }
            
        }

        public async Task<APIGatewayProxyResponse> CheckItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Random rnd = new Random();
            
            string text = request.Body;

            ItemRepository repo = new ItemRepository(context.Logger);

            Item item = await repo.GetItemByText(text);

            if(item == null)
            {
                await repo.CreateItem(text);
                return Responses.Ok("Item created");
            }
            else
            {
                return Responses.Ok(JsonConvert.SerializeObject(item));
            }
        }

        public async Task<APIGatewayProxyResponse> Review(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ItemRepository repo = new ItemRepository(context.Logger);
            Review review = JsonConvert.DeserializeObject<Review>(request.Body);
            Item response = await repo.Review(review.itemId, review.goodReview);
            return Responses.Ok(JsonConvert.SerializeObject(response));
        
        }
    }

}
