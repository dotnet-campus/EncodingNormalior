using System;

namespace EncodingNormalior
{
    class EncodingNormaliorException : Exception
    {
        public EncodingNormaliorException()
        {
        }

        public EncodingNormaliorException(string message) : base(message)
        {
        }

        public EncodingNormaliorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}