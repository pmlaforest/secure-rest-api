using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Models;
using System;

namespace TodoApi.Models
{
    public class User : IdentityUser<Guid>
    {
        public long Id1 { get; set; }
        public string Username1 { get; set; }
        public string Password { get; set; }   
        public string Mail { get; set; }   
    }
    public class UserRoleEntity : IdentityRole<Guid>
    {
        public UserRoleEntity()
            : base()
        { }

        public UserRoleEntity(string roleName)
            : base(roleName)
        { }
    }
}