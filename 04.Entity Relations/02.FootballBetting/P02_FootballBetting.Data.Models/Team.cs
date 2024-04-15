using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            HomeGames = new HashSet<Game>();
            AwayGames = new HashSet<Game>();
            Players = new HashSet<Player>();
        }
        [Key]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.TeamNameLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ValidationConstants.LogoUrlLength)]
        public string? LogoUrl { get; set; }

        [Required]
        [MaxLength(ValidationConstants.InitialsLength)]
        public string Initials { get; set; } = null!;

        public decimal Budget { get; set; } // decimal type is required by default, no need to use attribute

        public int PrimaryKitColorId { get; set; } // int is also required by default

        [ForeignKey(nameof(PrimaryKitColorId))]
        public virtual Color PrimaryKitColor { get; set; } = null!;

        public int SecondaryKitColorId { get; set;}

        [ForeignKey(nameof(SecondaryKitColorId))]
        public virtual Color SecondaryKitColor { get; set; } = null!;

        public int TownId { get; set; }

        [ForeignKey(nameof(TownId))]
        public virtual Town Town { get; set; } = null!;

        [InverseProperty(nameof(Game.HomeTeam))]
        public virtual ICollection<Game> HomeGames { get; set;} = null!;

        [InverseProperty(nameof(Game.AwayTeam))]
        public virtual ICollection<Game> AwayGames { get; set;} =null!;

        public virtual ICollection<Player> Players { get; set; } = null!;
    }
}
