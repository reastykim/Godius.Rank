using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Godius.Data.Models
{
    public class Character
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name = "캐릭터명")]
        public string Name { get; set; }

        [Display(Name = "활성화 여부")]
        [DefaultValue(true)]
        public bool IsActivated { get; set; } = true;

        [ForeignKey("GuildId")]
        [Display(Name = "길드")]
        public Guild Guild { get; set; }
        [Display(Name = "길드")]
        public Guid GuildId { get; set; }

        [Display(Name = "길드 직급")]
        public GuildPositions? GuildPosition { get; set; }

        public ICollection<Rank> Ranks { get; set; }

        public ICollection<WeeklyRank> WeeklyRanks { get; set; }
    }
}