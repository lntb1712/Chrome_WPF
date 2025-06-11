using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.InventoryDTO
{
    public class ProductWithLocationsDTO
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public List<LocationDetailDTO> Locations { get; set; } = new();
    }
}
