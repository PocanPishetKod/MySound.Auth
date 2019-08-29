using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MySound.Auth.Api.IdentityServer 
{
    public static class Rsa 
    {
        public static RsaSecurityKey GenerateKeys() 
        {
            var provider = RSA.Create(2048);
            return new RsaSecurityKey(provider);
        }
    }
}