using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Import
{
    public class ImportUserDTO
    {
        // this attribute comes with NewtonSoft and is not necessary in this case because the names of the JSON property and the DTO property are the same,
        // however if they were not, the attribute would clarify that the DTO property is reflecting the given JSON property
        [JsonProperty("firstName")] 
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; } = null!;

        [JsonProperty("age")]
        public int? age { get; set; }
    }
}
