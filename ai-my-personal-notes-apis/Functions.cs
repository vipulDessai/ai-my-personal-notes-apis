using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ai_my_personal_notes_apis;

public class Functions
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
    }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <remarks>
    /// This uses the <see href="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md">Lambda Annotations</see> 
    /// programming model to bridge the gap between the Lambda programming model and a more idiomatic .NET model.
    /// 
    /// This automatically handles reading parameters from an APIGatewayProxyRequest
    /// as well as syncing the function definitions to serverless.template each time you build.
    /// 
    /// If you do not wish to use this model and need to manipulate the API Gateway 
    /// objects directly, see the accompanying Readme.md for instructions.
    /// </remarks>
    /// <param name="context">Information about the invocation, function, and execution environment</param>
    /// <returns>The response as an implicit <see cref="APIGatewayProxyResponse"/></returns>
    [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
    [RestApi(LambdaHttpMethod.Get, "/")]
    public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get Request\n");

        string reqString = JsonSerializer.Serialize(request);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "this is actually generate data from AIs",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
