using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ScarletLock
{
    public class BlockedException : Exception
    {
        public BlockedException() { }

        public BlockedException(string message)
            : base(message)
        { }

        public BlockedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected BlockedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
