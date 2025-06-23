using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IMethodService 
    {
        Task<object> GetAllMethods();
        Task<object> GetMethodById(int id);
        Task<object> UpdateMethod(int id, UpdateMethodDto method);
        Task<object> DeleteMethod(int id);
        Task<object> AddMethod(MethodDto method);
    }
}
