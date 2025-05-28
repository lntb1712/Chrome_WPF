using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.CategoryDTO
{
    public class CategoryRequestDTO
    {
        public string CategoryId { get; set; } = null!;
        public string? CategoryName { get; set; }
    }
}
