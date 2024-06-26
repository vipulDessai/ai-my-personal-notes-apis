﻿using ai_my_personal_notes_api.Models;
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

        var (BatchSize, Page, FilterKey, FilterValue) = input;

        FilterDefinition<NoteSchema> filter;
        if (string.IsNullOrEmpty(FilterKey))
        {
            filter = Builders<NoteSchema>.Filter.Empty;
        }
        else
        {
            // mongoDb contains query
            filter = Builders<NoteSchema>.Filter.Regex(
                FilterKey,
                new BsonRegularExpression(FilterValue, "i")
            );
        }
        var findOptions = new FindOptions { BatchSize = BatchSize };

        var res = new Dictionary<string, NoteSchema>();
        using (
            var c = _globalCollection.Find(filter, findOptions).Skip(BatchSize * Page).ToCursor()
        )
        {
            if (c.MoveNext())
            {
                var d = c.Current.ToList();

                for (int i = 0; i < d.Count; i++)
                {
                    res[d[i].Id.ToString()] = d[i];
                }
            }
        }

        return Task.FromResult(new GetNotesOutput { Notes = res });
    }

    [Authorize]
    public Task<GetTagsOutput> GetTags(GetTagsReqInput input)
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var tagsCollection = db.GetCollection<NoteTags>("tags");

        var (BatchSize, Page, TagsIds, TagsName) = input;
        var findOptions = new FindOptions { BatchSize = BatchSize };
        FilterDefinition<NoteTags> filter;
        if (TagsIds != null && TagsIds.Length > 0)
        {
            filter = Builders<NoteTags>.Filter.In(
                "_id",
                TagsIds.Select(tagIds => ObjectId.Parse(tagIds))
            );
        }
        else if (TagsName != null && TagsName.Length > 0)
        {
            filter = Builders<NoteTags>.Filter.In("name", TagsName);
        }
        else
        {
            filter = Builders<NoteTags>.Filter.Empty;
        }

        var res = new Dictionary<string, NoteTags>();
        using (
            var cursor = tagsCollection.Find(filter, findOptions).Skip(BatchSize * Page).ToCursor()
        )
        {
            if (cursor.MoveNext())
            {
                var d = cursor.Current.ToList();

                for (int i = 0; i < d.Count; i++)
                {
                    res[d[i].Id.ToString()] = d[i];
                }
            }
        }

        return Task.FromResult(new GetTagsOutput { Tags = res });
    }

    [Authorize]
    public Task<GetNotesByTagsOutput> GetNotesByTags(GetNotesByTagsReqInput input)
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var _globalCollection = db.GetCollection<NoteSchema>("collection");

        var (BatchSize, Page, TagsIds) = input;
        FilterDefinition<NoteSchema> filter = Builders<NoteSchema>.Filter.In("tags", TagsIds);
        var findOptions = new FindOptions { BatchSize = BatchSize };

        var notes = new Dictionary<string, NoteSchema>();
        using (
            var cursor = _globalCollection
                .Find(filter, findOptions)
                .Skip(BatchSize * Page)
                .ToCursor()
        )
        {
            if (cursor.MoveNext())
            {
                var d = cursor.Current.ToList();

                for (int i = 0; i < d.Count; i++)
                {
                    notes[d[i].Id.ToString()] = d[i];
                }
            }
        }

        return Task.FromResult(new GetNotesByTagsOutput { notes = notes });
    }
}
