using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DetektivKollektiv.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task<APIGatewayProxyResponse> GetItemByIdAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {

            try
            {
                string checkId = request.Body;

                ItemRepository repo = new ItemRepository();

                Item item = await repo.GetItemById(checkId);

                return Ok(JsonConvert.SerializeObject(item));
            }
            catch (FormatException)
            {
                return NoContent();
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
            context.Logger.LogLine("Get Request\n");

            var itemRepo = new ItemRepository();

            var randomItem = await itemRepo.GetRandomItem();

            return Ok(JsonConvert.SerializeObject(randomItem));
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

            ItemRepository repo = new ItemRepository();
            
            Item item = await repo.GetItemByText(checkText);

            if (item == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(JsonConvert.SerializeObject(item));
            }        
        }

        public async Task<APIGatewayProxyResponse> CreateNewItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                string text = request.Body; 
                ItemRepository repo = new ItemRepository();

                Item responseItem = await repo.CreateItem(text);

                if (responseItem == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(responseItem));
                }
            }
            catch(Exception)
            {
                return NoContent();
            }
            
        }

        public async Task<APIGatewayProxyResponse> CheckItem(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Random rnd = new Random();
            
            string text = request.Body;

            ItemRepository repo = new ItemRepository();

            Item item = await repo.GetItemByText(text);

            if(item == null)
            {
                await repo.CreateItem(text);
                return Ok("Item created");
            }
            else
            {
                return Ok(JsonConvert.SerializeObject(item));
            }
        }

        public async Task<APIGatewayProxyResponse> Review(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ItemRepository repo = new ItemRepository();
            Review review = JsonConvert.DeserializeObject<Review>(request.Body);
            Item response = await repo.Review(review.itemId, review.goodReview);
            return Ok(JsonConvert.SerializeObject(response));
        
        }
        private APIGatewayProxyResponse Ok(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

        private APIGatewayProxyResponse NoContent()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NoContent,
                Body = null,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }
    }

}
