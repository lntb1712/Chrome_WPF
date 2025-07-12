using Chrome_WPF.Helpers;
using Chrome_WPF.Models.PurchaseOrderDetailDTO;
using Chrome_WPF.Models.StockInDetailDTO;
using Chrome_WPF.Services.NotificationService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.PurchaseOrderViewModel
{
    public class BackOrderDialogViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private ObservableCollection<PurchaseOrderDetailResponseDTO> _purchaseOrderDetails;
        private bool _createBackorder;
        private bool _noBackorder;
        private bool _isClosed;
        private DateTime? _selectedDate;

        public ObservableCollection<PurchaseOrderDetailResponseDTO> PurchaseOrderDetails
        {
            get => _purchaseOrderDetails;
            set
            {
                _purchaseOrderDetails = value;
                OnPropertyChanged();
            }
        }

        public bool CreateBackorder
        {
            get => _createBackorder;
            set
            {
                _createBackorder = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NoBackorder)); // Update to ensure mutual exclusivity
            }
        }

        public bool NoBackorder
        {
            get => _noBackorder;
            set
            {
                _noBackorder = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CreateBackorder)); // Update to ensure mutual exclusivity
            }
        }

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                _isClosed = value;
                OnPropertyChanged();
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateBackorderCommand { get; }
        public ICommand NoBackorderCommand { get; }
        public ICommand DiscardCommand { get; }

        public BackOrderDialogViewModel(
            INotificationService notificationService,
            ObservableCollection<PurchaseOrderDetailResponseDTO> PurchaseOrderDetails)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _purchaseOrderDetails = PurchaseOrderDetails ?? throw new ArgumentNullException(nameof(PurchaseOrderDetails));
            _selectedDate = DateTime.Now; // Default to current date

            CreateBackorderCommand = new RelayCommand(_ => ExecuteCreateBackorder(), CanExecuteCreateBackorder);
            NoBackorderCommand = new RelayCommand(_ => ExecuteNoBackorder());
            DiscardCommand = new RelayCommand(_ => ExecuteDiscard());
        }

        private void ExecuteCreateBackorder()
        {
            if (PurchaseOrderDetails.Any(d => d.QuantityReceived < d.Quantity))
            {
                if (!SelectedDate.HasValue || SelectedDate.Value < DateTime.Today)
                {
                    _notificationService.ShowMessage("Vui lòng chọn ngày dự kiến hoàn thành hợp lệ (từ hôm nay trở đi).", "OK", isError: true);
                    return;
                }

                CreateBackorder = true;
                NoBackorder = false;
                IsClosed = true;
                CloseDialog();
            }
            else
            {
                _notificationService.ShowMessage("Không cần tạo backorder vì tất cả sản phẩm đã đủ số lượng.", "OK", isError: true);
            }
        }

        private bool CanExecuteCreateBackorder(object parameter)
        {
            return SelectedDate.HasValue && SelectedDate.Value >= DateTime.Today;
        }

        private void ExecuteNoBackorder()
        {
            CreateBackorder = false;
            NoBackorder = true;
            IsClosed = true;
            CloseDialog();
        }

        private void ExecuteDiscard()
        {
            CreateBackorder = false;
            NoBackorder = false;
            IsClosed = true;
            CloseDialog();
        }

        private void CloseDialog()
        {
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}
