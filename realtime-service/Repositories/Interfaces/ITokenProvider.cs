namespace realtime_service.Repositories.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync();
    }
}
