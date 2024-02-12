using System.Net;
using System.Text.Json;
using ai_my_personal_notes_api.Common;
using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using Amazon.Lambda.APIGatewayEvents;
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
    public async Task<AuthorPayload> AddAuthor(AuthorInput input, [Service] Repository repository)
    {
        var author = new Author(Guid.NewGuid(), input.name);
        await repository.AddAuthor(author);
        return new AuthorPayload(author);
    }

    public async Task<BookPayload> AddBook(BookInput input, [Service] Repository repository)
    {
        var author =
            await repository.GetAuthor(input.author) ?? throw new Exception("Author not found");
        var book = new Book(Guid.NewGuid(), input.title, author);
        await repository.AddBook(book);
        return new BookPayload(book);
    }

    [Authorize]
    public async Task<APIGatewayProxyResponse> AddNote(
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

                void replaceTags(List<string> curTags)
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

                replaceTags(note.Tags);

                // TODO: recursively replace the tags value for the
                // child input fields as well
                void recursivelyReplaceTagsWithIds(NoteInputs[] curNoteInputs)
                {
                    for (int i = 0; i < curNoteInputs.Length; ++i)
                    {
                        var curNoteInput = curNoteInputs[i];
                        replaceTags(curNoteInput.Tags);

                        if (curNoteInput.ChildInputs.Length > 0)
                        {
                            recursivelyReplaceTagsWithIds(curNoteInput.ChildInputs);
                        }
                    }
                }

                recursivelyReplaceTagsWithIds(note.InputData);
            }
        }

        await globalCollection.InsertOneAsync(note);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize("Note Inserted"),
            Headers = Constants.Headers,
        };

        return response;
    }

    [Authorize]
    public async Task<APIGatewayProxyResponse> DeleteTags(
        [GlobalState("currentUser")] CurrentUser user,
        List<string> tags
    )
    {
        var dbServer = new MongoDbServer();
        var db = dbServer.client.GetDatabase("db");
        var tagsCollection = db.GetCollection<NoteTags>("tags");
        var filter = Builders<NoteTags>.Filter.In("name", tags);
        var res = await tagsCollection.DeleteManyAsync(filter);

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(res),
            Headers = Constants.Headers,
        };
    }
}
