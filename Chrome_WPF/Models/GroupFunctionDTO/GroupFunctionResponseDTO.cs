using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.GroupFunctionDTO
{
    public class GroupFunctionResponseDTO
    {
        public string GroupId { get; set; } = null!;
        public string FunctionId { get; set; } = null!;
        public string FunctionName { get; set; } = null!;
        public bool? IsEnable { get; set; }
        public ObservableCollection<ApplicableLocationResponseDTO> LstApplicableLocations { get; set; }
        = new ObservableCollection<ApplicableLocationResponseDTO>();
    }
}
