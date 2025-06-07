using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chrome_WPF.Models.AccountManagementDTO
{
    public class AccountManagementRequestDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _fullName = string.Empty;
        private string _groupID = string.Empty;
        private bool _isValidationRequested;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới")]
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        [Required(ErrorMessage = "Tên đầy đủ không được để trống")]
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        [Required(ErrorMessage = "Vui lòng chọn nhóm người dùng")]
        
        public string GroupID
        {
            get => _groupID;
            set
            {
                _groupID = value;
                OnPropertyChanged(nameof(GroupID));
            }
        }
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (!_isValidationRequested)
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
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(GroupID));
            // Không cần gọi OnPropertyChanged cho UpdateBy vì không validate
 // Thông báo toàn bộ để đảm bảo UI cập nhật
        }

        public void ClearValidation()
        {
            _isValidationRequested = false;
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(GroupID));

        }
    }
}