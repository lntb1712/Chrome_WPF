using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductMasterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ProductMasterService
{
    public interface IProductMasterService
    {
        Task<ApiResult<PagedResponse<ProductMasterResponseDTO>>> GetAllProductMaster(int page, int pageSize);
        Task<ApiResult<PagedResponse<ProductMasterResponseDTO>>> GetAllProductWithCategoryId(string categoryId, int page, int pageSize);
        Task<ApiResult<PagedResponse<ProductMasterResponseDTO>>> SearchProductInList(string textToSearch, int page, int pageSize);
        Task<ApiResult<ProductMasterResponseDTO>> GetProductMasterWithProductCode(string productCode);
        Task<ApiResult<bool>> AddProductMaster(ProductMasterRequestDTO productMasterRequestDTO);
        Task<ApiResult<bool>> UpdateProductMaster(ProductMasterRequestDTO productMasterRequestDTO);
        Task<ApiResult<bool>> DeleteProductMaster(string productCode);
        Task<ApiResult<int>> GetTotalProductCount();

    }
}
