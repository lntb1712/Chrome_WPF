using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.StorageProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.StorageProductService
{
    public interface IStorageProductService
    {
        Task<ApiResult<PagedResponse<StorageProductResponseDTO>>> GetAllStorageProducts(int page, int pageSize);
        Task<ApiResult<PagedResponse<StorageProductResponseDTO>>> SearchStorageProducts(string textToSearch, int page, int pageSize);
        Task<ApiResult<StorageProductResponseDTO>> GetStorageProductByCode(string storageProductId);
        Task<ApiResult<bool>> AddStorageProduct(StorageProductRequestDTO storageProduct);
        Task<ApiResult<bool>> DeleteStorageProduct(string storageProductId);
        Task<ApiResult<bool>> UpdateStorageProduct(StorageProductRequestDTO storageProduct);
        Task<ApiResult<int>> GetTotalStorageProductCount();
    }
}
