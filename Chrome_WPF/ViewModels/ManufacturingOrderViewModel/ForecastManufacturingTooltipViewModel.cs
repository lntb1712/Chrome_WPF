using Chrome_WPF.Models.ManufacturingOrderDetailDTO;
using Chrome_WPF.Services.ManufacturingOrderDetailService;
using Chrome_WPF.Services.NotificationService;

namespace Chrome_WPF.ViewModels.ManufacturingOrderViewModel
{
    public class ForecastManufacturingTooltipViewModel : BaseViewModel
    {

        private readonly IManufacturingOrderDetailService _manufacturingOrderDetailService;
        private readonly INotificationService _notificationService;
        private ForecastManufacturingOrderDetailDTO _forecastData;
        private string _productName;

        public ForecastManufacturingOrderDetailDTO ForecastData
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

        public ForecastManufacturingTooltipViewModel(
            IManufacturingOrderDetailService manufacturingOrderDetailService,
            INotificationService notificationService)
        {
            _manufacturingOrderDetailService = manufacturingOrderDetailService ?? throw new ArgumentNullException(nameof(manufacturingOrderDetailService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _forecastData = new ForecastManufacturingOrderDetailDTO();
            _productName = string.Empty;
        }

        public async Task LoadForecastDataAsync(string manufacturingOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                ForecastData = null!;
                return;
            }

            try
            {
                var result = await _manufacturingOrderDetailService.GetForecastManufacturingOrderDetail(manufacturingOrderCode, productCode);
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


