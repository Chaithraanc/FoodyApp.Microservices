using Foody.Web.Models;
using Foody.Web.Service.IService;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using static Foody.Web.Utility.SD;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Foody.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory , ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto , bool withBearer = true)
        {
            HttpRequestMessage message = new();
            HttpClient client = _httpClientFactory.CreateClient("FoodyAPI");
            
            //string userId =User.Cliams.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //message.Headers.Add("X_UserId" ,userId);

            if (requestDto.ContentType == ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }

           


            if (withBearer)
            {
                var token = _tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {token}");
                // Add Bearer token to the request header
             

            }
            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.ContentType == ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();
                foreach (var prop in requestDto.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(requestDto.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null && file.Length > 0)
                        {
                            var streamContent = new StreamContent(file.OpenReadStream());
                            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                            content.Add(streamContent, prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                   
                }
                message.Content = content;
            }
            else
            {
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");

                }
            }
                
            HttpResponseMessage? apiResponse = null;

            switch(requestDto.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default : 
                    message.Method = HttpMethod.Get;
                    break;


            }
            if (message.Content != null)
            {
                var json = await message.Content.ReadAsStringAsync();
               // Console.WriteLine("Outgoing JSON: " + json);
            }

            apiResponse = await client.SendAsync(message);
            try
            {
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };

                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };

                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Forbidden" };

                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };

                    case System.Net.HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };

                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }

            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;


            }


        }
    }
}
