using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using GraphQLAuthDemo;
using HotChocolate.Authorization;
using MongoDB.Driver;

namespace ai_my_personal_notes_api;

public class Mutation
{
    public async Task<string> GetToken(
        string email,
        string password,
        [Service] IIdentityService identityService
    )
    {
        return await identityService.Authenticate(email, password);
    }

    [Authorize]
    public async Task<AddNoteOutput> AddNote(
        [GlobalState("currentUser")] CurrentUser user,
        AddNotesReqInput input
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var globalCollection = db.GetCollection<NoteSchema>("collection");

        var (note, newTags) = input;
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

        await globalCollection.InsertOneAsync(note);

        return new AddNoteOutput { Message = "Note Inserted" };
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
}
