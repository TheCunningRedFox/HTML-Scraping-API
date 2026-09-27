namespace TestAssignment.Exceptions
{
    public sealed class Base64DecodeException : Exception
    {
        public string ErrorCode { get; }

        public Base64DecodeException(
            string errorCode,
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
