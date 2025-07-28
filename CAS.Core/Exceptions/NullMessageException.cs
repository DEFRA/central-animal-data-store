namespace CAS.Core.Exceptions
{
    public sealed class NullMessageException : DomainException
    {
        public override string Title => "Invalid Operation";
        public override string Message => null!;
        public NullMessageException() : base(string.Empty) { }
    }
}