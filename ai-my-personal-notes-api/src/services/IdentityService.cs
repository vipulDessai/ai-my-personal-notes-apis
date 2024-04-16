using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GraphQLAuthDemo
{
    public interface IIdentityService
    {
        Task<string> Authenticate(string email, string password);
    }

    public class IdentityService : IIdentityService
    {
        public Task<string> Authenticate(string email, string password)
        {
            // Your custom logic here (e.g. database query)
            // Mocked for a sake of simplicity
            var roles = new List<string>();

            if (email.Contains("hr"))
            {
                roles.Add("hr");
            }

            if (email.Contains("dev"))
            {
                roles.Add("dev");
            }

            if (email.Contains("leader"))
            {
                roles.Add("leader");
            }

            if (email.Contains("employee"))
            {
                roles.Add("employee");
            }

            if (roles.Count > 0)
            {
                var token = GenerateAccessToken(email, Guid.NewGuid().ToString(), roles.ToArray());
                return Task.FromResult(token);
            }

            throw new AuthenticationException();
        }

        private string GenerateAccessToken(string email, string userId, string[] roles)
        {
            try
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        $"{Environment.GetEnvironmentVariable("IDENTITY_SERVER_SECRET")}"
                    )
                );

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, email)
                };

                claims = claims
                    .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))
                    .ToList();

                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var handler = new JwtSecurityTokenHandler();

                var token = handler.CreateJwtSecurityToken(
                    "issuer",
                    "audience",
                    new ClaimsIdentity(claims),
                    expires: DateTime.Now.AddDays(90),
                    signingCredentials: signingCredentials
                );

                return handler.WriteToken(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return "jwt creation failed";
            }
        }
    }
}
