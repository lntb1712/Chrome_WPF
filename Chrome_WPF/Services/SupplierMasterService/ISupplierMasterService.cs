using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.SupplierMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.SupplierMasterService
{
    public interface ISupplierMasterService
    {
        Task<ApiResult<PagedResponse<SupplierMasterResponseDTO>>> GetAllSupplierMaster(int page, int pageSize);
        Task<ApiResult<PagedResponse<SupplierMasterResponseDTO>>> SearchSupplierMaster(string textToSearch, int page, int pageSize);
        Task<ApiResult<SupplierMasterResponseDTO>> GetSupplierWithSupplierCode(string supplierCode);
        Task<ApiResult<bool>> AddSupplierMaster(SupplierMasterRequestDTO supplierMaster);
        Task<ApiResult<bool>> DeleteSupplierMaster(string supplierCode);    
        Task<ApiResult<bool>> UpdateSupplierMaster(SupplierMasterRequestDTO supplierMaster);
        Task<ApiResult<int>> GetTotalSupplierCount();
    }
}
