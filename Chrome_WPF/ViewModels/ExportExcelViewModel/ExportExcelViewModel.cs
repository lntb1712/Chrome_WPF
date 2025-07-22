using Chrome_WPF.Helpers;
using Chrome_WPF.Services.NotificationService;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.ExportExcelViewModels
{
    public class ExportExcelViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private int? _dialogMonth;
        private int? _dialogYear;
        private ObservableCollection<int> _months;
        private ObservableCollection<int> _years;
        private bool _isClosed;

        public int? DialogMonth
        {
            get => _dialogMonth;
            set
            {
                _dialogMonth = value;
                OnPropertyChanged();
            }
        }

        public int? DialogYear
        {
            get => _dialogYear;
            set
            {
                _dialogYear = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Months
        {
            get => _months;
            set
            {
                _months = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Years
        {
            get => _years;
            set
            {
                _years = value;
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

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public ExportExcelViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _dialogMonth = DateTime.Now.Month;
            _dialogYear = DateTime.Now.Year;
            _months = new ObservableCollection<int>(Enumerable.Range(1, 12));
            _years = new ObservableCollection<int>(Enumerable.Range(2020, 6)); // 2020–2025
            _isClosed = false;

            OkCommand = new RelayCommand(_ => ExecuteOkCommand(), _ => CanExecuteOkCommand());
            CancelCommand = new RelayCommand(_ => ExecuteCancelCommand());
        }

        private void ExecuteOkCommand()
        {
            if (!DialogMonth.HasValue || !DialogYear.HasValue)
            {
                _notificationService.ShowMessage("Vui lòng chọn tháng và năm.", "OK", isError: true);
                return;
            }

            IsClosed = true;
            CloseDialog(true);
        }

        private bool CanExecuteOkCommand()
        {
            return DialogMonth.HasValue && DialogYear.HasValue;
        }

        private void ExecuteCancelCommand()
        {
            IsClosed = true;
            CloseDialog(false);
        }

        private void CloseDialog(bool dialogResult)
        {
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.DialogResult = dialogResult;
                window.Close();
            }
        }
    }
}