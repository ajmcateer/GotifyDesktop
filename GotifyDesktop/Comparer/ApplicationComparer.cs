using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GotifyDesktop.Comparer
{
    public class ApplicationComparer : IEqualityComparer<ApplicationModel>
    {
        public bool Equals([AllowNull] ApplicationModel x, [AllowNull] ApplicationModel y)
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
            return x.id == y.id;
        }

        public int GetHashCode([DisallowNull] ApplicationModel obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj.id.GetHashCode();
        }
    }
}
