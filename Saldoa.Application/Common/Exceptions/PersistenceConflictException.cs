namespace Saldoa.Application.Common.Exceptions
{
    public sealed class PersistenceConflictException : Exception
    {
        public PersistenceConflictException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
