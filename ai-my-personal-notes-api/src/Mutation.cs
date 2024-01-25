using System.Net;
using System.Text.Json;
using ai_my_personal_notes_api.Models;
using ai_my_personal_notes_api.services;
using Amazon.Lambda.APIGatewayEvents;
using GraphQLAuthDemo;
using HotChocolate.Authorization;

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

    public async Task<APIGatewayProxyResponse> AddNote(
        [GlobalState("currentUser")] CurrentUser user,
        AddNotesReqInput input
    )
    {
        var db = new MongoDb();
        var globalCollection = db.client.GetDatabase("db").GetCollection<NoteSchema>("collection");

        var note = input.note;
        globalCollection.InsertOne(note);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize("Note Inserted"),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
