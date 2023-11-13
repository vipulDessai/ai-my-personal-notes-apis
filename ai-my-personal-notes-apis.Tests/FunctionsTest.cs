using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Xunit;

namespace ai_my_personal_notes_apis.Tests;

public class FunctionTest
{
    public FunctionTest()
    {
    }

    [Fact]
    public void TestGetMethod()
    {
        var context = new TestLambdaContext();
        var functions = new Functions();

        APIGatewayProxyRequest request = new APIGatewayProxyRequest();

        var response = functions.Get(request, context);

        Assert.Equal(200, response.StatusCode);

        // TODO: use serialization like JSON stringify if required
        //var serializationOptions = new HttpResultSerializationOptions { Format = HttpResultSerializationOptions.ProtocolFormat.RestApi };
        //var apiGatewayResponse = new StreamReader(response.Serialize(serializationOptions)).ReadToEnd();

        Assert.Contains("Hello AWS Serverless", response.Body);
    }
}
