using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Foody.Services.ShoppingCartAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        // This class is used to handle authentication for backend API calls.
        // It can be extended to include logic for adding authentication headers, etc.
        // Currently, it does not implement any specific functionality.
        // You can add methods or properties as needed for your authentication logic.

        private readonly IHttpContextAccessor _accessor;
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            // You can initialize any other dependencies or properties here if needed.
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add authentication logic here if needed
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
   
}
