namespace twitter.api.Repositories
{
    public interface ITokenRepository
    {
        void GenerateTokenAndSetCookie(Guid userId, HttpResponse response);
    }
}
