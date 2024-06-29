using AutoMapper;
using test.Interfaces;
using test.Models;

namespace test.AutoMapperConfigurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CommonControl, object>()
                .ForMember("Key", opt => opt.MapFrom(src => src.Key))
                .ForMember("Type", opt => opt.MapFrom(src => src.Type))
                .As<ICurrentControl>(); // Configure as ICurrentControl
        }
    }
}
