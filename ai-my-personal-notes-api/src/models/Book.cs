public record Book(Guid Id, string Title, Author Author);

public record BookPayload(Book? record, string? error = null);

public record BookInput(string title, Guid author);
