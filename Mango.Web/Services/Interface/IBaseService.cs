using Mango.Web.Models;

namespace Mango.Web.Services.Interface
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto);
    }
}
