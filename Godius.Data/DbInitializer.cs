using Godius.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godius.Data
{
    public class DbInitializer
    {
        public static void Initialize(RankContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Guilds.Any())
            {
                return;   // DB has been seeded
            }

            var guilds = new Guild[]
            {
                new Guild { Name = "기사단" },
                new Guild { Name = "어벤져스" },
            };
            foreach (var guild in guilds)
            {
                context.Guilds.Add(guild);
            }
            context.SaveChanges();
        }
    }
}