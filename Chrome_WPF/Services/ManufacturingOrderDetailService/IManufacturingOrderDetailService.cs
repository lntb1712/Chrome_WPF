using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.ManufacturingOrderDetailDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ManufacturingOrderDetailService
{
    public interface IManufacturingOrderDetailService
    {
        Task<ApiResult<PagedResponse<ManufacturingOrderDetailResponseDTO>>> GetManufacturingOrderDetail(string manufacturingOrderCode);
        Task<ApiResult<ManufacturingOrderDetailResponseDTO>> GetManufacturingOrderDetail(string manufacturingOrderCode, string productCode);
        Task<ApiResult<ForecastManufacturingOrderDetailDTO>> GetForecastManufacturingOrderDetail(string manufacturingOrderCode, string productCode);
    }
}
