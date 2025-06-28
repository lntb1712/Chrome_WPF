using Chrome_WPF.Models.ReservationDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.ReservationDTO
{
    public class ReservationAndDetailResponseDTO
    {
        public string ReservationCode { get; set; } = null!;

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }

        public string? OrderId { get; set; }

        public string? ReservationDate { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }

        public List<ReservationDetailResponseDTO> reservationDetailResponseDTOs { get; set; } = new List<ReservationDetailResponseDTO>();
    }
}
