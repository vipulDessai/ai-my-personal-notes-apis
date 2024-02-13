using ai_my_personal_notes_api.Common;
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
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("sample_restaurants");
        var globalCollection = db.GetCollection<NoteSchema>("collection");
        var _restaurantsCollection = db.GetCollection<Restaurant>("restaurants");
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

    [Authorize]
    public async Task<APIGatewayProxyResponse> GetNotes(
        [GlobalState("currentUser")] CurrentUser user,
        GetNotesReqInput input
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var _globalCollection = db.GetCollection<NoteSchema>("collection");

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
            Headers = Constants.Headers,
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

    public Task<APIGatewayProxyResponse> GetTags(GetTagsReqInput input)
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var tagsCollection = db.GetCollection<NoteTags>("tags");

        var (BatchSize, TagsIds, TagsName) = input;
        var findOptions = new FindOptions { BatchSize = BatchSize };

        // TODO: find by ID is not working - fix it
        FilterDefinition<NoteTags> filter;
        if (TagsIds != null && TagsIds.Length > 0)
        {
            filter = Builders<NoteTags>.Filter.In("id", TagsIds);
        }
        else
        {
            filter = Builders<NoteTags>.Filter.In("name", TagsName);
        }

        var res = new List<NoteTags>();
        using (var cursor = tagsCollection.Find(filter, findOptions).ToCursor())
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

        var result = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(res),
            Headers = Constants.Headers,
        };

        return Task.FromResult(result);
    }
}
