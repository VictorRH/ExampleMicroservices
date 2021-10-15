using AutoMapper;
using MediatR;
using Microservices.API.Security.Core.Dto;
using Microservices.API.Security.Core.Entities;
using Microservices.API.Security.Infrastructure;
using Microservices.API.Security.Infrastructure.JwtLogic;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.API.Security.Aplication
{
    public class CurrentUser
    {
        public class CurrentUserCommand : IRequest<UserDto> { }

        public class Handler : IRequestHandler<CurrentUserCommand, UserDto>
        {
            private readonly UserManager<Users> userManager;
            private readonly IUserSession userSession;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IMapper mapper;

            public Handler(UserManager<Users> userManager, IUserSession userSession, IJwtGenerator jwtGenerator, IMapper mapper)
            {
                this.userManager = userManager;
                this.userSession = userSession;
                this.jwtGenerator = jwtGenerator;
                this.mapper = mapper;
            }

            public async Task<UserDto> Handle(CurrentUserCommand request, CancellationToken cancellationToken)
            {
                var currentUser = await userManager?.FindByNameAsync(userSession?.GetUserSession());
                var resultRols = await userManager.GetRolesAsync(currentUser);
                var listRols = new List<string>(resultRols);
                if (currentUser != null)
                {
                    var userDto = mapper.Map<Users, UserDto>(currentUser);
                    userDto.Token = jwtGenerator.CreateToken(currentUser, listRols);
                    return userDto;
                }
                throw new HandlerException(HttpStatusCode.NotFound, new { message = "Error : User not found" });
            }
        }
    }

}
