using Chrome_WPF.Helpers;
using Chrome_WPF.Models.StockInDetailDTO;
using Chrome_WPF.Services.NotificationService;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.StockInViewModel
{
    public class BackOrderDialogViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private bool _isClosed;

        public ObservableCollection<StockInDetailResponseDTO> StockInDetails { get; set; }
        public ICommand CreateBackorderCommand { get; }
        public ICommand NoBackorderCommand { get; }
        public ICommand DiscardCommand { get; }

        public BackOrderDialogViewModel(INotificationService notificationService, ObservableCollection<StockInDetailResponseDTO> stockInDetails)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            StockInDetails = stockInDetails ?? new ObservableCollection<StockInDetailResponseDTO>();
            CreateBackorderCommand = new RelayCommand(OnCreateBackorder);
            NoBackorderCommand = new RelayCommand(OnNoBackorder);
            DiscardCommand = new RelayCommand(OnDiscard);
        }

        private void OnCreateBackorder(object parameter)
        {
            _isClosed = true;
            OnPropertyChanged(nameof(IsClosed)); // Đảm bảo thông báo thay đổi
        }

        private void OnNoBackorder(object parameter)
        {
            _isClosed = true;
            OnPropertyChanged(nameof(IsClosed)); // Đảm bảo thông báo thay đổi
        }

        private void OnDiscard(object parameter)
        {
            _isClosed = true;
            OnPropertyChanged(nameof(IsClosed)); // Đảm bảo thông báo thay đổi
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
    }
}