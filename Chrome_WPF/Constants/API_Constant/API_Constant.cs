using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Constants.API_Constant
{
    public class API_Constant
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7016/api/";

        public API_Constant()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
        }
        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
