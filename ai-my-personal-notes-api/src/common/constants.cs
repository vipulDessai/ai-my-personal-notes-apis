namespace ai_my_personal_notes_api.Common;

public class AppConstants
{
    public static Dictionary<string, string> DB_NAMES = new Dictionary<string, string>
    {
        { "TAGS_DB", "tags" },
        { "NOTES_DB", "collection" },
    };
    public static Dictionary<string, string> Headers = new Dictionary<string, string>
    {
        { "Content-Type", "text/plain" }
    };
}
