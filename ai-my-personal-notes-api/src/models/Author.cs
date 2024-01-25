public record Author(Guid Id, string Name);

public record AuthorPayload(Author record);

public record AuthorInput(string name);

public record GetAuthorInput(Guid authorId);
