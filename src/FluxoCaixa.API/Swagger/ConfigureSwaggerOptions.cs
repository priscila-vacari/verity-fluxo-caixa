using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.API.Swagger
{
    /// <summary>
    /// Classe de configuração de swagger
    /// </summary>
    /// <param name="apiVersionDescriptionProvider"></param>
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider = apiVersionDescriptionProvider;

        /// <summary>
        /// Configura descrições das versões
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options) {
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo { 
                    Title = $"MS-FluxoCaixa Api - {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = $"MS-FluxoCaixa Api - version {description.ApiVersion}",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact {
                        Name = "Priscila M. F. Vacari",
                        Email = "priscilamonteirof@hotmail.com",
                        Url = new Uri("https://www.linkedin.com/in/priscila-monteiro-f-vacari-91817840")
                    }
                });
            }
        }
    }
}
