using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;

namespace TodoApi.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        public UsersController(UserTodoContext context)
        {
        }      
        
        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return DataAccess.GetUsersList();
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetById(long id)
        {
           var item = DataAccess.GetUser(id);
          
           if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        } 

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            const int pass_min_char = 8;

            if (user == null)
            {
                return BadRequest();
            }

            if (user.Password.Length < pass_min_char) {
                return BadRequest();
            }
            DataAccess.AddUser(user);

            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public IActionResult Post([FromBody] User user)
        {
            var userID = DataAccess.GetConnectedUser();
            if(userID == -1)
            {
                return Unauthorized();
            }

            var claims = new[] {new Claim(JwtRegisteredClaimNames.Sub, user.UserName) };

            var token = new JwtSecurityToken
                (
                    issuer: "http://localhost",
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(60),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("System.ArgumentOutOfRangeException")),
                    SecurityAlgorithms.HmacSha256)
                );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] User user)
        {
            if (user == null || user.Id1 != id)
            {
                return BadRequest();
            }

            User updated_user = null;

            updated_user.Id1 = id;
            updated_user.Username1 = user.Username1;
            updated_user.Password = user.Password;
            updated_user.Mail     = user.Mail;

            bool ret_Data_Access = DataAccess.UpdateUser(updated_user);

            if (ret_Data_Access == false) 
            {
                 return NotFound();
            }

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {

            var todo = DataAccess.GetUser(id);
            
            if (todo == null)
            {
                return NotFound();
            }

            DataAccess.DeleteUser(id);
            return new NoContentResult();
        }
    }
}