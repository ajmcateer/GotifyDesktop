using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class DatabaseContextFactory
    {
        public DatabaseContext CreateContext()
        {
            DatabaseContext context = new DatabaseContext();
            return context;
        }
    }
}
