using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.NotificationService
{
    public interface INotificationService
    {
        void ShowMessage(string message, string actionContent = null!, Action action = null!, TimeSpan? duration = null, bool isError = false);
        void RegisterSnackbar(Snackbar snackbar);
        void QueueMessageForNextSnackbar(string message, string actionContent = null!, Action action = null!, TimeSpan? duration = null, bool isError = false);
        void ShowPendingMessage();
    }
}
