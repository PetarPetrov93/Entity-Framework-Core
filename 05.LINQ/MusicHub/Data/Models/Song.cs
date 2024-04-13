using Castle.Core.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MusicHub.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Song
    {
        public Song()
        {
            SongPerformers = new HashSet<SongPerformer>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constraints.SongNameLength)]
        public string Name { get; set; } = null!;

        public TimeSpan Duration { get; set; }

        public DateTime CreatedOn { get; set; }

        public Genre Genre { get; set; }

        public int? AlbumId { get; set;}

        [ForeignKey(nameof(AlbumId))]
        public Album? Album { get; set; }

        public int WriterId { get; set; }

        [ForeignKey(nameof(WriterId))]
        public Writer Writer { get; set; } = null!;

        public decimal Price { get; set; }

        public ICollection<SongPerformer> SongPerformers { get; set;}
    }
}
