using System;
using System.Runtime.Serialization;

namespace ScarletLock
{
    public class IdentityGenerationException : Exception
    {
        public IdentityGenerationException() { }

        public IdentityGenerationException(string message) 
            : base(message) { }

        public IdentityGenerationException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected IdentityGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
