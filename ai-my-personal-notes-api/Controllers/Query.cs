public class Query
{
    public Task<List<Book>> GetBooks([Service] Repository repository) => repository.GetBooksAsync();

    public Task<Author?> GetAuthor(GetAuthorInput input, [Service] Repository r) =>
        r.GetAuthor(input.authorId);
}
