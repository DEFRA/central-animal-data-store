namespace CAS.Core.Exceptions
{
    public sealed class ValidationException : DomainException
    {
        public override string Title => "Unprocessable Content";
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}