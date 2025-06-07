using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CustomerMasterDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.CustomerMasterService
{
    public interface ICustomerMasterService
    {
        Task<ApiResult<PagedResponse<CustomerMasterResponseDTO>>> GetAllCustomerMaster(int page, int pageSize);
        Task<ApiResult<PagedResponse<CustomerMasterResponseDTO>>> SearchCustomerMaster(string textToSearch, int page, int pageSize);
        Task<ApiResult<CustomerMasterResponseDTO>> GetCustomerWithCustomerCode(string customerCode);
        Task<ApiResult<bool>> AddCustomerMaster(CustomerMasterRequestDTO customerMaster);
        Task<ApiResult<bool>> DeleteCustomerMaster(string customerCode);    
        Task<ApiResult<bool>> UpdateCustomerMaster(CustomerMasterRequestDTO customerMaster);
        Task<ApiResult<int>> GetTotalCustomerCount();
    }
}
