using GotifyDesktop.Models;
using gotifySharp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ApplicationModel> Applications { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ServerInfo> Server { get; set; }

        public DatabaseContext(DbContextOptionsBuilder<DbContext> options) : base(options.Options) { }
    }
}
