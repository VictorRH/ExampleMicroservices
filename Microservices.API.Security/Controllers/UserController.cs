using MediatR;
using Microservices.API.Security.Aplication;
using Microservices.API.Security.Core.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Microservices.API.Security.Controllers
{

    public class UserController : MyControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(Register.UserRegisterCommand data)
        {
            return await Mediator.Send(data);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(Login.UserLoginCommand data)
        {
            return await Mediator.Send(data);
        }
        [Authorize(Roles = "Admin,Demo")]
        [HttpGet]
        public async Task<ActionResult<UserDto>> CurrentUser()
        {
            return await Mediator.Send(new CurrentUser.CurrentUserCommand());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("createRol")]
        public async Task<ActionResult<Unit>> CreateRol(CreateRol.CreateRolCommand data)
        {
            return await Mediator.Send(data);
        }
    }
}
