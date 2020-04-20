using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DatektivKollektiv.Shared;
using DetektivKollektiv.DataLayer;
using DetektivKollektiv.LambdaFunctions;
using Newtonsoft.Json;
using System;
using System.Linq;

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
        public APIGatewayProxyResponse GetItems(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("GetAllItems Initiated. Checking if body exists.");
            ItemRepository repo = new ItemRepository(context.Logger);

            if (request.Body == null)
            {
                context.Logger.LogInfo("No Body found. Checking if table is empty.");
                var allItems = repo.GetAllItems();
                if (allItems.Any())
                {
                    context.Logger.LogInfo("Table not empty. Returning all items");
                    return Responses.Ok(JsonConvert.SerializeObject(allItems));
                }
                else
                {
                    context.Logger.LogInfo("No items found in database. Returning 204.");
                    return Responses.NoContent("No items in database");
                }
            }
            else
            {
                context.Logger.LogInfo("Body exists. Trying to return item");
                string checkText = request.Body;
                Item item = repo.GetItemByText(checkText);

                if (item == null)
                {
                    context.Logger.LogWarning("Item does not exist. Returning 404.");
                    return Responses.NotFound("No item found");
                }
                else
                {
                    context.Logger.LogInfo("Item found. Returning 200.");
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
        public APIGatewayProxyResponse GetItemById(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("GetItem initiated");
            ItemRepository repo = new ItemRepository(context.Logger);

            try
            {
                context.Logger.LogInfo("Checking if itemId is in path");
                
                if (request.PathParameters.TryGetValue("itemId", out string checkIdString))
                {
                    Guid checkId = new Guid(checkIdString);
                    context.Logger.LogInfo("itemId found in path. Checking database for item");

                    Item item = repo.GetItemById(checkId);
                    if (item == null)
                    {
                        context.Logger.LogInfo("No database entry found with the specified id. Returning 404");
                        return Responses.NotFound("No item found with the specified id");
                    }
                    else
                    {
                        context.Logger.LogInfo("Item found. Returning item.");
                        return Responses.Ok(JsonConvert.SerializeObject(item));
                    }
                }
                else
                {
                    context.Logger.LogError("No itemId in path.");
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
        public APIGatewayProxyResponse GetRandomItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("GetRandomItem initiated. Checking if random item exists");

            var itemRepo = new ItemRepository(context.Logger);

            var randomItem = itemRepo.GetRandomItem();

            if (randomItem != null)
            {
                context.Logger.LogInfo("Got random item. Returning 200.");
                return Responses.Ok(JsonConvert.SerializeObject(randomItem));
            }
            else
            {
                context.Logger.LogInfo("No item found. Returning 204");
                return Responses.NoContent("Could not retrieve a random item from database. The table seems to be empty.");
            }
        }

        /// <summary>
        /// Creates a new <see cref="Item"/> in the database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>If the <see cref="Item"/> was created succesfully, HTTP code 201 and the newly created <see cref="Item"/></returns>
        /// <returns>If the <see cref="Item"/> could not be created, HTTP code 400.</returns>
        public APIGatewayProxyResponse CreateNewItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("CreateNewItem initiated");
            try
            {
                string text = request.Body; 
                ItemRepository repo = new ItemRepository(context.Logger);
                context.Logger.LogInfo("Checking if item exists already");

                Item item = repo.GetItemByText(text);
                if(item == null)
                {
                    context.Logger.LogInfo("Item does not exist yet. Trying to create new item");
                    Item responseItem = repo.CreateItem(text); 
                    if (responseItem == null)
                    {
                        context.Logger.LogError("Item could not be created.");
                        return Responses.InternalServerError("Could not create new item");
                    }
                    else
                    {
                        context.Logger.LogInfo("Item successfully created. Returning 201.");
                        return Responses.Created(JsonConvert.SerializeObject(responseItem));
                    }
                }
                else
                {
                    context.Logger.LogWarning("Item already exists. Returning 400.");
                    return Responses.BadRequest("Item already exists");
                }
            }
            catch(Exception)
            {
                context.Logger.LogError("Caught an exception. Returning 400");
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
        public APIGatewayProxyResponse CheckItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("CheckItem initiated. Checking if item exists.");
            string text = request.Body;

            ItemRepository repo = new ItemRepository(context.Logger);

            Item item = repo.GetItemByText(text);

            if(item == null)
            {
                context.Logger.LogInfo("Item does not exist. Creating new item and returning 201");
                Item newItem = repo.CreateItem(text);
                return Responses.Created(JsonConvert.SerializeObject(newItem));
            }
            else
            {
                context.Logger.LogInfo("Item exists already. Returning 200");
                return Responses.Ok(JsonConvert.SerializeObject(item));
            }
        }
        
        /// <summary>
        /// Creating a new review and updated the item accordingly.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The updated item</returns>
        public APIGatewayProxyResponse ReviewItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInfo("Review initiated. Trying to update item.");
            try 
            {                 
                ItemRepository repo = new ItemRepository(context.Logger);
                Review review = JsonConvert.DeserializeObject<Review>(request.Body);
                Item response = repo.Review(review.ItemId, review.GoodReview);
                context.Logger.LogInfo("Review successful. Returning 201.");
                return Responses.Created(JsonConvert.SerializeObject(response));
            }
            catch (Exception e)
            {
                context.Logger.LogError("Something went wrong. Returning 400");
                context.Logger.LogLine(e.Message);
                return Responses.BadRequest("Could not create Review");
            }
        }
    }
}
