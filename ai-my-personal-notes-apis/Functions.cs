using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ai_my_personal_notes_apis;

//[BsonElement("PopulationAsOn")]
//public DateTime? PopulationAsOn { get; set; }

//[BsonRepresentation(BsonType.String)]
//[BsonElement("CountryId")]
//public int CountryId { get; set; }

//[Required(AllowEmptyStrings = false)]
//[StringLength(5)]
//[BsonIgnoreIfDefault]
//public virtual string CountryCode { get; set; }

//[BsonIgnoreIfDefault]
//public IList<States> States { get; set; }

//[BsonExtraElements]
//public BsonDocument ExtraElements { get; set; }
class StoredSingleDbRecord
{
    [BsonId]
    public string id { get; set; }

    [BsonRepresentation(BsonType.String)]
    [BsonElement("plot")]
    public int plot { get; set; }
}

public class Functions
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
    }

    private static MongoClient CreateMongoClient()
    {
        var mongoClientSettings = MongoClientSettings.FromConnectionString($"{Environment.GetEnvironmentVariable("MONGODB_URI")}");
        mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1, strict: true);
        return new MongoClient(mongoClientSettings);
    }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <remarks>
    /// Store the form data in the mongo db
    /// </remarks>
    /// <params>
    /// <param name="request">request data from the client</param>
    /// <param name="context">Information about the invocation, function, and execution environment</param>
    /// <params>
    /// <returns>
    /// status of the data stored in the DB
    /// </returns>
    [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
    [RestApi(LambdaHttpMethod.Get, "/")]
    public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get Request\n");

        string reqString = JsonSerializer.Serialize(request);

        var client = CreateMongoClient();
        var collection = client.GetDatabase("sample_mflix").GetCollection<BsonDocument>("movies");
        var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
        var document = collection.Find(filter, new FindOptions() { BatchSize = 1000 }).First();
        Console.WriteLine(document);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "this is actually generate data from AIs",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
