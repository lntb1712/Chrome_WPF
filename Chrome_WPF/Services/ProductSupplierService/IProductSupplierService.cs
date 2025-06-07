using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductSupplierDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ProductSupplierService
{
    public interface IProductSupplierService
    {
        Task<ApiResult<PagedResponse<ProductSupplierResponseDTO>>> GetAllProductSupplier(string productCode, int page, int pageSize);
        Task<ApiResult<bool>> AddProductSupplier(ProductSupplierRequestDTO productSupplier);
        Task<ApiResult<bool>> DeleteProductSupplier(string productCode, string supplierCode);
        Task<ApiResult<bool>> UpdateProductSupplier(ProductSupplierRequestDTO productSupplier);
    }
}
