using Chrome_WPF.Models.StockOutDetailDTO;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Services.StockOutDetailService;
using System;
using System.Threading.Tasks;

namespace Chrome_WPF.ViewModels.StockOutViewModel
{
    public class ForecastTooltipViewModel : BaseViewModel
    {
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly INotificationService _notificationService;
        private ForecastStockOutDetailDTO _forecastData;
        private string _productName;

        public ForecastStockOutDetailDTO ForecastData
        {
            get => _forecastData;
            set
            {
                _forecastData = value;
                OnPropertyChanged();
            }
        }

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
            }
        }

        public ForecastTooltipViewModel(
            IStockOutDetailService stockOutDetailService,
            INotificationService notificationService)
        {
            _stockOutDetailService = stockOutDetailService ?? throw new ArgumentNullException(nameof(stockOutDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _forecastData = new ForecastStockOutDetailDTO();
            _productName = string.Empty;
        }

        public async Task LoadForecastDataAsync(string stockOutCode, string productCode)
        {
            if (string.IsNullOrEmpty(stockOutCode) || string.IsNullOrEmpty(productCode))
            {
                ForecastData = null!;
                return;
            }

            try
            {
                var result = await _stockOutDetailService.GetForecastStockOutDetail(stockOutCode, productCode);
                if (result.Success && result.Data != null)
                {
                    ForecastData = result.Data;
                }
                else
                {
                    ForecastData = null!;
                    _notificationService.ShowMessage(result.Message ?? "Không thể tải dữ liệu dự báo.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                ForecastData = null!;
                _notificationService.ShowMessage($"Lỗi khi tải dữ liệu dự báo: {ex.Message}", "OK", isError: true);
            }
        }
    }
}