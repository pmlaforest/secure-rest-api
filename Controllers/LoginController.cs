using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

/*  VALIDATE MAIL AND USERNAME */

namespace TodoApi.Controllers
{
    [Route("api/login")]
    public class loginController : Controller
    {
        public loginController(UserTodoContext context)
        {
        }      

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;

        }
    
        [HttpPost]
        public string Login([FromBody] User user)
        {
            if (user == null)
            {
                return "No user entered";
            }

            /* Set the connected user if authentication succeeded */
            if (!DataAccess.VerifyLogin(user))
                return "Username or password incorrect";
            
            DataAccess.connectUserToDB(user.Id1);

             return "User Connected !";
        }
    }
}