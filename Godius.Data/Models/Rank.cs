using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Godius.Data.Models
{
    public class Rank
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("CharacterId")]
        [Display(Name = "캐릭터")]
        public Character Character { get; set; }
        [Display(Name = "캐릭터")]
        public Guid CharacterId { get; set; }

        [Display(Name = "날짜")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "랭킹")]
        public int Ranking { get; set; }
    }
}
