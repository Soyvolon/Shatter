using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NitroSharp.Modules
{
    public class InvalidModuleException : Exception
    {
        public InvalidModuleException() : base()
        {

        }

        public InvalidModuleException(string? message)
            : base(message)
        {

        }

        public InvalidModuleException(string? message, Exception? innerException)
            : base(message, innerException)
        {

        }

        public InvalidModuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
