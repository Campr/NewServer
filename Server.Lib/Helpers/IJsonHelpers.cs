namespace Server.Lib.Helpers
{
    public interface IJsonHelpers
    {
        string ToJsonString(object obj);
        T FromJsonString<T>(string src);
    }
}