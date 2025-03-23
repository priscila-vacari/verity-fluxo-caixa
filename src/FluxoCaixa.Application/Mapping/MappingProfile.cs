using AutoMapper;
using FluxoCaixa.Application.DTOs;
using FluxoCaixa.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Application.Mapping
{
    [ExcludeFromCodeCoverage]
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<LaunchDTO, Launch>().ReverseMap();
            CreateMap<ConsolidationDTO, Consolidation>().ReverseMap();
        }
    }
}
