using Godius.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godius.Data
{
    public class RankContext : DbContext
    {
        public RankContext(DbContextOptions<RankContext> options) : base(options)
        {
        }

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<WeeklyRank> WeeklyRanks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guild>().ToTable("Guild");
            modelBuilder.Entity<Character>().ToTable("Character");
            modelBuilder.Entity<Rank>().ToTable("Rank");
            modelBuilder.Entity<WeeklyRank>().ToTable("WeeklyRank");
        }
    }
}
