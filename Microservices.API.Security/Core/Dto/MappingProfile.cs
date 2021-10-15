using AutoMapper;
using Microservices.API.Security.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.API.Security.Core.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users, UserDto>();

        }
    }
}
