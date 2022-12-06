using AutoMapper;
using TamboliyaApi.Data;
using TamboliyaLibrary.Identity;
using TamboliyaLibrary.Models;

namespace TamboliyaApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationModel, AppUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
