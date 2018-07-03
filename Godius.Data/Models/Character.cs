using System;
using System.Collections.Generic;
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

        [ForeignKey("GuildId")]
        [Display(Name = "길드")]
        public Guild Guild { get; set; }
        [Display(Name = "길드")]
        public Guid GuildId { get; set; }

        [Display(Name = "길드 직급")]
        public GuildPositions? GuildPosition { get; set; }

        public ICollection<Rank> Ranks { get; set; }
    }
}