using System.Net;
using System.Text.Json;
using ai_my_personal_notes_api.Common;
using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using Amazon.Lambda.APIGatewayEvents;
using HotChocolate.Authorization;
using MongoDB.Driver;

namespace ai_my_personal_notes_api;

public class Query
{
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
