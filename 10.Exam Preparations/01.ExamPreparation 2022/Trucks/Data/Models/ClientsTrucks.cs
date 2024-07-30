using System.ComponentModel.DataAnnotations.Schema;

namespace Trucks.Data.Models
{
    public class ClientsTrucks
    {
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        public int TruckId { get; set; }

        [ForeignKey(nameof(TruckId))]
        public Truck Truck { get; set; } = null!;
    }
}
