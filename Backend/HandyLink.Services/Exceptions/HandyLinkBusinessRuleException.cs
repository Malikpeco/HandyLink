using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when a request violates an application
    /// or domain business rule.
    /// </summary>
    public class HandyLinkBusinessRuleException : Exception
    {
        public HandyLinkBusinessRuleException(string message) : base(message)
        {
            
        }
    }
}
