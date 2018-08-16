using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Godius.Data.Models
{
    public class WeeklyRank : IComparable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name = "길드")]
        public Guild Guild { get; set; }
        [Display(Name = "길드")]
        public Guid? GuildId { get; set; }

        [Display(Name = "캐릭터")]
        public Character Character { get; set; }
        [Display(Name = "캐릭터")]
        public Guid? CharacterId { get; set; }

        [Display(Name = "날짜")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "랭킹")]
        public int Ranking { get; set; }

        public int CompareTo(object obj)
        {
            Rank other = obj as Rank;
            if (other == null)
                return 1;
            else
                return DateTime.Compare(Date.GetValueOrDefault(), other.Date.GetValueOrDefault());
        }
    }
}
