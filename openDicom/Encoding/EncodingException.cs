using System;
using openDicom.DataStructure;

namespace openDicom.Encoding
{
    public class EncodingException : DicomException
    {
        public EncodingException(string paramName, string paramValue) :
            base("An encoding exception occurred.", paramName, paramValue,
                null)
        {
        }

        public EncodingException(string paramName, string paramValue,
            Exception innerException) :
                base("An encoding exception occurred.", paramName, paramValue,
                    innerException)
        {
        }

        public EncodingException(string message, string paramName,
            string paramValue) :
                base(message, paramName, paramValue)
        {
        }

        public EncodingException(string message, string paramName,
            string paramValue, Exception innerException) :
                base(message, paramName, paramValue, innerException)
        {
        }

        public EncodingException(string message, long streamPosition,
            string paramName, string paramValue) :
                base(message, streamPosition, paramName, paramValue)
        {
        }

        public EncodingException(string message, long streamPosition,
            string paramName, string paramValue, Exception innerException) :
                base(message, streamPosition, paramName, paramValue,
                    innerException)
        {
        }

        public EncodingException(string message, Tag tag, string paramName,
            string paramValue) :
                base(message, tag, paramName, paramValue)
        {
        }

        public EncodingException(string message, Tag tag, string paramName,
            string paramValue, Exception innerException) :
                base(message, tag, paramName, paramValue, innerException)
        {
        }

        public EncodingException(string message, Tag tag, long streamPosition,
            string paramName, string paramValue) :
                base(message, tag, streamPosition, paramName, paramValue)
        {
        }

        public EncodingException(string message, Tag tag, long streamPosition,
            string paramName, string paramValue, Exception innerException) :
                base(message, tag, streamPosition, paramName, paramValue,
                    innerException)
        {
        }
    }
}