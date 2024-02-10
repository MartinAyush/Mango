namespace Mango.Web.Services.Interface
{
    public interface ITokenProvider
    {
        string? GetToken();
        void SetToken(string token);
        void ClearToken();
    }
}
