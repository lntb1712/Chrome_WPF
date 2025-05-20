
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static MaterialDesignThemes.Wpf.Theme;

namespace Chrome_WPF.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private Snackbar? _snackbar;
        private (string message, string actionContent, Action action, TimeSpan? duration, bool isError)? _pendingMessage;

        public void RegisterSnackbar(Snackbar snackbar)
        {
            _snackbar = snackbar ?? throw new ArgumentNullException(nameof(snackbar), "Snackbar cannot be null");

            if (_snackbar.MessageQueue == null)
            {
                _snackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            }

            // Hiển thị thông báo đang chờ nếu có
            ShowPendingMessage();
        }

        public void QueueMessageForNextSnackbar(string message, string actionContent = null!, Action action = null!, TimeSpan? duration = null, bool isError = true)
        {
            // Lưu thông báo để hiển thị sau
            _pendingMessage = (message, actionContent, action, duration, !isError);
        }

        public void ShowPendingMessage()
        {
            if (_pendingMessage.HasValue && _snackbar != null)
            {
                var (message, actionContent, action, duration, isError) = _pendingMessage.Value;
                ShowMessage(message, actionContent, action, duration, isError);
                _pendingMessage = null; // Xóa thông báo sau khi hiển thị
            }
        }

        public void ShowMessage(string message, string actionContent = null!, Action action = null!, TimeSpan? duration = null, bool isError = true)
        {
            if (_snackbar == null)
            {
                System.Diagnostics.Debug.WriteLine("Snackbar is not registered yet.");
                // Lưu thông báo nếu Snackbar chưa sẵn sàng
                QueueMessageForNextSnackbar(message, actionContent, action, duration, !isError);
                return;
            }

            if (_snackbar.MessageQueue == null)
            {
                _snackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            }


            if (isError)
            {
                _snackbar.Background = new SolidColorBrush(Color.FromRgb(211, 47, 47)); // Đỏ cho lỗi
                _snackbar.Foreground = Brushes.White;
                _snackbar.ActionButtonStyle = new Style(typeof(System.Windows.Controls.Button))
                {
                    Setters =
                    {
                        new Setter(System.Windows.Controls.Control.ForegroundProperty, Brushes.White),
                        new Setter(System.Windows.Controls.Control.BackgroundProperty, Brushes.Transparent), // hoặc màu xanh, hoặc trong suốt
                        new Setter(System.Windows.Controls.Control.BorderBrushProperty, Brushes.Transparent),
                        new Setter(System.Windows.Controls.Control.FontWeightProperty, FontWeights.Bold)
                    }
                };


            }
            else
            {
                _snackbar.Background = new SolidColorBrush(Color.FromRgb(56, 142, 60)); // Xanh lá cho thành công
                _snackbar.Foreground = Brushes.White;
                _snackbar.ActionButtonStyle = new Style(typeof(System.Windows.Controls.Button))
                {
                    Setters =
                    {
                        new Setter(System.Windows.Controls.Control.ForegroundProperty, Brushes.White),
                        new Setter(System.Windows.Controls.Control.BackgroundProperty, Brushes.Transparent), // hoặc màu xanh, hoặc trong suốt
                        new Setter(System.Windows.Controls.Control.BorderBrushProperty, Brushes.Transparent),
                        new Setter(System.Windows.Controls.Control.FontWeightProperty, FontWeights.Bold)
                    }
                };
            }


            _snackbar.MessageQueue.Enqueue(
                message,
                actionContent,
                param => action?.Invoke(),
                null,
                false,
                true,
                duration ?? TimeSpan.FromSeconds(3));
        }
    }
}
