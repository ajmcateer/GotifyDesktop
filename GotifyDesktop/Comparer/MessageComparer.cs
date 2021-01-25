using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GotifyDesktop.Comparer
{
    public class MessageComparer : IEqualityComparer<MessageModel>
    {
        public bool Equals([AllowNull] MessageModel x, [AllowNull] MessageModel y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null) ||
                object.ReferenceEquals(y, null))
            {
                return false;
            }
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] MessageModel obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj.Id.GetHashCode();
        }
    }
}
