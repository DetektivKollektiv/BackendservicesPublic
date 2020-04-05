using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using DetektivKollektiv.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.LambdaJsonSerializer))]

namespace DetektivKollektiv.LambdaFunctions
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
        /// A Lambda function to get a random <see cref="Item"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context">The <see cref="ILambdaContext"/> for the function call.</param>
        /// <returns>A random <see cref="Item"/> from the database.</returns>
        public async Task<APIGatewayProxyResponse> GetRandomItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var randomItem = await InternalGetRandomItemAsync();
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(randomItem),
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function to check 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> CheckItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string checkText = request.Body;

            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUCentral1))
            using (var dbContext = new DynamoDBContext(client))
            {
                var conditions = new List<ScanCondition> { new ScanCondition("Text", ScanOperator.Equal, checkText) };
                var result = await dbContext.ScanAsync<Item>(conditions).GetRemainingAsync();
                if (result.Count == 0)
                {
                    var response = new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Body = "Not found",
                        Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
                    };
                    return response;
                }
                else
                {
                    var response = new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Body = JsonConvert.SerializeObject(result),
                        Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" } ,
                            { "Access-Control-Allow-Origin", "*" }
                        }
                    };
                    return response;
                }
            }
        }

        private async Task<Item> InternalGetRandomItemAsync()
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
    }
}
