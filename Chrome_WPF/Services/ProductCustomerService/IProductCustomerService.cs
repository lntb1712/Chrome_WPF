using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.ProductCustomerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.ProductCustomerService
{
    public interface IProductCustomerService
    {
        Task<ApiResult<PagedResponse<ProductCustomerResponseDTO>>> GetAllProductCustomers(string productCode,int page, int pageSize);
        Task<ApiResult<bool>> AddProductCustomer(ProductCustomerRequestDTO productCustomer);
        Task<ApiResult<bool>> DeleteProductCustomer(string productCode,string customerCode);    
        Task<ApiResult<bool>> UpdateProductCustomer(ProductCustomerRequestDTO productCustomer);
    }
}
