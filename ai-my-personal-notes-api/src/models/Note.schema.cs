using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ai_my_personal_notes_api.Models;

public class NoteInputs
{
    [BsonElement("value")]
    public string? Value { get; set; }

    [BsonElement("child_inputs")]
    public NoteInputs[]? ChildInputs { get; set; }

    [BsonElement("tags")]
    public List<string>? Tags { get; set; }
}

public class NoteSchema
{
    [GraphQLIgnore]
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("tags")]
    public List<string>? Tags { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("input_data")]
    public NoteInputs[]? InputData { get; set; }
}

public class NoteTags
{
    [GraphQLIgnore]
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }
}

public record AddNotesReqInput(NoteSchema Note, NoteTags[] NewTags);

public class AddNoteOutput
{
    public string Message { get; set; } = "Note adding failed";
}

public record GetNotesReqInput(int BatchSize, string? FilterKey, string? FilterValue);

public class GetNotesOutput
{
    public List<NoteSchema> Notes { get; set; } = new List<NoteSchema>();
}

public record GetTagsReqInput(int BatchSize, string[]? TagsIds, string[]? TagsName);

public class GetTagsOutput
{
    public List<NoteTags> Tags { get; set; } = new List<NoteTags>();
}

public class DeleteTagOutput
{
    public string Message { get; set; } = "Note adding failed";
    public DeleteResult? Data { get; set; }
}

public record GetNotesByTagsReqInput(int BatchSize, string[]? TagsIds);

public class GetNotesByTagsOutput
{
    public List<NoteSchema> notes { get; set; } = new List<NoteSchema>();
}
