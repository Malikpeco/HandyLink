using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when the user does not have
    /// permission to perform the requested action.
    /// </summary>
    public class HandyLinkForbiddenException : Exception
    {
        public HandyLinkForbiddenException(string message) : base(message) 
        {
        }
    }
}
