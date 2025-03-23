using Serilog;
using FluxoCaixa.Application;
using FluxoCaixa.Infra;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Service
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Iniciando o serviço Worker...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "O serviço Worker encontrou um erro e foi encerrado.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    #region Services DI
                    ServiceDependencyRegister.RegisterServices(services);
                    ApplicationDependencyRegister.RegisterServices(services);
                    InfraDependencyRegister.RegisterServices(services, hostContext.Configuration);
                    #endregion

                    #region AutoMapper
                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                    #endregion
                });
    }
}