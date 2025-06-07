using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_WPF.Services.MessengerService
{
    public interface IMessengerService
    {
        Task SendMessageAsync(string key, object? payLoad=null);
        Task RegisterMessageAsync(string key,Func<object?, Task> callback);
    }
}
