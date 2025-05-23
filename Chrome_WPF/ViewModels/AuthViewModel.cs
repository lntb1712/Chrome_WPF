using Chrome_WPF.Models.AccountManagementDTO;
using Chrome_WPF.Models.LoginDTO;
using Chrome_WPF.Services.AuthServices;
using Chrome_WPF.Services.LoginServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Chrome_WPF.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private string? _token;
        private UserInformationDTO? _userInformationDTO;
        private readonly ILoginService _loginService;
        private readonly IAuthService _authService;
        private List<string> _permission = new List<string>();
        private bool _canAccountManagement;
        private bool _canGroupManagement;
        private bool _canProductMaster;
        private bool _canSupplierMaster;
        private bool _canCustomerMaster;
        private bool _canWarehouserMaster;
        private bool _canBOMList;
        private bool _canStockIn;
        private bool _canStockOut;
        private bool _canTransfer;
        private bool _canMovement;
        private bool _canPickList;
        private bool _canPutAway;
        private bool _canStockTake;
        private bool _canProductionOrder;
        private string _currentDateTime;
        private readonly DispatcherTimer _timer;

        public string Token
        {
            get => _token!;
            set
            {
                if (_token != value)
                {
                    _token = value;
                    _ = ParseToken();
                    OnPropertyChanged(nameof(Token));
                }
            }
        }

        public List<string> Permission
        {
            get => _permission;
            set
            {
                if (_permission != value)
                {
                    _permission = value;
                    OnPropertyChanged(nameof(Permission));
                }
            }
        }

        public UserInformationDTO? UserInformationDTO
        {
            get => _userInformationDTO;
            set
            {
                _userInformationDTO = value;
                _userInformationDTO!.ImageInitial = string.IsNullOrEmpty(value!.FullName) ? "?" : value.FullName[0].ToString().ToUpper();
                OnPropertyChanged(nameof(UserInformationDTO));
            }
        }


        public bool CanAccountManagement
        {
            get => _canAccountManagement;
            set
            {
                _canAccountManagement = value;
                OnPropertyChanged(nameof(CanAccountManagement));
            }
        }
        public bool CanGroupManagement
        {
            get => _canGroupManagement;
            set
            {
                _canGroupManagement = value;
                OnPropertyChanged(nameof(CanGroupManagement));
            }
        }
        public bool CanProductMaster
        {
            get => _canProductMaster;
            set
            {
                _canProductMaster = value;
                OnPropertyChanged(nameof(CanProductMaster));
            }
        }
        public bool CanSupplierMaster
        {
            get => _canSupplierMaster;
            set
            {
                _canSupplierMaster = value;
                OnPropertyChanged(nameof(CanSupplierMaster));
            }
        }
        public bool CanCustomerMaster
        {
            get => _canCustomerMaster;
            set
            {
                _canCustomerMaster = value;
                OnPropertyChanged(nameof(CanCustomerMaster));
            }
        }
        public bool CanWarehouserMaster
        {
            get => _canWarehouserMaster;
            set
            {
                _canWarehouserMaster = value;
                OnPropertyChanged(nameof(CanWarehouserMaster));
            }
        }
        public bool CanBOMList
        {
            get => _canBOMList;
            set
            {
                _canBOMList = value;
                OnPropertyChanged(nameof(CanBOMList));
            }
        }
        public bool CanStockIn
        {
            get => _canStockIn;
            set
            {
                _canStockIn = value;
                OnPropertyChanged(nameof(CanStockIn));
            }
        }
        public bool CanStockOut
        {
            get => _canStockOut;
            set
            {
                _canStockOut = value;
                OnPropertyChanged(nameof(CanStockOut));
            }
        }
        public bool CanTransfer
        {
            get => _canTransfer;
            set
            {
                _canTransfer = value;
                OnPropertyChanged(nameof(CanTransfer));
            }
        }
        public bool CanMovement
        {
            get => _canMovement;
            set
            {
                _canMovement = value;
                OnPropertyChanged(nameof(CanMovement));
            }
        }
        public bool CanPickList
        {
            get => _canPickList;
            set
            {
                _canPickList = value;
                OnPropertyChanged(nameof(CanPickList));
            }
        }
        public bool CanPutAway
        {
            get => _canPutAway;
            set
            {
                _canPutAway = value;
                OnPropertyChanged(nameof(CanPutAway));
            }
        }
        public bool CanStockTake
        {
            get => _canStockTake;
            set
            {
                _canStockTake = value;
                OnPropertyChanged(nameof(CanStockTake));
            }
        }
        public bool CanProductionOrder
        {
            get => _canProductionOrder;
            set
            {
                _canProductionOrder = value;
                OnPropertyChanged(nameof(CanProductionOrder));
            }
        }
        public string CurrentDateTime
        {
            get => _currentDateTime;
            set
            {
                _currentDateTime = value;
                OnPropertyChanged();
            }
        }

        public AuthViewModel(ILoginService loginService, IAuthService authService)
        {
            _loginService = loginService;
            _authService = authService;
            LoadTokenFromSettings();
            _currentDateTime = DateTime.Now.ToString("dd MMMM, yyyy HH:mm");
            // Initialize timer
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1) // Update every second
            };
            _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now.ToString("dd MMMM, yyyy HH:mm");
            _timer.Start();
        }

        private async void LoadUserInformation()
        {
            try
            {
                var userInfo = await _loginService.GetUserInformation(Properties.Settings.Default.UserName);
                if (userInfo != null)
                {
                    UserInformationDTO = userInfo.Data;
                }
            }
            catch
            {
                UserInformationDTO= new UserInformationDTO()
                {
                    FullName = "Người dùng không xác định",
                    ImageInitial = "?"
                };
            }
        }
        private async void LoadTokenFromSettings()
        {
            var tokenString = Properties.Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(tokenString))
            {
                Token = tokenString;
                await ParseToken();
                LoadUserInformation();

            }
        }
        private async Task ParseToken()
        {
            Permission = await _authService.GetPermissionFromToken(Token);

            CanAccountManagement = Permission.Contains("ucAccountManagement");
            CanGroupManagement = Permission.Contains("ucGroupManagement");
            CanProductMaster = Permission.Contains("ucProductMaster");
            CanSupplierMaster = Permission.Contains("ucSupplierMaster");
            CanCustomerMaster = Permission.Contains("ucCustomerMaster");
            CanWarehouserMaster = Permission.Contains("ucWarehouserMaster");
            CanBOMList = Permission.Contains("ucBOMList");
            CanStockIn = Permission.Contains("ucStockIn");
            CanStockOut = Permission.Contains("ucStockOut");
            CanTransfer = Permission.Contains("ucTransfer");
            CanMovement = Permission.Contains("ucMovement");
            CanPickList = Permission.Contains("ucPickList");
            CanPutAway = Permission.Contains("ucPutAway");
            CanStockTake = Permission.Contains("ucStockTake");
            CanProductionOrder = Permission.Contains("ucProductionOrder");
        }
    }
}
