using AutoMapper;
using rest_api_dotnet_template.Models;

namespace UserService.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, RegisterDto>().ReverseMap();
        }
    }
}