using Chrome_WPF.Models.GroupFunctionDTO;

namespace Chrome_WPF.Models.FunctionDTO
{
    public class FunctionResponseDTO
    {
        public string? FunctionId { get; set; }
        public string? FunctionName { get; set; }
        public bool IsEnable { get; set; }
        public List<ApplicableLocationResponseDTO> ApplicableLocations { get; set; }
         = new List<ApplicableLocationResponseDTO>();
    }
}
