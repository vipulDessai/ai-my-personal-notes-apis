using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ai_my_personal_notes_api.Models;

public class NoteInputs
{
    [BsonElement("value")]
    public string Value { get; set; }

    [BsonElement("child_inputs")]
    public NoteInputs[] ChildInputs { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; }
}

public class NoteSchema
{
    [GraphQLIgnore]
    public ObjectId Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("input_data")]
    public NoteInputs InputData { get; set; }
}

public record AddNotesReqInput(NoteSchema note);

public record GetNotesReqInput(int BatchSize, string? FilterKey, string? FilterValue);
