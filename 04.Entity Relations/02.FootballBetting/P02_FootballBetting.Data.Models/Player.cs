
using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            PlayersStatistics = new HashSet<PlayerStatistic>();
        }
        [Key]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PlayerNameLength)]
        public string Name { get; set; } = null!;

        public int SquadNumber { get; set; }

        public bool IsInjured { get; set; } // bool is also required by default

        public int PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        public virtual Position Position { get; set; } = null!;

        public int TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; } = null!;

        public int TownId { get; set; }

        [ForeignKey(nameof(TownId))]
        public virtual Town Town { get; set; } = null!;

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }

    }
}
