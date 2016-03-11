using System;

namespace Scrumfish.WebApi.v2.Objects
{
    public class MissingApiConfigEntryException : Exception
    {
        public MissingApiConfigEntryException()
            : base()
        {
            
        }

        public MissingApiConfigEntryException(string message)
            : base(message)
        {
            
        }
    }
}
