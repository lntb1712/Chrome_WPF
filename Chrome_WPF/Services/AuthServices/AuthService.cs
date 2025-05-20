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
        public Task<Dictionary<string, object>> DecodeJWT(LoginResponseDTO loginResponse)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(loginResponse.Token) as JwtSecurityToken;
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

        public async Task<string> GetName(LoginResponseDTO loginResponse)
        {
            var claims = await DecodeJWT(loginResponse); // Await the DecodeJWT method to get the result
            if (claims != null && claims.ContainsKey("name"))
            {
                return claims["name"].ToString()!;
            }
            return string.Empty;
        }

        public async Task<List<string>> GetPermissionFromToken(LoginResponseDTO loginResponse)
        {
            var claims = await DecodeJWT(loginResponse); // Await the DecodeJWT method to get the result
            if (claims != null && claims.ContainsKey("Permission"))
            {
                return JsonSerializer.Deserialize<List<string>>(claims["Permission"].ToString()!)!;
            }
            return new List<string>();
        }
    }
}
