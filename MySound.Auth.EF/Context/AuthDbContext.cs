using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySound.Auth.Models.Domain;

namespace MySound.Auth.EF.Context
{
    public class AuthDbContext : IdentityDbContext<Account>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            
        }
    }
}