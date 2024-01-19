using System.Net;
using System.Text.Json;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace Controllers.Bak;

public class Restaurant
{
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("restaurant_id")]
    public string RestaurantId { get; set; }

    [BsonElement("cuisine")]
    public string Cuisine { get; set; }

    [BsonElement("address")]
    public Address Address { get; set; }

    [BsonElement("borough")]
    public string Borough { get; set; }

    [BsonElement("grades")]
    public List<GradeEntry> Grades { get; set; }
}

public class GradeEntry
{
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("grade")]
    public string Grade { get; set; }

    [BsonElement("score")]
    public float Score { get; set; }
}

public class Address
{
    [BsonElement("building")]
    public string Building { get; set; }

    [BsonElement("coord")]
    public double[] Coordinates { get; set; }

    [BsonElement("street")]
    public string Street { get; set; }

    [BsonElement("zipcode")]
    public string ZipCode { get; set; }
}

public class Functions
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions() { }

    private static MongoClient CreateMongoClient()
    {
        var mongoClientSettings = MongoClientSettings.FromConnectionString(
            $"{Environment.GetEnvironmentVariable("MONGODB_URI")}"
        );
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
    [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 120)]
    [RestApi(LambdaHttpMethod.Get, "/")]
    public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var client = CreateMongoClient();
        var _restaurantsCollection = client
            .GetDatabase("sample_restaurants")
            .GetCollection<Restaurant>("restaurants");
        var filter = Builders<Restaurant>.Filter.Eq("cuisine", "Pizza");
        var findOptions = new FindOptions { BatchSize = 3 };

        var res = new List<string>();
        using (var cursor = _restaurantsCollection.Find(filter, findOptions).ToCursor())
        {
            if (cursor.MoveNext())
            {
                var d = cursor.Current.ToList();

                for (int i = 0; i < d.Count; i++)
                {
                    res.Add(d[i].Name);
                }
            }

            //while (cursor.MoveNext())
            //{

            //}
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(res.ToArray()),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
