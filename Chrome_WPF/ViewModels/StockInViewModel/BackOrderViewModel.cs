using Chrome_WPF.Helpers;
using Chrome_WPF.Models.StockInDetailDTO;
using Chrome_WPF.Services.NotificationService;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockInViewModel
{
    public class BackOrderDialogViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private ObservableCollection<StockInDetailResponseDTO> _stockInDetails; // Removed readonly modifier
        private bool _createBackorder;
        private bool _noBackorder;
        private bool _isClosed;

        public ObservableCollection<StockInDetailResponseDTO> StockInDetails
        {
            get => _stockInDetails;
            set
            {
                _stockInDetails = value;
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
            }
        }

        public bool NoBackorder
        {
            get => _noBackorder;
            set
            {
                _noBackorder = value;
                OnPropertyChanged();
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

        public ICommand CreateBackorderCommand { get; }
        public ICommand NoBackorderCommand { get; }
        public ICommand DiscardCommand { get; }

        public BackOrderDialogViewModel(
            INotificationService notificationService,
            ObservableCollection<StockInDetailResponseDTO> stockInDetails)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _stockInDetails = stockInDetails ?? throw new ArgumentNullException(nameof(stockInDetails));

            CreateBackorderCommand = new RelayCommand(_ => ExecuteCreateBackorder());
            NoBackorderCommand = new RelayCommand(_ => ExecuteNoBackorder());
            DiscardCommand = new RelayCommand(_ => ExecuteDiscard());
        }

        private void ExecuteCreateBackorder()
        {
            if (StockInDetails.Any(d => d.Quantity < d.Demand))
            {
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