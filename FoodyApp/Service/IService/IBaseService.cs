using Foody.Web.Models;

namespace Foody.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto , bool withBearer = true);   
    }
}
