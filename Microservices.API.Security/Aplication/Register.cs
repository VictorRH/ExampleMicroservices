using FluentValidation;
using MediatR;
using Microservices.API.Security.Core.Dto;
using Microservices.API.Security.Core.Entities;
using Microservices.API.Security.Core.Persistence;
using Microservices.API.Security.Infrastructure.JwtLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.API.Security.Aplication
{
    public class Register
    {
        public class UserRegisterCommand : IRequest<UserDto>
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string RolUser { get; set; }
        }

        public class FluentValidation : AbstractValidator<UserRegisterCommand>
        {
            public FluentValidation()
            {
                RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().WithMessage("Error: Enter valid email");
                RuleFor(x => x.Username).NotEmpty().NotNull();
                RuleFor(x => x.Password).NotEmpty().NotNull();
                RuleFor(x => x.RolUser).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<UserRegisterCommand, UserDto>
        {
            private readonly SecurityDbContext context;
            private readonly UserManager<Users> userManager;
            private readonly IJwtGenerator jWtGenerator;
            private readonly RoleManager<IdentityRole> roleManager;

            public Handler(SecurityDbContext context, UserManager<Users> userManager, IJwtGenerator jWtGenerator, RoleManager<IdentityRole> roleManager)
            {
                this.context = context;
                this.userManager = userManager;
                this.jWtGenerator = jWtGenerator;
                this.roleManager = roleManager;
            }
            public async Task<UserDto> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
            {
                var validationEmail = await context.Users.Where(x => x.Email == request.Email || x.UserName == request.Username).AnyAsync(cancellationToken);
                if (validationEmail)
                {
                    throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Error: email or username already exists in the database" });
                }
                var roleUser = await roleManager.FindByNameAsync(request.RolUser);
                if (roleUser == null)
                {
                    throw new HandlerException(HttpStatusCode.NotFound, new { message = "Error: rol not found or invalid" });
                }
                var user = new Users
                {
                    Email = request.Email,
                    UserName = request.Username,
                    RegisterDate = DateTime.UtcNow

                };

                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    var userIdentityToRol = await userManager.FindByNameAsync(request.Username);
                    var resultadoRoles = await userManager.GetRolesAsync(userIdentityToRol);
                    var rolUserAdd = await userManager.AddToRoleAsync(userIdentityToRol, request.RolUser);
                    var listRoles = new List<string>(resultadoRoles);
                    return new UserDto
                    {
                        UserName = request.Username,
                        Email = request.Email,
                        Token = jWtGenerator.CreateToken(user, listRoles),
                        RefreshToken = jWtGenerator.CreateToken(user, listRoles),

                    };

                }

                throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Erro: Failed register user" });
            }
        }
    }
}
