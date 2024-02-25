using ai_my_personal_notes_api.Common;
using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using GraphQLAuthDemo;
using HotChocolate.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ai_my_personal_notes_api;

public class Mutation
{
    public Task<string> GetToken(
        string email,
        string password,
        [Service] IIdentityService identityService
    )
    {
        return identityService.Authenticate(email, password);
    }

    [Authorize]
    public async Task<UpdateNoteOutput> UpdateNote(
        [GlobalState("currentUser")] CurrentUser user,
        UpdateNotesReqInput input
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var globalCollection = db.GetCollection<NoteSchema>("collection");

        var (note, newTags, NoteId) = input;
        if (newTags.Length > 0)
        {
            var tagsCollection = db.GetCollection<NoteTags>("tags");
            await tagsCollection.InsertManyAsync(newTags);

            var findOptions = new FindOptions { BatchSize = newTags.Length };

            var tagsName = newTags.Select(tag => tag.Name);
            var filter = Builders<NoteTags>.Filter.In("name", tagsName);

            var tagsIdsRes = new List<NoteTags>();
            using (var c = tagsCollection.Find(filter, findOptions).ToCursor())
            {
                if (c.MoveNext())
                {
                    var d = c.Current.ToList();

                    for (int i = 0; i < d.Count; i++)
                    {
                        tagsIdsRes.Add(d[i]);
                    }
                }

                void replaceTags(List<string>? curTags)
                {
                    if (curTags != null)
                    {
                        foreach (var t in tagsIdsRes)
                        {
                            for (int i = 0; i < curTags.Count; i++)
                            {
                                if (curTags[i] == t.Name)
                                {
                                    curTags[i] = t.Id.ToString();
                                }
                            }
                        }
                    }
                }

                replaceTags(note.Tags);

                void recursivelyReplaceTagsWithIds(NoteInputs[]? curNoteInputs)
                {
                    if (curNoteInputs == null)
                        return;

                    for (int i = 0; i < curNoteInputs.Length; ++i)
                    {
                        var curNoteInput = curNoteInputs[i];
                        replaceTags(curNoteInput.Tags);

                        if (curNoteInput.ChildInputs != null && curNoteInput.ChildInputs.Length > 0)
                        {
                            recursivelyReplaceTagsWithIds(curNoteInput.ChildInputs);
                        }
                    }
                }

                recursivelyReplaceTagsWithIds(note.InputData);
            }
        }

        if (string.IsNullOrEmpty(NoteId))
        {
            await globalCollection.InsertOneAsync(note);

            return new UpdateNoteOutput { Message = "Note Inserted" };
        }
        else
        {
            var filter = Builders<NoteSchema>.Filter.Eq("_id", ObjectId.Parse(NoteId));
            var update = Builders<NoteSchema>
                .Update.Set("input_data", note.InputData)
                .Set("updated_date", note.UpdatedDate)
                .Set("tags", note.Tags)
                .Set("title", note.Title);

            var res = await globalCollection.UpdateOneAsync(filter, update);
            return new UpdateNoteOutput { Message = "Note Updated" };
        }
    }

    [Authorize]
    public async Task<DeleteTagOutput> DeleteTags(
        [GlobalState("currentUser")] CurrentUser user,
        List<string> tags
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var tagsCollection = db.GetCollection<NoteTags>("tags");
        var filter = Builders<NoteTags>.Filter.In("name", tags);
        var res = await tagsCollection.DeleteManyAsync(filter);

        return new DeleteTagOutput { Data = res, Message = "Delete operation completed" };
    }

    [Authorize]
    public async Task<DeleteNotesOutput> DeleteNotes(
        [GlobalState("currentUser")] CurrentUser user,
        DeleteNotesReqInput input
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var globalCollection = db.GetCollection<NoteSchema>("collection");

        var (NotesIds, TagsIds) = input;

        FilterDefinition<NoteSchema> filter;
        if (NotesIds != null && NotesIds.Count > 0)
        {
            filter = Builders<NoteSchema>.Filter.In(
                "_id",
                NotesIds.Select(noteId => ObjectId.Parse(noteId))
            );
        }
        else
        {
            filter = Builders<NoteSchema>.Filter.In("tags", TagsIds);
        }

        var res = await globalCollection.DeleteManyAsync(filter);

        return new DeleteNotesOutput { Data = res, Message = "Delete operation completed" };
    }

    [Authorize]
    public async Task<UpdateTagsOutput> UpdateTags(
        [GlobalState("currentUser")] CurrentUser user,
        UpdateTagsReqInput input
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var globalCollection = db.GetCollection<NoteTags>(AppConstants.DB_NAMES["TAGS_DB"]);

        var (updateTagsData, newTags) = input;

        if (updateTagsData != null)
        {
            var res = new List<UpdateNoteTagOpResult>();
            foreach (var (tagId, newTagValue) in updateTagsData)
            {
                var filter = Builders<NoteTags>.Filter.Eq("_id", ObjectId.Parse(tagId));
                var update = Builders<NoteTags>.Update.Set("name", newTagValue);

                var updateOpResult = await globalCollection.UpdateOneAsync(filter, update);

                res.Add(
                    new UpdateNoteTagOpResult()
                    {
                        MatchedCount = updateOpResult.MatchedCount,
                        ModifiedCount = updateOpResult.ModifiedCount
                    }
                );
            }

            return new UpdateTagsOutput { Message = "update tags operation completed", Data = res };
        }
        else
        {
            await globalCollection.InsertManyAsync(newTags);

            return new UpdateTagsOutput { Message = "add tags operation completed" };
        }
    }
}
