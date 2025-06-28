using Chrome_WPF.Models.PickListDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.PickListDTO
{
    public class PickAndDetailResponseDTO
    {
        public string PickNo { get; set; } = null!;

        public string? ReservationCode { get; set; }

        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? PickDate { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public List<PickListDetailResponseDTO> pickListDetailResponseDTOs { get; set; } = new List<PickListDetailResponseDTO>();
    }
}
