using AutoMapper;
using test.Interfaces;
using test.Models;
using test.Variants;


namespace test.AutoMapperConfigurations
{
    internal class MappingProfileVar5 : Profile
    {
        public MappingProfileVar5()
        {
            //// Get the dynamically created type
            //var dynamicType = DynamicTypeFactoryVar5.CreateDynamicImplementationType();

            //// CreateMap from CommonControl to the dynamically created type
            //CreateMap<CommonControl, dynamicType>()
            //    .ForMember("Key", opt => opt.MapFrom(src => src.Key))
            //    .ForMember("Type", opt => opt.MapFrom(src => src.Type))
            //    .As(typeof(ICurrentControl)); // Configure as ICurrentControl

            CreateMap<CommonControl, ICurrentControl>()
                .ConvertUsing(src => DynamicProxyFactoryVar5.CreateProxy(src));
        }
    }
}
