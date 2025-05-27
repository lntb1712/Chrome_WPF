using Chrome_WPF.Models.GroupFunctionDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Models.GroupManagementDTO
{
    public class GroupManagementRequestDTO: INotifyPropertyChanged, IDataErrorInfo
    {
        private string _groupId =string.Empty;
        private string _groupName = string.Empty;
        private string? _groupDescription;
        private string _updateBy = string.Empty;

        private bool _isValidationRequested;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Mã nhóm không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Mã nhóm  chỉ được chứa chữ, số và dấu gạch dưới")]
        public string GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                OnPropertyChanged(nameof(GroupId));
            }
        }

        [Required(ErrorMessage = "Tên nhóm không được để trống")]
        public string GroupName
        {
            get => _groupName;
            set
            {
                _groupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }

        public string? GroupDescription
        {
            get => _groupDescription;
            set
            {
                _groupDescription = value;
                OnPropertyChanged(nameof(GroupDescription));
            }
        }

        public string UpdateBy
        {
            get => _updateBy;
            set
            {
                _updateBy = value;
                OnPropertyChanged(nameof(UpdateBy));
            }
        }
        public List<GroupFunctionResponseDTO> GroupFunctions { get; set; } = new List<GroupFunctionResponseDTO>();
        public string Error => string.Empty;
        
        public string this[string columnName]
        {
            get
            {
                if (!_isValidationRequested)
                    return string.Empty;

                // Bỏ qua validation cho một số property
                if (columnName == nameof(UpdateBy) || columnName == nameof(GroupDescription))
                    return string.Empty;

                // Lấy giá trị property hiện tại
                var property = GetType().GetProperty(columnName);
                if (property == null)
                    return string.Empty;

                var value = property.GetValue(this);
                var context = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();

                bool isValid = Validator.TryValidateProperty(value, context, results);

                return isValid ? string.Empty : results.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }
        public void RequestValidation()
        {
            _isValidationRequested = true;
            OnPropertyChanged(nameof(GroupId));
            OnPropertyChanged(nameof(GroupName));
            // Không cần gọi OnPropertyChanged cho UpdateBy vì không validate
            // Thông báo toàn bộ để đảm bảo UI cập nhật
        }
        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(GroupId));
            OnPropertyChanged(nameof(GroupName));
        }
    }
}
