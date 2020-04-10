using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;

namespace DetektivKollektiv.LambdaFunctions
{
    public static class Responses
    {
        public static APIGatewayProxyResponse Ok(string body)
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

        public static APIGatewayProxyResponse NoContent(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NoContent,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

        public static APIGatewayProxyResponse Created(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Created,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

        public static APIGatewayProxyResponse BadRequest(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

        public static APIGatewayProxyResponse NotFound(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

        public static APIGatewayProxyResponse InternalServerError(string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = body,
                Headers = new Dictionary<string, string> {
                            { "Content-Type", "application/json" },
                            { "Access-Control-Allow-Origin", "*" }
                        }
            };
        }

    }
}
