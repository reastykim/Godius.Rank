using Godius.Data.Helper;
using Godius.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godius.RankSite.Helpers
{
    public class GuildPositionsToImageConverter
    {
        public static string GetPosisionImage(GuildPositions guildPosition)
        {
            var displayName = EnumHelper.GetEnumDisplay(guildPosition);

            switch (guildPosition)
            {
                case GuildPositions.Master:
                    return $"<img src='images/positions/0.jpg' alt='{displayName}' class='img-responsive' align='left'/>";
                case GuildPositions.SubMaster:
                    return $"<img src='images/positions/4.jpg' alt='{displayName}' class='img-responsive' align='left'/>";
                case GuildPositions.Journeyman:
                    return $"<img src='images/positions/1.jpg' alt='{displayName}' class='img-responsive' align='left'/>";
                case GuildPositions.Apprentice:
                    return $"<img src='images/positions/2.jpg' alt='{displayName}' class='img-responsive' align='left'/>";
                case GuildPositions.Newbie:
                default:
                    return $"<img src='images/positions/3.jpg' alt='{displayName}' class='img-responsive' align='left'/>";
            }
            
        }
    }
}
