namespace Server.Lib.Helpers
{
    public interface IJsonHelpers
    {
        string ToJsonString(object obj);
        TObject FromJsonString<TObject>(string src);
    }
}