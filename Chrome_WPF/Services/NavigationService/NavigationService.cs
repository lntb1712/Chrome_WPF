using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Chrome_WPF.Services.NavigationService
{
    public class NavigationService : INavigationService
    {
        private ContentControl _mainContent;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, UserControl> _userControlCache = new();

        public NavigationService(ContentControl mainContent, IServiceProvider serviceProvider)
        {
            _mainContent = mainContent;
            _serviceProvider = serviceProvider;
        }
        public void SetContentControl(ContentControl contentControl)
        {
            _mainContent = contentControl ?? throw new ArgumentNullException(nameof(contentControl));
        }

        public void NavigateTo(UserControl userControl)
        {
            _mainContent.Content = userControl;
        }

        public void NavigateTo<T>() where T : UserControl
        {
            if (!_userControlCache.TryGetValue(typeof(T), out var userControl))
            {
                userControl = _serviceProvider.GetRequiredService<T>();
                _userControlCache[typeof(T)] = userControl;
            }

            _mainContent.Content = userControl;
        }
    }
}
