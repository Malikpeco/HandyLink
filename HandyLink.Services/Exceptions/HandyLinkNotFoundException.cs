using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when a requested resource cannot be found.
    /// This exception is used when an entity does not exist, was deleted,
    /// or is not accessible in the current context.
    /// </summary>
    public class HandyLinkNotFoundException : Exception
    {
        public HandyLinkNotFoundException(string message) : base(message) 
        {
        }
    }
}
