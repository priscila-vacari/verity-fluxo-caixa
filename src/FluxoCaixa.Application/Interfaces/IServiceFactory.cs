namespace FluxoCaixa.Application.Interfaces
{
    public interface IServiceFactory
    {
        ILaunchService CreateLaunchService();
        IConsolidationService CreateConsolidationService();
    }
}
