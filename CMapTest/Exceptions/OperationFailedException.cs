using System.Runtime.Serialization;

namespace CMapTest.Exceptions
{
    public sealed class OperationFailedException : Exception
    {
        public OperationFailedException()
        {
        }

        public OperationFailedException(string? message) : base(message)
        {
        }

        public OperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OperationFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
