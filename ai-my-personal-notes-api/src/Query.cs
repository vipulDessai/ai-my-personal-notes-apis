using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using HotChocolate.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ai_my_personal_notes_api;

public class Query
{
    [Authorize]
    public Task<GetNotesOutput> GetNotes(
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

        return Task.FromResult(new GetNotesOutput { Notes = res });
    }

    public Task<GetTagsOutput> GetTags(GetTagsReqInput input)
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var tagsCollection = db.GetCollection<NoteTags>("tags");

        var (BatchSize, TagsIds, TagsName) = input;
        var findOptions = new FindOptions { BatchSize = BatchSize };
        FilterDefinition<NoteTags> filter;
        if (TagsIds != null && TagsIds.Length > 0)
        {
            filter = Builders<NoteTags>.Filter.In(
                "_id",
                TagsIds.Select(tag => ObjectId.Parse(tag))
            );
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

        return Task.FromResult(new GetTagsOutput { Tags = res });
    }
}
