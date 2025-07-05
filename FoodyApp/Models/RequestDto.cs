
using System.Security.AccessControl;

using Microsoft.AspNetCore.Mvc;
using static Foody.Web.Utility.SD;

namespace Foody.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }

        public string AccessToken { get; set; }

        public ContentType ContentType { get; set; } = ContentType.Json;    


    }
}
