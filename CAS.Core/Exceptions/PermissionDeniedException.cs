namespace CAS.Core.Exceptions
{
    public sealed class PermissionDeniedException : DomainException
    {
        public override string Title => "Forbidden";
        public PermissionDeniedException(string message) : base(message) { }
        public PermissionDeniedException(string message, Exception innerException) : base(message, innerException) { }
    }
}