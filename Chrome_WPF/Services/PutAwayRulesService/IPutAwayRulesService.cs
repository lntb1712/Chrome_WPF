using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.PagedResponse;
using Chrome_WPF.Models.PutAwayRulesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.PutAwayRulesService
{
    public interface IPutAwayRulesService 
    {
        Task<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>> GetAllPutAwayRules(int page, int pageSize);
        Task<ApiResult<PagedResponse<PutAwayRulesResponseDTO>>> SearchPutAwayRules(string textToSearch, int page, int pageSize);
        Task<ApiResult<PutAwayRulesResponseDTO>> GetPutAwayRuleByCode(string putAwayRuleCode);
        Task<ApiResult<bool>> AddPutAwayRule(PutAwayRulesRequestDTO putAwayRule);
        Task<ApiResult<bool>> DeletePutAwayRule(string putAwayRuleCode);
        Task<ApiResult<bool>> UpdatePutAwayRule(PutAwayRulesRequestDTO putAwayRule);
        Task<ApiResult<int>> GetTotalPutAwayRuleCount();
    }
}
