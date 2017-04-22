using System;
using Microsoft.EntityFrameworkCore;
using NG.Data.Spanner.EF.Extensions;
using NG.Data.Spanner.Tests.Entities;

namespace NG.Data.Spanner.Tests
{
    public class AppDb : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }


        private readonly bool _configured;

        public AppDb()
        {
        }

        public AppDb(DbContextOptions options) : base(options)
        {
            _configured = true;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configured)
                return;

            optionsBuilder.UseSpanner(Program.SpannerConnectionString, options => options.MaxBatchSize(20));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
	        modelBuilder.Entity<GamePlayer>().HasKey(pg => new { pg.GameId, pg.PlayerId });
	        modelBuilder.Entity<GamePlayer>()
		        .HasOne(g => g.Player)
		        .WithMany(p => p.GamePlayers)
		        .HasForeignKey(p => p.PlayerId);

	        modelBuilder.Entity<GamePlayer>()
		        .HasOne(g => g.Game)
		        .WithMany(p => p.GamePlayers)
		        .HasForeignKey(p => p.GameId);
		}

        public string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
