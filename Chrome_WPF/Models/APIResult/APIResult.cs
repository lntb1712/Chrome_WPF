using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.APIResult
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        [JsonConstructor]
        public ApiResult(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Constructor cho trường hợp thành công không có dữ liệu
        public ApiResult(string message = "Thành công")
        {
            Success = true;
            Message = message;
        }

        // Constructor cho trường hợp lỗi
        public ApiResult(string errorMessage, bool success = false)
        {
            Success = success;
            Message = errorMessage;
        }
    }
}
