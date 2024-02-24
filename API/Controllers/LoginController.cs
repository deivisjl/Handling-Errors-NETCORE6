using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        [HttpPost(Name = "Index")]
        public IActionResult Index([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            return Ok(new { token = GenerateToken() });
        }

        private string GenerateToken()
        {
            try
            {
                var secretKey = "DwkdopIDAISOPDQWD59AS8D9AWD2ASD9sd59qwd";
                var key = Encoding.ASCII.GetBytes(secretKey);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "deivis"),
                new Claim(ClaimTypes.Email, "deivis@gmail.com"),
                new Claim(ClaimTypes.Name, "deivis"),
                new Claim(ClaimTypes.Surname, "lopez"),
            };

                claims.Add(new Claim(ClaimTypes.Role, "admin"));


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(tokenDescriptor);

                var s = tokenHandler.WriteToken(createdToken);

                return s;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
