using System;
using System.Runtime.Serialization;
using EncodingNormalior.Annotations;

namespace EncodingNormalior
{
    public class ArgumentCommadnException : ArgumentException
    {
        public ArgumentCommadnException()
        {
        }

        public ArgumentCommadnException(string message) : base(message)
        {
        }

        public ArgumentCommadnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArgumentCommadnException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        public ArgumentCommadnException(string message, string paramName) : base(message, paramName)
        {
        }

        protected ArgumentCommadnException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}