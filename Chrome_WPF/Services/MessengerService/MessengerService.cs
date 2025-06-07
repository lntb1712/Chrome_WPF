using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace Chrome_WPF.Services.MessengerService
{
    public class MessengerService : IMessengerService
    {
        private readonly Dictionary<string, List<Func<object?, Task>>> _subscribers = new();

        public  Task RegisterMessageAsync(string key, Func<object?, Task> callback)
        {
            if (!_subscribers.ContainsKey(key))
                _subscribers[key] = new List<Func<object?, Task>>();

            _subscribers[key].Add(callback);
            return Task.CompletedTask;
        }

        public async Task SendMessageAsync(string key, object? payload = null)
        {
            if (_subscribers.TryGetValue(key, out var actions))
            {
                foreach (var action in actions)
                {
                    await action.Invoke(payload);
                }
            }
        }
    }
}
