using Chrome_WPF.Models.APIResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.CodeGeneratorService
{
    public interface ICodeGenerateService
    {
        Task<ApiResult<string>> CodeGeneratorAsync(string warehouseCode,string type);
    }
}
