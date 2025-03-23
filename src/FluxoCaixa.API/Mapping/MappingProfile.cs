using AutoMapper;
using FluxoCaixa.API.Models;
using FluxoCaixa.Application.DTOs;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.API.Mapping
{
    /// <summary>
    /// Mapeamento de modelos
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MappingProfile: Profile
    {
        /// <summary>
        /// Perfis de mapeamento
        /// </summary>
        public MappingProfile()
        {
            CreateMap<LaunchRequestModel, LaunchDTO>().ReverseMap();
            CreateMap<LaunchResponseModel, LaunchDTO>().ReverseMap();
            CreateMap<ConsolidationResponseModel, ConsolidationDTO>().ReverseMap();
        }
    }
}
