using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Domain.Entities;

namespace FluxoCaixa.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<LaunchDTO, Launch>().ReverseMap();
            CreateMap<ConsolidationDTO, Consolidation>().ReverseMap();
        }
    }
}
