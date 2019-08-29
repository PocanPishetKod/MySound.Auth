using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySound.Auth.EF.Context;
using MySound.Auth.IdentityServer;
using MySound.Auth.Models.Domain;

namespace MySound.Auth.Api
{
    public class DbInitializer
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly AuthDbContext _userDbContext;
        private readonly Config _config;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ConfigurationDbContext configurationDbContext, PersistedGrantDbContext persistedGrantDbContext,
            AuthDbContext userDbContext, Config config, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _configurationDbContext = configurationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _userDbContext = userDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            _userDbContext.Database.Migrate();
            _configurationDbContext.Database.Migrate();
            _persistedGrantDbContext.Database.Migrate();

            _configurationDbContext.Database.BeginTransaction();

            try
            {
                if (!_userDbContext.Roles.Any(x => x.Name == "admin"))
                {
                    var role = new IdentityRole("admin");
                    var result = _roleManager.CreateAsync(role).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Ошибка добавления роли: {role.Name}");
                    }
                }

                if (!_userDbContext.Users.Any(x => x.UserName == "admin"))
                {
                    var user = _userManager.FindByNameAsync("admin").Result;
                    if (user == null)
                    {
                        user = new User()
                        {
                            UserName = "admin",
                            Email = "themarcus77@yandex.ru"
                        };

                        var userResult = _userManager.CreateAsync(user, "qwerty").Result;
                        if (!userResult.Succeeded)
                        {
                            throw new Exception($"Ошибка добавления пользователя: {user.UserName}");
                        }
                    }

                    if (!_userManager.IsInRoleAsync(user, "admin").Result)
                    {
                        var roleResult = _userManager.AddToRoleAsync(user, "admin").Result;
                        if (!roleResult.Succeeded)
                        {
                            throw new Exception($"Ошибка добавления пользователя {user.UserName} в роль admin");
                        }
                    }

                    var claims = new Claim[]
                    {
                            new Claim(JwtClaimTypes.NickName, "admin_nick"),
                            new Claim(JwtClaimTypes.Role, "admin"),
                            new Claim(JwtClaimTypes.Email, "themarcus77@yandex.ru")
                    };

                    var userClaims = _userManager.GetClaimsAsync(user).Result;
                    var claimsForAdd = claims.Except(userClaims);

                    foreach (var claim in claimsForAdd)
                    {
                        var claimResult = _userManager.AddClaimAsync(user, claim).Result;
                        if (!claimResult.Succeeded)
                        {
                            throw new Exception($"Ошбка добавления клейма {claim.Type} поьзователю {user.UserName}");
                        }
                    }
                }

                foreach (var identityResource in _config.GetIdentityResources())
                {
                    if (!_configurationDbContext.IdentityResources.Any(x => x.Name == identityResource.Name))
                    {
                        _configurationDbContext.IdentityResources.Add(identityResource.ToEntity());
                    }
                }
                _configurationDbContext.SaveChanges();

                foreach (var apiResource in _config.GetApiResources())
                {
                    if (!_configurationDbContext.ApiResources.Any(x => x.Name == apiResource.Name))
                    {
                        _configurationDbContext.ApiResources.Add(apiResource.ToEntity());
                    }
                }
                _configurationDbContext.SaveChanges();

                foreach (var client in _config.GetClients())
                {
                    if (!_configurationDbContext.Clients.Any(x => x.ClientId == client.ClientId))
                    {
                        _configurationDbContext.Clients.Add(client.ToEntity());
                    }
                }
                _configurationDbContext.SaveChanges();
            }
            catch
            {
                _configurationDbContext.Database.RollbackTransaction();
                throw;
            }

            _configurationDbContext.Database.CommitTransaction();
        }
    }
}