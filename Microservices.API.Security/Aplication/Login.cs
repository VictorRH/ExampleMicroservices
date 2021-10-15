using AutoMapper;
using FluentValidation;
using MediatR;
using Microservices.API.Security.Core.Dto;
using Microservices.API.Security.Core.Entities;
using Microservices.API.Security.Core.Persistence;
using Microservices.API.Security.Infrastructure.JwtLogic;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.API.Security.Aplication
{
    public class Login
    {
        public class UserLoginCommand : IRequest<UserDto>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class FluentValidation : AbstractValidator<UserLoginCommand>
        {
            public FluentValidation()
            {
                RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().WithMessage("Enter valid email");
                RuleFor(x => x.Password).NotEmpty().NotNull();
            }
        }

        public class Handler : IRequestHandler<UserLoginCommand, UserDto>
        {
            private readonly SecurityDbContext context;
            private readonly UserManager<Users> userManager;
            private readonly IMapper mapper;
            private readonly IJwtGenerator jwtGenerator;
            private readonly SignInManager<Users> sigInManager;

            public Handler(SecurityDbContext context, UserManager<Users> userManager, IMapper mapper, IJwtGenerator jwtGenerator, SignInManager<Users> sigInManager)
            {
                this.context = context;
                this.userManager = userManager;
                this.mapper = mapper;
                this.jwtGenerator = jwtGenerator;
                this.sigInManager = sigInManager;
            }
            public async Task<UserDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                var validationUser = await userManager.FindByEmailAsync(request.Email);
                if (validationUser == null)
                {
                    throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Error: email or password incorrect" });
                }

                var result = await sigInManager.CheckPasswordSignInAsync(validationUser, request.Password, false);
                if (result.Succeeded)
                {
                    var userDto = mapper.Map<Users, UserDto>(validationUser);
                    userDto.Token = jwtGenerator.CreateToken(validationUser);
                    return userDto;
                }

                throw new HandlerException(HttpStatusCode.BadRequest, new { message = "Error: email or password incorrect" });
            }
        }
    }
}
