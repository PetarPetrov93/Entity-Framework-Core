﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            PerformerSongs = new HashSet<SongPerformer>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constraints.PerformerNameLength)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [MaxLength(Constraints.PerformerNameLength)]
        public string LastName { get; set; } = null!;

        public int Age { get; set; }

        public decimal NetWorth { get; set; }

        public ICollection<SongPerformer> PerformerSongs { get; set; }
    }
}
