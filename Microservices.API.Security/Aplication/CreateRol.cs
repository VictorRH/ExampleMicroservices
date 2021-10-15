using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.API.Security.Aplication
{
    public class CreateRol
    {
        public class CreateRolCommand : IRequest
        {
            public string NameRol { get; set; }
        }
        public class FluentValidation : AbstractValidator<CreateRolCommand>
        {
            public FluentValidation()
            {
                RuleFor(x => x.NameRol).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<CreateRolCommand>
        {
            private readonly RoleManager<IdentityRole> roleManager;

            public Handler(RoleManager<IdentityRole> roleManager)
            {
                this.roleManager = roleManager;
            }

            public async Task<Unit> Handle(CreateRolCommand request, CancellationToken cancellationToken)
            {
                var roleUser = await roleManager.FindByNameAsync(request.NameRol);
                if (roleUser != null)
                {
                    throw new HandlerException(HttpStatusCode.NotFound, new { message = "The role already exists" });
                }

                var result = await roleManager.CreateAsync(new IdentityRole(request.NameRol));
                if (result.Succeeded)
                {
                    return Unit.Value;
                }

                throw new Exception("Error create Rol User");
            }
        }
    }
}
