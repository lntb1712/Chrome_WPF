using Chrome_WPF.Models.LoginDTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        public Task<Dictionary<string, object>> DecodeJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken == null)
            {
                throw new Exception("Invalid token");
            }
            var payload = jsonToken.Payload;
            var claims = new Dictionary<string, object>();
            foreach (var claim in payload)
            {
                claims[claim.Key] = claim.Value;
            }
            return Task.FromResult(claims);
        }

        public async Task<string> GetName(string token)
        {
            var claims = await DecodeJWT(token); // Await the DecodeJWT method to get the result
            if (claims != null && claims.ContainsKey("name"))
            {
                return claims["name"].ToString()!;
            }
            return string.Empty;
        }

        public async Task<List<string>> GetPermissionFromToken(string token)
        {
            var claims = await DecodeJWT(token); // Await the DecodeJWT method to get the result
            if (claims != null && claims.ContainsKey("Permission"))
            {
                return JsonSerializer.Deserialize<List<string>>(claims["Permission"].ToString()!)!;
            }
            return new List<string>();
        }
    }
}
