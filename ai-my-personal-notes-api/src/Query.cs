using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using Amazon.Lambda.APIGatewayEvents;
using HotChocolate.Authorization;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;

namespace ai_my_personal_notes_api;

public class Query
{
    public async Task<APIGatewayProxyResponse> GetRestuarants(
        [GlobalState("currentUser")] CurrentUser user,
        GetRestuarantsInput input
    )
    {
        var db = new MongoDb();
        var _restaurantsCollection = db.client.GetDatabase("sample_restaurants")
            .GetCollection<Restaurant>("restaurants");
        var filter = Builders<Restaurant>.Filter.Eq(input.FilterKey, input.FilterValue);
        var findOptions = new FindOptions { BatchSize = input.BatchSize };

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
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(res.ToArray()),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }

    public async Task<APIGatewayProxyResponse> GetNotes(
        [GlobalState("currentUser")] CurrentUser user,
        GetNotesReqInput input
    )
    {
        var db = new MongoDb();
        var _globalCollection = db.client.GetDatabase("db").GetCollection<NoteSchema>("collection");

        var (BatchSize, FilterKey, FilterValue) = input;

        FilterDefinition<NoteSchema> filter;
        if (string.IsNullOrEmpty(FilterKey))
        {
            filter = Builders<NoteSchema>.Filter.Empty;
        }
        else
        {
            filter = Builders<NoteSchema>.Filter.Eq(FilterKey, FilterValue);
        }
        var findOptions = new FindOptions { BatchSize = BatchSize };

        var res = new List<NoteSchema>();
        using (var cursor = _globalCollection.Find(filter, findOptions).ToCursor())
        {
            if (cursor.MoveNext())
            {
                var d = cursor.Current.ToList();

                for (int i = 0; i < d.Count; i++)
                {
                    res.Add(d[i]);
                }
            }
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(res.ToArray()),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }

    public string Unauthorized()
    {
        return "unauthorized";
    }

    [Authorize(Policy = "hr")]
    public List<string> Authorized([Service] IHttpContextAccessor contextAccessor)
    {
        var res = contextAccessor
            .HttpContext.User.Claims.Select(x => $"{x.Type} : {x.Value}")
            .ToList();

        return res;
    }

    [Authorize(Policy = "hr")]
    public List<string> AuthorizedBetterWay([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Roles = new[] { "leader" })]
    public List<string> AuthorizedLeader([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Roles = new[] { "dev" })]
    public List<string> AuthorizedDev([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Policy = "DevDepartment")]
    public List<string> AuthorizedDevDepartment([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    public Task<List<Book>> GetBooks([Service] Repository repository) => repository.GetBooksAsync();

    public Task<Author?> GetAuthor(GetAuthorInput input, [Service] Repository r) =>
        r.GetAuthor(input.authorId);
}
