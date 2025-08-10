using realtime_service.Services.External.Models.CommonModel;
using realtime_service.Services.External.Models.UserServiceModel;
using Refit;

namespace realtime_service.Services.External
{
    [Headers("Accept: application/json")]
    public interface IUserService
    {
        [Get("/users/get-user-by-id")]
        Task<ApiResponseHandlder<UserModel>> GetUserByIdAsync([AliasAs("id")] int userId);
    }
}
