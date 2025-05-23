using Chrome_WPF.Models.APIResult;
using Chrome_WPF.Models.GroupFunctionDTO;
using Chrome_WPF.Models.GroupManagementDTO;
using Chrome_WPF.Models.PagedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.GroupManagementService
{
    public interface IGroupManagementService
    {
        Task<ApiResult<PagedResponse<GroupManagementResponseDTO>>> GetAllGroupManagement(int page, int pageSize);
        Task<ApiResult<PagedResponse<GroupManagementResponseDTO>>> SearchGroupInList(string textToSearch, int page, int pageSize);
        Task<ApiResult<List<GroupFunctionResponseDTO>>> GetGroupFunctionWithGroupID(string groupId);
        Task<ApiResult<List<GroupFunctionResponseDTO>>> GetAllGroupFunction();
        Task<ApiResult<Dictionary<string,int>>>GetTotalUserInGroup();
        Task<ApiResult<bool>> AddGroupManagement(GroupManagementRequestDTO groupManagementRequestDTO);
        Task<ApiResult<bool>> UpdateGroupManagement(GroupManagementRequestDTO groupManagementRequestDTO);
        Task<ApiResult<bool>> DeleteGroupManagement(string groupId);
    }
}
