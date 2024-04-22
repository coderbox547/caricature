using AutoMapper;
using Data.Contract.Request;
using Data.Contract.Response;
using Data.Domain;
namespace Api.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {

         
            CreateMap<UserResponse, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, UserRequest>().ReverseMap();
        }

    }
}
