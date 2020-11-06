using System;
using System.Runtime.Serialization;

using WireC.Common;

// ReSharper disable UnusedMember.Local

namespace WireC.FrontEnd
{
    public class ParseException : Exception
    {
        public ParseException(SourceSpan span, string message) : base(message)
        {
            Span = span;
        }

        private ParseException()
        {
        }

        private ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private ParseException(string message) : base(message)
        {
        }

        private ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SourceSpan Span { get; }
    }
}