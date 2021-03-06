using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClientData.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ClientData.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        //To implement custom password hasher - to store password without encryption
        private CustomPasswordHasher customPasswordHasher;
        private readonly TokenGenerator tokenGenerator;
        public AuthenticateController(UserManager<ApplicationUser> userManager, TokenGenerator tokenGenerator)
        {
            this.userManager = userManager;
            this.customPasswordHasher = new CustomPasswordHasher();
            userManager.PasswordHasher = customPasswordHasher;
            this.tokenGenerator = tokenGenerator;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if(user!=null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var cnt = userRoles.Count;
                var token = tokenGenerator.GenerateToken(user.UserName, userRoles[0]);
                return Ok(token);
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aaaaaaaaaaaaaaaa"));
                //var roleName = userRoles[0];
                //var tokenDescriptor = new SecurityTokenDescriptor()
                //{
                //    Subject= new ClaimsIdentity(
                //        new Claim[]
                //        {
                //            new Claim(ClaimTypes.Name, user.UserName),
                //            new Claim(ClaimTypes.Role, roleName)
                            
                //        }),
                //    Audience = "ExamProc2020",
                //    Issuer = "ExamProcAPI",

                //    Expires = DateTime.Now.AddHours(3),
                //    SigningCredentials= new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                //};
                //var token = tokenHandler.CreateToken(tokenDescriptor);
                //var tokenString = tokenHandler.WriteToken(token);
                //return Ok(
                //    new
                //    {
                      
                //        token = tokenString,
                //        expiration= token.ValidTo,
                //        roles = roleName
                //    });
            }
            return Unauthorized();
        }
        
    }
}