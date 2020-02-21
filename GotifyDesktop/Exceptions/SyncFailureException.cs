using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Exceptions
{
    public class SyncFailureException : Exception
    {
        public override string Message
        {
            get
            {
                return "Sync Failed";
            }
        }
    }
}
