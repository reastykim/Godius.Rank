using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Godius.Data.Models
{
    public class Guild
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name = "길드명")]
        public string Name { get; set; }

		[Display(Name = "별칭")]
		public string Alias { get; set; }

        [Display(Name = "길드마크")]
        public string Image { get; set; }

        public ICollection<Character> Characters { get; set; }

        public ICollection<WeeklyRank> WeeklyRanks { get; set; }
    }

    public enum GuildPositions
    {
        [Display(Name = "뉴비")]
        Newbie,
        [Display(Name = "어프렌티스")]
        Apprentice,
        [Display(Name = "저니맨")]
        Journeyman,
        [Display(Name = "서브마스터")]
        SubMaster,
        [Display(Name = "마스터")]
        Master
    }
}
