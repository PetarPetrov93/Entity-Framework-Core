﻿using System.ComponentModel.DataAnnotations;

namespace Trucks.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        public virtual ICollection<ClientsTrucks> ClientsTrucks { get; set; } = new HashSet<ClientsTrucks>();
    }
}
