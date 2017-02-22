namespace Server.Lib.Helpers
{
    public interface IJsonHelpers
    {
        string ToJsonString(object content);
        TObject FromJsonString<TObject>(string source);
    }
}