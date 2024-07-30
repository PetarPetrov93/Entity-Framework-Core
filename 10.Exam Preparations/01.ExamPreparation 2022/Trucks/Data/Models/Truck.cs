using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(8)]
        public string? RegistrationNumber { get; set; }

        [MaxLength(17)]
        [Required]
        public string VinNumber { get; set; } = null!;

        public int TankCapacity { get; set; }

        public int CargoCapacity { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        public MakeType MakeType { get; set; }

        [Required]
        public int DespatcherId { get; set; }

        [ForeignKey(nameof(DespatcherId))]
        public Despatcher Despatcher { get; set; } = null!;

        public virtual ICollection<ClientsTrucks> ClientsTrucks { get; set; } = new HashSet<ClientsTrucks>();
    }
}
