using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Models;
using System;

namespace TodoApi.Models
{
    public class UserTodoContext : IdentityDbContext<User, UserRoleEntity, Guid>
    {
        public new DbSet<User> Users{ get; set; }
        public DbSet<Todo> Todos{ get; set; }

        public UserTodoContext(DbContextOptions<UserTodoContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite(@"Data Source=IdentitySample.sqlite;");
        }

    }
}