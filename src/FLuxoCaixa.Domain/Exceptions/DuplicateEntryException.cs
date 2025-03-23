namespace FluxoCaixa.Domain.Exceptions
{
    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException(string message) : base(message) { }
    }
}
