using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            PrimaryKitTeams = new HashSet<Team>();
            SecondaryKitTeams = new HashSet<Team>();
        }
        [Key]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.ColorLength)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(Team.PrimaryKitColor))] // This is needed only because we have 2 foreign keys pointing to the same class (Color) and we need to specify which collection is for which navigation property
        public ICollection<Team> PrimaryKitTeams { get; set; }

        [InverseProperty(nameof(Team.SecondaryKitColor))]
        public ICollection<Team> SecondaryKitTeams { get; set; }
    }
}
