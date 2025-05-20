using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.NotificationService
{
    public class NotificationManager
    {
        public static NotificationManager Instance { get; } = new NotificationManager();

        public INotificationService? Service { get; set; }
    }
}
