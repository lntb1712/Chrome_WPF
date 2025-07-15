using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.DashboardDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ApiResult<DashboardResponseDTO>> GetDashboardInformation(DashboardRequestDTO dashboardRequest);
        Task<ApiResult<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(DashboardRequestDTO dashboardRequest);
    }
}
