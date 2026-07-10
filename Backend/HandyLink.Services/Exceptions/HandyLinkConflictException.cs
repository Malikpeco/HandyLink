using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when a request conflicts with existing data
    /// or with the current state of the system.
    /// This exception is used when the operation cannot be completed because it would
    /// create duplicate data or conflict with an existing record/state.
    /// </summary>
    public class HandyLinkConflictException : Exception
    {
        public HandyLinkConflictException(string message) : base(message)
        {

        }
    }
}
