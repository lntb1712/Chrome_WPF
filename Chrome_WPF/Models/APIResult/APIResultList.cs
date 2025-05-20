using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.APIResult
{
    public class ApiResultList<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<T>? DataList { get; set; }
        [JsonConstructor]
        // Constructor cho trường hợp thành công với danh sách
        public ApiResultList(bool success, string message, List<T> dataList)
        {
            Success = success;
            DataList = dataList;
            Message = message;
        }

        // Constructor cho trường hợp thành công không có dữ liệu
        public ApiResultList(string message = "Thành công")
        {
            Success = true;
            Message = message;
        }

        // Constructor cho trường hợp lỗi
        public ApiResultList(string errorMessage, bool success = false)
        {
            Success = success;
            Message = errorMessage;
        }
    }
}
