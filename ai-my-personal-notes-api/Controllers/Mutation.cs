public class Mutation
{
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
}

public record BookPayload(Book? record, string? error = null);

public record BookInput(string title, Guid author);

public record AuthorPayload(Author record);

public record AuthorInput(string name);
