using System;
using openDicom.DataStructure;

namespace openDicom
{
    public class DicomException : Exception
    {
        public DicomException() :
            this("An exception occurred.", DicomContext.CurrentTag,
                DicomContext.StreamPosition, null, null, null)
        {
        }

        public DicomException(Exception innerException) :
            this("An exception occurred.", DicomContext.CurrentTag,
                DicomContext.StreamPosition, null, null, innerException)
        {
        }

        public DicomException(string message) :
            this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                null, null, null)
        {
        }

        public DicomException(string message, Exception innerException) :
            this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                null, null, innerException)
        {
        }

        public DicomException(string message, long streamPosition) :
            this(message, DicomContext.CurrentTag, streamPosition, null, null,
                null)
        {
        }

        public DicomException(string message, long streamPosition,
            Exception innerException) :
                this(message, DicomContext.CurrentTag, streamPosition, null, null,
                    innerException)
        {
        }

        public DicomException(string message, Tag tag) :
            this(message, tag, DicomContext.StreamPosition, null, null, null)
        {
        }

        public DicomException(string message, Tag tag, Exception innerException) :
            this(message, tag, DicomContext.StreamPosition, null, null,
                innerException)
        {
        }

        public DicomException(string message, string paramName) :
            this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                paramName, null, null)
        {
        }

        public DicomException(string message, string paramName,
            Exception innerException) :
                this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                    paramName, null, innerException)
        {
        }

        public DicomException(string message, long streamPosition,
            string paramName) :
                this(message, DicomContext.CurrentTag, streamPosition, paramName,
                    null, null)
        {
        }

        public DicomException(string message, long streamPosition,
            string paramName, Exception innerException) :
                this(message, DicomContext.CurrentTag, streamPosition, paramName,
                    null, innerException)
        {
        }

        public DicomException(string message, Tag tag, string paramName) :
            this(message, tag, DicomContext.StreamPosition, paramName, null,
                null)
        {
        }

        public DicomException(string message, Tag tag, string paramName,
            Exception innerException) :
                this(message, tag, DicomContext.StreamPosition, paramName, null,
                    null)
        {
        }

        public DicomException(string message, string paramName,
            string paramValue) :
                this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                    paramName, paramValue, null)
        {
        }

        public DicomException(string message, string paramName,
            string paramValue, Exception innerException) :
                this(message, DicomContext.CurrentTag, DicomContext.StreamPosition,
                    paramName, paramValue, innerException)
        {
        }

        public DicomException(string message, long streamPosition,
            string paramName, string paramValue) :
                this(message, DicomContext.CurrentTag, streamPosition, paramName,
                    paramValue, null)
        {
        }

        public DicomException(string message, long streamPosition,
            string paramName, string paramValue, Exception innerException) :
                this(message, DicomContext.CurrentTag, streamPosition, paramName,
                    paramValue, innerException)
        {
        }

        public DicomException(string message, Tag tag, string paramName,
            string paramValue) :
                this(message, tag, DicomContext.StreamPosition, paramName,
                    paramValue, null)
        {
        }

        public DicomException(string message, Tag tag, string paramName,
            string paramValue, Exception innerException) :
                this(message, tag, DicomContext.StreamPosition, paramName,
                    paramValue, innerException)
        {
        }

        public DicomException(string message, Tag tag, long streamPosition,
            string paramName, string paramValue) :
                this(message, tag, streamPosition, paramName, paramValue, null)
        {
        }

        public DicomException(string message, Tag tag, long streamPosition,
            string paramName, string paramValue, Exception innerException) :
                base(message, innerException)
        {
            Tag = tag;
            if (streamPosition < -1)
                StreamPosition = -1;
            else
                StreamPosition = streamPosition;
            ParamName = paramName;
            ParamValue = paramValue;
        }

        public long StreamPosition { get; } = -1;
        public Tag Tag { get; }
        public string ParamName { get; }
        public string ParamValue { get; }

        public override string ToString()
        {
            var context = "";
            if (Tag != null)
                context += string.Format("   {0,-15} {1}\n", "Tag:", Tag);
            if (StreamPosition > -1)
                context += string.Format("   {0,-15} {1}\n", "StreamPosition:",
                    StreamPosition);
            if (ParamName != null)
                context += string.Format("   {0,-15} {1}\n", "ParamName:",
                    ParamName);
            if (ParamValue != null)
                context += string.Format("   {0,-15} {1}\n", "ParamValue:",
                    ParamValue);
            return string.Format("{0}:\n   {1}\n" +
                                 "Context:\n{2}" +
                                 "StackTrace:\n{3}",
                GetType(), Message, context, StackTrace);
        }
    }
}