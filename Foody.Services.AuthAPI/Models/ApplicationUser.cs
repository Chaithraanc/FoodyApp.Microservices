﻿using Microsoft.AspNetCore.Identity;

namespace Foody.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
