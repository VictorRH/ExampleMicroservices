using AutoMapper;
using FluentValidation;
using MediatR;
using Microservices.API.Security.Core.Dto;
using Microservices.API.Security.Core.Entities;
using Microservices.API.Security.Core.Persistence;
using Microservices.API.Security.Infrastructure.JwtLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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

        }

        public class FluentValidation : AbstractValidator<UserRegisterCommand>
        {
            public FluentValidation()
            {
                RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().WithMessage("Error: Enter valid email");
                RuleFor(x => x.Username).NotEmpty().NotNull();
                RuleFor(x => x.Password).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<UserRegisterCommand, UserDto>
        {
            private readonly SecurityDbContext context;
            private readonly UserManager<Users> userManager;
            private readonly IMapper mapper;
            private readonly IJwtGenerator jWtGenerator;

            public Handler(SecurityDbContext context, UserManager<Users> userManager, IMapper mapper, IJwtGenerator jWtGenerator)
            {
                this.context = context;
                this.userManager = userManager;
                this.mapper = mapper;
                this.jWtGenerator = jWtGenerator;
            }
            public async Task<UserDto> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
            {
                var validationEmail = await context.Users.Where(x => x.Email == request.Email || x.UserName == request.Username).AnyAsync(cancellationToken);
                if (validationEmail)
                {
                    throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Error: email or username already exists in the database" });
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
                    var userDto = mapper.Map<Users, UserDto>(user);
                    userDto.Token = jWtGenerator.CreateToken(user);
                    return userDto;
                }

                throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Erro: Failed register user" });
            }
        }
    }
}
