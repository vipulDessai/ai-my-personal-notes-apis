using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ai_my_personal_notes_api.Models;

public class Restaurant
{
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("restaurant_id")]
    public string RestaurantId { get; set; }

    [BsonElement("cuisine")]
    public string Cuisine { get; set; }

    [BsonElement("address")]
    public Address Address { get; set; }

    [BsonElement("borough")]
    public string Borough { get; set; }

    [BsonElement("grades")]
    public List<GradeEntry> Grades { get; set; }
}

public class GradeEntry
{
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("grade")]
    public string Grade { get; set; }

    [BsonElement("score")]
    public float Score { get; set; }
}

public class Address
{
    [BsonElement("building")]
    public string Building { get; set; }

    [BsonElement("coord")]
    public double[] Coordinates { get; set; }

    [BsonElement("street")]
    public string Street { get; set; }

    [BsonElement("zipcode")]
    public string ZipCode { get; set; }
}

public record GetRestuarantsInput(int BatchSize, string FilterKey, string FilterValue);
