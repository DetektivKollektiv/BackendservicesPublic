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
using System.Linq.Expressions;
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
        /// Returns all <see cref="Item"/>s from the database or the item, that matches the bodies conditions.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If the table is not empty, HTTP Code 200 and all <see cref="Item"/>s in JSON notation</returns>
        /// <returns>If the table is empty, HTTP Code 204</returns>
        /// <returns>If a body is specified and an item exists, HTTP Code 200 and the item</returns>
        /// <returns>If a body is specified and no item exists, HTTP Code 404.</returns>
        public async Task<APIGatewayProxyResponse> GetItemsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: GetAllItems Initiated. Checking if body exists.");
            ItemRepository repo = new ItemRepository(context.Logger);

            if (request.Body == null)
            {
                context.Logger.LogLine("INFO: No Body found. Checking if table is empty.");
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
            else
            {
                context.Logger.LogLine("INFO: Body exists. Trying to return item");
                string checkText = request.Body;
                Item item = await repo.GetItemByText(checkText);

                if (item == null)
                {
                    context.Logger.LogLine("WARNING: Item does not exist. Returning 404.");
                    return Responses.NotFound("No item found");
                }
                else
                {
                    context.Logger.LogLine("INFO: Item found. Returning 200.");
                    return Responses.Ok(JsonConvert.SerializeObject(item));
                }
            }

        }


        /// <summary>
        /// Returns an <see cref="Item"/> with the specified id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If an <see cref="Item"/> was found, HTTP Code 200 and the item in JSON notation</returns>
        /// <returns>If no <see cref="Item"/> was found, HTTP Code 204</returns>
        public async Task<APIGatewayProxyResponse> GetItemByIdAsync(APIGatewayProxyRequest request, ILambdaContext context)
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
                context.Logger.LogLine("INFO: Got random item. Returning 200.");
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
        /*public async Task<APIGatewayProxyResponse> GetItemByTextAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: GetItemByText initiated. Checking if item exists");
            
            string checkText = request.Body;

            ItemRepository repo = new ItemRepository(context.Logger);
            
            Item item = await repo.GetItemByText(checkText);

            if (item == null)
            {
                context.Logger.LogLine("WARNING: Item does not exist. Returning 400.");
                return Responses.BadRequest("No item found");
            }
            else
            {
                context.Logger.LogLine("INFO: Item found. Returning 200.");
                return Responses.Ok(JsonConvert.SerializeObject(item));
            }        
        }*/

        /// <summary>
        /// Creates a new <see cref="Item"/> in the database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If the <see cref="Item"/> was created succesfully, HTTP code 201 and the newly created <see cref="Item"/></returns>
        /// <returns>If the <see cref="Item"/> could not be created, HTTP code 400.</returns>
        public async Task<APIGatewayProxyResponse> CreateNewItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: CreateNewItem initiated");
            try
            {
                string text = request.Body; 
                ItemRepository repo = new ItemRepository(context.Logger);
                context.Logger.LogLine("INFO: Checking if item exists already");

                Item item = await repo.GetItemByText(text);
                if(item == null)
                {
                    context.Logger.LogLine("INFO: Item does not exist yet. Trying to create new item");
                    Item responseItem = await repo.CreateItem(text); 
                    if (responseItem == null)
                    {
                        context.Logger.LogLine("ERROR: Item could not be created.");
                        return Responses.InternalServerError("Could not create new item");
                    }
                    else
                    {
                        context.Logger.LogLine("INFO: Item successfully created. Returning 201.");
                        return Responses.Created(JsonConvert.SerializeObject(responseItem));
                    }
                }
                else
                {
                    context.Logger.LogLine("WARNING: Item already exists. Returning 400.");
                    return Responses.BadRequest("Item already exists");
                }
            }
            catch(Exception)
            {
                context.Logger.LogLine("ERROR: Caught an exception. Returning 400");
                return Responses.BadRequest("The item could not be created");
            }
            
        }
        
        /// <summary>
        /// Checks, if an item exists. If it exists, it is returned. If it does not exist, it is created.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If the item was created, HTTP status code 201 and the new item.</returns>
        /// <returns>If the item exists already, HTTP status code 200 and the existing item.</returns>
        public async Task<APIGatewayProxyResponse> CheckItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: CheckItem initiated. Checking if item exists.");
            string text = request.Body;

            ItemRepository repo = new ItemRepository(context.Logger);

            Item item = await repo.GetItemByText(text);

            if(item == null)
            {
                context.Logger.LogLine("INFO: Item does not exist. Creating new item and returning 201");
                Item newItem = await repo.CreateItem(text);
                return Responses.Created(JsonConvert.SerializeObject(newItem));
            }
            else
            {
                context.Logger.LogLine("INFO: Item exists already. Returning 200");
                return Responses.Ok(JsonConvert.SerializeObject(item));
            }
        }
        
        /// <summary>
        /// Creating a new review and updated the item accordingly.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The updated item</returns>
        public async Task<APIGatewayProxyResponse> ReviewItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("INFO: Review initiated. Trying to update item.");
            try 
            {                 
                ItemRepository repo = new ItemRepository(context.Logger);
                Review review = JsonConvert.DeserializeObject<Review>(request.Body);
                Item response = await repo.Review(review.itemId, review.goodReview);
                context.Logger.LogLine("INFO: Review successful. Returning 201.");
                return Responses.Created(JsonConvert.SerializeObject(response));
            }
            catch (Exception)
            {
                context.Logger.LogLine("ERROR: Something went wrong. Returning 400");
                return Responses.BadRequest("Could not create Review");
            }
        }
    }

}
