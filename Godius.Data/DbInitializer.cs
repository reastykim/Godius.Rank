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
            context.SaveChanges();
        }
    }
}