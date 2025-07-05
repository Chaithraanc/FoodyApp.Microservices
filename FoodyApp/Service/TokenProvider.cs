using Foody.Web.Service.IService;
using Foody.Web.Utility;

namespace Foody.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetToken(string token)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token);
            }
        }

        public string? GetToken()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.TokenCookie, out string? token);
                return token;
            }
            return null;
        }

        public void ClearToken()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
            }
        }


    }
}
