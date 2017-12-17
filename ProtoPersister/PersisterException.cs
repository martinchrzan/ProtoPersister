using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Proto
{
    public class PersisterException : Exception
    {
        public PersisterException()
        {
        }

        public PersisterException(string message) : base(message)
        {
        }

        public PersisterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
