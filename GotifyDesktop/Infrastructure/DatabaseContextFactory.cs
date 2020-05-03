using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class DatabaseContextFactory
    {
        DbContextOptionsBuilder<DbContext> options;

        public DatabaseContextFactory(DbContextOptionsBuilder<DbContext> options)
        {
            this.options = options;
        }

        public DatabaseContext CreateContext()
        {
            DatabaseContext context = new DatabaseContext(options);
            return context;
        }
    }
}
