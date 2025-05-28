using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ApiResult<List<CategoryResponseDTO>>> GetAllCategories();
        Task<ApiResult<List<CategorySummaryDTO>>> GetCategorySummary();
    }
}
