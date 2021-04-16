using AutoMapper;
using PosCFG.Dto;
using PosCFG.Models;

namespace PosCFG
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SystemPOS,GetSystemPOSDto>();
            CreateMap<UpdateSystemPOSDto,GetSystemPOSDto>();
            CreateMap<SystemPOS,UpdateSystemPOSDto>();
            CreateMap<UpdateSystemPOSDto,SystemPOS>();
            
            CreateMap<AddSystemPOSDto,SystemPOS>();
            CreateMap<UpdateTerminalDto,GetTerminalDto>();
            CreateMap<Terminal,UpdateTerminalDto>();
            CreateMap<Terminal,GetTerminalDto>();
            CreateMap<GetTerminalDto,Terminal>();
            CreateMap<Terminal,GetTerminalsExternalDto>();
            CreateMap<CargaMasivaAux,GetCargaMasivaDto>();
            CreateMap<GetCargaMasivaDto,CargaMasivaAux>();
            
        }
    }
}