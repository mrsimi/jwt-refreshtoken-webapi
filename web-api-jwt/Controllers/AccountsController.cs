using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace web_api_jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        
        public AccountsController(IConfiguration configuration, AppDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo request)
        {
            
            if (request.Username == "Jane" && request.Password == "Password!")
            {
                var userclaim= new[] { new Claim(ClaimTypes.Name, request.Username)};
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: userclaim,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: creds);

                var _refreshTokenObj = new RefreshToken
                {
                    Username = request.Username,
                    Refreshtoken = Guid.NewGuid().ToString()
                };
                _db.RefreshTokens.Add(_refreshTokenObj);
                _db.SaveChanges();

                return Ok(new {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _refreshTokenObj.Refreshtoken
                });
            }

            return BadRequest("Could not verify username and password");

            
        }

        [HttpPost("{refreshToken}/refresh")]
        public IActionResult RefreshToken([FromRoute]string refreshToken)
        {
            var _refreshToken = _db.RefreshTokens.SingleOrDefault(m => m.Refreshtoken == refreshToken);

            if (_refreshToken == null)
            {
                return NotFound("Refresh token not found");
            }
            var userclaim = new[] { new Claim(ClaimTypes.Name, _refreshToken.Username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: userclaim,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            _refreshToken.Refreshtoken = Guid.NewGuid().ToString();
            _db.RefreshTokens.Update(_refreshToken);
            _db.SaveChanges();

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), refToken = _refreshToken.Refreshtoken });
        }
    }
}
