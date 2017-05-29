namespace Server.Lib.Models.Other
{
    public interface IHawkSignatureFactory
    {
        IHawkSignature FromAuthorizationHeader(string header);
        IHawkSignature FromBewit(string bewit);
        //IHawkSignature FromCredentials(TentPost<TentContentCredentials> credentials);
    }
}